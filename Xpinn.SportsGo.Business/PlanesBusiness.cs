using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Models;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class PlanesBusiness
    {


        #region Metodos Planes


        public async Task<WrapperSimpleTypesDTO> CrearPlan(Planes planParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);

                planParaCrear.Archivos.CodigoTipoArchivo = (int)TipoArchivo.Imagen;
                planRepository.CrearPlan(planParaCrear);

                WrapperSimpleTypesDTO wrapperCrearPlan = new WrapperSimpleTypesDTO();

                wrapperCrearPlan.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearPlan.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearPlan.Exitoso = true;
                    wrapperCrearPlan.ConsecutivoCreado = planParaCrear.Consecutivo;

                    if (planParaCrear.PlanDefault == (int)SiNoEnum.Si)
                    {
                        await planRepository.DesasignarPlanDefaultDeUnPerfilMenosActual(planParaCrear.Consecutivo, planParaCrear.CodigoTipoPerfil);
                    }

                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                    Notificaciones notificacion = new Notificaciones
                    {
                        CodigoTipoNotificacion = (int)TipoNotificacionEnum.NuevoPlan,
                        CodigoPlanNuevo = planParaCrear.Consecutivo,
                        Creacion = DateTime.Now
                    };

                    noticiasRepo.CrearNotificacion(notificacion);

                    wrapperCrearPlan.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    //if (notificacion.Consecutivo > 0)
                    //{
                    //    TimeLineNotificaciones timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    //}
                }

                return wrapperCrearPlan;
            }
        }

        public async Task<Planes> BuscarPlan(Planes planParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                Planes planExistente = await planRepository.BuscarPlan(planParaBuscar);

                return planExistente;
            }
        }

        public async Task<Planes> BuscarPlanDefaultDeUnPerfil(Planes planParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                Planes planExistente = await planRepository.BuscarPlanDefaultDeUnPerfil(planParaBuscar);

                return planExistente;
            }
        }

        public async Task<List<PlanesDTO>> ListarPlanesAdministrador(Planes planParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                List<PlanesDTO> listaPlanes = await planRepository.ListarPlanesPorIdioma(planParaListar);

                return listaPlanes;
            }
        }

        public async Task<List<PlanesDTO>> ListarPlanesPorIdioma(Planes planParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                List<PlanesDTO> listaPlanes = await planRepository.ListarPlanesPorIdioma(planParaListar);

                PagosRepository pagosRepo = new PagosRepository(context);
                Monedas monedaDeLaPersona = await pagosRepo.BuscarMonedaDeUnPais(planParaListar.CodigoPaisParaBuscarMoneda);

                if (monedaDeLaPersona.MonedaEnum != MonedasEnum.PesosColombianos)
                {
                    Monedas monedaColombiana = await pagosRepo.BuscarMonedaColombiana();

                    QueryMoneyExchanger queryExchanger = new QueryMoneyExchanger();
                    YahooExchangeEntity exchangeEntity = await queryExchanger.QueryMoneyExchange(monedaColombiana.AbreviaturaMoneda, monedaDeLaPersona.AbreviaturaMoneda);

                    Monedas monedaBuscada = await pagosRepo.BuscarMoneda(monedaDeLaPersona.MonedaEnum);
                    if (exchangeEntity != null)
                    {
                        monedaBuscada.CambioMoneda = exchangeEntity.Query.Results.Rate.Rate;
                    }

                    foreach (PlanesDTO planes in listaPlanes)
                    {
                        planes.Precio *= monedaBuscada.CambioMoneda;
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

                return listaPlanes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPlan(Planes planParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);

                planParaModificar.CodigoTipoPerfil = (int)planParaModificar.TipoPerfil;

                Planes planExistente = await planRepository.ModificarPlan(planParaModificar);

                if (planParaModificar.PlanesContenidos != null && planParaModificar.PlanesContenidos.Count > 0)
                {
                    foreach (var planContenido in planParaModificar.PlanesContenidos)
                    {
                        if (planContenido.Consecutivo > 0)
                        {
                            await planRepository.ModificarPlanContenido(planContenido);
                        }
                        else
                        {
                            planRepository.CrearPlanContenido(planContenido);
                        }
                    }
                }

                // Si lo voy a desasignar como default valido que no me quede sin defaults
                if (planExistente.PlanDefault == 1 && planParaModificar.PlanDefault == 0)
                {
                    TipoPerfil tipoPerfil = planParaModificar.CodigoTipoPerfil.ToEnum<TipoPerfil>();
                    int numeroDePlanesDefault = await planRepository.NumeroPlanesDefault(tipoPerfil);

                    if (numeroDePlanesDefault <= 1)
                    {
                        throw new InvalidOperationException("No puedes quedarte sin planes default para el perfil de " + tipoPerfil.ToString() + "!.");
                    }

                    planExistente.PlanDefault = 0;
                }
                else
                {
                    // Si lo voy a asignar entonces mando a desasignar todos los default de este perfil y procedo a asignar el que estoy modificando
                    if (planExistente.PlanDefault == 0 && planParaModificar.PlanDefault == 1)
                    {
                        await planRepository.DesasignarPlanDefaultDeUnPerfilMenosActual(planExistente.Consecutivo, planExistente.CodigoTipoPerfil);
                    }

                    planExistente.PlanDefault = planParaModificar.PlanDefault;
                }

                WrapperSimpleTypesDTO wrapperModificarPlan = new WrapperSimpleTypesDTO();

                wrapperModificarPlan.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarPlan.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarPlan.Exitoso = true;
                }

                return wrapperModificarPlan;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoPlan(Planes planArchivoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivo = new Archivos
                {
                    Consecutivo = planArchivoParaModificar.CodigoArchivo,
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = planArchivoParaModificar.ArchivoContenido
                };

                archivoRepo.ModificarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperModificarArchivoCategoria = new WrapperSimpleTypesDTO();

                wrapperModificarArchivoCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarArchivoCategoria.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarArchivoCategoria.Exitoso = true;
                }

                return wrapperModificarArchivoCategoria;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarPlanDefault(Planes planParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                Planes planExistente = await planRepository.AsignarPlanDefault(planParaAsignar);

                // Para dejar el plan existente como el unico default
                await planRepository.DesasignarPlanDefaultDeUnPerfilMenosActual(planExistente.Consecutivo, planExistente.CodigoTipoPerfil);

                WrapperSimpleTypesDTO wrapperAsignarPlanDefault = new WrapperSimpleTypesDTO();

                wrapperAsignarPlanDefault.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarPlanDefault.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarPlanDefault.Exitoso = true;
                }

                return wrapperAsignarPlanDefault;
            }
        }

        public async Task<WrapperSimpleTypesDTO> DesasignarPlanDefault(Planes planParaDesasignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);

                int? codigoTipoPerfilPlan = await planRepository.BuscarCodigoTipoPerfilDeUnPlan(planParaDesasignar);

                if (!codigoTipoPerfilPlan.HasValue)
                {
                    throw new InvalidOperationException("El plan no tiene un tipo de perfil especificado!. BUUUUUGGGGGG!.");
                }

                TipoPerfil tipoPerfil = codigoTipoPerfilPlan.Value.ToEnum<TipoPerfil>();
                int numeroDePlanesDefault = await planRepository.NumeroPlanesDefault(tipoPerfil);

                if (numeroDePlanesDefault <= 1)
                {
                    throw new InvalidOperationException("No puedes quedarte sin planes default para el perfil de " + tipoPerfil.ToString() + "!.");
                }

                Planes planExistente = await planRepository.DesasignarPlanDefault(planParaDesasignar);

                WrapperSimpleTypesDTO wrapperDesasignarPlanDefault = new WrapperSimpleTypesDTO();

                wrapperDesasignarPlanDefault.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperDesasignarPlanDefault.NumeroRegistrosAfectados > 0)
                {
                    wrapperDesasignarPlanDefault.Exitoso = true;
                }

                return wrapperDesasignarPlanDefault;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPlan(Planes planParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);

                int? codigoTipoPerfilPlan = await planRepository.BuscarCodigoTipoPerfilDeUnPlan(planParaEliminar);

                if (!codigoTipoPerfilPlan.HasValue)
                {
                    throw new InvalidOperationException("El plan no tiene un tipo de perfil especificado!. BUUUUUGGGGGG!.");
                }

                TipoPerfil tipoPerfil = codigoTipoPerfilPlan.Value.ToEnum<TipoPerfil>();
                int numeroDePlanesDefault = await planRepository.NumeroPlanesDefault(tipoPerfil);

                if (numeroDePlanesDefault <= 1)
                {
                    bool esPlanDefault = await planRepository.BuscarSiPlanEsDefault(planParaEliminar);

                    if (esPlanDefault)
                    {
                        throw new InvalidOperationException("No puedes quedarte sin planes default para el perfil de " + tipoPerfil.ToString() + "!.");
                    }
                }

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacion = new Notificaciones
                {
                    CodigoPlanNuevo = planParaEliminar.Consecutivo
                };
                noticiasRepo.EliminarNotificacionesDeUnPlan(notificacion);

                PlanesContenidos planContenido = new PlanesContenidos
                {
                    CodigoPlan = planParaEliminar.Consecutivo
                };
                planRepository.EliminarMultiplesPlanesContenidos(planContenido);

                planRepository.EliminarPlan(planParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                Archivos archivo = new Archivos
                {
                    Consecutivo = planParaEliminar.CodigoArchivo
                };
                archivoRepo.EliminarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperEliminarPlan = new WrapperSimpleTypesDTO();

                wrapperEliminarPlan.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarPlan.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarPlan.Exitoso = true;
                }

                return wrapperEliminarPlan;
            }
        }


        #endregion


        #region Metodos PlanesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearPlanesContenidos(List<PlanesContenidos> planesContenidosParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);
                planesRepo.CrearPlanesContenidos(planesContenidosParaCrear);

                WrapperSimpleTypesDTO wrapperCrearPlanesContenidos = new WrapperSimpleTypesDTO();

                wrapperCrearPlanesContenidos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearPlanesContenidos.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearPlanesContenidos.Exitoso = true;
                }

                return wrapperCrearPlanesContenidos;
            }
        }

        public async Task<PlanesContenidos> BuscarPlanContenido(PlanesContenidos planContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);
                PlanesContenidos planContenidoBuscado = await planesRepo.BuscarPlanContenido(planContenidoParaBuscar);

                return planContenidoBuscado;
            }
        }

        public async Task<List<PlanesContenidos>> ListarContenidoDeUnPlan(PlanesContenidos planContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);
                List<PlanesContenidos> listaPlanes = await planesRepo.ListarContenidoDeUnPlan(planContenidoParaListar);

                return listaPlanes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPlanContenido(PlanesContenidos planContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);
                PlanesContenidos planContenidoExistente = await planesRepo.ModificarPlanContenido(planContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarPlanContenido = new WrapperSimpleTypesDTO();

                wrapperModificarPlanContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarPlanContenido.NumeroRegistrosAfectados > 0) wrapperModificarPlanContenido.Exitoso = true;

                return wrapperModificarPlanContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPlanContenido(PlanesContenidos planContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);
                planesRepo.EliminarPlanContenido(planContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarPlanContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarPlanContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarPlanContenido.NumeroRegistrosAfectados > 0) wrapperEliminarPlanContenido.Exitoso = true;

                return wrapperEliminarPlanContenido;
            }
        }


        #endregion


        #region Metodos PlanesUsuarios



        public async Task<PlanesUsuariosDTO> BuscarPlanUsuario(PlanesUsuarios planUsuarioParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                PlanesUsuariosDTO planUsuarioExistente = await planRepository.BuscarPlanUsuario(planUsuarioParaBuscar);

                int? codigoUsuario = await planRepository.BuscarCodigoUsuarioPorCodigoPlanUsuario(planUsuarioExistente.Consecutivo);

                PagosRepository pagosRepo = new PagosRepository(context);
                Monedas monedaDelUsuario = await pagosRepo.BuscarMonedaDeUnUsuario(codigoUsuario.Value);

                if (monedaDelUsuario.MonedaEnum != MonedasEnum.PesosColombianos)
                {
                    Monedas monedaColombiana = await pagosRepo.BuscarMonedaColombiana();
                    QueryMoneyExchanger queryExchanger = new QueryMoneyExchanger();
                    YahooExchangeEntity exchangeEntity = await queryExchanger.QueryMoneyExchange(monedaColombiana.AbreviaturaMoneda, monedaDelUsuario.AbreviaturaMoneda);

                    if (exchangeEntity != null)
                    {
                        monedaDelUsuario.CambioMoneda = exchangeEntity.Query.Results.Rate.Rate;
                    }

                    planUsuarioExistente.Planes.Precio *= monedaDelUsuario.CambioMoneda;
                }

                try
                {
                    // No es obligatorio para el paso que actualize el cambio de moneda
                    await context.SaveChangesAsync();
                }
                catch (Exception)
                {

                }

                return planUsuarioExistente;
            }
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiPlanSoportaLaOperacion(PlanesUsuarios planUsuarioParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);
                bool esPosible = await planRepository.VerificarSiPlanSoportaLaOperacion(planUsuarioParaBuscar, planUsuarioParaBuscar.TipoOperacionBase);

                WrapperSimpleTypesDTO wrapperVerificarPlanOperacion = new WrapperSimpleTypesDTO
                {
                    EsPosible = esPosible
                };

                return wrapperVerificarPlanOperacion;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CambiarDePlanUsuario(PlanesUsuarios planParaCambiar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepository = new PlanesRepository(context);

                DateTime fechaVencimientoPlan = await planRepository.CalcularFechaVencimientoPlanUsuario(planParaCambiar);

                planParaCambiar.Vencimiento = fechaVencimientoPlan;
                PlanesUsuarios planUsuarioExistente = await planRepository.CambiarDePlanUsuario(planParaCambiar);

                WrapperSimpleTypesDTO wrapperCambiarDePlanUsuario = new WrapperSimpleTypesDTO();

                wrapperCambiarDePlanUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCambiarDePlanUsuario.NumeroRegistrosAfectados > 0)
                {
                    wrapperCambiarDePlanUsuario.Exitoso = true;
                    wrapperCambiarDePlanUsuario.Vencimiento = fechaVencimientoPlan;
                }

                return wrapperCambiarDePlanUsuario;
            }
        }

        internal async Task<WrapperSimpleTypesDTO> CambiarPlanUsuarioADefaultPerfilPorVencimiento(PlanesUsuariosDTO planesUsuariosVencidoParaCambiar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planesRepo = new PlanesRepository(context);

                Planes planParaBuscar = new Planes
                {
                    Consecutivo = planesUsuariosVencidoParaCambiar.CodigoPlan
                };
                int? codigoPerfil = await planesRepo.BuscarCodigoTipoPerfilDeUnPlan(planParaBuscar);

                if (!codigoPerfil.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo de perfil!. BUUUUG");
                }

                TipoPerfil tipoPerfil = codigoPerfil.Value.ToEnum<TipoPerfil>();
                int? codigoPlanDefault = await planesRepo.BuscarCodigoPlanDefault(tipoPerfil);

                if (!codigoPlanDefault.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo del plan default para este perfil!. BUUUUG");
                }

                PlanesUsuarios planParaCambiar = new PlanesUsuarios
                {
                    Consecutivo = planesUsuariosVencidoParaCambiar.Consecutivo,
                    CodigoPlanDeseado = codigoPlanDefault.Value
                };

                WrapperSimpleTypesDTO wrapper = await CambiarDePlanUsuario(planParaCambiar);

                return wrapper;
            }
        }


        #endregion


    }
}
