using Acr.UserDialogs;
using FreshMvvm;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Infraestructure
{
    [AddINotifyPropertyChangedInterface]
    abstract class BasePageModel : FreshBasePageModel
    {
        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

        bool _haveInternet = true;
        public bool IsNotConnected { get { return !CrossConnectivity.Current.IsConnected || !_haveInternet; } }
        public bool IsCameraAvailable { get { return CrossMedia.Current.IsCameraAvailable; } }
        public IEnumerable<ConnectionType> ConnectionTypes { get { return CrossConnectivity.Current.ConnectionTypes; } }
        public IEnumerable<UInt64> Bandwidths { get { return CrossConnectivity.Current.Bandwidths; } }
        protected IUserDialogs Dialogs { get; }

        protected BasePageModel()
        {
            Dialogs = UserDialogs.Instance;
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            try
            {
                _haveInternet = await CrossConnectivity.Current.IsRemoteReachable(URL.Host, msTimeout: 1000);
                RaisePropertyChanged("IsNotConnected");

                CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;

                if (_haveInternet && string.IsNullOrWhiteSpace(App.ConnectionIDChatHub) && App.Persona != null)
                {
                    await App.ReconectarChatHub();
                }
            }
            catch (Exception)
            {

            }
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            CrossConnectivity.Current.ConnectivityChanged -= Current_ConnectivityChanged;
        }

        protected async void Current_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            try
            {
                _haveInternet = await CrossConnectivity.Current.IsRemoteReachable(URL.Host, msTimeout: 1000);
                RaisePropertyChanged("IsNotConnected");

                if (ConnectionChanged != null)
                {
                    ConnectionChanged(this, new ConnectionChangedEventArgs(_haveInternet));
                }

                if (_haveInternet && string.IsNullOrWhiteSpace(App.ConnectionIDChatHub))
                {
                    await App.ReconectarChatHub();
                }
            }
            catch (Exception)
            {

            }
        }

        protected async Task<FingerprintAuthenticationResult> AuthenticateAsync()
        {
            if (await CrossFingerprint.Current.IsAvailableAsync())
            {
                AuthenticationRequestConfiguration pluginDialogConfiguration = new AuthenticationRequestConfiguration(SportsGoResources.AutenticacionSportsGo);
                pluginDialogConfiguration.CancelTitle = SportsGoResources.Cancelar;
                pluginDialogConfiguration.UseDialog = true;

                return await CrossFingerprint.Current.AuthenticateAsync(pluginDialogConfiguration);
            }
            else
            {
                return null;
            }
        }

        protected async Task<MediaFile> TakePhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.NoCamara, SportsGoResources.CamaraNoDisponible, "OK");
                return null;
            }

            if (await CheckCameraPermissionAndAskForIt() && await CheckStoragePermissionAndAskForIt())
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    CompressionQuality = 80,
                    PhotoSize = PhotoSize.Large,
                    //AllowCropping = true
                });

                return file;
            }
            else
            {
                if (await CoreMethods.DisplayAlert(SportsGoResources.PermisoDenegado, SportsGoResources.NoSePudoTomarFoto, "OK", SportsGoResources.Cancelar))
                {
                    //On iOS you may want to send your user to the settings screen.
                    CrossPermissions.Current.OpenAppSettings();
                }

                return null;
            }
        }

        protected async Task<MediaFile> PickPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.NoGaleria, SportsGoResources.GaleriaNoDisponible, "OK");
                return null;
            }

            if (await CheckStoragePermissionAndAskForIt())
            {
                MediaFile file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    CompressionQuality = 80,
                    PhotoSize = PhotoSize.Large,
                });

                return file;
            }
            else
            {
                if (await CoreMethods.DisplayAlert(SportsGoResources.PermisoDenegado, SportsGoResources.NoSePudoPickearFoto, "OK", SportsGoResources.Cancelar))
                {
                    //On iOS you may want to send your user to the settings screen.
                    CrossPermissions.Current.OpenAppSettings();
                }

                return null;
            }
        }

        protected async Task<MediaFile> TakeVideosync()
        {
            await CrossMedia.Current.Initialize();

            if (!IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.NoCamara, SportsGoResources.CamaraNoDisponible, "OK");
                return null;
            }

            if (await CheckCameraPermissionAndAskForIt() && await CheckStoragePermissionAndAskForIt())
            {
                MediaFile file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                {
                    SaveToAlbum = true,
                    Quality = VideoQuality.Medium,
                    DesiredLength = TimeSpan.FromSeconds(App.Usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo),
                    CompressionQuality = 70,
                    DesiredSize = 100000000 
                });

                return file;
            }
            else
            {
                if (await CoreMethods.DisplayAlert(SportsGoResources.PermisoDenegado, SportsGoResources.NosePudoTomarVideo, "OK", SportsGoResources.Cancelar))
                {
                    //On iOS you may want to send your user to the settings screen.
                    CrossPermissions.Current.OpenAppSettings();
                }

                return null;
            }
        }

        protected async Task<MediaFile> PickVideoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickVideoSupported)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.NoGaleria, SportsGoResources.GaleriaNoDisponible, "OK");
                return null;
            }

            if (await CheckStoragePermissionAndAskForIt())
            {
                MediaFile file = await CrossMedia.Current.PickVideoAsync();

                return file;
            }
            else
            {
                if (await CoreMethods.DisplayAlert(SportsGoResources.PermisoDenegado, SportsGoResources.NoSePudoPickearVideo, "OK", SportsGoResources.Cancelar))
                {
                    //On iOS you may want to send your user to the settings screen.
                    CrossPermissions.Current.OpenAppSettings();
                }

                return null;
            }
        }

        protected List<int> CategoriasDelPerfil(PersonasDTO persona)
        {
            List<int> categoriasDelPerfil = null;

            switch (persona.TipoPerfil)
            {
                case TipoPerfil.Candidato:
                    categoriasDelPerfil = persona.CandidatoDeLaPersona.CategoriasCandidatos.Select(x => x.CodigoCategoria).ToList();
                    break;
                case TipoPerfil.Grupo:
                    categoriasDelPerfil = persona.GrupoDeLaPersona.CategoriasGrupos.Select(x => x.CodigoCategoria).ToList();
                    break;
                case TipoPerfil.Representante:
                    categoriasDelPerfil = persona.RepresentanteDeLaPersona.CategoriasRepresentantes.Select(x => x.CodigoCategoria).ToList();
                    break;
            }

            return categoriasDelPerfil;
        }

        async Task<bool> CheckCameraPermissionAndAskForIt()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);

            if (cameraStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera });
                cameraStatus = results[Permission.Camera];
            }

            return cameraStatus == PermissionStatus.Granted;
        }

        async Task<bool> CheckStoragePermissionAndAskForIt()
        {
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage });
                storageStatus = results[Permission.Storage];
            }

            return storageStatus == PermissionStatus.Granted;
        }
    }
}