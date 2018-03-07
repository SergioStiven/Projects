using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class PaymentController : Controller
    {
        PagosServices _pagosService;

        public PaymentController()
        {
            _pagosService = new PagosServices();
        }

        // GET: Payment
        [Route("Payment/PayU/{id}")]
        public async Task<ActionResult> Index(int id)
        {
            HistorialPagosPersonasDTO historial = new HistorialPagosPersonasDTO
            {
                Consecutivo = id
            };

            PayUModel payModel = await _pagosService.ConfigurarPayU(historial);

            return View(payModel);
        }

        [HttpPost]
        public async Task Confirmacion(PayUConfirmationModel paymentResponse)
        {
            if (paymentResponse != null && paymentResponse.TransaccionFueAprobada)
            {
                if (!string.IsNullOrWhiteSpace(paymentResponse.reference_sale))
                {
                    HistorialPagosPersonasDTO pagoParaBuscar = new HistorialPagosPersonasDTO
                    {
                        Consecutivo = Convert.ToInt32(paymentResponse.reference_sale)
                    };

                    HistorialPagosPersonasDTO pagoParaAprobar = await _pagosService.BuscarHistorialPagoPersona(pagoParaBuscar);
                    pagoParaAprobar.EstadoDelPago = EstadoDeLosPagos.Aprobado;
                    pagoParaAprobar.ReferenciaPago = paymentResponse.reference_pol;
                    pagoParaAprobar.ObservacionesCliente = paymentResponse.description;
                    pagoParaAprobar.EsPagoPorPayU = true;

                    WrapperSimpleTypesDTO wrapper = await _pagosService.ModificarEstadoPagoPersona(pagoParaAprobar);
                }
            }
        }
    }
}