using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Models;

namespace Xpinn.SportsGo.Business
{
    public class PagosBusiness
    {


        #region Metodos FacturasFormato


        public async Task<WrapperSimpleTypesDTO> AsignarFacturaFormato(FacturaFormato facturaFormatoParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                bool existe = await pagosRepo.BuscarSiExisteFacturaFormato(facturaFormatoParaAsignar);

                if (existe)
                {
                    FacturaFormato facturaFormatoExistente = await pagosRepo.ModificarFacturaFormato(facturaFormatoParaAsignar);
                }
                else
                {
                    pagosRepo.CrearFacturaFormato(facturaFormatoParaAsignar);
                }

                WrapperSimpleTypesDTO wrapperAsignarFacturaFormato = new WrapperSimpleTypesDTO();

                wrapperAsignarFacturaFormato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarFacturaFormato.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarFacturaFormato.Exitoso = true;
                    wrapperAsignarFacturaFormato.ConsecutivoCreado = facturaFormatoParaAsignar.Consecutivo;
                }

                return wrapperAsignarFacturaFormato;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarFacturaFormatoLista(List<FacturaFormato> facturaFormatoParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                foreach (FacturaFormato factura in facturaFormatoParaAsignar)
                {
                    bool existe = await pagosRepo.BuscarSiExisteFacturaFormato(factura);

                    if (existe)
                    {
                        FacturaFormato facturaFormatoExistente = await pagosRepo.ModificarFacturaFormato(factura);
                    }
                    else
                    {
                        pagosRepo.CrearFacturaFormato(factura);
                    }
                }

                WrapperSimpleTypesDTO wrapperAsignarFacturaFormatoLista = new WrapperSimpleTypesDTO();

                wrapperAsignarFacturaFormatoLista.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarFacturaFormatoLista.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarFacturaFormatoLista.Exitoso = true;
                }

                return wrapperAsignarFacturaFormatoLista;
            }
        }

        public async Task<FacturaFormato> BuscarFacturaFormato(FacturaFormato facturaFormatoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                FacturaFormato facturaFormatoBuscado = await pagosRepo.BuscarFacturaFormato(facturaFormatoParaBuscar);

                return facturaFormatoBuscado;
            }
        }

        public async Task<List<FacturaFormato>> ListarFacturasFormatos()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                List<FacturaFormato> listaFacturasFormato = await pagosRepo.ListarFacturasFormatos();

                return listaFacturasFormato;
            }
        }


        #endregion


        #region Metodos HistorialPagosPersonas


        public async Task<WrapperSimpleTypesDTO> CrearHistorialPagoPersona(HistorialPagosPersonas historialPagoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                bool existePagoEnTramite = await pagosRepo.VerificarQueNoExistaUnPagoEnTramite(historialPagoParaCrear);

                if (existePagoEnTramite)
                {
                    throw new InvalidOperationException("Ya existe un pago en tramite, espera por la aprobacion o cancelalo si esta en pendiente!.");
                }

                historialPagoParaCrear.FechaPago = DateTime.Now;
                historialPagoParaCrear.EstadoDelPago = EstadoDeLosPagos.EsperaPago;

                Monedas monedaDelPais = await pagosRepo.BuscarMonedaDeUnPais(historialPagoParaCrear.CodigoPais);
                historialPagoParaCrear.CodigoMoneda = monedaDelPais.Consecutivo;

                PlanesRepository planRepo = new PlanesRepository(context);
                decimal? precioDelPlan = await planRepo.BuscarPrecioDeUnPlan(historialPagoParaCrear.CodigoPlan);

                historialPagoParaCrear.Precio = precioDelPlan.Value;

                historialPagoParaCrear.Paises = null;
                historialPagoParaCrear.Monedas = null;
                historialPagoParaCrear.Personas = null;
                historialPagoParaCrear.Planes = null;
                historialPagoParaCrear.EstadoPago = null;

                pagosRepo.CrearHistorialPagoPersona(historialPagoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearHistorialPagoPersona = new WrapperSimpleTypesDTO();

                wrapperCrearHistorialPagoPersona.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearHistorialPagoPersona.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearHistorialPagoPersona.Exitoso = true;
                    wrapperCrearHistorialPagoPersona.ConsecutivoCreado = historialPagoParaCrear.Consecutivo;
                }

                return wrapperCrearHistorialPagoPersona;
            }
        }

        public async Task<HistorialPagosPersonasDTO> BuscarHistorialPagoPersona(HistorialPagosPersonas historialPagoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                HistorialPagosPersonasDTO historialPagoBuscado = await pagosRepo.BuscarHistorialPagoPersona(historialPagoParaBuscar);

                if (historialPagoBuscado != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    historialPagoBuscado.FechaPago = helper.ConvertDateTimeFromAnotherTimeZone(historialPagoParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, historialPagoBuscado.FechaPago);
                }

                return historialPagoBuscado;
            }
        }

        public async Task<HistorialPagosPersonasDTO> BuscarPagoEnTramiteDeUnaPersona(HistorialPagosPersonas historialPagoParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                HistorialPagosPersonasDTO pagoEnTramite = await pagosRepo.BuscarPagoEnTramiteDeUnaPersona(historialPagoParaVerificar);

                if (pagoEnTramite != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    pagoEnTramite.FechaPago = helper.ConvertDateTimeFromAnotherTimeZone(historialPagoParaVerificar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, pagoEnTramite.FechaPago);
                }

                return pagoEnTramite;
            }
        }

        public async Task<WrapperSimpleTypesDTO> VerificarQueNoExistaUnPagoEnTramite(HistorialPagosPersonas historialPagoParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                bool pagoEstaEnTramite = await pagosRepo.VerificarQueNoExistaUnPagoEnTramite(historialPagoParaVerificar);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO
                {
                    PagoEnTramite = pagoEstaEnTramite
                };

                return wrapper;
            }
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosDeUnaPersona(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                List<HistorialPagosPersonasDTO> listaHistorialPagos = await pagosRepo.ListarHistorialPagosDeUnaPersona(buscador);

                if (listaHistorialPagos != null && listaHistorialPagos.Count > 0)
                {
                    Monedas monedaColombiana = await pagosRepo.BuscarMonedaColombiana();

                    Dictionary<MonedasEnum, string> diccionarioAbreviaturas = listaHistorialPagos.Where(x => x.Monedas.MonedaEnum != MonedasEnum.PesosColombianos).Select(x => x.Monedas).DistinctBy(x => x.MonedaEnum).ToDictionary(x => x.MonedaEnum, x => x.AbreviaturaMoneda);
                    Dictionary<MonedasEnum, decimal> diccionarioCambios = new Dictionary<MonedasEnum, decimal>();

                    QueryMoneyExchanger queryExchanger = new QueryMoneyExchanger();
                    foreach (MonedasEnum monedaParaConsultar in diccionarioAbreviaturas.Keys)
                    {
                        if (monedaParaConsultar != MonedasEnum.PesosColombianos)
                        {
                            YahooExchangeEntity exchangeEntity = await queryExchanger.QueryMoneyExchange(monedaColombiana.AbreviaturaMoneda, diccionarioAbreviaturas[monedaParaConsultar]);
                            Monedas monedaBuscada = await pagosRepo.BuscarMoneda(monedaParaConsultar);

                            if (exchangeEntity != null)
                            {
                                monedaBuscada.CambioMoneda = exchangeEntity.Query.Results.Rate.Rate;
                            }

                            diccionarioCambios.Add(monedaParaConsultar, monedaBuscada.CambioMoneda);
                        }
                        else
                        {
                            diccionarioCambios.Add(monedaParaConsultar, monedaColombiana.CambioMoneda);
                        }
                    }

                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    foreach (HistorialPagosPersonasDTO historial in listaHistorialPagos)
                    {
                        if (historial.Monedas.MonedaEnum != MonedasEnum.PesosColombianos)
                        {
                            historial.Precio *= diccionarioCambios[historial.Monedas.MonedaEnum];
                        }

                        historial.FechaPago = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, historial.FechaPago);
                    }
                }

                try
                {
                    // No es obligatorio para el paso que actualize el cambio de moneda
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {

                }

                return listaHistorialPagos;
            }
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosPersonas(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                List<HistorialPagosPersonasDTO> listaHistorialPagos = await pagosRepo.ListarHistorialPagosPersonas(buscador);

                if (listaHistorialPagos != null && listaHistorialPagos.Count > 0)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    foreach (HistorialPagosPersonasDTO historial in listaHistorialPagos)
                    {
                        historial.FechaPago = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, historial.FechaPago);
                    }
                }

                return listaHistorialPagos;
            }
        }

        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> ModificarEstadoPagoPersona(HistorialPagosPersonas historialPagoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                Notificaciones notificacion = null;

                // Modifico el estado y la fecha de pago de este historial
                HistorialPagosPersonas historialPagoExistente = await pagosRepo.ModificarEstadoPagoPersona(historialPagoParaModificar);

                // Si el pago fue aprobado
                if (historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.Aprobado)
                {
                    PlanesRepository planRepo = new PlanesRepository(context);

                    // Busco el codigo de plan de la persona asignado cuando se registro
                    // Tabla PlanUsuarios, no es el plan en si, es el registro que lleva el control del plan para ese usuario
                    int? codigoPlanUsuario = await planRepo.BuscarCodigoPlanUsuarioPorCodigoPersona(historialPagoExistente.CodigoPersona);

                    PlanesBusiness planBusiness = new PlanesBusiness();

                    // Armo la entidad del nuevo plan
                    PlanesUsuarios planUsuario = new PlanesUsuarios
                    {
                        Consecutivo = codigoPlanUsuario.Value,
                        CodigoPlanDeseado = historialPagoExistente.CodigoPlan
                    };

                    // Cambio el plan para esa persona
                    await planBusiness.CambiarDePlanUsuario(planUsuario);

                    // Si el pago que me es mandando esta marcado como PayU
                    // Actualizo las observaciones y la referencia del pago ya que es todo en uno
                    if (historialPagoParaModificar.EsPagoPorPayU)
                    {
                        historialPagoExistente.ReferenciaPago = historialPagoParaModificar.ReferenciaPago;
                        historialPagoExistente.ObservacionesCliente = historialPagoParaModificar.ObservacionesCliente;
                    }

                    // Armamos una notificacion para el nuevo plan
                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                    notificacion = new Notificaciones
                    {
                        CodigoTipoNotificacion = (int)TipoNotificacionEnum.PlanAprobado,
                        CodigoPlanNuevo = historialPagoExistente.CodigoPlan,
                        CodigoPersonaDestinoAccion = historialPagoExistente.CodigoPersona,
                        Creacion = DateTime.Now
                    };

                    noticiasRepo.CrearNotificacion(notificacion);
                }
                else if (historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.Rechazado)
                {
                    historialPagoExistente.ObservacionesAdministrador = historialPagoParaModificar.ObservacionesAdministrador;

                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                    notificacion = new Notificaciones
                    {
                        CodigoTipoNotificacion = (int)TipoNotificacionEnum.PlanRechazado,
                        CodigoPlanNuevo = historialPagoParaModificar.CodigoPlan,
                        CodigoPersonaDestinoAccion = historialPagoParaModificar.CodigoPersona,
                        Creacion = DateTime.Now
                    };

                    noticiasRepo.CrearNotificacion(notificacion);
                }
                else if (historialPagoParaModificar.EstadoDelPago == EstadoDeLosPagos.PendientePorAprobar)
                {
                    if (string.IsNullOrWhiteSpace(historialPagoParaModificar.ReferenciaPago) || historialPagoParaModificar.CodigoArchivo <= 0)
                    {
                        throw new InvalidOperationException("No puedes reportar el pago de un plan si no ofreces la referencia del pago y/o el archivo");
                    }

                    historialPagoExistente.ReferenciaPago = historialPagoParaModificar.ReferenciaPago;
                    historialPagoExistente.CodigoArchivo = historialPagoParaModificar.CodigoArchivo;
                    historialPagoExistente.ObservacionesCliente = historialPagoParaModificar.ObservacionesCliente;
                }

                WrapperSimpleTypesDTO wrapperModificarEstadoPagoPersona = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperModificarEstadoPagoPersona.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarEstadoPagoPersona.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarEstadoPagoPersona.Exitoso = true;

                    if (notificacion != null && notificacion.Consecutivo > 0)
                    {
                        NoticiasRepository noticiasRepo = new NoticiasRepository(context);

                        if (notificacion.CodigoPersonaDestinoAccion.HasValue && notificacion.CodigoPersonaDestinoAccion > 0)
                        {
                            PersonasRepository personaRepo = new PersonasRepository(context);

                            int codigoIdioma = await personaRepo.BuscarCodigoIdiomaDeLaPersona(notificacion.CodigoPersonaDestinoAccion.Value);
                            notificacion.CodigoIdiomaUsuarioBase = codigoIdioma;
                        }

                        timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    }
                }

                return Tuple.Create(wrapperModificarEstadoPagoPersona, timeLineNotificacion);
            }
        }

        public async Task<TimeLineNotificaciones> VerificarSiPagoEstaAprobadoYTraerNotificacion(HistorialPagosPersonas pagoParaVerificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                int? codigoEstado = await pagosRepo.BuscarEstadoDeUnPago(pagoParaVerificar);

                if (!codigoEstado.HasValue)
                {
                    throw new InvalidOperationException("No se pudo encontrar el estado del pago");
                }

                TimeLineNotificaciones timeLineNotificacion = null;
                if (codigoEstado.Value == (int)EstadoDeLosPagos.Aprobado)
                {
                    Notificaciones notificacionDelPago = await pagosRepo.BuscarNotificacionDeUnPago(pagoParaVerificar);

                    PersonasRepository personaRepo = new PersonasRepository(context);
                    int codigoIdioma = await personaRepo.BuscarCodigoIdiomaDeLaPersona(notificacionDelPago.CodigoPersonaDestinoAccion.Value);
                    notificacionDelPago.CodigoIdiomaUsuarioBase = codigoIdioma;

                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                    timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacionDelPago));

                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    timeLineNotificacion.CreacionNotificacion = helper.ConvertDateTimeFromAnotherTimeZone(pagoParaVerificar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, timeLineNotificacion.CreacionNotificacion);
                }

                return timeLineNotificacion;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPagoPendientePorPagar(HistorialPagosPersonas historialPagoPersonaParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                bool estaEnPendiente = await pagosRepo.VerificarQuePagoEstaPendientePorPagar(historialPagoPersonaParaEliminar);

                if (!estaEnPendiente)
                {
                    throw new InvalidOperationException("Solo puedes eliminar un pago si esta en estado pendiente por pagar!.");
                }

                int? codigoArchivo = await pagosRepo.BuscarCodigoArchivoDelHistorico(historialPagoPersonaParaEliminar);
                pagosRepo.EliminarHistorialPagoPersona(historialPagoPersonaParaEliminar);

                if (codigoArchivo.HasValue && codigoArchivo > 0)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);

                    Archivos archivoParaBorrar = new Archivos
                    {
                        Consecutivo = codigoArchivo.Value
                    };

                    archivoRepo.EliminarArchivo(archivoParaBorrar);
                }

                WrapperSimpleTypesDTO wrapperEliminarPagoPendientePorPagar = new WrapperSimpleTypesDTO();

                wrapperEliminarPagoPendientePorPagar.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarPagoPendientePorPagar.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarPagoPendientePorPagar.Exitoso = true;
                }

                return wrapperEliminarPagoPendientePorPagar;
            }
        }


        #endregion


        #region Metodos Monedas


        public async Task<Monedas> BuscarMoneda(Monedas monedaParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                Monedas monedaBuscada = await pagosRepo.BuscarMoneda(monedaParaBuscar);

                return monedaBuscada;
            }
        }

        public async Task<List<Monedas>> ListarMonedas()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                List<Monedas> listaMonedas = await pagosRepo.ListarMonedas();

                QueryMoneyExchanger exchanger = new QueryMoneyExchanger();
                Monedas monedaColombiana = listaMonedas.Where(x => x.MonedaEnum == MonedasEnum.PesosColombianos).FirstOrDefault();

                foreach (Monedas moneda in listaMonedas.Where(x => x.MonedaEnum != MonedasEnum.PesosColombianos))
                {
                    YahooExchangeEntity exchangeEntity = await exchanger.QueryMoneyExchange(monedaColombiana.AbreviaturaMoneda, moneda.AbreviaturaMoneda);

                    if (exchangeEntity != null)
                    {
                        moneda.CambioMoneda = exchangeEntity.Query.Results.Rate.Rate;
                    }
                }

                try
                {
                    // No es obligatorio para el paso que actualize el cambio de moneda
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {

                }

                return listaMonedas;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMoneda(Monedas monedaParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);
                Monedas monedaExistente = await pagosRepo.ModificarMoneda(monedaParaModificar);

                WrapperSimpleTypesDTO wrapperModificarMoneda = new WrapperSimpleTypesDTO();

                wrapperModificarMoneda.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarMoneda.NumeroRegistrosAfectados > 0) wrapperModificarMoneda.Exitoso = true;

                return wrapperModificarMoneda;
            }
        }


        #endregion


        #region Metodos PayU


        public async Task<PayUModel> ConfigurarPayU(HistorialPagosPersonas pagoParaProcesar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PagosRepository pagosRepo = new PagosRepository(context);

                HistorialPagosPersonasDTO pagoBuscado = await pagosRepo.BuscarHistorialPagoPersona(pagoParaProcesar);

                if (pagoBuscado.EstadoDelPago != EstadoDeLosPagos.EsperaPago && pagoBuscado.EstadoDelPago != EstadoDeLosPagos.Rechazado)
                {
                    throw new InvalidOperationException("El pago no se puede procesar porque no esta en un estado valido");
                }

                string descripcion = string.Empty;
                switch (pagoBuscado.Personas.IdiomaDeLaPersona)
                {
                    case Idioma.Español:
                        descripcion = "PayU Pago para el plan " + pagoBuscado.Planes.DescripcionIdiomaBuscado;
                        break;
                    case Idioma.Ingles:
                        descripcion = "PayU Payment for plan " + pagoBuscado.Planes.DescripcionIdiomaBuscado;
                        break;
                    case Idioma.Portugues:
                        descripcion = "PayU Plano de pagamento " + pagoBuscado.Planes.DescripcionIdiomaBuscado;
                        break;
                }

                HasherCryptoService<MD5Cng> hashingService = new HasherCryptoService<MD5Cng>();
                string signatureBuilder = AppConstants.PayUApiKey + "~" + AppConstants.PayUMerchantID + "~" + pagoBuscado.Consecutivo + "~" + Math.Round(pagoBuscado.Precio, 2, MidpointRounding.AwayFromZero).ToString().Replace(",", ".") + "~COP";

                PayUModel payUModel = new PayUModel
                {
                    merchantId = AppConstants.PayUMerchantID,
                    accountId = AppConstants.PayUAccountID,
                    tax = 0,
                    taxReturnBase = 0,
                    test = AppConstants.PayUTest,
                    Url = AppConstants.PayUURL,
                    currency = "COP",
                    amount = pagoBuscado.Precio,
                    referenceCode = pagoBuscado.Consecutivo.ToString(),
                    buyerEmail = pagoBuscado.Personas.Usuarios.Email,
                    description = descripcion,
                    signature = hashingService.GetStringHash(signatureBuilder)
                };

                return payUModel;
            }
        }


        #endregion


    }
}
