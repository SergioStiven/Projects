using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Repositories
{
    public class PagosRepository
    {
        SportsGoEntities _context;

        public PagosRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos FacturaFormato


        public void CrearFacturaFormato(FacturaFormato facturaFormatoParaCrear)
        {
            _context.FacturaFormato.Add(facturaFormatoParaCrear);
        }

        public async Task<FacturaFormato> BuscarFacturaFormato(FacturaFormato facturaFormatoParaBuscar)
        {
            FacturaFormato facturaFormatoBuscado = await _context.FacturaFormato.Where(x => x.CodigoIdioma == facturaFormatoParaBuscar.CodigoIdioma && x.CodigoPais == facturaFormatoParaBuscar.CodigoPais).AsNoTracking().FirstOrDefaultAsync();

            return facturaFormatoBuscado;
        }

        public async Task<bool> BuscarSiExisteFacturaFormato(FacturaFormato facturaFormatoParaBuscar)
        {
            bool existe = await _context.FacturaFormato.Where(x => x.CodigoIdioma == facturaFormatoParaBuscar.CodigoIdioma && x.CodigoPais == facturaFormatoParaBuscar.CodigoPais).AnyAsync();

            return existe;
        }

        public async Task<List<FacturaFormato>> ListarFacturasFormatos()
        {
            List<FacturaFormato> listarFacturasFormato = await _context.FacturaFormato.AsNoTracking().ToListAsync();

            return listarFacturasFormato;
        }

        public async Task<FacturaFormato> ModificarFacturaFormato(FacturaFormato facturaFormatoParaModificar)
        {
            FacturaFormato facturaFormatoExistente = await _context.FacturaFormato.Where(x => x.CodigoIdioma == facturaFormatoParaModificar.CodigoIdioma && x.CodigoPais == facturaFormatoParaModificar.CodigoPais).FirstOrDefaultAsync();

            facturaFormatoExistente.Texto = facturaFormatoParaModificar.Texto;

            return facturaFormatoExistente;
        }


        #endregion


        #region Metodos HistorialPagosPersonas


        public void CrearHistorialPagoPersona(HistorialPagosPersonas historialPagoPersonaParaCrear)
        {
            _context.HistorialPagosPersonas.Add(historialPagoPersonaParaCrear);
        }

        public async Task<HistorialPagosPersonasDTO> BuscarHistorialPagoPersona(HistorialPagosPersonas historialPagoParaBuscar)
        {
            HistorialPagosPersonasDTO historialPagoBuscado = await _context.HistorialPagosPersonas.Where(x => x.Consecutivo == historialPagoParaBuscar.Consecutivo)
                .Select(x => new HistorialPagosPersonasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoArchivo = x.CodigoArchivo,
                    CodigoEstado = x.CodigoEstado,
                    CodigoMoneda = x.CodigoMoneda,
                    CodigoPais = x.CodigoPais,
                    CodigoPersona = x.CodigoPersona,
                    CodigoPlan = x.CodigoPlan,
                    FechaPago = x.FechaPago,
                    ObservacionesAdministrador = x.ObservacionesAdministrador,
                    ObservacionesCliente = x.ObservacionesCliente,
                    Precio = x.Precio,
                    ReferenciaPago = x.ReferenciaPago,
                    TextoFacturaFormato = x.TextoFacturaFormato,
                    Personas = new PersonasDTO
                    {
                        CodigoIdioma = x.Personas.CodigoIdioma,
                        Usuarios = new UsuariosDTO
                        {
                            Email = x.Personas.Usuarios.Email
                        }
                    },
                    Planes = new PlanesDTO
                    {
                        DescripcionIdiomaBuscado = x.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == x.Personas.CodigoIdioma).Select(y => y.Descripcion).FirstOrDefault()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return historialPagoBuscado;
        }

        public async Task<bool> VerificarQueNoExistaUnPagoEnTramite(HistorialPagosPersonas historialPagoParaVerificar)
        {
            int estadoPendiente = (int)EstadoDeLosPagos.EsperaPago;
            int estadoPorAprobar = (int)EstadoDeLosPagos.PendientePorAprobar;

            bool pagoEnTramite = await _context.HistorialPagosPersonas.Where(x => x.CodigoPersona == historialPagoParaVerificar.CodigoPersona && (x.CodigoEstado == estadoPendiente || x.CodigoEstado == estadoPorAprobar)).AnyAsync();

            return pagoEnTramite;
        }

        public async Task<HistorialPagosPersonasDTO> BuscarPagoEnTramiteDeUnaPersona(HistorialPagosPersonas historialPagoParaVerificar)
        {
            int estadoPendiente = (int)EstadoDeLosPagos.EsperaPago;
            int estadoPorAprobar = (int)EstadoDeLosPagos.PendientePorAprobar;

            HistorialPagosPersonasDTO pagoEnTramite = await _context.HistorialPagosPersonas.Where(x => x.CodigoPersona == historialPagoParaVerificar.CodigoPersona && (x.CodigoEstado == estadoPendiente || x.CodigoEstado == estadoPorAprobar))
                                                                    .Select(x => new HistorialPagosPersonasDTO
                                                                    {
                                                                        Consecutivo = x.Consecutivo,
                                                                        CodigoEstado = x.CodigoEstado,
                                                                        FechaPago = x.FechaPago,
                                                                        CodigoPais = x.CodigoPais,
                                                                        CodigoPersona = x.CodigoPersona,
                                                                        CodigoPlan = x.CodigoPlan,
                                                                        Precio = x.Precio,
                                                                        TextoFacturaFormato = x.TextoFacturaFormato,
                                                                        CodigoArchivo = x.CodigoArchivo,
                                                                        ReferenciaPago = x.ReferenciaPago,
                                                                        ObservacionesAdministrador = x.ObservacionesAdministrador,
                                                                        ObservacionesCliente = x.ObservacionesCliente,
                                                                        Planes = new PlanesDTO
                                                                        {
                                                                            Consecutivo = x.Planes.Consecutivo,
                                                                            CodigoArchivo = x.Planes.CodigoArchivo,
                                                                            DescripcionIdiomaBuscado = x.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == historialPagoParaVerificar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                                                                        },
                                                                        Paises = new PaisesDTO
                                                                        {
                                                                            Consecutivo = x.Paises.Consecutivo,
                                                                            CodigoArchivo = x.Paises.CodigoArchivo,
                                                                            DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == historialPagoParaVerificar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                                                                        }
                                                                    })
                                                                    .AsNoTracking()
                                                                    .FirstOrDefaultAsync();

            return pagoEnTramite;
        }

        public async Task<bool> VerificarQuePagoEstaPendientePorPagar(HistorialPagosPersonas historialPagoParaVerificar)
        {
            int estadoPendiente = (int)EstadoDeLosPagos.EsperaPago;

            bool estaEnPendiente = await _context.HistorialPagosPersonas.Where(x => x.CodigoPersona == historialPagoParaVerificar.CodigoPersona && x.CodigoEstado == estadoPendiente && x.Consecutivo == historialPagoParaVerificar.Consecutivo).AnyAsync();

            return estaEnPendiente;
        }

        public async Task<int?> BuscarEstadoDeUnPago(HistorialPagosPersonas historialPagoParaVerificar)
        {
            int? codigoEstado = await _context.HistorialPagosPersonas.Where(x => x.Consecutivo == historialPagoParaVerificar.Consecutivo).Select(x => x.CodigoEstado).FirstOrDefaultAsync();

            return codigoEstado;
        }

        public async Task<int?> BuscarCodigoArchivoDelHistorico(HistorialPagosPersonas historialPagoParaVerificar)
        {
            int? codigoArchivo = await _context.HistorialPagosPersonas.Where(x => x.Consecutivo == historialPagoParaVerificar.Consecutivo).Select(x => x.CodigoArchivo).FirstOrDefaultAsync();

            return codigoArchivo;
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosDeUnaPersona(BuscadorDTO buscador)
        {
            IQueryable<HistorialPagosPersonas> queryHistorial = _context.HistorialPagosPersonas.Where(x => x.CodigoPersona == buscador.ConsecutivoPersona).AsQueryable();

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (buscador.EstadosParaBuscar != null && buscador.EstadosParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.EstadosParaBuscar.Contains(x.CodigoEstado));
            }

            if (buscador.PlanesParaBuscar != null && buscador.PlanesParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.PlanesParaBuscar.Contains(x.CodigoPlan));
            }

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryHistorial = queryHistorial.Where(x => x.FechaPago >= buscador.FechaInicio);
            }

            if (buscador.FechaFinal != DateTime.MinValue)
            {
                queryHistorial = queryHistorial.Where(x => x.FechaPago <= buscador.FechaFinal);
            }

            List<HistorialPagosPersonasDTO> listaHistorialPagoPersonas = await queryHistorial
                .Select(x => new HistorialPagosPersonasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoEstado = x.CodigoEstado,
                    FechaPago = x.FechaPago,
                    CodigoPais = x.CodigoPais,
                    CodigoPersona = x.CodigoPersona,
                    CodigoPlan = x.CodigoPlan,
                    Precio = x.Precio,
                    TextoFacturaFormato = x.TextoFacturaFormato,
                    CodigoArchivo = x.CodigoArchivo,
                    ReferenciaPago = x.ReferenciaPago,
                    ObservacionesAdministrador = x.ObservacionesAdministrador,
                    ObservacionesCliente = x.ObservacionesCliente,
                    CodigoMoneda = x.CodigoMoneda,
                    Monedas = new MonedasDTO
                    {
                        Consecutivo = x.Monedas.Consecutivo,
                        Descripcion = x.Monedas.Descripcion,
                        CambioMoneda = x.Monedas.CambioMoneda,
                        AbreviaturaMoneda = x.Monedas.AbreviaturaMoneda
                    },
                    Planes = new PlanesDTO
                    {
                        Consecutivo = x.Planes.Consecutivo,
                        CodigoArchivo = x.Planes.CodigoArchivo,
                        CodigoPeriodicidad = x.Planes.CodigoPeriodicidad,
                        DescripcionIdiomaBuscado = x.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    },
                    Paises = new PaisesDTO
                    {
                        Consecutivo = x.Paises.Consecutivo,
                        CodigoArchivo = x.Paises.CodigoArchivo,
                        CodigoMoneda = x.Paises.CodigoMoneda,
                        DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    }
                })
                .OrderByDescending(x => x.FechaPago)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaHistorialPagoPersonas;
        }

        public async Task<List<HistorialPagosPersonasDTO>> ListarHistorialPagosPersonas(BuscadorDTO buscador)
        {
            IQueryable<HistorialPagosPersonas> queryHistorial = _context.HistorialPagosPersonas.AsQueryable();

            if (buscador.ConsecutivoPersona != 0)
            {
                queryHistorial = queryHistorial.Where(x => x.CodigoPersona == buscador.ConsecutivoPersona);
            }

            if (buscador.PaisesParaBuscar != null && buscador.PaisesParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.PaisesParaBuscar.Contains(x.CodigoPais));
            }

            if (buscador.EstadosParaBuscar != null && buscador.EstadosParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.EstadosParaBuscar.Contains(x.CodigoEstado));
            }

            if (buscador.PlanesParaBuscar != null && buscador.PlanesParaBuscar.Count > 0)
            {
                queryHistorial = queryHistorial.Where(x => buscador.PlanesParaBuscar.Contains(x.CodigoPlan));
            }

            if (buscador.FechaInicio != DateTime.MinValue)
            {
                queryHistorial = queryHistorial.Where(x => x.FechaPago >= buscador.FechaInicio);
            }

            if (buscador.FechaFinal != DateTime.MinValue)
            {
                queryHistorial = queryHistorial.Where(x => x.FechaPago <= buscador.FechaFinal);
            }

            List<HistorialPagosPersonasDTO> listaHistorialPagoPersonas = await queryHistorial
                .Select(x => new HistorialPagosPersonasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoEstado = x.CodigoEstado,
                    FechaPago = x.FechaPago,
                    CodigoPais = x.CodigoPais,
                    CodigoPersona = x.CodigoPersona,
                    CodigoPlan = x.CodigoPlan,
                    Precio = x.Precio,
                    TextoFacturaFormato = x.TextoFacturaFormato,
                    CodigoArchivo = x.CodigoArchivo,
                    ReferenciaPago = x.ReferenciaPago,
                    CodigoMoneda = x.CodigoMoneda,
                    ObservacionesAdministrador = x.ObservacionesAdministrador,
                    ObservacionesCliente = x.ObservacionesCliente,
                    Personas = new PersonasDTO
                    {
                        Consecutivo = x.Personas.Consecutivo,
                        Nombres = x.Personas.Nombres,
                        Apellidos = x.Personas.Apellidos,
                        CodigoArchivoImagenPerfil = x.Personas.CodigoArchivoImagenPerfil,
                        CodigoTipoPerfil = x.Personas.CodigoTipoPerfil,
                        Skype = x.Personas.Skype,
                        Telefono = x.Personas.Telefono,
                        CiudadResidencia = x.Personas.CiudadResidencia,
                        CodigoUsuario = x.Personas.CodigoUsuario,
                        Usuarios = new UsuariosDTO
                        {
                            Consecutivo = x.Personas.Usuarios.Consecutivo,
                            Usuario = x.Personas.Usuarios.Usuario,
                            Email = x.Personas.Usuarios.Email,
                        }
                    },
                    Planes = new PlanesDTO
                    {
                        Consecutivo = x.Planes.Consecutivo,
                        CodigoArchivo = x.Planes.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Planes.PlanesContenidos.Where(y => y.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    },
                    Paises = new PaisesDTO
                    {
                        Consecutivo = x.Paises.Consecutivo,
                        CodigoArchivo = x.Paises.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == buscador.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault()
                    }
                })
                .OrderByDescending(x => x.FechaPago)
                .Skip(() => buscador.SkipIndexBase)
                .Take(() => buscador.TakeIndexBase)
                .AsNoTracking()
                .ToListAsync();

            return listaHistorialPagoPersonas;
        }

        public async Task<Notificaciones> BuscarNotificacionDeUnPago(HistorialPagosPersonas pagoParaVerificar)
        {
            int codigoNotificacionAprobada = (int)TipoNotificacionEnum.PlanAprobado;
            Notificaciones notificacionDelPago = await _context.Notificaciones.Where(x => x.CodigoPlanNuevo == _context.HistorialPagosPersonas.Where(z => z.Consecutivo == pagoParaVerificar.Consecutivo).Select(z => z.CodigoPlan).FirstOrDefault() && x.CodigoPersonaDestinoAccion == _context.HistorialPagosPersonas.Where(z => z.Consecutivo == pagoParaVerificar.Consecutivo).Select(z => z.CodigoPersona).FirstOrDefault() && x.CodigoTipoNotificacion == codigoNotificacionAprobada).OrderByDescending(x => x.Creacion).FirstOrDefaultAsync();

            return notificacionDelPago;
        }

        public async Task<HistorialPagosPersonas> AsignarArchivoHistorialPago(HistorialPagosPersonas historialPagoPersonaParaModificar)
        {
            HistorialPagosPersonas historialPagoExistente = await _context.HistorialPagosPersonas.Where(x => x.Consecutivo == historialPagoPersonaParaModificar.Consecutivo).FirstOrDefaultAsync();

            historialPagoExistente.CodigoArchivo = historialPagoPersonaParaModificar.CodigoArchivo;

            return historialPagoExistente;
        }

        public async Task<HistorialPagosPersonas> ModificarEstadoPagoPersona(HistorialPagosPersonas historialPagoPersonaParaModificar)
        {
            HistorialPagosPersonas historialPagoExistente = await _context.HistorialPagosPersonas.Where(x => x.Consecutivo == historialPagoPersonaParaModificar.Consecutivo).FirstOrDefaultAsync();

            historialPagoExistente.FechaPago = DateTime.Now;
            historialPagoExistente.CodigoEstado = historialPagoPersonaParaModificar.CodigoEstado;

            return historialPagoExistente;
        }

        public void EliminarHistorialPagoPersona(HistorialPagosPersonas historialPagoPersonaParaEliminar)
        {
            _context.HistorialPagosPersonas.Attach(historialPagoPersonaParaEliminar);
            _context.HistorialPagosPersonas.Remove(historialPagoPersonaParaEliminar);
        }


        #endregion


        #region Metodos Moneda


        public async Task<Monedas> BuscarMonedaColombiana()
        {
            int monedaColombianaCodigo = (int)MonedasEnum.PesosColombianos;

            Monedas monedaColombiana = await _context.Monedas.Where(x => x.Consecutivo == monedaColombianaCodigo).AsNoTracking().FirstOrDefaultAsync();

            return monedaColombiana;
        }

        public async Task<Monedas> BuscarMonedaDeUnUsuario(int codigoUsuario)
        {
            Monedas monedaDeUnUsuario = await (from persona in _context.Personas
                                               where persona.CodigoUsuario == codigoUsuario
                                               select persona.Paises.Monedas).FirstOrDefaultAsync();

            return monedaDeUnUsuario;
        }

        public async Task<Monedas> BuscarMonedaDeUnPais(int codigoPais)
        {
            Monedas monedaDelPais = await _context.Paises.Where(x => x.Consecutivo == codigoPais).Select(x => x.Monedas).AsNoTracking().FirstOrDefaultAsync();

            return monedaDelPais;
        }

        public async Task<Monedas> ModificarMoneda(Monedas monedaParaModificar)
        {
            Monedas monedaExistente = await _context.Monedas.Where(x => x.Consecutivo == monedaParaModificar.Consecutivo).FirstOrDefaultAsync();

            monedaExistente.CambioMoneda = monedaParaModificar.CambioMoneda;
            monedaExistente.AbreviaturaMoneda = monedaParaModificar.AbreviaturaMoneda;

            return monedaExistente;
        }

        public async Task<Monedas> BuscarMoneda(Monedas monedaParaBuscar)
        {
            Monedas monedaBuscada = await (from moneda in _context.Monedas
                                           where moneda.Consecutivo == monedaParaBuscar.Consecutivo
                                           select moneda).FirstOrDefaultAsync();
            return monedaBuscada;
        }

        public async Task<Monedas> BuscarMoneda(MonedasEnum monedaParaBuscar)
        {
            int codigoMoneda = (int)monedaParaBuscar;
            Monedas monedaBuscada = await (from moneda in _context.Monedas
                                           where moneda.Consecutivo == codigoMoneda
                                           select moneda).FirstOrDefaultAsync();
            return monedaBuscada;
        }

        public async Task<List<Monedas>> ListarMonedas()
        {
            List<Monedas> listaMonedas = await (from moneda in _context.Monedas
                                                select moneda).ToListAsync();

            return listaMonedas;
        }


        #endregion


    }
}
