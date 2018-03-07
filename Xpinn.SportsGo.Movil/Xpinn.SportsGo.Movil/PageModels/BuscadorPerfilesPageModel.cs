using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Windows.Input;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;
using FreshMvvm;
using Xpinn.SportsGo.Movil.Resources;
using System.Collections.ObjectModel;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class BuscadorPerfilesPageModel : BasePageModel
    {
        CandidatosServices _candidatoServices;
        GruposServices _gruposServices;
        CategoriasModel _categoriaSeleccionada;
        AdministracionServices _administracionServices;
        IDateTimeHelper _dateTimeHelper;

        string _textoBuscador;
        public string TextoBuscador
        {
            get
            {
                return _textoBuscador;
            }
            set
            {
                _textoBuscador = value;
                SkipIndexWithText = 0;
                NoHayNadaMasParaCargar = false;
                LoadMoreBusqueda.Execute(null);
            }
        }

        public int SkipIndexWithText { get; set; }
        public bool EstaFiltrandoCandidatos { get; set; }

        ObservableRangeCollection<BuscadorModel> _busqueda;
        public ObservableRangeCollection<BuscadorModel> Busqueda
        {
            get
            {
                IEnumerable<BuscadorModel> listaFiltrada = null;

                if (_busqueda != null)
                {
                    listaFiltrada = _busqueda.Where(x => x.TipoBusqueda == TipoBusqueda);

                    // Si tengo una descripcion en el searchbar control, filtro por esa descripcion
                    listaFiltrada = listaFiltrada.Where(x =>
                    {
                        bool siCumple = true;

                        if (x.TipoBusqueda == TipoBusqueda.Candidato)
                        {
                            siCumple = x.Estatura >= MinimaAltura && x.Estatura <= MaximaAltura && x.Peso >= MinimoPeso && x.Peso <= MaximaAltura && x.Edad >= MinimaEdad && x.Edad <= MaximaEdad && (PaisSeleccionado == null || PaisSeleccionado.Consecutivo == 0 || PaisSeleccionado.Consecutivo == x.CodigoPais);

                            if (!siCumple)
                            {
                                return false;
                            }
                        }
                        else if (x.TipoBusqueda == TipoBusqueda.Evento)
                        {
                            var añoInicio = FechaInicio.Year;
                            var mesInicio = FechaInicio.Month;
                            var diaInicio = FechaInicio.Day;
                            var horaInicio = HoraInicio.Hours;
                            var minutosInicio = HoraInicio.Minutes;

                            DateTime fechaInicioFiltro = new DateTime(añoInicio, mesInicio, diaInicio, horaInicio, minutosInicio, 0);

                            var añoFinal = FechaFinal.Year;
                            var mesFinal = FechaFinal.Month;
                            var diaFinal = FechaFinal.Day;
                            var horaFinal = HoraFinal.Hours;
                            var minutosFinal = HoraFinal.Minutes;

                            DateTime fechaFinalFiltro = new DateTime(añoFinal, mesFinal, diaFinal, horaFinal, minutosFinal, 0);

                            siCumple = x.FechaInicio >= fechaInicioFiltro && x.FechaFinal <= fechaFinalFiltro;

                            if (!siCumple)
                            {
                                return false;
                            }
                        }

                        // Puede suceder y sucedio que la descripcion viene vacia, por un error al guardar puede ser
                        if (!string.IsNullOrWhiteSpace(x.IdentificadorPrincipal) && !string.IsNullOrWhiteSpace(TextoBuscador))
                        {
                            siCumple = x.IdentificadorPrincipal.ToUpperInvariant().Contains(TextoBuscador.ToUpperInvariant());
                        }

                        return siCumple;
                    });

                    return new ObservableRangeCollection<BuscadorModel>(listaFiltrada);
                }
                else
                {
                    return new ObservableRangeCollection<BuscadorModel>();
                }
            }
        }

        public bool NoHayNadaMasParaCargar { get; set; }

        int _minimaAltura;
        public int MinimaAltura
        {
            get
            {
                return _minimaAltura;
            }
            set
            {
                _minimaAltura = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        int _maximaAltura;
        public int MaximaAltura
        {
            get
            {
                return _maximaAltura;
            }
            set
            {
                _maximaAltura = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        int _minimoPeso;
        public int MinimoPeso
        {
            get
            {
                return _minimoPeso;
            }
            set
            {
                _minimoPeso = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        int _maximoPeso;
        public int MaximoPeso
        {
            get
            {
                return _maximoPeso;
            }
            set
            {
                _maximoPeso = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        int _minimaEdad;
        public int MinimaEdad
        {
            get
            {
                return _minimaEdad;
            }
            set
            {
                _minimaEdad = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        int _maximaEdad;
        public int MaximaEdad
        {
            get
            {
                return _maximaEdad;
            }
            set
            {
                _maximaEdad = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        DateTime _fechaInicio;
        public DateTime FechaInicio
        {
            get
            {
                DateTime fechaInicio = AppConstants.MinimumDate;

                if (_fechaInicio != DateTime.MinValue && _fechaInicio != AppConstants.MinimumDate)
                {
                    fechaInicio = _fechaInicio;
                }

                return fechaInicio;
            }
            set
            {
                _fechaInicio = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        DateTime _fechaFinal;
        public DateTime FechaFinal
        {
            get
            {
                DateTime fechaFinal = AppConstants.MaximumDate;

                if (_fechaFinal != DateTime.MinValue && _fechaFinal != AppConstants.MaximumDate)
                {
                    fechaFinal = _fechaFinal;
                }

                return fechaFinal;
            }
            set
            {
                _fechaFinal = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        TimeSpan _horaInicio;
        public TimeSpan HoraInicio
        {
            get
            {
                return _horaInicio;
            }
            set
            {
                _horaInicio = value;
                NoHayNadaMasParaCargar = false;
            }
        }

        TimeSpan _horaFinal;
        public TimeSpan HoraFinal
        {
            get
            {
                return _horaFinal;
            }
            set
            {
                _horaFinal = value;
                NoHayNadaMasParaCargar = false;
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
                _selectIndexSegmentBar = value;
                SkipIndexWithText = 0;
                NoHayNadaMasParaCargar = false;

                if (_busqueda == null || Busqueda.Count <= 0 || !string.IsNullOrWhiteSpace(TextoBuscador))
                {
                    LoadMoreBusqueda.Execute(null);
                }
            }
        }

        public TipoBusqueda TipoBusqueda
        {
            get
            {
                TipoBusqueda tipoBusqueda = TipoBusqueda.SinTipoBusqueda;

                if (SelectIndexSegmentBar == 0)
                {
                    tipoBusqueda = TipoBusqueda.Candidato;
                }
                else if (SelectIndexSegmentBar == 1)
                {
                    tipoBusqueda = TipoBusqueda.Grupo;
                }
                else if (SelectIndexSegmentBar == 2)
                {
                    tipoBusqueda = TipoBusqueda.Evento;
                }

                return tipoBusqueda;
            }
        }

        public bool BuscandoCandidatos
        {
            get
            {
                return TipoBusqueda == TipoBusqueda.Candidato;
            }
        }

        public bool BuscandoEventos
        {
            get
            {
                return TipoBusqueda == TipoBusqueda.Evento;
            }
        }

        public string TextoBotonFiltrarCandidatos
        {
            get
            {
                string texto = string.Empty;
                if (EstaFiltrandoCandidatos)
                {
                    texto = SportsGoResources.AplicarFiltro;
                }
                else
                {
                    texto = SportsGoResources.Filtrar;
                }

                return texto;
            }
        }

        public PaisesDTO PaisSeleccionado { get; set; }
        public ObservableCollection<PaisesDTO> Paises { get; set; }

        public BuscadorPerfilesPageModel()
        {
            _candidatoServices = new CandidatosServices();
            _gruposServices = new GruposServices();
            _administracionServices = new AdministracionServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
            _minimaAltura = 130;
            _minimoPeso = 20;
            _maximaAltura = 220;
            _maximoPeso = 120;
            _maximaEdad = 80;
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            _categoriaSeleccionada = initData as CategoriasModel;

            try
            {
                await ListarPersonasFiltradas(0, 5);

                PaisesDTO paises = new PaisesDTO
                {
                    IdiomaBase = App.IdiomaPersona
                };
                Paises = new ObservableCollection<PaisesDTO>(await _administracionServices.ListarPaisesPorIdioma(paises) ?? new List<PaisesDTO>());
            }
            catch (Exception)
            {

            }
        }

        protected override void ViewIsAppearing(object sender, EventArgs e)
        {
            base.ViewIsAppearing(sender, e);

            NoHayNadaMasParaCargar = false;
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            NoHayNadaMasParaCargar = true;
        }

        public ICommand FiltrarCandidatos
        {
            get
            {
                return new FreshAwaitCommand((parameter, tcs) =>
                {
                    EstaFiltrandoCandidatos = !EstaFiltrandoCandidatos;

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreBusqueda
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (_busqueda != null)
                        {
                            if (!string.IsNullOrWhiteSpace(TextoBuscador))
                            {
                                await ListarPersonasFiltradas(SkipIndexWithText, 5);
                                SkipIndexWithText += 5;
                            }
                            else
                            {
                                await ListarPersonasFiltradas(Busqueda.Count, 5);
                            }
                        }
                        else
                        {
                            await ListarPersonasFiltradas(0, 5);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand RegistroSeleccionado
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    BuscadorModel buscadorModel = parameter as BuscadorModel;

                    if (buscadorModel.TipoBusqueda == TipoBusqueda.Candidato || buscadorModel.TipoBusqueda == TipoBusqueda.Grupo)
                    {
                        PersonasDTO personaParaVer = new PersonasDTO
                        {
                            Consecutivo = buscadorModel.CodigoPrincipal,
                            IdiomaDeLaPersona = App.IdiomaPersona
                        };

                        if (buscadorModel.TipoBusqueda == TipoBusqueda.Candidato && App.Usuario.PlanesUsuarios.Planes.ConsultaCandidatos != 1)
                        {
                            await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.ConsultaCandidatos));
                            tcs.SetResult(true);
                            return;
                        }
                        else if (buscadorModel.TipoBusqueda == TipoBusqueda.Grupo && App.Usuario.PlanesUsuarios.Planes.ConsultaGrupos != 1)
                        {
                            await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.ConsultaGrupos));
                            tcs.SetResult(true);
                            return;
                        }

                        await CoreMethods.PushPageModel<PerfilPageModel>(personaParaVer);
                    }
                    else if (buscadorModel.TipoBusqueda == TipoBusqueda.Evento)
                    {
                        PublicacionModel publicacionParaVer = new PublicacionModel
                        {
                            CodigoPublicacion = buscadorModel.CodigoPrincipal,
                            TipoPerfil = TipoPerfil.Grupo,
                            CodigoArchivo = buscadorModel.CodigoArchivoPrincipal,
                        };

                        if (App.Usuario.PlanesUsuarios.Planes.ConsultaEventos != 1)
                        {
                            await CoreMethods.PushPageModel<OperacionNoSoportadaPageModel>(new OperacionControlModel(TipoOperacion.ConsultaEventos));
                        }
                        else
                        {
                            await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionParaVer);
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task ListarPersonasFiltradas(int skipIndex, int takeIndex)
        {
            List<BuscadorModel> listaBuscadorModel = new List<BuscadorModel>();

            if (!NoHayNadaMasParaCargar)
            {
                BuscadorDTO buscadorDTO = new BuscadorDTO
                {
                    CategoriasParaBuscar = new List<int>
                    {
                        _categoriaSeleccionada.CodigoCategoria
                    },
                    EstaturaInicial = MinimaAltura,
                    EstaturaFinal = MaximaAltura,
                    PesoInicial = MinimoPeso,
                    PesoFinal = MaximoPeso,
                    EdadInicio = MinimaEdad,
                    EdadFinal = MaximaEdad,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    IdiomaBase = App.IdiomaPersona,
                    IdentificadorParaBuscar = TextoBuscador,
                    FechaInicio = FechaInicio,
                    FechaFinal = FechaFinal,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (PaisSeleccionado != null && PaisSeleccionado.Consecutivo > 0)
                {
                    buscadorDTO.PaisesParaBuscar = new List<int>
                    {
                        PaisSeleccionado.Consecutivo
                    };
                }

                if (TipoBusqueda == TipoBusqueda.Candidato)
                {
                    if (IsNotConnected) return;
                    List<CandidatosDTO> listaCandidatos = await _candidatoServices.ListarCandidatos(buscadorDTO) ?? new List<CandidatosDTO>();

                    listaBuscadorModel = (BuscadorModel.CrearListaBuscadorModel(listaCandidatos));
                }
                else if (TipoBusqueda == TipoBusqueda.Grupo)
                {
                    if (IsNotConnected) return;
                    List<GruposDTO> listaGrupos = await _gruposServices.ListarGrupos(buscadorDTO) ?? new List<GruposDTO>();

                    listaBuscadorModel = BuscadorModel.CrearListaBuscadorModel(listaGrupos);
                }
                else if (TipoBusqueda == TipoBusqueda.Evento)
                {
                    if (IsNotConnected) return;
                    List<GruposEventosDTO> listaEventos = await _gruposServices.ListarEventos(buscadorDTO);

                    listaBuscadorModel = BuscadorModel.CrearListaBuscadorModel(listaEventos);
                }

                if (listaBuscadorModel != null)
                {
                    if (listaBuscadorModel.Count > 0)
                    {
                        if (_busqueda == null)
                        {
                            _busqueda = new ObservableRangeCollection<BuscadorModel>(listaBuscadorModel);
                        }
                        else
                        {
                            listaBuscadorModel = listaBuscadorModel.Where(x => !_busqueda.Any(y => y.CodigoPrincipal == x.CodigoPrincipal && y.TipoBusqueda == x.TipoBusqueda)).ToList();
                            _busqueda.AddRange(listaBuscadorModel);
                        }

                        RaisePropertyChanged("Busqueda");
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaBuscadorModel.Count <= 0;
                    }
                }
            }
        }
    }
}