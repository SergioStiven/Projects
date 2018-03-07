using Acr.UserDialogs;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FreshMvvm;
using PCLStorage;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PublicacionPageModel : BasePageModel
    {
        WrapperSimpleTypesDTO _wrapperArchivo;
        bool _archivoModificado;

        CandidatosServices _candidatoService;
        GruposServices _gruposService;
        ArchivosServices _archivosService;
        AdministracionServices _administracionService;
        CategoriasServices _categoriasService;
        IDateTimeHelper _dateTimeHelper;

        public PublicacionModel PublicacionSeleccionada { get; set; }
        public ObservableCollection<PaisesDTO> Paises { get; set; }
        public ObservableCollection<CategoriasDTO> Categorias { get; set; }
        public DateTime FechaMinimaPermitida { get { return DateTime.Now; } }
        public bool PersonaAsiste { get; set; }
        public int NumeroAsistentes { get; set; }

        CategoriasDTO _categoriaSeleccionada;
        public CategoriasDTO CategoriaSeleccionada
        {
            get
            {
                CategoriasDTO categoriaSeleccioanda = null;

                if (EsGrupo)
                {
                    if (_categoriaSeleccionada != null)
                    {
                        categoriaSeleccioanda = _categoriaSeleccionada;
                    }
                    else if (PublicacionSeleccionada != null && PublicacionSeleccionada.CategoriaDelEvento != null && Categorias != null)
                    {
                        categoriaSeleccioanda = Categorias.Where(x => x.Consecutivo == PublicacionSeleccionada.CategoriaDelEvento.Categorias.Consecutivo).FirstOrDefault();
                    }
                }

                return categoriaSeleccioanda;
            }
            set
            {
                _categoriaSeleccionada = value;

                if (value != null && PublicacionSeleccionada.CategoriaDelEvento != null && PublicacionSeleccionada.CategoriaDelEvento.Categorias != null)
                {
                    PublicacionSeleccionada.CategoriaDelEvento.Categorias = value;
                }
            }
        }

        PaisesDTO _paisSeleccionado;
        public PaisesDTO PaisSeleccionado
        {
            get
            {
                PaisesDTO paisSeleccionado = null;

                if (EsGrupo)
                {
                    if (_paisSeleccionado != null)
                    {
                        paisSeleccionado = _paisSeleccionado;
                    }
                    else if (PublicacionSeleccionada != null && PublicacionSeleccionada.PaisDelEvento != null && Paises != null)
                    {
                        paisSeleccionado = Paises.Where(x => x.Consecutivo == PublicacionSeleccionada.PaisDelEvento.Consecutivo).FirstOrDefault();
                    }
                }

                return paisSeleccionado;
            }
            set
            {
                if (value != null)
                {
                    _paisSeleccionado = value;

                    PublicacionSeleccionada.PaisDelEvento = _paisSeleccionado;
                }
            }
        }

        public TimeSpan HoraInicio
        {
            get
            {
                return PublicacionSeleccionada.FechaInicio.TimeOfDay;
            }
        }

        public TimeSpan HoraFinal
        {
            get
            {
                return PublicacionSeleccionada.FechaInicio.TimeOfDay;
            }
        }

        public string HoraInicioString
        {
            get
            {
                DateTime horaInicio = Convert.ToDateTime(HoraInicio.ToString());

                return horaInicio.ToString("hh:mm tt");
            }
        }

        public string HoraFinalString
        {
            get
            {
                DateTime horaFinal = Convert.ToDateTime(HoraFinal.ToString());

                return horaFinal.ToString("hh:mm tt");
            }
        }


        public string ImagenPais
        {
            get
            {
                string imagenPais = string.Empty;

                if (PublicacionSeleccionada.PaisDelEvento != null)
                {
                    imagenPais = PublicacionSeleccionada.PaisDelEvento.UrlArchivo;
                }

                return imagenPais;
            }
        }

        public bool EsCandidato
        {
            get
            {
                return PublicacionSeleccionada != null && PublicacionSeleccionada.TipoPerfil == TipoPerfil.Candidato;
            }
        }

        public bool EsGrupo
        {
            get
            {
                return PublicacionSeleccionada != null && PublicacionSeleccionada.TipoPerfil == TipoPerfil.Grupo;
            }
        }

        public bool EsRegistroPublicacion
        {
            get
            {
                return PublicacionSeleccionada == null || PublicacionSeleccionada.CodigoPublicacion <= 0;
            }
        }

        public bool EsMiPersonaYNoRegistro
        {
            get
            {
                return PublicacionSeleccionada.EsMiPersona && !EsRegistroPublicacion;
            }
        }

        public bool NoEsRegistroYEsEvento
        {
            get
            {
                return !EsRegistroPublicacion && EsGrupo;
            }
        }

        public bool NoEsMiPersonaYNoRegistroYEsEvento
        {
            get
            {
                return NoEsRegistroYEsEvento && !PublicacionSeleccionada.EsMiPersona;
            }
        }

        public bool EsRegistroPublicacionOPublicacionConArchivo
        {
            get
            {
                return EsRegistroPublicacion || PublicacionSeleccionada.CodigoArchivo.HasValue;
            }
        }

        public bool PuedeCambiarArchivo
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UrlArchivo) && PublicacionSeleccionada.EsMiPersona;
            }
        }

        public bool SeVeEspacioBoton
        {
            get
            {
                return PuedeCambiarArchivo || NoEsMiPersonaYNoRegistroYEsEvento;
            }
        }

        public string MensajeMaximoVideo
        {
            get
            {
                if (App.Persona.IdiomaDeLaPersona != Idioma.Ingles)
                {
                    return SportsGoResources.TiempoMaximoVideoPermitido + " " + App.Usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo + " seg";
                }
                else
                {
                    return SportsGoResources.TiempoMaximoVideoPermitido + " " + App.Usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo + " sec";
                }
            }
        }

        IdiomaModel _idiomaSeleccionado;
        public IdiomaModel IdiomaSeleccionado
        {
            get
            {
                IdiomaModel idioma = _idiomaSeleccionado;

                if (PublicacionSeleccionada != null)
                {
                    idioma = PublicacionSeleccionada.IdiomaDelEvento;
                }

                if (idioma == null)
                {
                    idioma = new IdiomaModel(Idioma.SinIdioma);
                }

                return App.ListaIdioma.Where(x => x.Idioma == idioma.Idioma).FirstOrDefault();
            }
            set
            {
                _idiomaSeleccionado = value;

                if (PublicacionSeleccionada != null)
                {
                    PublicacionSeleccionada.IdiomaDelEvento = value;
                }
            }
        }

        string _urlArchivo;
        public string UrlArchivo
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_urlArchivo))
                {
                    _urlArchivo = PublicacionSeleccionada.UrlArchivo;
                }

                return _urlArchivo;
            }
            set
            {
                _urlArchivo = value;
                PublicacionSeleccionada.UrlArchivo = _urlArchivo;
            }
        }

        public string UrlImagenPerfil
        {
            get
            {
                string urlImagenPerfil = string.Empty;

                if (PublicacionSeleccionada != null && PublicacionSeleccionada.PersonaDeLaPublicacion != null && !string.IsNullOrWhiteSpace(PublicacionSeleccionada.PersonaDeLaPublicacion.UrlImagenPerfil))
                {
                    urlImagenPerfil = PublicacionSeleccionada.PersonaDeLaPublicacion.UrlImagenPerfil;
                }

                return urlImagenPerfil;
            }
        }

        public string TituloPage
        {
            get
            {
                string tituloPage = string.Empty;

                if (EsRegistroPublicacion)
                {
                    tituloPage = SportsGoResources.CrearPublicacion;
                }
                else if (EsMiPersonaYNoRegistro)
                {
                    tituloPage = SportsGoResources.EditarPublicacion;
                }
                else
                {
                    tituloPage = SportsGoResources.Publicacion;
                }

                return tituloPage;
            }
        }

        string DeleteActionSheetOption
        {
            get
            {
                string deleteOption = null;

                if (!string.IsNullOrWhiteSpace(PublicacionSeleccionada.UrlArchivo) && !EsCandidato)
                {
                    deleteOption = SportsGoResources.Borrar;
                }

                return deleteOption;
            }
        }

        public PublicacionPageModel()
        {
            _candidatoService = new CandidatosServices();
            _gruposService = new GruposServices();
            _archivosService = new ArchivosServices();
            _administracionService = new AdministracionServices();
            _categoriasService = new CategoriasServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            PublicacionSeleccionada = initData as PublicacionModel;

            try
            {
                if (EsGrupo)
                {
                    if (!EsRegistroPublicacion)
                    {
                        GruposEventosDTO grupoEvento = new GruposEventosDTO
                        {
                            Consecutivo = PublicacionSeleccionada.CodigoPublicacion,
                            Idioma = App.IdiomaPersona,
                            ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                        };

                        if (IsNotConnected) return;
                        GruposEventosDTO eventoBuscado = await _gruposService.BuscarGrupoEventoPorConsecutivo(grupoEvento);

                        PublicacionSeleccionada.CodigoPublicacion = eventoBuscado.Consecutivo;
                        PublicacionSeleccionada.Creacion = eventoBuscado.Creacion;
                        PublicacionSeleccionada.CodigoArchivo = eventoBuscado.CodigoArchivo.HasValue ? eventoBuscado.CodigoArchivo.Value : 0;
                        PublicacionSeleccionada.UrlArchivo = eventoBuscado.UrlArchivo;
                        PublicacionSeleccionada.Titulo = eventoBuscado.Titulo;
                        PublicacionSeleccionada.Descripcion = eventoBuscado.Descripcion;
                        PublicacionSeleccionada.CategoriaDelEvento = eventoBuscado.CategoriasEventos.FirstOrDefault();
                        PublicacionSeleccionada.IdiomaDelEvento = new IdiomaModel(eventoBuscado.Idiomas);
                        IdiomaSeleccionado = PublicacionSeleccionada.IdiomaDelEvento;
                        PublicacionSeleccionada.PaisDelEvento = eventoBuscado.Paises;
                        PublicacionSeleccionada.FechaInicio = eventoBuscado.FechaInicio;
                        PublicacionSeleccionada.FechaTerminacion = eventoBuscado.FechaTerminacion;
                        PublicacionSeleccionada.Ubicacion = eventoBuscado.Ubicacion;
                        PublicacionSeleccionada.CodigoPerfil = eventoBuscado.Grupos.Consecutivo;
                        PublicacionSeleccionada.PersonaDeLaPublicacion = eventoBuscado.Grupos.Personas;
                        PublicacionSeleccionada.TipoArchivoPublicacion = eventoBuscado.TipoArchivo;
                        PublicacionSeleccionada.TipoPerfil = TipoPerfil.Grupo;

                        UrlArchivo = PublicacionSeleccionada.UrlArchivo;
                        NumeroAsistentes = eventoBuscado.NumeroEventosAsistentes;

                        GruposEventosAsistentesDTO grupoEventoAsistente = new GruposEventosAsistentesDTO
                        {
                            CodigoEvento = PublicacionSeleccionada.CodigoPublicacion,
                            CodigoPersona = App.Persona.Consecutivo
                        };

                        if (IsNotConnected) return;
                        WrapperSimpleTypesDTO wrapperAsistente = await _gruposService.BuscarSiPersonaAsisteAGrupoEvento(grupoEventoAsistente);

                        if (wrapperAsistente != null)
                        {
                            PersonaAsiste = wrapperAsistente.Existe;
                        }
                    }
                    else
                    {
                        PublicacionSeleccionada.FechaInicio = FechaMinimaPermitida;
                        PublicacionSeleccionada.FechaTerminacion = FechaMinimaPermitida;
                    }

                    if (PublicacionSeleccionada.EsMiPersona)
                    {
                        PaisesDTO paises = new PaisesDTO
                        {
                            IdiomaBase = App.IdiomaPersona
                        };

                        if (IsNotConnected) return;
                        Paises = new ObservableCollection<PaisesDTO>(await _administracionService.ListarPaisesPorIdioma(paises) ?? new List<PaisesDTO>());

                        CategoriasDTO categoria = new CategoriasDTO
                        {
                            IdiomaBase = App.IdiomaPersona
                        };

                        if (IsNotConnected) return;
                        Categorias = new ObservableCollection<CategoriasDTO>(await _categoriasService.ListarCategoriasPorIdioma(categoria) ?? new List<CategoriasDTO>());
                    }
                    else
                    {
                        Paises = new ObservableCollection<PaisesDTO> { PublicacionSeleccionada.PaisDelEvento };
                        Categorias = new ObservableCollection<CategoriasDTO> { PublicacionSeleccionada.CategoriaDelEvento.Categorias };
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            ImagenEditorModel imagenModel = returnedData as ImagenEditorModel;

            if (imagenModel != null)
            {
                UrlArchivo = imagenModel.Source;
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

        public ICommand IrVerAsistentes
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    GruposEventosDTO eventoParaVer = new GruposEventosDTO
                    {
                        Consecutivo = PublicacionSeleccionada.CodigoPublicacion,
                    };

                    await CoreMethods.PushPageModel<ListaAsistentesPageModel>(eventoParaVer);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand ToogleAsistirPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    GruposEventosAsistentesDTO gruposAsistentes = new GruposEventosAsistentesDTO
                    {
                        CodigoEvento = PublicacionSeleccionada.CodigoPublicacion,
                        CodigoPersona = App.Persona.Consecutivo
                    };

                    try
                    {
                        if (PersonaAsiste)
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapperEliminar = await _gruposService.EliminarGrupoEventoAsistente(gruposAsistentes);

                            if (wrapperEliminar != null && wrapperEliminar.Exitoso)
                            {
                                PersonaAsiste = false;
                                NumeroAsistentes -= 1;
                            }
                        }
                        else
                        {
                            if (IsNotConnected)
                            {
                                tcs.SetResult(true);
                                return;
                            }
                            WrapperSimpleTypesDTO wrapperAsiste = await _gruposService.CrearGruposEventosAsistentes(gruposAsistentes);

                            if (wrapperAsiste != null && wrapperAsiste.Exitoso)
                            {
                                PersonaAsiste = true;
                                NumeroAsistentes += 1;
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (EsMiPersonaYNoRegistro)
                    {
                        try
                        {
                            WrapperSimpleTypesDTO wrapper = null;

                            if (EsGrupo)
                            {
                                GruposEventosDTO grupoEvento = new GruposEventosDTO
                                {
                                    Consecutivo = PublicacionSeleccionada.CodigoPublicacion
                                };

                                if (IsNotConnected)
                                {
                                    tcs.SetResult(true);
                                    return;
                                }
                                wrapper = await _gruposService.EliminarGrupoEvento(grupoEvento);
                            }
                            else if (EsCandidato)
                            {
                                CandidatosVideosDTO candidatoVideo = new CandidatosVideosDTO
                                {
                                    Consecutivo = PublicacionSeleccionada.CodigoPublicacion,
                                    CodigoArchivo = PublicacionSeleccionada.CodigoArchivo.Value
                                };

                                if (IsNotConnected)
                                {
                                    tcs.SetResult(true);
                                    return;
                                }
                                wrapper = await _candidatoService.EliminarCandidatoVideo(candidatoVideo);
                            }

                            if (wrapper != null && wrapper.Exitoso)
                            {
                                PublicacionSeleccionada.FueBorrado = true;
                                await CoreMethods.PopPageModel(PublicacionSeleccionada);
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

        public ICommand InteractuarPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    bool? esClickBoton = parameter as bool?;

                    if (!esClickBoton.HasValue && PublicacionSeleccionada.EsVideo && !string.IsNullOrWhiteSpace(PublicacionSeleccionada.UrlArchivo))
                    {
                        PublicacionModalModel publicacion = new PublicacionModalModel
                        {
                            UrlArchivo = PublicacionSeleccionada.UrlArchivo,
                            TipoArchivoPublicacion = TipoArchivo.Video
                        };

                        await CoreMethods.PushPageModel<PublicacionModalPageModel>(publicacion, true);
                    }
                    else if (PublicacionSeleccionada.EsMiPersona)
                    {
                        try
                        {
                            List<string> opciones = new List<string>
                            {
                                SportsGoResources.TomarVideo,
                                SportsGoResources.ElegirVideo,
                            };

                            if (EsGrupo)
                            {
                                opciones.Add(SportsGoResources.TomarFoto);
                                opciones.Add(SportsGoResources.ElegirFoto);
                            }

                            string actionTaken = await CoreMethods.DisplayActionSheet(string.Empty, SportsGoResources.Cancelar, DeleteActionSheetOption, opciones.ToArray());
                            TipoArchivo tipoArchivoPublicacion = TipoArchivo.SinTipoArchivo;

                            MediaFile file = null;

                            if (actionTaken == SportsGoResources.ElegirFoto)
                            {
                                file = await PickPhotoAsync();
                                tipoArchivoPublicacion = TipoArchivo.Imagen;
                            }
                            else if (actionTaken == SportsGoResources.TomarFoto)
                            {
                                file = await TakePhotoAsync();
                                tipoArchivoPublicacion = TipoArchivo.Imagen;
                            }
                            else if (actionTaken == SportsGoResources.ElegirVideo)
                            {
                                file = await PickVideoAsync();
                                tipoArchivoPublicacion = TipoArchivo.Video;
                            }
                            else if (actionTaken == SportsGoResources.TomarVideo)
                            {
                                file = await TakeVideosync();
                                tipoArchivoPublicacion = TipoArchivo.Video;
                            }
                            else if (actionTaken == SportsGoResources.Borrar)
                            {
                                UrlArchivo = string.Empty;
                                PublicacionSeleccionada.UrlArchivo = string.Empty;
                                PublicacionSeleccionada.CodigoArchivo = 0;

                                tipoArchivoPublicacion = TipoArchivo.SinTipoArchivo;
                            }

                            if (file != null)
                            {
                                UrlArchivo = file.Path;
                                PublicacionSeleccionada.TipoArchivoPublicacion = tipoArchivoPublicacion;
                                _archivoModificado = true;

                                RaisePropertyChanged(nameof(EsRegistroPublicacionOPublicacionConArchivo));

                                if (tipoArchivoPublicacion == TipoArchivo.Imagen)
                                {
                                    ImagenEditorModel imagenModel = new ImagenEditorModel
                                    {
                                        Source = file.Path,
                                        EsPrimerRegistro = true,
                                        Persona = App.Persona,
                                        EsEvento = true,
                                        CodigoEvento = PublicacionSeleccionada.CodigoPublicacion
                                    };

                                    await CoreMethods.PushPageModel<EditorImagePageModel>(imagenModel);
                                }

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

        public ICommand AsignarPublicacion
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    if (PublicacionSeleccionada.EsMiPersona && await ValidarPublicacion())
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
                            try
                            {
                                // Si el archivo ya fue creado y se quedo a medias en las proximas operacion evitamos crearlo de nuevo y lo reusamos
                                if (_wrapperArchivo == null || !_wrapperArchivo.Exitoso || _wrapperArchivo.ConsecutivoArchivoCreado <= 0)
                                {
                                    // Si soy grupo no estoy obligado a crear un archivo, si lo cree entonces procedo a crearlo
                                    // Si soy candidato estoy obligado a crear un archivo
                                    // Se hace seguimiento a una booleana para saber si el archivo fue modificado
                                    if (_archivoModificado)
                                    {
                                        _wrapperArchivo = await AsignarArchivoPublicacion();

                                        // Si se intento crear el archivo y fue fallido entonces muestro el error y no prosigo
                                        if (_wrapperArchivo == null || !_wrapperArchivo.Exitoso || _wrapperArchivo.ConsecutivoArchivoCreado <= 0)
                                        {
                                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                                            tcs.SetResult(true);
                                            return;
                                        }
                                    }
                                }

                                WrapperSimpleTypesDTO wrapperPublicacion = await AsignarPublicacionServicio(_wrapperArchivo);

                                if (wrapperPublicacion != null && wrapperPublicacion.Exitoso)
                                {
                                    if (_wrapperArchivo != null && _wrapperArchivo.Exitoso)
                                    {
                                        await CachedImage.InvalidateCache(PublicacionSeleccionada.UrlArchivo, CacheType.All, true);
                                        PublicacionSeleccionada.CodigoArchivo = (int)_wrapperArchivo.ConsecutivoArchivoCreado;
                                        //PublicacionSeleccionada.UrlArchivo = ArchivosDTO.CrearUrlArchivo(PublicacionSeleccionada.TipoArchivoPublicacion, PublicacionSeleccionada.CodigoArchivo.Value);
                                        PublicacionSeleccionada.UrlArchivo = UrlArchivo;
                                    }

                                    if (EsRegistroPublicacion)
                                    {
                                        PublicacionSeleccionada.Creacion = DateTime.Now;
                                        PublicacionSeleccionada.CodigoPublicacion = (int)wrapperPublicacion.ConsecutivoCreado;
                                        PublicacionSeleccionada.UrlArchivo = UrlArchivo;
                                    }

                                    await CoreMethods.PopPageModel(PublicacionSeleccionada);

                                    // Si la creacion fue correcta ya puedo desmarcar el archivo como modificado
                                    _archivoModificado = false;
                                }
                                else
                                {
                                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorPublicacion, "OK");
                                }
                            }
                            catch (Exception)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorPublicacion, "OK");
                            }
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<WrapperSimpleTypesDTO> AsignarPublicacionServicio(WrapperSimpleTypesDTO wrapperArchivo)
        {
            WrapperSimpleTypesDTO wrapperPublicacion = null;

            if (EsCandidato)
            {
                CandidatosVideosDTO candidatoVideo = new CandidatosVideosDTO
                {
                    Consecutivo = PublicacionSeleccionada.CodigoPublicacion,
                    CodigoCandidato = PublicacionSeleccionada.CodigoPerfil,
                    Titulo = PublicacionSeleccionada.Titulo,
                    Descripcion = PublicacionSeleccionada.Descripcion
                };

                if (wrapperArchivo != null && wrapperArchivo.Exitoso)
                {
                    candidatoVideo.CodigoArchivo = (int)wrapperArchivo.ConsecutivoArchivoCreado;
                }
                else
                {
                    candidatoVideo.CodigoArchivo = PublicacionSeleccionada.CodigoArchivo.Value;
                }

                if (EsRegistroPublicacion)
                {
                    if (IsNotConnected) return null;
                    wrapperPublicacion = await _candidatoService.CrearCandidatoVideo(candidatoVideo);
                }
                else
                {
                    if (IsNotConnected) return null;
                    wrapperPublicacion = await _candidatoService.ModificarCandidatoVideo(candidatoVideo);
                }
            }
            if (EsGrupo)
            {
                var añoInicio = PublicacionSeleccionada.FechaInicio.Year;
                var mesInicio = PublicacionSeleccionada.FechaInicio.Month;
                var diaInicio = PublicacionSeleccionada.FechaInicio.Day;
                var horaInicio = HoraInicio.Hours;
                var minutosInicio = HoraInicio.Minutes;

                PublicacionSeleccionada.FechaInicio = new DateTime(añoInicio, mesInicio, diaInicio, horaInicio, minutosInicio, 0);

                var añoFinal = PublicacionSeleccionada.FechaTerminacion.Year;
                var mesFinal = PublicacionSeleccionada.FechaTerminacion.Month;
                var diaFinal = PublicacionSeleccionada.FechaTerminacion.Day;
                var horaFinal = HoraFinal.Hours;
                var minutosFinal = HoraFinal.Minutes;

                PublicacionSeleccionada.FechaTerminacion = new DateTime(añoFinal, mesFinal, diaFinal, horaFinal, minutosFinal, 0);

                GruposEventosDTO grupoEvento = new GruposEventosDTO
                {
                    Idioma = PublicacionSeleccionada.IdiomaDelEvento.Idioma,
                    CodigoPais = PaisSeleccionado.Consecutivo,
                    Titulo = PublicacionSeleccionada.Titulo,
                    Descripcion = PublicacionSeleccionada.Descripcion,
                    CodigoGrupo = PublicacionSeleccionada.CodigoPerfil,
                    CategoriasEventos = new List<CategoriasEventosDTO> { new CategoriasEventosDTO { CodigoCategoria = CategoriaSeleccionada.Consecutivo } },
                    FechaInicio = PublicacionSeleccionada.FechaInicio,
                    FechaTerminacion = PublicacionSeleccionada.FechaTerminacion,
                    Consecutivo = PublicacionSeleccionada.CodigoPublicacion,
                    Ubicacion = PublicacionSeleccionada.Ubicacion,
                    CodigoArchivo = PublicacionSeleccionada.CodigoArchivo
                };

                if (wrapperArchivo != null && wrapperArchivo.Exitoso && wrapperArchivo.ConsecutivoArchivoCreado > 0)
                {
                    grupoEvento.CodigoArchivo = (int)wrapperArchivo.ConsecutivoArchivoCreado;
                }

                if (EsRegistroPublicacion)
                {
                    if (IsNotConnected) return null;
                    wrapperPublicacion = await _gruposService.CrearGrupoEvento(grupoEvento);
                }
                else
                {
                    if (IsNotConnected) return null;
                    wrapperPublicacion = await _gruposService.ModificarInformacionGrupoEvento(grupoEvento);
                }
            }

            return wrapperPublicacion;
        }

        async Task<WrapperSimpleTypesDTO> AsignarArchivoPublicacion()
        {
            WrapperSimpleTypesDTO wrapper = null;

            IFile file = await FileSystem.Current.GetFileFromPathAsync(UrlArchivo);
            using (Stream streamSource = await file.OpenAsync(FileAccess.Read))
            {
                if (EsRegistroPublicacion)
                {
                    if (IsNotConnected) return null;
                    wrapper = await _archivosService.CrearArchivoStreamYControlarDuracionVideo((int)PublicacionSeleccionada.TipoArchivoPublicacion, App.Usuario.PlanesUsuarios.Planes.TiempoPermitidoVideo, streamSource);
                }
                else
                {
                    if (EsGrupo)
                    {
                        if (IsNotConnected) return null;
                        wrapper = await _archivosService.ModificarArchivoEventos((int)PublicacionSeleccionada.TipoArchivoPublicacion, PublicacionSeleccionada.CodigoPublicacion, PublicacionSeleccionada.CodigoArchivo, streamSource);
                    }
                    if (EsCandidato)
                    {
                        wrapper = await _archivosService.ModificarArchivoCandidatoVideos((int)PublicacionSeleccionada.TipoArchivoPublicacion, PublicacionSeleccionada.CodigoPublicacion, PublicacionSeleccionada.CodigoArchivo.Value, streamSource);
                    }
                }
            }

            return wrapper;
        }

        public async Task<bool> ValidarPublicacion()
        {
            bool publicacionValida = true;

            if (string.IsNullOrWhiteSpace(PublicacionSeleccionada.Titulo))
            {
                publicacionValida = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPublicacion, "OK");
            }
            else if (EsGrupo && (PaisSeleccionado == null || CategoriaSeleccionada == null || PublicacionSeleccionada.IdiomaDelEvento == null))
            {
                publicacionValida = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaDatosPublicacion, "OK");
            }
            else if (EsCandidato && (string.IsNullOrWhiteSpace(UrlArchivo) && (!PublicacionSeleccionada.CodigoArchivo.HasValue || PublicacionSeleccionada.CodigoArchivo < 0)))
            {
                publicacionValida = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FaltaVideoPublicacion, "OK");
            }
            else if (PublicacionSeleccionada.FechaTerminacion < PublicacionSeleccionada.FechaInicio)
            {
                publicacionValida = false;
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.FechasEventoInvalida, "OK");
            }

            return publicacionValida;
        }
    }
}