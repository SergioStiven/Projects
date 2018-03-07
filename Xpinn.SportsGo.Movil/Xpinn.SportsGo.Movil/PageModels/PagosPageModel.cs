using Acr.UserDialogs;
using FreshMvvm;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class PagosPageModel : BasePageModel
    {
        ArchivosServices _archivoService;
        PagosServices _pagosService;
        Stream _streamArchivo;
        IDateTimeHelper _dateTimeHelper;

        public HistorialPagosModel Pago { get; set; }

        public PagosPageModel()
        {
            _archivoService = new ArchivosServices();
            _pagosService = new PagosServices();
            _dateTimeHelper = FreshIOC.Container.Resolve<IDateTimeHelper>();
        }

        protected override void ViewIsDisappearing(object sender, EventArgs e)
        {
            base.ViewIsDisappearing(sender, e);

            if (_streamArchivo != null)
            {
                _streamArchivo.Dispose();
            }
        }

        public override async void Init(object initData)
        {
            base.Init(initData);

            Pago = initData as HistorialPagosModel;

            // Significa que es un nuevo pago
            if (Pago == null)
            {
                PlanesDTO planParaComprar = initData as PlanesDTO;

                try
                {
                    FacturaFormatoDTO facturaFormatoParaBuscar = new FacturaFormatoDTO
                    {
                        IdiomaDeLaFacturaFormato = App.IdiomaPersona,
                        CodigoPais = App.Persona.CodigoPais
                    };

                    FacturaFormatoDTO facturaFormato = await _pagosService.BuscarFacturaFormato(facturaFormatoParaBuscar);

                    if (facturaFormato != null)
                    {
                        HistorialPagosPersonasDTO historialPago = new HistorialPagosPersonasDTO();

                        historialPago.TextoFacturaFormato = facturaFormato.Texto;
                        historialPago.CodigoPais = App.Persona.CodigoPais;
                        historialPago.CodigoMoneda = App.Persona.Paises.CodigoMoneda;
                        historialPago.Paises = App.Persona.Paises;
                        historialPago.Monedas = App.Persona.Paises.Monedas;
                        historialPago.CodigoPersona = App.Persona.Consecutivo;
                        historialPago.Planes = planParaComprar;
                        historialPago.CodigoPlan = planParaComprar.Consecutivo;
                        historialPago.Precio = planParaComprar.Precio;

                        Pago = new HistorialPagosModel(historialPago);

                        RaisePropertyChanged(nameof(Pago));
                    }
                    else
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorAlConsultarInfoPagos, "OK");
                    }
                }
                catch (Exception)
                {
                    await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorAlConsultarInfoPagos, "OK");
                }
            }
        }

        public ICommand CambiarArchivo
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        List<string> opciones = new List<string>
                        {
                            SportsGoResources.TomarFoto,
                            SportsGoResources.ElegirFoto,
                        };

                        string actionTaken = await CoreMethods.DisplayActionSheet(string.Empty, SportsGoResources.Cancelar, null, opciones.ToArray());

                        MediaFile file = null;

                        if (actionTaken == SportsGoResources.ElegirFoto)
                        {
                            file = await PickPhotoAsync();
                        }
                        else if (actionTaken == SportsGoResources.TomarFoto)
                        {
                            file = await TakePhotoAsync();
                        }

                        if (file != null)
                        {
                            Pago.UrlArchivo = file.Path;

                            _streamArchivo = file.GetStream();

                            RaisePropertyChanged(nameof(Pago));

                            file.Dispose();
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteractuarBoton
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago)
                        {
                            WrapperSimpleTypesDTO wrapperVerificar = await _pagosService.VerificarQueNoExistaUnPagoEnTramite(Pago.HistorialPago);

                            if (wrapperVerificar.PagoEnTramite)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoYaHayEnTramite, "OK");
                                tcs.SetResult(true);
                                return;
                            }

                            WrapperSimpleTypesDTO wrapper = await _pagosService.CrearHistorialPagoPersona(Pago.HistorialPago);

                            if (wrapper.Exitoso)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Notificacion, SportsGoResources.PagoSeRegistroEsperandoPago, "OK");
                                await CoreMethods.PopPageModel();
                            }
                            else
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoNoSePudoRegistrar, "OK");
                            }
                        }
                        else if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado)
                        {
                            // Si estoy esperando un pago y no tengo archivo, reboto porque lo necesito
                            if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago && _streamArchivo == null)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ArchivoNecesarioParaElPago, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                            else if (string.IsNullOrWhiteSpace(Pago.HistorialPago.ReferenciaPago))
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoReferenciaPagoNecesaria, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                            else
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
                                    // Si estoy esperando el pago, o si estoy en rechazado y tengo datos en el stream (Significa que lo modifique), procedo a modificar el archivo
                                    if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago || (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.Rechazado && _streamArchivo != null))
                                    {
                                        WrapperSimpleTypesDTO wrapperModificarArchivoRecibo = await _archivoService.AsignarArchivoReciboPago(Pago.HistorialPago.Consecutivo, Pago.HistorialPago.CodigoArchivo, _streamArchivo);

                                        if (wrapperModificarArchivoRecibo != null && wrapperModificarArchivoRecibo.Exitoso)
                                        {
                                            Pago.HistorialPago.CodigoArchivo = Convert.ToInt32(wrapperModificarArchivoRecibo.ConsecutivoArchivoCreado);
                                        }
                                        else
                                        {
                                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                                            tcs.SetResult(true);
                                            return;
                                        }
                                    }

                                    Pago.HistorialPago.EstadoDelPago = EstadoDeLosPagos.PendientePorAprobar;

                                    WrapperSimpleTypesDTO wrapper = await _pagosService.ModificarEstadoPagoPersona(Pago.HistorialPago);

                                    if (wrapper != null && wrapper.Exitoso)
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Notificacion, SportsGoResources.PagoSeRegistroEsperandoAprobacion, "OK");

                                        Pago.EsActualizarPlan = true;
                                        await CoreMethods.PopPageModel(Pago);
                                    }
                                    else
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoNoSePudoRegistrar, "OK");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoNoSePudoRegistrar, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand InteractuarPayU
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago)
                        {
                            WrapperSimpleTypesDTO wrapperVerificar = await _pagosService.VerificarQueNoExistaUnPagoEnTramite(Pago.HistorialPago);

                            if (wrapperVerificar.PagoEnTramite)
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoYaHayEnTramite, "OK");
                                tcs.SetResult(true);
                                return;
                            }

                            WrapperSimpleTypesDTO wrapper = await _pagosService.CrearHistorialPagoPersona(Pago.HistorialPago);

                            if (wrapper.Exitoso)
                            {
                                Pago.HistorialPago.Consecutivo = Convert.ToInt32(wrapper.ConsecutivoCreado);
                            }
                            else
                            {
                                await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoNoSePudoRegistrar, "OK");
                                tcs.SetResult(true);
                                return;
                            }
                        }

                        Device.OpenUri(new Uri(URL.UrlWeb + "Payment/Index/" + Pago.HistorialPago.Consecutivo));

                        await Task.Delay(1000);
                        await CoreMethods.DisplayAlert(SportsGoResources.Notificacion, SportsGoResources.PagoSeRegistroEsperandoPago, "OK");
                        await CoreMethods.PopPageModel();

                        await Task.Delay(1000);
                        HistorialPagosPersonasDTO historial = new HistorialPagosPersonasDTO
                        {
                            Consecutivo = Pago.HistorialPago.Consecutivo,
                            ZonaHorariaGMTBase = _dateTimeHelper.DifferenceBetweenGMTAndLocalTimeZone
                        };
                        TimeLineNotificaciones timeLineNotificacion = await _pagosService.VerificarSiPagoEstaAprobadoYTraerNotificacion(historial);

                        if (timeLineNotificacion != null)
                        {
                            ChatsServices chatService = new ChatsServices();
                            chatService.ReproducirNotificacionFake(timeLineNotificacion);
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.PagoNoSePudoRegistrar, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }

        public ICommand BorrarPago
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    try
                    {
                        if (Pago.HistorialPago.EstadoDelPago == EstadoDeLosPagos.EsperaPago)
                        {
                            WrapperSimpleTypesDTO wrapper = await _pagosService.EliminarPagoPendientePorPagar(Pago.HistorialPago);

                            if (wrapper != null && wrapper.Exitoso)
                            {
                                await CoreMethods.PopPageModel(Pago);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorArchivo, "OK");
                    }

                    tcs.SetResult(true);
                });
            }
        }
    }
}