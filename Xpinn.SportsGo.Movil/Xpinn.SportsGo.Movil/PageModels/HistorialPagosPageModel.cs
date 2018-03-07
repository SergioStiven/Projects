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
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class HistorialPagosPageModel : BasePageModel
    {
        PagosServices _pagosService;
        IDateTimeHelper _dateTimeHelper;

        public bool NoHayNadaMasParaCargar { get; set; }

        public ObservableRangeCollection<HistorialPagosModel> Historial { get; set; }

        public HistorialPagosPageModel()
        {
            _pagosService = new PagosServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            try
            {
                await CargarPagos(0, 10);
            }
            catch (Exception)
            {
                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorConsultarHistorialPagos, "OK");
            }
        }

        public override void ReverseInit(object returnedData)
        {
            base.ReverseInit(returnedData);

            HistorialPagosModel pagoBorrado = returnedData as HistorialPagosModel;

            if (pagoBorrado != null && !pagoBorrado.EsActualizarPlan)
            {
                Historial.Remove(pagoBorrado);
            }
            else
            {
                Historial.Remove(pagoBorrado);
                Historial.Insert(0, pagoBorrado);
            }
        }

        public ICommand RegistroSeleccionado
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    HistorialPagosModel historialSeleccionado = parameter as HistorialPagosModel;
                    await CoreMethods.PushPageModel<PagosPageModel>(historialSeleccionado);

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand LoadMoreChats
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (Historial != null)
                        {
                            await CargarPagos(Historial.Count, 10);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task CargarPagos(int skipIndex, int takeIndex)
        {
            if (!NoHayNadaMasParaCargar)
            {
                BuscadorDTO buscador = new BuscadorDTO
                {
                    ConsecutivoPersona = App.Persona.Consecutivo,
                    IdiomaBase = App.IdiomaPersona,
                    SkipIndexBase = skipIndex,
                    TakeIndexBase = takeIndex,
                    ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                };

                if (IsNotConnected) return;
                List<HistorialPagosPersonasDTO> listaPagos = await _pagosService.ListarHistorialPagosDeUnaPersona(buscador);

                if (listaPagos != null)
                {
                    if (listaPagos.Count > 0)
                    {
                        if (Historial == null)
                        {
                            Historial = new ObservableRangeCollection<HistorialPagosModel>(HistorialPagosModel.CrearListaHistorialPagosModel(listaPagos));
                        }
                        else
                        {
                            foreach (HistorialPagosPersonasDTO pago in listaPagos)
                            {
                                HistorialPagosModel pagoExistente = Historial.Where(x => x.HistorialPago.Consecutivo == pago.Consecutivo).FirstOrDefault();

                                if (pagoExistente != null)
                                {
                                    Historial.Remove(pagoExistente);
                                }

                                Historial.Add(new HistorialPagosModel(pago));
                            }
                        }
                    }
                    else
                    {
                        NoHayNadaMasParaCargar = listaPagos.Count <= 0;
                    }
                }
            }
        }
    }
}
