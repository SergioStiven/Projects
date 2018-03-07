using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Business
{
    public class AdministracionBusiness
    {


        #region Metodos Administracion Usuarios


        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> ModificarUsuario(Usuarios usuarioParamodificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                Usuarios usuarioExistente = await adminRepo.ModificarUsuario(usuarioParamodificar);

                Notificaciones notificacion = null;
                if (usuarioExistente.PlanesUsuarios.CodigoPlan != usuarioParamodificar.PlanesUsuarios.CodigoPlanDeseado)
                {
                    PlanesBusiness planBusiness = new PlanesBusiness();

                    // Cambio el plan para esa persona
                    await planBusiness.CambiarDePlanUsuario(usuarioParamodificar.PlanesUsuarios);

                    // Armamos una notificacion para el nuevo plan
                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                    notificacion = new Notificaciones
                    {
                        CodigoTipoNotificacion = (int)TipoNotificacionEnum.PlanAprobado,
                        CodigoPlanNuevo = usuarioParamodificar.PlanesUsuarios.CodigoPlanDeseado,
                        CodigoPersonaDestinoAccion = usuarioExistente.Personas.First().Consecutivo,
                        Creacion = DateTime.Now
                    };

                    noticiasRepo.CrearNotificacion(notificacion);
                }

                WrapperSimpleTypesDTO wrapperModificarUsuario = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperModificarUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarUsuario.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarUsuario.Exitoso = true;

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

                return Tuple.Create(wrapperModificarUsuario, timeLineNotificacion);
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarUsuario(Usuarios usuarioParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                int? codigoPersona = await adminRepo.BuscarCodigoPersonaDeUnUsuario(usuarioParaEliminar.Consecutivo);

                if (!codigoPersona.HasValue)
                {
                    throw new InvalidOperationException("La persona que se intenta borrar no existe, BUUUGGG BUAAGHHH!.");
                }

                int? codigoTipoPerfil = await adminRepo.BuscarTipoPerfilDeUnaPersona(codigoPersona.Value);

                if (!codigoTipoPerfil.HasValue)
                {
                    throw new InvalidOperationException("La persona que se intenta borrar no tiene un tipo de perfil valido, BUUUGGG BUAAGHHH!.");
                }

                TipoPerfil tipoPerfilDeEstaPersona = codigoTipoPerfil.Value.ToEnum<TipoPerfil>();

                int? codigoPlanUsuario = await adminRepo.BuscarCodigoPlanUsuarioDeUnUsuario(usuarioParaEliminar.Consecutivo);
                if (!codigoPlanUsuario.HasValue)
                {
                    throw new InvalidOperationException("El usuario que se intenta borrar no tiene un plan de usuario valido, BUUUGGG BUAAGHHH!.");
                }

                adminRepo.EliminarNotificacionesDeUnaPersona(codigoPersona.Value);

                List<int> listaCodigoArchivoHistorialPago = await adminRepo.ListarCodigoArchivosDeTodosLosHistorialPagoDeUnaPersona(codigoPersona.Value);

                adminRepo.EliminarHistorialPagoDeUnaPersona(codigoPersona.Value);

                if (listaCodigoArchivoHistorialPago != null && listaCodigoArchivoHistorialPago.Count > 0)
                {
                    foreach (var codigoArchivo in listaCodigoArchivoHistorialPago)
                    {
                        adminRepo.EliminarArchivo(codigoArchivo);
                    }
                }

                List<int> listaCodigoChats = await adminRepo.ListarCodigoChatDeUnaPersona(codigoPersona.Value);
                if (listaCodigoChats != null && listaCodigoChats.Count > 0)
                {
                    foreach (var codigoChat in listaCodigoChats)
                    {
                        adminRepo.EliminarChatMensajesDeUnChat(codigoChat);
                    }
                }

                adminRepo.EliminarChatsDeUnaPersona(codigoPersona.Value);
                adminRepo.EliminarContactosDeUnaPersona(codigoPersona.Value);
                adminRepo.EliminarGrupoEventosAsistentesDeUnaPersona(codigoPersona.Value);

                switch (tipoPerfilDeEstaPersona)
                {
                    case TipoPerfil.Candidato:
                        await EliminarCandidato(adminRepo, codigoPersona);
                        break;
                    case TipoPerfil.Grupo:
                        await EliminarGrupo(adminRepo, codigoPersona);
                        break;
                    case TipoPerfil.Representante:
                        await EliminarRepresentante(adminRepo, codigoPersona);
                        break;
                    case TipoPerfil.Anunciante:
                        await EliminarAnunciante(adminRepo, codigoPersona);
                        break;
                }

                adminRepo.EliminarPersona(codigoPersona.Value);

                int? codigoArchivoImagenPerfil = await adminRepo.BuscarCodigoArchivoDeImagenPerfilDeUnaPersona(codigoPersona.Value);
                int? codigoArchivoImagenBanner = await adminRepo.BuscarCodigoArchivoDeImagenBannerDeUnaPersona(codigoPersona.Value);

                if (codigoArchivoImagenPerfil.HasValue)
                {
                    adminRepo.EliminarArchivo(codigoArchivoImagenPerfil.Value);
                }

                if (codigoArchivoImagenBanner.HasValue)
                {
                    adminRepo.EliminarArchivo(codigoArchivoImagenBanner.Value);
                }

                adminRepo.EliminarPlanUsuario(codigoPlanUsuario.Value);
                adminRepo.EliminarUsuario(usuarioParaEliminar.Consecutivo);

                WrapperSimpleTypesDTO wrapperEliminarUsuario = new WrapperSimpleTypesDTO();

                wrapperEliminarUsuario.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarUsuario.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarUsuario.Exitoso = true;
                }

                return wrapperEliminarUsuario;
            }
        }

        async Task EliminarRepresentante(AdministracionRepository adminRepo, int? codigoPersona)
        {
            int? codigoRepresentante = await adminRepo.BuscarCodigoRepresentanteDeUnaPersona(codigoPersona.Value);
            if (codigoRepresentante.HasValue)
            {
                adminRepo.EliminarCategoriaRepresentantesDeUnRepresentante(codigoRepresentante.Value);
                adminRepo.EliminarRepresentante(codigoRepresentante.Value);
            }
            else
            {
                throw new InvalidOperationException("No existe un Representante para esta persona que se quiere borrar, BUUUGGG BUAAGHHH!.");
            }
        }

        async Task EliminarAnunciante(AdministracionRepository adminRepo, int? codigoPersona)
        {
            int? codigoAnunciante = await adminRepo.BuscarCodigoAnuncianteDeUnaPersona(codigoPersona.Value);
            if (codigoAnunciante.HasValue)
            {
                List<int> listaCodigoAnuncios = await adminRepo.ListarCodigoAnunciosDeUnAnunciante(codigoAnunciante.Value);
                if (listaCodigoAnuncios != null && listaCodigoAnuncios.Count > 0)
                {
                    foreach (var codigoAnuncio in listaCodigoAnuncios)
                    {
                        adminRepo.EliminarAnunciosContenidosDeUnAnuncio(codigoAnuncio);
                        adminRepo.EliminarAnunciosPaisesDeUnAnuncio(codigoAnuncio);
                        adminRepo.EliminarCategoriasAnunciosDeUnAnuncio(codigoAnuncio);
                    }
                }

                adminRepo.EliminarAnunciosDeUnAnunciante(codigoAnunciante.Value);

                List<int> listaCodigoArchivoAnuncios = await adminRepo.ListarCodigoArchivosDeTodosLosAnunciosDeUnAnunciante(codigoAnunciante.Value);

                adminRepo.EliminarAnunciante(codigoAnunciante.Value);

                if (listaCodigoArchivoAnuncios != null && listaCodigoArchivoAnuncios.Count > 0)
                {
                    foreach (var codigoArchivo in listaCodigoArchivoAnuncios)
                    {
                        adminRepo.EliminarArchivo(codigoArchivo);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No existe un Anunciante para esta persona que se quiere borrar, BUUUGGG BUAAGHHH!.");
            }
        }

        async Task EliminarGrupo(AdministracionRepository adminRepo, int? codigoPersona)
        {
            int? codigoGrupo = await adminRepo.BuscarCodigoGrupoDeUnaPersona(codigoPersona.Value);
            if (codigoGrupo.HasValue)
            {
                List<int> listaCodigoEventos = await adminRepo.ListarCodigoEventosDeUnGrupo(codigoGrupo.Value);
                if (listaCodigoEventos != null && listaCodigoEventos.Count > 0)
                {
                    foreach (var codigoEvento in listaCodigoEventos)
                    {
                        adminRepo.EliminarGruposAsistentesDeUnEvento(codigoEvento);
                        adminRepo.EliminarCategoriasEventosDeUnEvento(codigoEvento);
                    }
                }

                adminRepo.EliminarEventosDeUnGrupo(codigoGrupo.Value);
                adminRepo.EliminarCategoriasGruposDeUnGrupo(codigoGrupo.Value);

                List<int> listaCodigoArchivoGrupo = await adminRepo.ListarCodigoArchivosDeTodosLosGrupoEventosDeUnGrupo(codigoGrupo.Value);

                adminRepo.EliminarGrupo(codigoGrupo.Value);

                if (listaCodigoArchivoGrupo != null && listaCodigoArchivoGrupo.Count > 0)
                {
                    foreach (var codigoArchivo in listaCodigoArchivoGrupo)
                    {
                        adminRepo.EliminarArchivo(codigoArchivo);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No existe un Grupo para esta persona que se quiere borrar, BUUUGGG BUAAGHHH!.");
            }
        }

        async Task EliminarCandidato(AdministracionRepository adminRepo, int? codigoPersona)
        {
            int? codigoCandidato = await adminRepo.BuscarCodigoCandidatoDeUnaPersona(codigoPersona.Value);
            if (codigoCandidato.HasValue)
            {
                List<int> listaCodigoCategorias = await adminRepo.ListarCodigoCategoriasDeUnCandidato(codigoCandidato.Value);
                if (listaCodigoCategorias != null && listaCodigoCategorias.Count > 0)
                {
                    foreach (var codigoCategoria in listaCodigoCategorias)
                    {
                        adminRepo.EliminarHabilidadesDeUnaCategoriaCandidato(codigoCategoria);
                    }
                }

                adminRepo.EliminarCategoriasCandidatoDeUnCandidato(codigoCandidato.Value);
                adminRepo.EliminarCandidatosVideoDeUnCandidato(codigoCandidato.Value);

                int? codigoResponsable = await adminRepo.BuscarCodigoResponsableDeUnCandidato(codigoCandidato.Value);

                List<int> listaCodigoArchivoVideosCandidatos = await adminRepo.ListarCodigoArchivosDeTodosLosCandidatoVideosDeUnaCandidato(codigoCandidato.Value);

                adminRepo.EliminarCandidato(codigoCandidato.Value);

                if (codigoResponsable.HasValue)
                {
                    adminRepo.EliminarCandidatosResponsables(codigoResponsable.Value);
                }

                if (listaCodigoArchivoVideosCandidatos != null && listaCodigoArchivoVideosCandidatos.Count > 0)
                {
                    foreach (var codigoArchivo in listaCodigoArchivoVideosCandidatos)
                    {
                        adminRepo.EliminarArchivo(codigoArchivo);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("No existe un Candidato para esta persona que se quiere borrar, BUUUGGG BUAAGHHH!.");
            }
        }



        #endregion


        #region Metodos TerminosCondiciones


        public async Task<WrapperSimpleTypesDTO> AsignarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                bool existe = await adminRepo.BuscarSiExisteTerminosYCondiciones(terminosCondicionesParaAsignar);

                if (existe)
                {
                    TerminosCondiciones terminosCondicionesExistentes = await adminRepo.ModificarTerminosCondiciones(terminosCondicionesParaAsignar);
                }
                else
                {
                    adminRepo.CrearTerminosCondiciones(terminosCondicionesParaAsignar);
                }

                WrapperSimpleTypesDTO wrapperAsignarTerminosCondiciones = new WrapperSimpleTypesDTO();

                wrapperAsignarTerminosCondiciones.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarTerminosCondiciones.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarTerminosCondiciones.Exitoso = true;
                    wrapperAsignarTerminosCondiciones.ConsecutivoCreado = terminosCondicionesParaAsignar.Consecutivo;
                }

                return wrapperAsignarTerminosCondiciones;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarTerminosCondicionesLista(List<TerminosCondiciones> terminosCondicionesParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                foreach (TerminosCondiciones terminos in terminosCondicionesParaAsignar)
                {
                    bool existe = await adminRepo.BuscarSiExisteTerminosYCondiciones(terminos);

                    if (existe)
                    {
                        TerminosCondiciones terminosCondicionesExistentes = await adminRepo.ModificarTerminosCondiciones(terminos);
                    }
                    else
                    {
                        adminRepo.CrearTerminosCondiciones(terminos);
                    }
                }

                WrapperSimpleTypesDTO wrapperAsignarTerminosCondicionesLista = new WrapperSimpleTypesDTO();

                wrapperAsignarTerminosCondicionesLista.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarTerminosCondicionesLista.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarTerminosCondicionesLista.Exitoso = true;
                }

                return wrapperAsignarTerminosCondicionesLista;
            }
        }

        public async Task<TerminosCondiciones> BuscarTerminosCondiciones(TerminosCondiciones terminosCondicionesParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                TerminosCondiciones terminosCondicionesBuscados = await adminRepo.BuscarTerminosCondiciones(terminosCondicionesParaBuscar);

                return terminosCondicionesBuscados;
            }
        }

        public async Task<List<TerminosCondiciones>> ListarTerminosCondiciones()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                List<TerminosCondiciones> listaTerminosCondicionesBuscados = await adminRepo.ListarTerminosCondiciones();

                return listaTerminosCondicionesBuscados;
            }
        }


        #endregion


        #region Metodos ImagenesPerfilAdministradores


        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfilAdministrador(int codigoUsuario, Stream streamSource)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                ArchivosBusiness archivoBuss = new ArchivosBusiness();

                ImagenesPerfilAdministradores imagenPerfilExistente = await adminRepo.BuscarImagenPerfilAdministrador(codigoUsuario);
                int? codigoArchivoExistente = null;

                WrapperSimpleTypesDTO wrapper = await archivoBuss.CrearArchivoStream((int)TipoArchivo.Imagen, streamSource);

                if (imagenPerfilExistente == null)
                {
                    ImagenesPerfilAdministradores imagenParaCrear = new ImagenesPerfilAdministradores
                    {
                        CodigoUsuario = codigoUsuario,
                        CodigoArchivo = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado)
                    };

                    adminRepo.CrearImagenPerfilAdministrador(imagenParaCrear);
                }
                else
                {
                    // Guardo el viejo codigo de archivo para borrarlo luego
                    codigoArchivoExistente = imagenPerfilExistente.CodigoArchivo;

                    imagenPerfilExistente = await adminRepo.AsignarImagenPerfil(codigoUsuario, Convert.ToInt32(wrapper.ConsecutivoArchivoCreado));
                }

                // Borro el archivo viejo
                if (codigoArchivoExistente.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    Archivos archivoParaBorrar = new Archivos
                    {
                        Consecutivo = codigoArchivoExistente.Value
                    };

                    archivoRepo.EliminarArchivo(archivoParaBorrar);
                }

                WrapperSimpleTypesDTO wrapperAsignarImagenPerfilAdministrador = new WrapperSimpleTypesDTO();

                wrapperAsignarImagenPerfilAdministrador.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperAsignarImagenPerfilAdministrador.NumeroRegistrosAfectados > 0)
                {
                    wrapperAsignarImagenPerfilAdministrador.Exitoso = true;
                    wrapperAsignarImagenPerfilAdministrador.ConsecutivoCreado = imagenPerfilExistente.Consecutivo;
                    wrapperAsignarImagenPerfilAdministrador.ConsecutivoArchivoCreado = wrapper.ConsecutivoArchivoCreado;
                }

                return wrapperAsignarImagenPerfilAdministrador;
            }
        }


        #endregion


        #region Metodos Paises


        public async Task<WrapperSimpleTypesDTO> CrearPais(Paises paisParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);

                paisParaCrear.Archivos.CodigoTipoArchivo = (int)TipoArchivo.Imagen;
                adminRepo.CrearPais(paisParaCrear);

                WrapperSimpleTypesDTO wrapperCrearPais = new WrapperSimpleTypesDTO();

                wrapperCrearPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearPais.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearPais.Exitoso = true;
                    wrapperCrearPais.ConsecutivoCreado = paisParaCrear.Consecutivo;
                    wrapperCrearPais.ConsecutivoArchivoCreado = paisParaCrear.CodigoArchivo;
                }

                return wrapperCrearPais;
            }
        }

        public async Task<Paises> BuscarPais(Paises paisParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                Paises paisBuscado = await adminRepo.BuscarPais(paisParaBuscar);

                return paisBuscado;
            }
        }

        public async Task<List<PaisesDTO>> ListarPaisesPorIdioma(Paises paisParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                List<PaisesDTO> listaPaises = await adminRepo.ListarPaisesPorIdioma(paisParaListar);

                return listaPaises;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoPais(Paises paisParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivo = new Archivos
                {
                    Consecutivo = paisParaModificar.CodigoArchivo,
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = paisParaModificar.ArchivoContenido
                };

                archivoRepo.ModificarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperModificarArchivoPais = new WrapperSimpleTypesDTO();

                wrapperModificarArchivoPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarArchivoPais.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarArchivoPais.Exitoso = true;
                }

                return wrapperModificarArchivoPais;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPais(Paises paisParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                Paises paisExistente = await adminRepo.ModificarPais(paisParaModificar);

                foreach (var item in paisParaModificar.PaisesContenidos)
                {
                    await adminRepo.ModificarPaisContenido(item);
                }

                WrapperSimpleTypesDTO wrapperModificarPais = new WrapperSimpleTypesDTO();

                wrapperModificarPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarPais.NumeroRegistrosAfectados > 0) wrapperModificarPais.Exitoso = true;

                return wrapperModificarPais;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPais(Paises paisParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivo = new Archivos
                {
                    Consecutivo = paisParaEliminar.CodigoArchivo
                };

                AdministracionRepository adminRepo = new AdministracionRepository(context);
                PaisesContenidos paisContenidoParaBorrar = new PaisesContenidos
                {
                    CodigoPais = paisParaEliminar.Consecutivo
                };

                adminRepo.EliminarMultiplesPaisesContenidos(paisContenidoParaBorrar);
                adminRepo.EliminarPais(paisParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperEliminarPais = new WrapperSimpleTypesDTO();

                wrapperEliminarPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarPais.NumeroRegistrosAfectados > 0) wrapperEliminarPais.Exitoso = true;

                return wrapperEliminarPais;
            }
        }


        #endregion


        #region Metodos PaisesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearPaisesContenidos(List<PaisesContenidos> paisContenidoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                adminRepo.CrearPaisesContenidos(paisContenidoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearPaisesContenidos = new WrapperSimpleTypesDTO();

                wrapperCrearPaisesContenidos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearPaisesContenidos.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearPaisesContenidos.Exitoso = true;
                }

                return wrapperCrearPaisesContenidos;
            }
        }

        public async Task<PaisesContenidos> BuscarPaisContenido(PaisesContenidos paisContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                PaisesContenidos paisContenidoBuscado = await adminRepo.BuscarPaisContenido(paisContenidoParaBuscar);

                return paisContenidoBuscado;
            }
        }

        public async Task<List<PaisesContenidos>> ListarContenidoDeUnPais(PaisesContenidos paisContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                List<PaisesContenidos> listaPaises = await adminRepo.ListarContenidoDeUnPais(paisContenidoParaListar);

                return listaPaises;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPaisContenido(PaisesContenidos paisContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                PaisesContenidos paisContenidoExistente = await adminRepo.ModificarPaisContenido(paisContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarPaisContenido = new WrapperSimpleTypesDTO();

                wrapperModificarPaisContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarPaisContenido.NumeroRegistrosAfectados > 0) wrapperModificarPaisContenido.Exitoso = true;

                return wrapperModificarPaisContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPaisContenido(PaisesContenidos paisContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AdministracionRepository adminRepo = new AdministracionRepository(context);
                adminRepo.EliminarPaisContenido(paisContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarPaisContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarPaisContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarPaisContenido.NumeroRegistrosAfectados > 0) wrapperEliminarPaisContenido.Exitoso = true;

                return wrapperEliminarPaisContenido;
            }
        }


        #endregion


    }
}
