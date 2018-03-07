using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class PagosController : ApiController
    {
        PagosBusiness _pagosBusiness;

        public PagosController()
        {
            _pagosBusiness = new PagosBusiness();
        }


        #region Metodos FacturaFormato


        public async Task<IHttpActionResult> AsignarFacturaFormato(FacturaFormato facturaFormatoParaAsignar)
        {
            if (facturaFormatoParaAsignar == null || string.IsNullOrWhiteSpace(facturaFormatoParaAsignar.Texto) || facturaFormatoParaAsignar.CodigoIdioma <= 0
                || facturaFormatoParaAsignar.IdiomaDeLaFacturaFormato == Idioma.SinIdioma)
            {
                return BadRequest("facturaFormatoParaAsignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarFacturaFormato = await _pagosBusiness.AsignarFacturaFormato(facturaFormatoParaAsignar);

                return Ok(wrapperAsignarFacturaFormato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarFacturaFormatoLista(List<FacturaFormato> facturaFormatoParaAsignar)
        {
            if (facturaFormatoParaAsignar == null || facturaFormatoParaAsignar.Count <= 0 
                || !facturaFormatoParaAsignar.TrueForAll(x => !string.IsNullOrWhiteSpace(x.Texto) && x.IdiomaDeLaFacturaFormato != Idioma.SinIdioma && x.CodigoPais > 0))
            {
                return BadRequest("facturaFormatoParaAsignar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarFacturaFormatoLista = await _pagosBusiness.AsignarFacturaFormatoLista(facturaFormatoParaAsignar);

                return Ok(wrapperAsignarFacturaFormatoLista);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarFacturaFormato(FacturaFormato facturaFormatoParaBuscar)
        {
            if (facturaFormatoParaBuscar == null || facturaFormatoParaBuscar.IdiomaDeLaFacturaFormato == Idioma.SinIdioma || facturaFormatoParaBuscar.CodigoPais <= 0)
            {
                return BadRequest("terminosCondicionesParaBuscar vacio y/o invalido!.");
            }

            try
            {
                FacturaFormato facturaFormatoBuscada = await _pagosBusiness.BuscarFacturaFormato(facturaFormatoParaBuscar);

                return Ok(facturaFormatoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarFacturasFormatos()
        {
            try
            {
                List<FacturaFormato> listaFacturaFormatos = await _pagosBusiness.ListarFacturasFormatos();

                return Ok(listaFacturaFormatos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos HistorialPagosPersonas


        public async Task<IHttpActionResult> CrearHistorialPagoPersona(HistorialPagosPersonas historialPagoParaCrear)
        {
            if (historialPagoParaCrear == null || string.IsNullOrWhiteSpace(historialPagoParaCrear.TextoFacturaFormato) || historialPagoParaCrear.CodigoPersona <= 0
                || historialPagoParaCrear.CodigoPais <= 0 || historialPagoParaCrear.CodigoPlan <= 0)
            {
                return BadRequest("historialPagoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearHistorialPagoPersona = await _pagosBusiness.CrearHistorialPagoPersona(historialPagoParaCrear);

                return Ok(wrapperCrearHistorialPagoPersona);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarHistorialPagoPersona(HistorialPagosPersonas historialPagoParaBuscar)
        {
            if (historialPagoParaBuscar == null || historialPagoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("historialPagoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                HistorialPagosPersonasDTO historialPagoBuscado = await _pagosBusiness.BuscarHistorialPagoPersona(historialPagoParaBuscar);

                return Ok(historialPagoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarPagoEnTramiteDeUnaPersona(HistorialPagosPersonas historialPagoParaBuscar)
        {
            if (historialPagoParaBuscar == null || historialPagoParaBuscar.CodigoPersona <= 0 || historialPagoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("historialPagoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                HistorialPagosPersonasDTO pagoEnTramite = await _pagosBusiness.BuscarPagoEnTramiteDeUnaPersona(historialPagoParaBuscar);

                return Ok(pagoEnTramite);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarQueNoExistaUnPagoEnTramite(HistorialPagosPersonas historialPagoParaVerificar)
        {
            if (historialPagoParaVerificar == null || historialPagoParaVerificar.CodigoPersona <= 0)
            {
                return BadRequest("historialPagoParaVerificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperVerificarQueNoExistaUnPagoEnTramite = await _pagosBusiness.VerificarQueNoExistaUnPagoEnTramite(historialPagoParaVerificar);

                return Ok(wrapperVerificarQueNoExistaUnPagoEnTramite);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarHistorialPagosDeUnaPersona(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.ConsecutivoPersona <= 0 || buscador.IdiomaBase == Idioma.SinIdioma || buscador.SkipIndexBase < 0 && buscador.TakeIndexBase <= 0)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<HistorialPagosPersonasDTO> listaHistorialPagos = await _pagosBusiness.ListarHistorialPagosDeUnaPersona(buscador);

                return Ok(listaHistorialPagos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarHistorialPagosPersonas(BuscadorDTO buscador)
        {
            if (buscador == null)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<HistorialPagosPersonasDTO> listaHistorialPagos = await _pagosBusiness.ListarHistorialPagosPersonas(buscador);

                return Ok(listaHistorialPagos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarEstadoPagoPersona(HistorialPagosPersonas historialPagoParaModificar)
        {
            if (historialPagoParaModificar == null || historialPagoParaModificar.Consecutivo <= 0 || historialPagoParaModificar.CodigoPersona <= 0 || historialPagoParaModificar.CodigoPlan <= 0
                || historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago)
            {
                return BadRequest("monedaParaModificar vacio y/o invalido!.");
            }
            else if (historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.PendientePorAprobar && historialPagoParaModificar.CodigoArchivo <= 0 && string.IsNullOrWhiteSpace(historialPagoParaModificar.ReferenciaPago))
            {
                return BadRequest("historialPagoParaModificar debe tener un archivo y codigo de referencia si se va a reportar!.");
            }

            try
            {
                Tuple<WrapperSimpleTypesDTO,TimeLineNotificaciones> tupleWrapper = await _pagosBusiness.ModificarEstadoPagoPersona(historialPagoParaModificar);

                if (tupleWrapper.Item1.Exitoso && tupleWrapper.Item2 != null)
                {
                    NoticiasBusiness noticiasBusiness = new NoticiasBusiness();

                    IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
                    hubContext.Clients.Group(ChatHub._prefixChatGroupName + tupleWrapper.Item2.CodigoPersonaDestino.ToString()).receiveNotification(tupleWrapper.Item2);
                }

                return Ok(tupleWrapper.Item1);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarPagoPendientePorPagar(HistorialPagosPersonas historialPagoParaEliminar)
        {
            if (historialPagoParaEliminar == null || historialPagoParaEliminar.CodigoPersona <= 0 || historialPagoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("historialPagoParEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarPagoPendientePorPagar = await _pagosBusiness.EliminarPagoPendientePorPagar(historialPagoParaEliminar);

                return Ok(wrapperEliminarPagoPendientePorPagar);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion


        #region Metodos Monedas


        public async Task<IHttpActionResult> BuscarMoneda(Monedas monedaParaBuscar)
        {
            if (monedaParaBuscar == null || monedaParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("monedaParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Monedas monedaBuscada = await _pagosBusiness.BuscarMoneda(monedaParaBuscar);

                return Ok(monedaBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarMonedas()
        {
            try
            {
                List<Monedas> listaMonedas = await _pagosBusiness.ListarMonedas();

                return Ok(listaMonedas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarMoneda(Monedas monedaParaModificar)
        {
            if (monedaParaModificar == null || monedaParaModificar.Consecutivo <= 0 || monedaParaModificar.CambioMoneda <= 0 || string.IsNullOrWhiteSpace(monedaParaModificar.AbreviaturaMoneda))
            {
                return BadRequest("monedaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarMoneda = await _pagosBusiness.ModificarMoneda(monedaParaModificar);

                return Ok(wrapperModificarMoneda);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos PayU


        public async Task<IHttpActionResult> ConfigurarPayU(HistorialPagosPersonas pagoParaProcesar)
        {
            if (pagoParaProcesar == null || pagoParaProcesar.Consecutivo <= 0)
            {
                return BadRequest("pagoParaProcesar vacio y/o invalido!.");
            }

            try
            {
                PayUModel payUModel = await _pagosBusiness.ConfigurarPayU(pagoParaProcesar);

                return Ok(payUModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> VerificarSiPagoEstaAprobadoYTraerNotificacion(HistorialPagosPersonas pagoParaVerificar)
        {
            if (pagoParaVerificar == null || pagoParaVerificar.Consecutivo <= 0)
            {
                return BadRequest("pagoParaVerificar vacio y/o invalido!.");
            }

            try
            {
                TimeLineNotificaciones timeLineNotificacion = await _pagosBusiness.VerificarSiPagoEstaAprobadoYTraerNotificacion(pagoParaVerificar);

                return Ok(timeLineNotificacion);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion

    }
}
