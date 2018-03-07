using FFImageLoading;
using MvvmHelpers;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using System.Windows.Input;
using FFImageLoading.Forms;
using FFImageLoading.Cache;
using Acr.UserDialogs;
using Xpinn.SportsGo.Util.Portable.Args;
using Xpinn.SportsGo.Util.Portable.Abstract;
using FreshMvvm;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PerfilPageModel : BasePageModel
    {
        public static event EventHandler<PersonaBorradaArgs> OnPersonaBorrada;

        ArchivosServices _archivoServices;
        PersonasServices _personaServices;
        CandidatosServices _candidatosServices;
        GruposServices _gruposServices;
        ChatsServices _chatServices;
        IDateTimeHelper _dateTimeHelper;

        public PersonasDTO Persona { get; set; }

        public bool EstaAgregadoContacto { get; set; }
        public bool NoHayNadaMasParaCargarPublicacion { get; set; }
        public bool NoHayNadaMasParaCargarContactos { get; set; }
        public bool IsRefreshing { get; set; }
        public DateTime LastRefresh { get; set; }
        public int NumeroContactosBuscados { get; set; }

        ObservableRangeCollection<BiografiaTimeLineModel> _timeLine;
        public ObservableRangeCollection<BiografiaTimeLineModel> TimeLine
        {
            get
            {
                IEnumerable<BiografiaTimeLineModel> listaTimeLine = _timeLine;

                if (listaTimeLine != null && listaTimeLine.Count() > 0)
                {
                    listaTimeLine = listaTimeLine.Where(x => x.TipoTimeLine == TipoItemParaListar);

                    return new ObservableRangeCollection<BiografiaTimeLineModel>(listaTimeLine);
                }
                else
                {
                    return new ObservableRangeCollection<BiografiaTimeLineModel>();
                }
            }
        }

        int _selectIndexSegmentBar;
        public int SelectIndexSegmentBar
        {
            get
            {
                return _selectIndexSegmentBar;
            }
            set
            {
                // Si estoy en la transicion de publicaciones a contactos
                _selectIndexSegmentBar = value;

                if (ViendoContactos)
                {
                    ReiniciarContactos();
                    NoHayNadaMasParaCargarContactos = false;
                    LoadMoreTimeLine.Execute(null);
                }
            }
        }

        public TipoItemTimeLine TipoItemParaListar
        {
            get
            {
                return (TipoItemTimeLine)Enum.Parse(typeof(TipoItemTimeLine), (SelectIndexSegmentBar + 1).ToString());
            }
        }

        public bool ViendoPublicaciones
        {
            get
            {
                return SelectIndexSegmentBar == 0;
            }
        }

        public bool ViendoContactos
        {
            get
            {
                return SelectIndexSegmentBar == 1;
            }
        }

        BiografiaTimeLineModel _selectedTimeLine;
        public BiografiaTimeLineModel SelectedTimeLine
        {
            get
            {
                return _selectedTimeLine;
            }
            set
            {
                _selectedTimeLine = null;
            }
        }

        public string NombreTitulo
        {
            get
            {
                string nombreTitulo = string.Empty;

                if (Persona != null)
                {
                    nombreTitulo = Persona.Nombres + " " + Persona.Apellidos;
                }
                else
                {
                    nombreTitulo = SportsGoResources.SinNombre;
                }

                return nombreTitulo;
            }
        }

        public bool EsMiPersona
        {
            get
            {
                return App.Persona != null && Persona != null && Persona.Consecutivo == App.Persona.Consecutivo;
            }
        }

        public bool PuedeInteractuarPublicaciones
        {
            get
            {
                return EsMiPersona && (EsCandidato || EsGrupo);
            }
        }

        public bool EsCandidato
        {
            get
            {
                return Persona != null && Persona.TipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool EsGrupo
        {
            get
            {
                return Persona != null && Persona.TipoPerfil == TipoPerfil.Grupo;
            }
        }

        public bool EsGrupoOCandidato
        {
            get
            {
                return EsCandidato || EsGrupo;
            }
        }

        string _imagenPerfil;
        public string ImagenPerfil
        {
            get
            {
                string defaultImagenPerfil = (string)App.Current.Resources["RutaDefaultImagenPerfil"];

                if (Persona != null && !string.IsNullOrWhiteSpace(Persona.UrlImagenPerfil))
                {
                    _imagenPerfil = Persona.UrlImagenPerfil;
                }
                else
                {
                    _imagenPerfil = defaultImagenPerfil;
                }

                return _imagenPerfil;
            }
            set
            {
                _imagenPerfil = value;
            }
        }

        string _imagenBanner;
        public string ImagenBanner
        {
            get
            {
                string defaultImagenBanner = (string)App.Current.Resources["RutaDefaultImagenBanner"];

                if (Persona != null && !string.IsNullOrWhiteSpace(Persona.UrlImagenBanner))
                {
                    _imagenBanner = Persona.UrlImagenBanner;
                }
                else
                {
                    _imagenBanner = defaultImagenBanner;
                }

                return _imagenBanner;
            }
            set
            {
                _imagenBanner = value;
            }
        }

        string DeleteActionSheetOptionPerfil
        {
            get
            {
                string defaultImagenPerfil = (string)App.Current.Resources["RutaDefaultImagenPerfil"];
                string deleteOption = null;

                if (!string.IsNullOrWhiteSpace(ImagenPerfil) && ImagenPerfil != defaultImagenPerfil)
                {
                    deleteOption = SportsGoResources.BorrarFoto;
                }

                return deleteOption;
            }
        }

        string DeleteActionSheetOptionBanner
        {
            get
            {
                string defaultImagenBanner = (string)App.Current.Resources["RutaDefaultImagenBanner"];
                string deleteOption = null;

                if (!string.IsNullOrWhiteSpace(ImagenBanner) && ImagenBanner != defaultImagenBanner)
                {
                    deleteOption = SportsGoResources.BorrarFoto;
                }

                return deleteOption;
            }
        }

        public PerfilPageModel()
        {
            _archivoServices = new ArchivosServices();
            _personaServices = new PersonasServices();
            _candidatosServices = new CandidatosServices();
            _gruposServices = new GruposServices();
            _chatServices = new ChatsServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            PersonasDTO personaParaBuscar = initData as PersonasDTO;
            LastRefresh = DateTime.Now;

            try
            {
                if (personaParaBuscar.Consecutivo == App.Persona.Consecutivo)
                {
                    Persona = App.Persona;
                }
                else
                {
                    personaParaBuscar.IdiomaDeLaPersona = App.IdiomaPersona;
                    personaParaBuscar.ConsecutivoViendoPersona = App.Persona.Consecutivo;

                    if (IsNotConnected) return;
                    Persona = await _personaServices.BuscarPersona(personaParaBuscar);

                    await VerificarSiPersonaEstaAgregada();
                }

                await CargarItemsEnTimeLineSegunPerfil(0, 5);
            }
            catch (Exception)
            {

            }
        }

        public override void ReverseInit(object returnData)
        {
            base.ReverseInit(returnData);

            if (returnData is PublicacionModel)
            {
                PublicacionModel publicacionSeleccionada = returnData as PublicacionModel;

                BiografiaTimeLineModel nuevoTimeLine = new BiografiaTimeLineModel(publicacionSeleccionada);

                if (_timeLine == null)
                {
                    _timeLine = new ObservableRangeCollection<BiografiaTimeLineModel>();
                }

                BiografiaTimeLineModel biografiaVieja = _timeLine.Where(x => x.CodigoPublicacion == nuevoTimeLine.CodigoPublicacion).FirstOrDefault();

                if (publicacionSeleccionada.FueBorrado)
                {
                    if (biografiaVieja != null)
                    {
                        _timeLine.Remove(biografiaVieja);
                    }
                }
                else
                {
                    if (biografiaVieja != null)
                    {
                        int viejoIndice = _timeLine.IndexOf(biografiaVieja);
                        _timeLine.Remove(biografiaVieja);

                        RaisePropertyChanged("TimeLine");
                        _timeLine.Insert(viejoIndice, nuevoTimeLine);
                    }
                    else
                    {
                        _timeLine.Insert(0, nuevoTimeLine);
                    }
                }

                RaisePropertyChanged("TimeLine");
            }
            else if (returnData is ImagenEditorModel)
            {
                ImagenEditorModel imagenModel = returnData as ImagenEditorModel;

                if (imagenModel.EsImagenPerfil)
                {
                    ImagenPerfil = imagenModel.Source;
                    Persona.CodigoArchivoImagenPerfil = imagenModel.CodigoArchivoCreado;
                    App.Persona.CodigoArchivoImagenPerfil = imagenModel.CodigoArchivoCreado;
                }
                else
                {
                    ImagenBanner = imagenModel.Source;
                    Persona.CodigoArchivoImagenBanner = imagenModel.CodigoArchivoCreado;
                    App.Persona.CodigoArchivoImagenBanner = imagenModel.CodigoArchivoCreado;
                }
            }
        }

        protected override async void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            NoHayNadaMasParaCargarPublicacion = false;
            NoHayNadaMasParaCargarContactos = false;

            if (ViendoContactos)
            {
                LoadMoreTimeLine.Execute(null);
            }

            if (!EsMiPersona)
            {
                try
                {
                    await VerificarSiPersonaEstaAgregada();
                }
                catch (Exception)
                {

                }
            }

            RaisePropertyChanged("NombreTitulo");
            RaisePropertyChanged("ImagenPerfil");
            RaisePropertyChanged("ImagenBanner");
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            NoHayNadaMasParaCargarPublicacion = true;
            NoHayNadaMasParaCargarContactos = true;

            if (ViendoContactos)
            {
                ReiniciarContactos();
            }
        }

        public ICommand IrListaEventos
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (Persona != null)
                    {
                        await CoreMethods.PushPageModel<ListaEventosPageModel>(Persona);
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrInformacionPerfil
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (Persona != null)
                    {
                        await CoreMethods.PushPageModel<InformacionPerfilPageModel>(Persona);
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteracturarModal
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    BiografiaTimeLineModel biografiaSeleccionada = parameter as BiografiaTimeLineModel;

                    if (biografiaSeleccionada != null && biografiaSeleccionada.TipoTimeLine == TipoItemTimeLine.Publicacion)
                    {
                        if (biografiaSeleccionada.EsVideo)
                        {
                            PublicacionModalModel publicacion = new PublicacionModalModel
                            {
                                CodigoArchivo = biografiaSeleccionada.CodigoArchivo,
                                UrlArchivo = biografiaSeleccionada.UrlArchivo,
                                TipoArchivoPublicacion = TipoArchivo.Video
                            };

                            await CoreMethods.PushPageModel<PublicacionModalPageModel>(publicacion, true);
                        }
                        else if (biografiaSeleccionada.EsImagen)
                        {
                            PublicacionModel publicacionParaVer = new PublicacionModel
                            {
                                CodigoPublicacion = biografiaSeleccionada.CodigoPublicacion,
                                TipoPerfil = TipoPerfil.Grupo,
                            };

                            await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionParaVer);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (ViendoPublicaciones)
                        {
                            IsRefreshing = true;
                            await CargarItemsEnTimeLineSegunPerfil(0, 20, true);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    IsRefreshing = false;
                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreTimeLine
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (ViendoPublicaciones)
                        {
                            await CargarItemsEnTimeLineSegunPerfil(TimeLine.Count, 5);
                        }
                        else
                        {
                            await CargarContactos(NumeroContactosBuscados, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrConfiguracion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersona)
                    {
                        await CoreMethods.PushPageModel<ConfiguracionPageModel>();
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrPersona
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    PersonasDTO persona = parameter as PersonasDTO;
                    persona.IdiomaDeLaPersona = App.IdiomaPersona;

                    await CoreMethods.PushPageModel<PerfilPageModel>(parameter);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand ToogleContacto
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (!EsMiPersona)
                    {
                        try
                        {
                            ContactosDTO contacto = new ContactosDTO
                            {
                                CodigoPersonaOwner = App.Persona.Consecutivo,
                                CodigoPersonaContacto = Persona.Consecutivo
                            };

                            if (EstaAgregadoContacto)
                            {
                                contacto.Consecutivo = Persona.ConsecutivoContacto;
                                EstaAgregadoContacto = false;

                                if (IsNotConnected)
                                {
                                    EstaAgregadoContacto = true;
                                    tcs.SetResult(true);
                                    return;
                                }
                                await _chatServices.EliminarContacto(contacto);
                            }
                            else
                            {
                                EstaAgregadoContacto = true;
                                if (IsNotConnected)
                                {
                                    EstaAgregadoContacto = false;
                                    tcs.SetResult(true);
                                    return;
                                }
                                await _chatServices.CrearContacto(contacto);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand CerrarSesion
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    App.RecordedPerson = string.Empty;
                    App.RecordedIdiomPerson = string.Empty;
                    App.RecordedUser = string.Empty;
                    App.RecordedPasswordUser = string.Empty;

                    App.Persona = null;
                    App.Usuario = null;

                    App.DisposeChatEvents();
                    ChatsServices.DisposeChatHub();

                    var currentPage = App.Current.MainPage;
                    FreshNavigationContainer basicNavContainer = App.ConfigureNavigationContainer();
                    CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.AuthenticationContainer);
                    currentPage = null;

                    GC.Collect(2, GCCollectionMode.Forced);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarContacto
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersona && !IsNotConnected)
                    {
                        ContactosDTO contacto = parameter as ContactosDTO;

                        BiografiaTimeLineModel contactoPrimeroParaBorrar = _timeLine.Where(x => x.PrimerContacto != null && x.PrimerContacto.Consecutivo == contacto.Consecutivo).FirstOrDefault();
                        BiografiaTimeLineModel contactoSegundoParaBorrar = _timeLine.Where(x => x.SegundoContacto != null && x.SegundoContacto.Consecutivo == contacto.Consecutivo).FirstOrDefault();

                        if (contactoPrimeroParaBorrar != null)
                        {
                            contactoPrimeroParaBorrar.PrimerContacto = null;

                            if (contactoPrimeroParaBorrar.SegundoContacto == null)
                            {
                                _timeLine.Remove(contactoPrimeroParaBorrar);
                            }
                            else
                            {
                                contactoPrimeroParaBorrar.PrimerContacto = contactoPrimeroParaBorrar.SegundoContacto;
                                contactoPrimeroParaBorrar.SegundoContacto = null;
                            }
                        }
                        else if (contactoSegundoParaBorrar != null)
                        {
                            contactoSegundoParaBorrar.SegundoContacto = null;

                            if (contactoSegundoParaBorrar.PrimerContacto == null)
                            {
                                _timeLine.Remove(contactoSegundoParaBorrar);
                            }
                        }

                        RaisePropertyChanged("TimeLine");

                        try
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            await _chatServices.EliminarContacto(contacto);

                            if (OnPersonaBorrada != null)
                            {
                                OnPersonaBorrada(this, new PersonaBorradaArgs(contacto.CodigoPersonaContacto));
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrInteractuarPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    BiografiaTimeLineModel biografiaSeleccionada = parameter as BiografiaTimeLineModel;

                    int codigoPerfil = 0;
                    TipoPerfil tipoPerfil = TipoPerfil.SinTipoPerfil;

                    if (EsCandidato)
                    {
                        codigoPerfil = Persona.CandidatoDeLaPersona.Consecutivo;
                        tipoPerfil = TipoPerfil.Candidato;

                        if (App.Usuario.PlanesUsuarios.Planes.VideosPerfil != 1)
                        {
                            await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.VideosPerfil));
                            tcs.SetResult(true);
                            return;
                        }
                    }
                    else if (EsGrupo)
                    {
                        codigoPerfil = Persona.GrupoDeLaPersona.Consecutivo;
                        tipoPerfil = TipoPerfil.Grupo;
                    }

                    PublicacionModel publicacionModel = new PublicacionModel
                    {
                        CodigoPerfil = codigoPerfil,
                        TipoPerfil = tipoPerfil,
                        PersonaDeLaPublicacion = Persona
                    };

                    if (biografiaSeleccionada != null)
                    {
                        publicacionModel.CodigoArchivo = biografiaSeleccionada.CodigoArchivo;
                        publicacionModel.UrlArchivo = biografiaSeleccionada.UrlArchivo;
                        publicacionModel.CodigoPublicacion = biografiaSeleccionada.CodigoPublicacion;
                        publicacionModel.TipoArchivoPublicacion = biografiaSeleccionada.TipoArchivoTimeLine;
                        publicacionModel.Titulo = biografiaSeleccionada.Titulo;
                        publicacionModel.Descripcion = biografiaSeleccionada.Descripcion;
                        publicacionModel.Creacion = biografiaSeleccionada.DateTimeCreacion;
                    }

                    await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionModel);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteractuarFotoPerfil
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersona)
                    {
                        try
                        {
                            string actionTaken = await CoreMethods.DisplayActionSheet(string.Empty, SportsGoResources.Cancelar, DeleteActionSheetOptionPerfil, SportsGoResources.TomarFoto, SportsGoResources.ElegirFoto);
                            MediaFile file = null;

                            if (actionTaken == SportsGoResources.ElegirFoto)
                            {
                                file = await PickPhotoAsync();
                            }
                            else if (actionTaken == SportsGoResources.TomarFoto)
                            {
                                file = await TakePhotoAsync();
                            }
                            else if (actionTaken == SportsGoResources.BorrarFoto)
                            {
                                if (Persona.CodigoArchivoImagenPerfil.HasValue && Persona.CodigoArchivoImagenPerfil > 0)
                                {
                                    PersonasDTO personaImagenPerfil = new PersonasDTO
                                    {
                                        Consecutivo = Persona.Consecutivo,
                                        CodigoArchivoImagenPerfil = Persona.CodigoArchivoImagenPerfil
                                    };

                                    try
                                    {
                                        if (IsNotConnected)
                                        {
                                            tcs.SetResult(true);
                                            return;
                                        }
                                        WrapperSimpleTypesDTO wrapper = await _personaServices.EliminarImagenPerfil(personaImagenPerfil);
                                        if (wrapper != null && wrapper.Exitoso)
                                        {
                                            await CachedImage.InvalidateCache(Persona.UrlImagenPerfil, CacheType.All, true);
                                            Persona.CodigoArchivoImagenPerfil = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorEliminarImagenes, "OK");
                                        tcs.SetResult(true);
                                        return;
                                    }
                                }

                                ImagenPerfil = string.Empty;
                            }

                            if (file != null)
                            {
                                ImagenEditorModel imagenModel = new ImagenEditorModel
                                {
                                    Source = file.Path,
                                    Persona = Persona,
                                    EsImagenPerfil = true
                                };

                                await CoreMethods.PushPageModel<EditorImagePageModel>(imagenModel);

                                file.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteractuarFotoBanner
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersona)
                    {
                        try
                        {
                            string actionTaken = await CoreMethods.DisplayActionSheet(string.Empty, SportsGoResources.Cancelar, DeleteActionSheetOptionBanner, SportsGoResources.TomarFoto, SportsGoResources.ElegirFoto);
                            MediaFile file = null;

                            if (actionTaken == SportsGoResources.ElegirFoto)
                            {
                                file = await PickPhotoAsync();
                            }
                            else if (actionTaken == SportsGoResources.TomarFoto)
                            {
                                file = await TakePhotoAsync();
                            }
                            else if (actionTaken == SportsGoResources.BorrarFoto)
                            {
                                if (Persona.CodigoArchivoImagenBanner.HasValue && Persona.CodigoArchivoImagenBanner > 0)
                                {
                                    PersonasDTO personaImagenBanner = new PersonasDTO
                                    {
                                        Consecutivo = Persona.Consecutivo,
                                        CodigoArchivoImagenBanner = Persona.CodigoArchivoImagenBanner
                                    };

                                    try
                                    {
                                        if (IsNotConnected)
                                        {
                                            tcs.SetResult(true);
                                            return;
                                        }
                                        WrapperSimpleTypesDTO wrapper = await _personaServices.EliminarImagenBanner(personaImagenBanner);
                                        if (wrapper != null && wrapper.Exitoso)
                                        {
                                            await CachedImage.InvalidateCache(Persona.UrlImagenBanner, CacheType.All, true);
                                            Persona.CodigoArchivoImagenBanner = null;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorEliminarImagenes, "OK");
                                        tcs.SetResult(true);
                                        return;
                                    }
                                }

                                ImagenBanner = string.Empty;
                            }

                            if (file != null)
                            {
                                ImagenEditorModel imagenModel = new ImagenEditorModel
                                {
                                    Source = file.Path,
                                    Persona = Persona,
                                    EsImagenBanner = true
                                };

                                await CoreMethods.PushPageModel<EditorImagePageModel>(imagenModel);

                                file.Dispose();
                            }
                        }
                        catch (Exception)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task AsignarImagenPerfil(Stream streamPerfil)
        {
            if (streamPerfil != null)
            {
                int codigoArchivo = Persona.CodigoArchivoImagenPerfil.HasValue ? Persona.CodigoArchivoImagenPerfil.Value : 0;

                if (IsNotConnected) return;
                WrapperSimpleTypesDTO wrapper = await _archivoServices.AsignarImagenPerfilPersona(Persona.Consecutivo, codigoArchivo, streamPerfil);

                if (wrapper != null && wrapper.Exitoso)
                {
                    await CachedImage.InvalidateCache(Persona.UrlImagenPerfil, CacheType.All, true);
                    Persona.CodigoArchivoImagenPerfil = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                    ImagenPerfil = Persona.UrlImagenPerfil;
                }
            }
        }

        async Task AsignarImagenBanner(Stream streamBanner)
        {
            if (streamBanner != null)
            {
                int codigoArchivo = Persona.CodigoArchivoImagenBanner.HasValue ? Persona.CodigoArchivoImagenBanner.Value : 0;

                if (IsNotConnected) return;
                WrapperSimpleTypesDTO wrapper = await _archivoServices.AsignarImagenBannerPersona(Persona.Consecutivo, codigoArchivo, streamBanner);

                if (wrapper != null && wrapper.Exitoso)
                {
                    await CachedImage.InvalidateCache(ImagenBanner, CacheType.All, true);
                    Persona.CodigoArchivoImagenBanner = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                    ImagenBanner = Persona.UrlImagenBanner;
                }
            }
        }

        async Task VerificarSiPersonaEstaAgregada()
        {
            ContactosDTO contacto = new ContactosDTO
            {
                CodigoPersonaOwner = App.Persona.Consecutivo,
                CodigoPersonaContacto = Persona.Consecutivo
            };

            if (IsNotConnected) return;
            ContactosDTO contactoBuscado = await _chatServices.VerificarSiLaPersonaEstaAgregadaContactos(contacto);

            EstaAgregadoContacto = contactoBuscado != null && contactoBuscado.Consecutivo > 0;
        }

        void ReiniciarContactos()
        {
            // Reinicio los contactos
            NumeroContactosBuscados = 0;

            if (_timeLine != null && _timeLine.Count > 0)
            {
                List<BiografiaTimeLineModel> listaParaBorrar = _timeLine?.Where(x => x.TipoTimeLine == TipoItemTimeLine.Contacto).ToList();
                _timeLine?.RemoveRange(listaParaBorrar);
            }
        }

        public static void DispararEventoPersonaBorrada(object sender, int codigoPersonaBorrada)
        {
            if (OnPersonaBorrada != null)
            {
                OnPersonaBorrada(sender, new PersonaBorradaArgs(codigoPersonaBorrada));
            }
        }

        async Task CargarItemsEnTimeLineSegunPerfil(int skipIndex, int takeIndex, bool isRefresh = false)
        {
            if (Persona != null && !NoHayNadaMasParaCargarPublicacion)
            {
                List<BiografiaTimeLineModel> listaTimeLine = new List<BiografiaTimeLineModel>();

                if (Persona.TipoPerfil == TipoPerfil.Candidato && Persona.CandidatoDeLaPersona != null)
                {
                    CandidatosVideosDTO candidatoVideo = new CandidatosVideosDTO
                    {
                        CodigoCandidato = Persona.CandidatoDeLaPersona.Consecutivo,
                        SkipIndexBase = skipIndex,
                        TakeIndexBase = takeIndex,
                        ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                    };

                    if (isRefresh && LastRefresh != DateTime.MinValue)
                    {
                        candidatoVideo.FechaFiltroBase = LastRefresh;
                        LastRefresh = DateTime.Now;
                    }

                    if (IsNotConnected) return;
                    listaTimeLine = BiografiaTimeLineModel.CrearListaBiografiaTimeLine(await _candidatosServices.ListarCandidatosVideosDeUnCandidato(candidatoVideo));
                }
                else if (Persona.TipoPerfil == TipoPerfil.Grupo && Persona.GrupoDeLaPersona != null)
                {
                    BuscadorDTO grupoEvento = new BuscadorDTO
                    {
                        ConsecutivoPerfil = Persona.GrupoDeLaPersona.Consecutivo,
                        SkipIndexBase = skipIndex,
                        TakeIndexBase = takeIndex,
                        ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                    };

                    if (isRefresh && LastRefresh != DateTime.MinValue)
                    {
                        grupoEvento.FechaFiltroBase = LastRefresh;
                        LastRefresh = DateTime.Now;
                    }

                    if (IsNotConnected) return;
                    listaTimeLine = BiografiaTimeLineModel.CrearListaBiografiaTimeLine(await _gruposServices.ListarEventosDeUnGrupo(grupoEvento));
                }

                if (listaTimeLine != null)
                {
                    if (listaTimeLine.Count > 0)
                    {
                        if (_timeLine == null)
                        {
                            _timeLine = new ObservableRangeCollection<BiografiaTimeLineModel>(listaTimeLine);
                        }
                        else
                        {
                            // Filtro para evitar tener publicaciones repetidas
                            listaTimeLine = listaTimeLine.Where(x => !_timeLine.Any(y => y.CodigoPublicacion == x.CodigoPublicacion && y.TipoTimeLine == TipoItemTimeLine.Publicacion)).ToList();

                            if (isRefresh)
                            {
                                // Reverso la lista para mantener el orden
                                listaTimeLine.Reverse();

                                foreach (var timeLine in listaTimeLine)
                                {
                                    _timeLine.Insert(0, timeLine);
                                }
                            }
                            else
                            {
                                _timeLine.AddRange(listaTimeLine);
                            }
                        }

                        RaisePropertyChanged(nameof(TimeLine));
                    }
                    else
                    {
                        NoHayNadaMasParaCargarPublicacion = listaTimeLine.Count <= 0 && !isRefresh;
                    }
                }
            }
        }

        async Task CargarContactos(int skipIndex, int takeIndex, bool isRefresh = false)
        {
            if (Persona != null && !NoHayNadaMasParaCargarContactos)
            {
                List<BiografiaTimeLineModel> listaTimeLine = new List<BiografiaTimeLineModel>();

                ContactosDTO contacto = new ContactosDTO
                {
                    CodigoPersonaOwner = Persona.Consecutivo,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex
                };

                if (IsNotConnected) return;
                List<ContactosDTO> listaContactos = await _chatServices.ListarContactos(contacto);

                listaTimeLine = BiografiaTimeLineModel.CrearListaBiografiaTimeLine(listaContactos);

                if (listaTimeLine != null)
                {
                    if (listaTimeLine.Count > 0)
                    {
                        if (_timeLine == null)
                        {
                            _timeLine = new ObservableRangeCollection<BiografiaTimeLineModel>(listaTimeLine);
                        }
                        else
                        {
                            _timeLine.AddRange(listaTimeLine);
                        }

                        NumeroContactosBuscados = listaContactos.Count;
                        RaisePropertyChanged(nameof(TimeLine));
                    }
                    else
                    {
                        NoHayNadaMasParaCargarContactos = listaTimeLine.Count <= 0;
                    }
                }
            }
        }
    }
}