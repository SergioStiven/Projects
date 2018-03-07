using FFImageLoading;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xpinn.SportsGo.Movil.Controls;
using System.Windows.Input;
using Acr.UserDialogs;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using System.Threading;
using FreshMvvm;
using PCLStorage;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class InformacionPerfilPageModel : BasePageModel
    {
        ArchivosServices _archivoServices;
        PersonasServices _personaServices;
        PlanesServices _planesServices;

        bool _personaRecordandose;

        public PersonasDTO Persona { get; set; }
        public ObservableCollection<CategoriasModel> Categorias { get; set; }

        public string NombreTitulo
        {
            get
            {
                string nombreTitulo = string.Empty;

                if (!string.IsNullOrWhiteSpace(Persona.Nombres))
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
                return App.Persona != null && Persona.Consecutivo == App.Persona.Consecutivo;
            }
        }

        public bool EsPrimerRegistro
        {
            get
            {
                return Persona != null && (Persona.Consecutivo <= 0 || App.Persona == null);
            }
        }

        public bool EsCandidato
        {
            get
            {
                return Persona.TipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool EsGrupo
        {
            get
            {
                return Persona.TipoPerfil == TipoPerfil.Grupo;
            }
        }

        public bool EsMiPersonaOPrimerRegistro
        {
            get
            {
                // Solo usado para el .xaml
                return EsPrimerRegistro || EsMiPersona;
            }
        }

        public string TituloPage
        {
            get
            {
                string tituloPage = SportsGoResources.VerPerfil;

                if (EsPrimerRegistro)
                {
                    tituloPage = SportsGoResources.CrearPerfil;
                }
                else if (EsMiPersona)
                {
                    tituloPage = SportsGoResources.EditarPerfil;
                }

                return tituloPage;
            }
        }

        string _imagenPerfil;
        public string ImagenPerfil
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_imagenPerfil))
                {
                    _imagenPerfil = Persona.UrlImagenPerfil;
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
                if (string.IsNullOrWhiteSpace(_imagenBanner))
                {
                    _imagenBanner = Persona.UrlImagenBanner;
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
                string deleteOption = null;

                if (!string.IsNullOrWhiteSpace(ImagenPerfil))
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
                string deleteOption = null;

                if (!string.IsNullOrWhiteSpace(ImagenBanner))
                {
                    deleteOption = SportsGoResources.BorrarFoto;
                }

                return deleteOption;
            }
        }

        public InformacionPerfilPageModel()
        {
            _archivoServices = new ArchivosServices();
            _personaServices = new PersonasServices();
            _planesServices = new PlanesServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);

            Persona = initData as PersonasDTO;
            _personaRecordandose = Persona.PersonaRecordandose;

            CrearListaCategoriaDeLaPersona();

            if (EsPrimerRegistro)
            {
                switch (Persona.TipoPerfil)
                {
                    case TipoPerfil.Candidato:
                        Persona.CandidatoDeLaPersona = new CandidatosDTO();
                        Persona.CandidatoDeLaPersona.CandidatosResponsables = new CandidatosResponsablesDTO();
                        break;
                    case TipoPerfil.Grupo:
                        Persona.GrupoDeLaPersona = new GruposDTO();
                        break;
                    case TipoPerfil.Representante:
                        Persona.RepresentanteDeLaPersona = new RepresentantesDTO();
                        break;
                }
            }
        }

        public override void ReverseInit(object returnData)
        {
            base.ReverseInit(returnData);

            if (returnData is PersonasDTO)
            {
                Persona = returnData as PersonasDTO;
                RaisePropertyChanged("NombreTitulo");

                CrearListaCategoriaDeLaPersona();
            }
            else if (returnData is ImagenEditorModel)
            {
                ImagenEditorModel imagenModel = returnData as ImagenEditorModel;

                if (imagenModel.EsImagenPerfil)
                {
                    ImagenPerfil = imagenModel.Source;

                    if (!imagenModel.EsPrimerRegistro)
                    {
                        Persona.CodigoArchivoImagenPerfil = imagenModel.CodigoArchivoCreado;
                        App.Persona.CodigoArchivoImagenPerfil = imagenModel.CodigoArchivoCreado;
                    }
                }
                else
                {
                    ImagenBanner = imagenModel.Source;

                    if (!imagenModel.EsPrimerRegistro)
                    {
                        Persona.CodigoArchivoImagenBanner = imagenModel.CodigoArchivoCreado;
                        App.Persona.CodigoArchivoImagenBanner = imagenModel.CodigoArchivoCreado;
                    }
                }
            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);
        }

        public ICommand InteractuarFotoPerfil
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersona || EsPrimerRegistro)
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
                                    EsPrimerRegistro = EsPrimerRegistro,
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
                    if (EsMiPersona || EsPrimerRegistro)
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
                                    EsPrimerRegistro = EsPrimerRegistro,
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

        public ICommand IrPersona
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    ControlPerfil control = new ControlPerfil
                    {
                        TipoPerfilControl = Persona.TipoPerfil,
                        PersonaParaVer = Persona
                    };

                    if (!EsMiPersona && !EsPrimerRegistro && EsCandidato && App.Usuario.PlanesUsuarios.Planes.DetalleCandidatos != 1)
                    {
                        await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.DetalleCandidatos));
                    }
                    else if (!EsMiPersona && !EsPrimerRegistro && EsGrupo && App.Usuario.PlanesUsuarios.Planes.DetalleGrupos != 1)
                    {
                        await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.DetalleGrupos));
                    }
                    else
                    {
                        await CoreMethods.PushPageModel<PersonaPageModel>(control);
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand IrCategoria
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        CategoriasModel categoriaSeleccionada = parameter as CategoriasModel;

                        if (EsMiPersona || EsPrimerRegistro || Persona.TipoPerfil == TipoPerfil.Candidato)
                        {
                            ControlPerfil control = new ControlPerfil
                            {
                                TipoPerfilControl = Persona.TipoPerfil,
                                CategoriaSeleccionada = categoriaSeleccionada,
                                PersonaParaVer = Persona,
                                CategoriasQueYaEstanAgregadas = CategoriasDelPerfil(Persona)
                            };

                            if (control.CategoriaSeleccionada.EsNuevaCategoria)
                            {
                                if (EsPrimerRegistro && Persona.Usuarios.PlanesUsuarios.Planes.NumeroCategoriasPermisibles <= control.CategoriasQueYaEstanAgregadas.Count())
                                {
                                    await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.MultiplesCategorias, true));
                                    tcs.SetResult(true);
                                    return;
                                }
                                else if (!EsPrimerRegistro && EsMiPersona)
                                {
                                    PlanesUsuariosDTO planUsuario = new PlanesUsuariosDTO
                                    {
                                        Consecutivo = App.Usuario.PlanesUsuarios.Consecutivo,
                                        TipoOperacionBase = TipoOperacion.MultiplesCategorias
                                    };

                                    if (IsNotConnected)
                                    {
                                        tcs.SetResult(true);
                                        return;
                                    }
                                    WrapperSimpleTypesDTO wrapper = await _planesServices.VerificarSiPlanSoportaLaOperacion(planUsuario);

                                    if (wrapper == null || !wrapper.EsPosible)
                                    {
                                        await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.MultiplesCategorias));
                                        tcs.SetResult(true);
                                        return;
                                    }
                                }
                            }

                            await CoreMethods.PushPageModel<CategoriaPageModel>(control);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand CrearPerfil
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsPrimerRegistro && await ValidarDatosPersona())
                    {
                        var config = new ProgressDialogConfig()
                                       .SetTitle(SportsGoResources.Cargando)
                                       .SetIsDeterministic(false);

                        if (Device.RuntimePlatform == Device.iOS)
                        {
                            config.SetMaskType(MaskType.Black);
                        }
                        else
                        {
                            config.SetMaskType(MaskType.Gradient);
                        }

                        using (Dialogs.Progress(config))
                        {
                            // Creamos el usuario y si explota lo notificamos y salimos
                            WrapperSimpleTypesDTO wrapper = null;
                            try
                            {
                                switch (Persona.TipoPerfil)
                                {
                                    case TipoPerfil.Candidato:
                                        if (await ValidarDatosCandidatos())
                                        {
                                            wrapper = await CrearCandidato();
                                        }
                                        break;
                                    case TipoPerfil.Grupo:
                                        wrapper = await CrearGrupo();
                                        break;
                                    case TipoPerfil.Representante:
                                        if (await ValidarDatosRepresentantes())
                                        {
                                            wrapper = await CrearRepresentante();
                                        }
                                        break;
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorPrimerRegistro, "OK");
                                tcs.SetResult(true);
                                return;
                            }

                            try
                            {
                                // Si se pudo crear satisfactoriamente, realizo el primer logeo
                                // Si no se pudo crear muestro error de primer registro
                                // Si falla mientras se esta haciendo el primer logeo, muestro el error de primer logeo y no de primer registro
                                if (wrapper != null && wrapper.Exitoso && wrapper.ConsecutivoPersonaCreado > 0 && wrapper.ConsecutivoCreado > 0 && wrapper.ConsecutivoUsuarioCreado > 0)
                                {
                                    try
                                    {
                                        Persona.Consecutivo = wrapper.ConsecutivoPersonaCreado;

                                        if (!string.IsNullOrWhiteSpace(ImagenBanner))
                                        {
                                            IFile file = await FileSystem.Current.GetFileFromPathAsync(ImagenBanner);
                                            using (Stream streamSource = await file.OpenAsync(FileAccess.Read))
                                            {
                                                await AsignarImagenBanner(streamSource);
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(ImagenPerfil))
                                        {
                                            IFile file = await FileSystem.Current.GetFileFromPathAsync(ImagenPerfil);
                                            using (Stream streamSource = await file.OpenAsync(FileAccess.Read))
                                            {
                                                await AsignarImagenBanner(streamSource);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorAsignarImagenes, "OK");
                                    }

                                    await CoreMethods.DisplayAlert(SportsGoResources.Notificacion, SportsGoResources.ProcedeValidarCorreo, "OK");

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

                                    // Usado para iniciar sesion directo
                                    // Se pidio que ahora primero se confirmara el correo
                                    //AuthenticateServices authenticateServices = new AuthenticateServices();

                                    //UsuariosDTO usuario = new UsuariosDTO
                                    //{
                                    //    Usuario = Persona.Usuarios.Usuario,
                                    //    Clave = Persona.Usuarios.Clave
                                    //};

                                    //App.Usuario = await authenticateServices.VerificarUsuario(usuario);

                                    //PersonasServices personaServices = new PersonasServices();
                                    //PersonasDTO persona = new PersonasDTO
                                    //{
                                    //    Consecutivo = wrapper.ConsecutivoPersonaCreado,
                                    //    IdiomaDeLaPersona = Persona.IdiomaDeLaPersona
                                    //};

                                    //App.Persona = await personaServices.BuscarPersona(persona);
                                    //App.Persona.Usuarios = App.Usuario;
                                    //App.IdiomaPersona = App.Persona.IdiomaDeLaPersona;

                                    //if (_personaRecordandose)
                                    //{
                                    //    App.RecordedPerson = App.Persona.Consecutivo.ToString();
                                    //    App.RecordedIdiomPerson = App.Persona.CodigoIdioma.ToString();
                                    //    App.RecordedUser = App.Usuario.Usuario;
                                    //    App.RecordedPasswordUser = App.Usuario.Clave;
                                    //    App.Persona.PersonaRecordandose = true;
                                    //}

                                    //BadgeColorTabbedNavigationContainer tabbedPage = App.ConfigureTabbedNavigationContainer(App.Persona);
                                    //CoreMethods.SwitchOutRootNavigation(NavigationContainerNames.MainTabbedContainer);
                                    //tabbedPage.CurrentPage = tabbedPage.Children[2];
                                    //await App.ConnectPersonToChatHub();
                                }
                                else
                                {
                                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorPrimerRegistro, "OK");
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorPrimerLogeo, "OK");
                            }
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<bool> ValidarDatosPersona()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Persona.Nombres) || string.IsNullOrWhiteSpace(Persona.Telefono) || string.IsNullOrWhiteSpace(Persona.CiudadResidencia)
                || Persona.CodigoIdioma <= 0 || Persona.CodigoPais <= 0 || string.IsNullOrWhiteSpace(Persona.Usuarios.Email))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPersona, "OK");
                esValido = false;
            }
            else if (Persona.TipoPerfil != TipoPerfil.Grupo && string.IsNullOrWhiteSpace(Persona.Apellidos))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPersona, "OK");
                esValido = false;
            }

            return esValido;
        }

        async Task<bool> ValidarDatosCandidatos()
        {
            bool esValido = true;

            if (Persona.CandidatoDeLaPersona.Estatura <= 0 || Persona.CandidatoDeLaPersona.Peso <= 0 || Persona.CandidatoDeLaPersona.FechaNacimiento == DateTime.MinValue
                || Persona.CandidatoDeLaPersona.TipoGenero == TipoGeneros.SinGenero)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPerfil, "OK");
                esValido = false;
            }
            else if (Persona.CandidatoDeLaPersona.CategoriasCandidatos.Count <= 0)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosDeporte, "OK");
                esValido = false;
            }

            return esValido;
        }

        async Task<bool> ValidarDatosRepresentantes()
        {
            bool esValido = true;

            if (string.IsNullOrWhiteSpace(Persona.RepresentanteDeLaPersona.NumeroIdentificacion))
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPerfil, "OK");
                esValido = false;
            }

            return esValido;
        }

        void CrearListaCategoriaDeLaPersona()
        {
            List<CategoriasModel> listaCategorias = CategoriasModel.CrearListaCategoriasDeUnaPersona(Persona);

            if (EsMiPersona || EsPrimerRegistro)
            {
                listaCategorias.Insert(0, new CategoriasModel());
            }

            Categorias = new ObservableCollection<CategoriasModel>(listaCategorias);
        }

        async Task<WrapperSimpleTypesDTO> CrearCandidato()
        {
            CandidatosDTO candidatoParaCrear = Persona.CandidatoDeLaPersona;
            candidatoParaCrear.Personas = Persona;

            CandidatosServices candidatoServices = new CandidatosServices();

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await candidatoServices.CrearCandidato(candidatoParaCrear);
            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> CrearGrupo()
        {
            GruposDTO grupoParaCrear = Persona.GrupoDeLaPersona;
            grupoParaCrear.Personas = Persona;

            GruposServices gruposServices = new GruposServices();

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await gruposServices.CrearGrupo(grupoParaCrear);
            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> CrearRepresentante()
        {
            RepresentantesDTO representanteParaCrear = Persona.RepresentanteDeLaPersona;
            representanteParaCrear.Personas = Persona;

            RepresentantesServices representanteServices = new RepresentantesServices();

            if (IsNotConnected) return null;
            WrapperSimpleTypesDTO wrapper = await representanteServices.CrearRepresentante(representanteParaCrear);
            return wrapper;
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
                    await CachedImage.InvalidateCache(Persona.UrlImagenBanner, CacheType.All, true);
                    Persona.CodigoArchivoImagenBanner = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                    ImagenBanner = Persona.UrlImagenBanner;
                }
            }
        }
    }
}