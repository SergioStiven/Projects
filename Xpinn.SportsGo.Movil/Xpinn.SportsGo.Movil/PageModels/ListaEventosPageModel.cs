using FreshMvvm;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ListaEventosPageModel : BasePageModel
    {
        PersonasDTO _persona;
        GruposServices _grupoService;
        IDateTimeHelper _dateTimeHelper;

        public bool NoHayNadaMasParaCargar { get; set; }
        public int SkipIndexWithText { get; set; }

        ObservableRangeCollection<GruposEventosAsistentesDTO> _eventos;
        public ObservableRangeCollection<GruposEventosAsistentesDTO> Eventos
        {
            get
            {
                if (_eventos != null)
                {
                    IEnumerable<GruposEventosAsistentesDTO> listaFiltrada = _eventos;

                    // Si tengo una descripcion en el searchbar control, filtro por esa descripcion
                    if (!string.IsNullOrWhiteSpace(TextoBuscador))
                    {
                        listaFiltrada = listaFiltrada.Where(x =>
                        {
                            // Puede suceder y sucedio que la descripcion viene vacia, por un error al guardar puede ser
                            if (x.GruposEventos != null && !string.IsNullOrWhiteSpace(x.GruposEventos.Titulo))
                            {
                                return x.GruposEventos.Titulo.ToUpperInvariant().Contains(TextoBuscador.ToUpperInvariant());
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
                _eventos = value;
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

        public ListaEventosPageModel()
        {
            _grupoService = new GruposServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            _persona = initData as PersonasDTO;

            try
            {
                await ListarEventosFiltrados(0, 5);
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

                    PublicacionModel publicacionParaVer = new PublicacionModel
                    {
                        CodigoPublicacion = grupoEventoAsistente.GruposEventos.Consecutivo,
                        TipoPerfil = TipoPerfil.Grupo,
                    };

                    await CoreMethods.PushPageModel<PublicacionPageModel>(publicacionParaVer);

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
                        if (_eventos != null)
                        {
                            if (!string.IsNullOrWhiteSpace(TextoBuscador))
                            {
                                await ListarEventosFiltrados(SkipIndexWithText, 5);
                            }
                            else
                            {
                                await ListarEventosFiltrados(Eventos.Count, 5);
                            }
                        }
                        else
                        {
                            await ListarEventosFiltrados(0, 5);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task ListarEventosFiltrados(int skipIndex, int takeIndex)
        {
            if (!NoHayNadaMasParaCargar)
            {
                BuscadorDTO buscadorDTO = new BuscadorDTO
                {
                    ConsecutivoPersona = _persona.Consecutivo,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    IdiomaBase = App.IdiomaPersona,
                    IdentificadorParaBuscar = TextoBuscador,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (IsNotConnected) return;
                List<GruposEventosAsistentesDTO> listaEventos = await _grupoService.ListarEventosAsistentesDeUnaPersona(buscadorDTO) ?? new List<GruposEventosAsistentesDTO>();

                if (listaEventos != null)
                {
                    if (listaEventos.Count > 0)
                    {
                        if (_eventos == null)
                        {
                            _eventos = new ObservableRangeCollection<GruposEventosAsistentesDTO>(listaEventos);
                        }
                        else
                        {
                            listaEventos = listaEventos.Where(x => !_eventos.Any(y => y.GruposEventos.Consecutivo == x.GruposEventos.Consecutivo)).ToList();
                            _eventos.AddRange(listaEventos);
                        }

                        RaisePropertyChanged(nameof(Eventos));
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaEventos.Count <= 0;
                    }
                }
            }
        }
    }
}
