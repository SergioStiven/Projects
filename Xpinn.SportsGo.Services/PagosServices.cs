using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class PagosServices : BaseService
    {


        #region Metodos FacturaFormato


        public async Task<WrapperSimpleTypesDTO> AsignarFacturaFormato(FacturaFormatoDTO facturaFormatoParaAsignar)
        {
            if (facturaFormatoParaAsignar == null) throw new ArgumentNullException("No puedes asignar el formato de una factura si facturaFormatoParaAsignar es nula!.");
            if (string.IsNullOrWhiteSpace(facturaFormatoParaAsignar.Texto) || facturaFormatoParaAsignar.CodigoIdioma <= 0
                || facturaFormatoParaAsignar.IdiomaDeLaFacturaFormato == Idioma.SinIdioma)
            {
                throw new ArgumentException("facturaFormatoParaAsignar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarFacturaFormato = await client.PostAsync<FacturaFormatoDTO, WrapperSimpleTypesDTO>("Pagos/AsignarFacturaFormato", facturaFormatoParaAsignar);

            return wrapperAsignarFacturaFormato;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarFacturaFormatoLista(List<FacturaFormatoDTO> facturaFormatoParaAsignar)
        {
            if (facturaFormatoParaAsignar == null) throw new ArgumentNullException("No puedes asignar el formato de una factura si facturaFormatoParaAsignar es nula!.");
            if (facturaFormatoParaAsignar.Count <= 0
                || !facturaFormatoParaAsignar.TrueForAll(x => !string.IsNullOrWhiteSpace(x.Texto) && x.IdiomaDeLaFacturaFormato != Idioma.SinIdioma && x.CodigoPais > 0))
            {
                throw new ArgumentException("facturaFormatoParaAsignar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarFacturaFormatoLista = await client.PostAsync<List<FacturaFormatoDTO>, WrapperSimpleTypesDTO>("Pagos/AsignarFacturaFormatoLista", facturaFormatoParaAsignar);

            return wrapperAsignarFacturaFormatoLista;
        }

        public async Task<FacturaFormatoDTO> BuscarFacturaFormato(FacturaFormatoDTO facturaFormatoParaBuscar)
        {
            if (facturaFormatoParaBuscar == null) throw new ArgumentNullException("No puedes buscar el formato de una factura si facturaFormatoParaBuscar es nula!.");
            if (facturaFormatoParaBuscar.IdiomaDeLaFacturaFormato == Idioma.SinIdioma || facturaFormatoParaBuscar.CodigoPais <= 0)
            {
                throw new ArgumentException("facturaFormatoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            FacturaFormatoDTO facturaFormatoBuscada = await client.PostAsync("Pagos/BuscarFacturaFormato", facturaFormatoParaBuscar);

            return facturaFormatoBuscada;
        }

        public async Task<List<FacturaFormatoDTO>> ListarFacturasFormatos()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<FacturaFormatoDTO> listaFacturaFormatos = await client.PostAsync<List<FacturaFormatoDTO>>("Pagos/ListarFacturasFormatos");

            return listaFacturaFormatos;
        }


        #endregion


        #region Metodos HistorialPagosPersonas


        public async Task<WrapperSimpleTypesDTO> CrearHistorialPagoPersona(HistorialPagosPersonasDTO historialPagoParaCrear)
        {
            if (historialPagoParaCrear == null) throw new ArgumentNullException("No puedes crear un historico de pago si historialPagoParaCrear es nula!.");
            if (string.IsNullOrWhiteSpace(historialPagoParaCrear.TextoFacturaFormato) || historialPagoParaCrear.CodigoPersona <= 0
                || historialPagoParaCrear.CodigoPais <= 0 || historialPagoParaCrear.CodigoPlan <= 0)
            {
                throw new ArgumentException("historialPagoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearHistorialPagoPersona = await client.PostAsync<HistorialPagosPersonasDTO, WrapperSimpleTypesDTO>("Pagos/CrearHistorialPagoPersona", historialPagoParaCrear);

            return wrapperCrearHistorialPagoPersona;
        }

        public async Task<HistorialPagosPersonasDTO> BuscarHistorialPagoPersona(HistorialPagosPersonasDTO historialPagoParaBuscar)
        {
            if (historialPagoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un historico de un pago si historialPagoParaBuscar es nula!.");
            if (historialPagoParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("historialPagoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            HistorialPagosPersonasDTO historialPagoBuscado = await client.PostAsync("Pagos/BuscarHistorialPagoPersona", historialPagoParaBuscar);

            return historialPagoBuscado;
        }

        public async Task<HistorialPagosPersonasDTO> BuscarPagoEnTramiteDeUnaPersona(HistorialPagosPersonasDTO historialPagoParaBuscar)
        {
            if (historialPagoParaBuscar == null) throw new ArgumentNullException("No puedes buscar el pago en tramite si historialPagoParaBuscar es nula!.");
            if (historialPagoParaBuscar.CodigoPersona <= 0 || historialPagoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("historialPagoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            HistorialPagosPersonasDTO pagoEnTramite = await client.PostAsync("Pagos/BuscarPagoEnTramiteDeUnaPersona", historialPagoParaBuscar);

            return pagoEnTramite;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarQueNoExistaUnPagoEnTramite(HistorialPagosPersonasDTO historialPagoParaVerificar)
        {
            if (historialPagoParaVerificar == null) throw new ArgumentNullException("No puedes verificar si hay un pago en tramite si historialPagoParaVerificar es nula!.");
            if (historialPagoParaVerificar.CodigoPersona <= 0)
            {
                throw new ArgumentException("historialPagoParaVerificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperVerificarQueNoExistaUnPagoEnTramite = await client.PostAsync<HistorialPagosPersonasDTO, WrapperSimpleTypesDTO>("Pagos/VerificarQueNoExistaUnPagoEnTramite", historialPagoParaVerificar);

            return wrapperVerificarQueNoExistaUnPagoEnTramite;
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosDeUnaPersona(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los historico de un pago de una persona si buscador es nula!.");
            if (buscador.ConsecutivoPersona <= 0 || buscador.IdiomaBase == Idioma.SinIdioma || buscador.SkipIndexBase < 0 && buscador.TakeIndexBase <= 0)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<HistorialPagosPersonasDTO> listaHistorialPagos = await client.PostAsync<BuscadorDTO, List<HistorialPagosPersonasDTO>>("Pagos/ListarHistorialPagosDeUnaPersona", buscador);

            return listaHistorialPagos;
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosPersonas(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los historico de un pago si buscador es nula!.");

            IHttpClient client = ConfigurarHttpClient();

            List<HistorialPagosPersonasDTO> listaHistorialPagos = await client.PostAsync<BuscadorDTO, List<HistorialPagosPersonasDTO>>("Pagos/ListarHistorialPagosPersonas", buscador);

            return listaHistorialPagos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarEstadoPagoPersona(HistorialPagosPersonasDTO historialPagoParaModificar)
        {
            if (historialPagoParaModificar == null) throw new ArgumentNullException("No puedes modificar el estado de un pago si historialPagoParaModificar es nulo!.");
            if (historialPagoParaModificar.Consecutivo <= 0 || historialPagoParaModificar.CodigoPersona <= 0 || historialPagoParaModificar.CodigoPlan <= 0
                || historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.SinEstadoDelPago)
            {
                throw new ArgumentException("historialPagoParaModificar vacio y/o invalido!.");
            }
            else if (historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.PendientePorAprobar && historialPagoParaModificar.CodigoArchivo <= 0 && string.IsNullOrWhiteSpace(historialPagoParaModificar.ReferenciaPago))
            {
                throw new ArgumentException("historialPagoParaModificar debe tener un archivo y codigo de referencia si se va a reportar!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarEstadoPagoPersona = await client.PostAsync<HistorialPagosPersonasDTO, WrapperSimpleTypesDTO>("Pagos/ModificarEstadoPagoPersona", historialPagoParaModificar);

            return wrapperModificarEstadoPagoPersona;
        }

        /// <summary>
        /// Solo se puede borrar pagos que esten en estado "Espera pago" o "1", devolvera false si no es asi
        /// </summary>
        public async Task<WrapperSimpleTypesDTO> EliminarPagoPendientePorPagar(HistorialPagosPersonasDTO historialPagoParaEliminar)
        {
            if (historialPagoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un pago si historialPagoParaEliminar es nulo!.");
            if (historialPagoParaEliminar.CodigoPersona <= 0 || historialPagoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("historialPagoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarPagoPendientePorPagar = await client.PostAsync<HistorialPagosPersonasDTO, WrapperSimpleTypesDTO>("Pagos/EliminarPagoPendientePorPagar", historialPagoParaEliminar);

            return wrapperEliminarPagoPendientePorPagar;
        }


        #endregion


        #region Metodos Moneda


        public async Task<MonedasDTO> BuscarMoneda(MonedasDTO monedaParaBuscar)
        {
            if (monedaParaBuscar == null) throw new ArgumentNullException("No puedes buscar una moneda si monedaParaBuscar es nulo!.");
            if (monedaParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("monedaParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            MonedasDTO monedaBuscada = await client.PostAsync("Pagos/BuscarMoneda", monedaParaBuscar);

            return monedaBuscada;
        }

        public async Task<List<MonedasDTO>> ListarMonedas()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<MonedasDTO> listaMonedas = await client.PostAsync<List<MonedasDTO>>("Pagos/ListarMonedas");

            return listaMonedas;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMoneda(MonedasDTO monedaParaModificar)
        {
            if (monedaParaModificar == null) throw new ArgumentNullException("No puedes modificar una moneda si monedaParaModificar es nulo!.");
            if (monedaParaModificar.Consecutivo <= 0 || monedaParaModificar.CambioMoneda <= 0 || string.IsNullOrWhiteSpace(monedaParaModificar.AbreviaturaMoneda))
            {
                throw new ArgumentException("monedaParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarMoneda = await client.PostAsync<MonedasDTO,WrapperSimpleTypesDTO>("Pagos/ModificarMoneda", monedaParaModificar);

            return wrapperModificarMoneda;
        }


        #endregion


        #region Metodos PayU


        public async Task<PayUModel> ConfigurarPayU(HistorialPagosPersonasDTO pagoParaProcesar)
        {
            if (pagoParaProcesar == null) throw new ArgumentNullException("No puedes procesar el pago en PayU si pagoParaProcesar es nulo!.");
            if (pagoParaProcesar.Consecutivo <= 0)
            {
                throw new ArgumentException("pagoParaProcesar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            PayUModel payUModel = await client.PostAsync<HistorialPagosPersonasDTO, PayUModel> ("Pagos/ConfigurarPayU", pagoParaProcesar);

            return payUModel;
        }

        public async Task<TimeLineNotificaciones> VerificarSiPagoEstaAprobadoYTraerNotificacion(HistorialPagosPersonasDTO pagoParaVerificar)
        {
            if (pagoParaVerificar == null) throw new ArgumentNullException("No puedes verificar un pago si pagoParaVerificar es nulo!.");
            if (pagoParaVerificar.Consecutivo <= 0)
            {
                throw new ArgumentException("pagoParaVerificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            TimeLineNotificaciones timeLineNotificacion = await client.PostAsync<HistorialPagosPersonasDTO, TimeLineNotificaciones>("Pagos/VerificarSiPagoEstaAprobadoYTraerNotificacion", pagoParaVerificar);

            return timeLineNotificacion;
        }


        #endregion


    }
}