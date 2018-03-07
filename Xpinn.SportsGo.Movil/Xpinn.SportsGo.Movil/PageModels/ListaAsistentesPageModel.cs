using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ListaAsistentesPageModel : BasePageModel
    {
        GruposEventosDTO _grupoEvento;
        GruposServices _grupoService;

        IDateTimeHelper _dateTimeHelper;

        public bool NoHayNadaMasParaCargar { get; set; }
        public int SkipIndexWithText { get; set; }

        ObservableRangeCollection<GruposEventosAsistentesDTO> _personas;
        public ObservableRangeCollection<GruposEventosAsistentesDTO> Personas
        {
            get
            {
                if (_personas != null)
                {
                    IEnumerable<GruposEventosAsistentesDTO> listaFiltrada = _personas;

                    // Si tengo una descripcion en el searchbar control, filtro por esa descripcion
                    if (!string.IsNullOrWhiteSpace(TextoBuscador))
                    {
                        listaFiltrada = listaFiltrada.Where(x =>
                        {
                            // Puede suceder y sucedio que la descripcion viene vacia, por un error al guardar puede ser
                            if (x.Personas != null && !string.IsNullOrWhiteSpace(x.Personas.NombreYApellido))
                            {
                                return x.Personas.NombreYApellido.ToUpperInvariant().Contains(TextoBuscador.ToUpperInvariant());
                            }

                            return false;
                        });
                    }

                    return new ObservableRangeCollection<GruposEventosAsistentesDTO>(listaFiltrada);
                }
                else
                {
                    return new ObservableRangeCollection<GruposEventosAsistentesDTO>();
                }
            }
            set
            {
                _personas = value;
            }
        }

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
            }
        }

        public ListaAsistentesPageModel()
        {
            _grupoService = new GruposServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            _grupoEvento = initData as GruposEventosDTO;

            try
            {
                await ListarPersonasFiltradas(0, 5);
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

        public ICommand RegistroSeleccionado
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    GruposEventosAsistentesDTO grupoEventoAsistente = parameter as GruposEventosAsistentesDTO;

                    PersonasDTO personaParaVer = new PersonasDTO
                    {
                        Consecutivo = grupoEventoAsistente.Personas.Consecutivo,
                        IdiomaDeLaPersona = App.IdiomaPersona
                    };

                    await CoreMethods.PushPageModel<PerfilPageModel>(personaParaVer);

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
                        if (_personas != null)
                        {
                            if (!string.IsNullOrWhiteSpace(TextoBuscador))
                            {
                                await ListarPersonasFiltradas(SkipIndexWithText, 5);
                            }
                            else
                            {
                                await ListarPersonasFiltradas(Personas.Count, 5);
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

        async Task ListarPersonasFiltradas(int skipIndex, int takeIndex)
        {
            if (!NoHayNadaMasParaCargar)
            {
                GruposEventosAsistentesDTO grupoEventoParaListar = new GruposEventosAsistentesDTO
                {
                    CodigoEvento = _grupoEvento.Consecutivo,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    IdiomaBase = App.IdiomaPersona,
                    IdentificadorParaBuscar = TextoBuscador,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (IsNotConnected) return;
                List<GruposEventosAsistentesDTO> listaPersonas = await _grupoService.ListarEventosAsistentesDeUnEvento(grupoEventoParaListar) ?? new List<GruposEventosAsistentesDTO>();

                if (listaPersonas != null)
                {
                    if (listaPersonas.Count > 0)
                    {
                        if (_personas == null)
                        {
                            _personas = new ObservableRangeCollection<GruposEventosAsistentesDTO>(listaPersonas);
                        }
                        else
                        {
                            listaPersonas = listaPersonas.Where(x => !_personas.Any(y => y.Personas.Consecutivo == x.Personas.Consecutivo)).ToList();
                            _personas.AddRange(listaPersonas);
                        }

                        RaisePropertyChanged(nameof(Personas));
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaPersonas.Count <= 0;
                    }
                }
            }
        }
    }
}
