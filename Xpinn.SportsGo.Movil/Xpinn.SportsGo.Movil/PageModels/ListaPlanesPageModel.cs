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

namespace Xpinn.SportsGo.Movil.PageModels
{
    class ListaPlanesPageModel : BasePageModel
    {
        PlanesServices _planesService;

        ObservableRangeCollection<PlanesDTO> _planes;
        public ObservableRangeCollection<PlanesDTO> Planes
        {
            get
            {
                ObservableRangeCollection<PlanesDTO> listaParaDevolver = null;

                if (_planes != null && _planes.Count > 0)
                {
                    listaParaDevolver = new ObservableRangeCollection<PlanesDTO>(_planes.Where(x => x.Consecutivo != App.Usuario.PlanesUsuarios.CodigoPlan));
                }

                return listaParaDevolver;
            }
            set
            {
                _planes = value;
            }
        }

        public bool NoHayNadaMasParaCargar { get; set; }

        public ListaPlanesPageModel()
        {
            _planesService = new PlanesServices();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                await CargarPlanes(0, 10);
            }
            catch (Exception)
            {
                
            }
        }

        public ICommand RegistroSeleccionado
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    await CoreMethods.PushPageModel<DetallePlanPageModel>(parameter);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMorePlanes
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (_planes != null)
                        {
                            await CargarPlanes(_planes.Count, 10);
                        }
                        else
                        {
                            await CargarPlanes(0, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarPlanes(int skipIndex, int takeIndex)
        {
            if (!NoHayNadaMasParaCargar)
            {
                PlanesDTO plan = new PlanesDTO
                {
                    CodigoPaisParaBuscarMoneda = App.Persona.CodigoPais,
                    CodigoIdiomaUsuarioBase = App.Persona.CodigoIdioma,
                    TipoPerfil = App.Persona.TipoPerfil,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex
                };

                if (IsNotConnected) return;
                List<PlanesDTO> listaPlanes = await _planesService.ListarPlanesPorIdioma(plan);

                if (listaPlanes != null)
                {
                    if (listaPlanes.Count > 0)
                    {
                        if (_planes == null)
                        {
                            _planes = new ObservableRangeCollection<PlanesDTO>(listaPlanes);
                        }
                        else
                        {
                            _planes.AddRange(listaPlanes);
                        }

                        RaisePropertyChanged(nameof(Planes));
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaPlanes.Count <= 0;
                    }
                }
            }
        }
    }
}
