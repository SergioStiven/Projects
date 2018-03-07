using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class GruposBusiness
    {


        #region Metodos Grupos


        public async Task<WrapperSimpleTypesDTO> CrearGrupo(Grupos grupoParaCrear, string urlLogo, string urlBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanDefault = await planRepo.BuscarCodigoPlanDefault(TipoPerfil.Representante);

                if (!codigoPlanDefault.HasValue)
                {
                    throw new InvalidOperationException("No existe un plan default para los grupo!.");
                }

                PlanesUsuarios planUsuarioDefault = new PlanesUsuarios
                {
                    CodigoPlan = codigoPlanDefault.Value,
                    Adquisicion = DateTime.Now,
                    Vencimiento = DateTime.MaxValue
                };

                grupoParaCrear.Personas.Usuarios.CuentaActiva = 0;
                grupoParaCrear.Personas.Usuarios.PlanesUsuarios = planUsuarioDefault;
                grupoParaCrear.Personas.Usuarios.TipoPerfil = grupoParaCrear.Personas.TipoPerfil;

                grupoParaCrear.Personas.Candidatos = null;
                grupoParaCrear.Personas.Paises = null;
                grupoParaCrear.Personas.Idiomas = null;
                grupoParaCrear.Personas.Anunciantes = null;
                grupoParaCrear.Personas.Grupos = null;
                grupoParaCrear.Personas.Representantes = null;
                grupoParaCrear.Personas.Usuarios.Personas = null;
                grupoParaCrear.Personas.Paises = null;
                grupoParaCrear.Personas.Idiomas = null;

                foreach (var categoriaGrupo in grupoParaCrear.CategoriasGrupos)
                {
                    categoriaGrupo.Categorias = null;
                }

                GruposRepository grupoRepository = new GruposRepository(context);
                grupoRepository.CrearGrupo(grupoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearGrupo = new WrapperSimpleTypesDTO();

                wrapperCrearGrupo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearGrupo.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearGrupo.Exitoso = true;
                    wrapperCrearGrupo.ConsecutivoCreado = grupoParaCrear.Consecutivo;
                    wrapperCrearGrupo.ConsecutivoPersonaCreado = grupoParaCrear.Personas.Consecutivo;
                    wrapperCrearGrupo.ConsecutivoUsuarioCreado = grupoParaCrear.Personas.Usuarios.Consecutivo;

                    AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                    string formatoEmail = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(grupoParaCrear.Personas.CodigoIdioma, TipoFormatosEnum.ConfirmacionCuenta);

                    if (!string.IsNullOrWhiteSpace(formatoEmail))
                    {
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderNombre, grupoParaCrear.Personas.Nombres);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenLogo, urlLogo);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenBanner, urlBanner);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlWeb, URL.UrlWeb);

                        string urlConfirmacionFormated = string.Format(URL.UrlWeb + @"Authenticate/ConfirmationOfRegistration?ID={0}&Language={1}", grupoParaCrear.Personas.Usuarios.Consecutivo, grupoParaCrear.Personas.CodigoIdioma);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlPaginaConfirmacion, urlConfirmacionFormated);

                        string tema = string.Empty;
                        switch (grupoParaCrear.Personas.IdiomaDeLaPersona)
                        {
                            case Idioma.Español:
                                tema = "Confirmacion de registro";
                                break;
                            case Idioma.Ingles:
                                tema = "Confirmation of registration";
                                break;
                            case Idioma.Portugues:
                                tema = "Confirmação da inscrição";
                                break;
                        }

                        // Recordar configurar la cuenta Gmail en este caso para que permita el logeo de manera insegura y poder mandar correos
                        // https://myaccount.google.com/lesssecureapps?pli=1
                        CorreoHelper correoHelper = new CorreoHelper(grupoParaCrear.Personas.Usuarios.Email.Trim(), AppConstants.CorreoAplicacion, AppConstants.ClaveCorreoAplicacion);
                        wrapperCrearGrupo.Exitoso = correoHelper.EnviarCorreoConHTML(formatoEmail, Correo.Gmail, tema, "SportsGo");
                    }
                    else
                    {
                        throw new InvalidOperationException("No hay formatos parametrizados para la confirmacion de la clave");
                    }
                }

                return wrapperCrearGrupo;
            }
        }

        public async Task<Grupos> BuscarGrupoPorCodigoPersona(Grupos grupoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                Grupos informacionGrupo = await grupoRepository.BuscarGrupoPorCodigoPersona(grupoParaBuscar);

                return informacionGrupo;
            }
        }

        public async Task<Grupos> BuscarGrupoPorCodigoGrupo(Grupos grupoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                Grupos informacionGrupo = await grupoRepository.BuscarGrupoPorCodigoGrupo(grupoParaBuscar);

                return informacionGrupo;
            }
        }

        public async Task<List<GruposDTO>> ListarGrupos(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                List<GruposDTO> listaInformacionGrupos = await grupoRepository.ListarGrupos(buscador);

                return listaInformacionGrupos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionGrupo(Grupos grupoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                Grupos grupoExistente = await grupoRepository.ModificarInformacionGrupo(grupoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarInformacionGrupo = new WrapperSimpleTypesDTO();

                wrapperModificarInformacionGrupo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarInformacionGrupo.NumeroRegistrosAfectados > 0) wrapperModificarInformacionGrupo.Exitoso = true;

                return wrapperModificarInformacionGrupo;
            }
        }


        #endregion


        #region Metodos GruposEventos


        public async Task<WrapperSimpleTypesDTO> CrearGrupoEvento(GruposEventos grupoEventoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);

                grupoRepository.CrearGrupoEvento(grupoEventoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearGrupoEvento = new WrapperSimpleTypesDTO();

                wrapperCrearGrupoEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearGrupoEvento.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearGrupoEvento.Exitoso = true;
                    wrapperCrearGrupoEvento.ConsecutivoCreado = grupoEventoParaCrear.Consecutivo;
                }

                return wrapperCrearGrupoEvento;
            }
        }

        public async Task<GruposEventosDTO> BuscarGrupoEventoPorConsecutivo(GruposEventos grupoEventoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                GruposEventosDTO grupoEventoBuscado = await grupoRepository.BuscarGrupoEventoPorConsecutivo(grupoEventoParaBuscar);

                if (grupoEventoBuscado != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                    grupoEventoBuscado.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(grupoEventoParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, grupoEventoBuscado.Creacion);
                    grupoEventoBuscado.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(grupoEventoParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, grupoEventoBuscado.FechaInicio);
                    grupoEventoBuscado.FechaTerminacion = helper.ConvertDateTimeFromAnotherTimeZone(grupoEventoParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, grupoEventoBuscado.FechaTerminacion);
                }

                return grupoEventoBuscado;
            }
        }

        public async Task<List<GruposEventosDTO>> ListarEventosDeUnGrupo(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaFiltroBase != DateTime.MinValue)
                {
                    buscador.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaFiltroBase);
                }

                List<GruposEventosDTO> listaEventosDeUnGrupo = await grupoRepository.ListarEventosDeUnGrupo(buscador);

                if (listaEventosDeUnGrupo != null && listaEventosDeUnGrupo.Count > 0)
                {
                    
                    foreach (var eventos in listaEventosDeUnGrupo)
                    {
                        eventos.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.Creacion);
                        eventos.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.FechaInicio);
                        eventos.FechaTerminacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.FechaTerminacion);
                    }
                }

                return listaEventosDeUnGrupo;
            }
        }

        public async Task<List<GruposEventosDTO>> ListarEventos(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaInicio != DateTime.MinValue)
                {
                    buscador.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaInicio);
                }

                if (buscador.FechaFinal != DateTime.MinValue)
                {
                    buscador.FechaFinal = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaFinal);
                }

                List<GruposEventosDTO> listaEventos = await grupoRepository.ListarEventos(buscador);

                if (listaEventos != null && listaEventos.Count > 0)
                {
                    
                    foreach (var eventos in listaEventos)
                    {
                        eventos.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.Creacion);
                        eventos.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.FechaInicio);
                        eventos.FechaTerminacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventos.FechaTerminacion);
                    }
                }

                return listaEventos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionGrupoEvento(GruposEventos grupoEventoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);

                if (!grupoEventoParaModificar.CodigoArchivo.HasValue || grupoEventoParaModificar.CodigoArchivo <= 0)
                {
                    int? codigoArchivo = await grupoRepository.BuscarArchivoGrupoEvento(grupoEventoParaModificar);

                    if (codigoArchivo.HasValue && codigoArchivo > 0)
                    {
                        ArchivosRepository archivoRepo = new ArchivosRepository(context);

                        Archivos archivo = new Archivos
                        {
                            Consecutivo = codigoArchivo.Value
                        };

                        archivoRepo.EliminarArchivo(archivo);
                    }
                }

                GruposEventos grupoEventoExistente = await grupoRepository.ModificarInformacionGrupoEvento(grupoEventoParaModificar);

                if (grupoEventoParaModificar.CategoriasEventos != null && grupoEventoParaModificar.CategoriasEventos.Count > 0)
                {
                    CategoriasRepository categoriaRepo = new CategoriasRepository(context);

                    CategoriasEventos categoriaEventos = new CategoriasEventos
                    {
                        CodigoEvento = grupoEventoParaModificar.Consecutivo
                    };
                    categoriaRepo.EliminarMultiplesCategoriasEventos(categoriaEventos);

                    categoriaRepo.CrearListaCategoriaEventos(grupoEventoParaModificar.CategoriasEventos);
                }

                WrapperSimpleTypesDTO wrapperModificarInformacionGrupoEvento = new WrapperSimpleTypesDTO();

                wrapperModificarInformacionGrupoEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                wrapperModificarInformacionGrupoEvento.Exitoso = true;

                return wrapperModificarInformacionGrupoEvento;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarGrupoEvento(GruposEventos grupoEventoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriaRepo = new CategoriasRepository(context);
                CategoriasEventos categoriaGrupoParaBorrar = new CategoriasEventos
                {
                    CodigoEvento = grupoEventoParaEliminar.Consecutivo
                };
                categoriaRepo.EliminarMultiplesCategoriasEventos(categoriaGrupoParaBorrar);

                GruposRepository grupoRepository = new GruposRepository(context);
                GruposEventosAsistentes gruposAsistentesParaBorrar = new GruposEventosAsistentes
                {
                    CodigoEvento = grupoEventoParaEliminar.Consecutivo
                };
                grupoRepository.EliminarMultiplesGrupoEventoAsistente(gruposAsistentesParaBorrar);

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacionEvento = new Notificaciones
                {
                    CodigoEvento = grupoEventoParaEliminar.Consecutivo
                };
                noticiasRepo.EliminarNotificacionesDeUnEvento(notificacionEvento);

                int? archivoGrupoEvento = await grupoRepository.BuscarArchivoGrupoEvento(grupoEventoParaEliminar);
                grupoRepository.EliminarGrupoEvento(grupoEventoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarGrupoEvento = new WrapperSimpleTypesDTO();

                wrapperEliminarGrupoEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (archivoGrupoEvento.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    Archivos archivoParaEliminar = new Archivos
                    {
                        Consecutivo = archivoGrupoEvento.Value,
                    };

                    archivoRepo.EliminarArchivo(archivoParaEliminar);
                }

                wrapperEliminarGrupoEvento.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                if (wrapperEliminarGrupoEvento.NumeroRegistrosAfectados > 0) wrapperEliminarGrupoEvento.Exitoso = true;

                return wrapperEliminarGrupoEvento;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoGrupoEvento(GruposEventos grupoEventoArchivoParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = grupoEventoArchivoParaBorrar.CodigoArchivo.Value,
                };

                GruposRepository grupoRepository = new GruposRepository(context);
                GruposEventos grupoEventoExistente = await grupoRepository.DesasignarArchivoGrupoEvento(grupoEventoArchivoParaBorrar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarArchivoGrupoEvento = new WrapperSimpleTypesDTO();

                wrapperEliminarArchivoGrupoEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarArchivoGrupoEvento.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarArchivoGrupoEvento.Exitoso = true;
                }

                return wrapperEliminarArchivoGrupoEvento;
            }
        }


        #endregion


        #region Metodos GruposEventosAsistentes


        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> CrearGruposEventosAsistentes(GruposEventosAsistentes grupoEventoAsistentesParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                grupoRepository.CrearGruposEventosAsistentes(grupoEventoAsistentesParaCrear);

                int? codigoPersonaGrupo = await grupoRepository.BuscarCodigoPersonaDeUnGrupo(grupoEventoAsistentesParaCrear.CodigoEvento);

                if (!codigoPersonaGrupo.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo persona del grupo dueño de este evento, BUUUUUUGGG!..");
                }

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacion = new Notificaciones
                {
                    CodigoEvento = grupoEventoAsistentesParaCrear.CodigoEvento,
                    CodigoPersonaDestinoAccion = codigoPersonaGrupo.Value,
                    CodigoPersonaOrigenAccion = grupoEventoAsistentesParaCrear.CodigoPersona,
                    CodigoTipoNotificacion = (int)TipoNotificacionEnum.InscripcionEventoUsuario,
                    Creacion = DateTime.Now
                };
                noticiasRepo.CrearNotificacion(notificacion);

                WrapperSimpleTypesDTO wrapperCrearGrupoEventoAsistentes = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperCrearGrupoEventoAsistentes.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearGrupoEventoAsistentes.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearGrupoEventoAsistentes.Exitoso = true;
                    wrapperCrearGrupoEventoAsistentes.ConsecutivoCreado = grupoEventoAsistentesParaCrear.Consecutivo;

                    if (notificacion.Consecutivo > 0)
                    {
                        timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    }
                }

                return Tuple.Create(wrapperCrearGrupoEventoAsistentes, timeLineNotificacion);
            }
        }

        public async Task<WrapperSimpleTypesDTO> BuscarNumeroAsistentesGruposEventos(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroAsistentes = await grupoRepository.BuscarNumeroAsistentesGruposEventos(grupoEventoAsistenteParaBuscar);

                return wrapperNumeroAsistentes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> BuscarSiPersonaAsisteAGrupoEvento(GruposEventosAsistentes grupoEventoAsistenteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                WrapperSimpleTypesDTO wrapperBuscarSiPersonaAsisteAGrupoEvento = await grupoRepository.BuscarSiPersonaAsisteAGrupoEvento(grupoEventoAsistenteParaBuscar);

                return wrapperBuscarSiPersonaAsisteAGrupoEvento;
            }
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnEvento(GruposEventosAsistentes grupoEventoAsistenteParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnEvento = await grupoRepository.ListarEventosAsistentesDeUnEvento(grupoEventoAsistenteParaListar);

                return listaEventosAsistentesDeUnEvento;
            }
        }

        public async Task<List<GruposEventosAsistentesDTO>> ListarEventosAsistentesDeUnaPersona(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaFiltroBase != DateTime.MinValue)
                {
                    buscador.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaFiltroBase);
                }

                List<GruposEventosAsistentesDTO> listaEventosAsistentesDeUnaPersona = await grupoRepository.ListarEventosAsistentesDeUnaPersona(buscador);

                if (listaEventosAsistentesDeUnaPersona != null && listaEventosAsistentesDeUnaPersona.Count > 0)
                {
                    foreach (var eventoAsistente in listaEventosAsistentesDeUnaPersona)
                    {
                        eventoAsistente.GruposEventos.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventoAsistente.GruposEventos.Creacion);
                        eventoAsistente.GruposEventos.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventoAsistente.GruposEventos.FechaInicio);
                        eventoAsistente.GruposEventos.FechaTerminacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, eventoAsistente.GruposEventos.FechaTerminacion);
                    }
                }

                return listaEventosAsistentesDeUnaPersona;
            }
        }

        public async Task<int> CalcularDuracionPermitidaVideoParaUnEvento(int codigoEvento)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository gruposRepo = new GruposRepository(context);
                int? duracionVideoPermitida = await gruposRepo.ConsultarDuracionVideoParaElPlanDeEsteEvento(codigoEvento);

                if (!duracionVideoPermitida.HasValue)
                {
                    throw new InvalidOperationException("No se encontro la duracion del video permitida del grupo para este evento!.");
                }

                return duracionVideoPermitida.Value;
            }
        }

        public async Task<Tuple<WrapperSimpleTypesDTO, TimeLineNotificaciones>> EliminarGrupoEventoAsistente(GruposEventosAsistentes grupoEventoAsistenteParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                GruposRepository grupoRepository = new GruposRepository(context);
                GruposEventosAsistentes grupoEventoAsistenteBorrado = await grupoRepository.EliminarGrupoEventoAsistente(grupoEventoAsistenteParaEliminar);

                int? codigoPersonaGrupo = await grupoRepository.BuscarCodigoPersonaDeUnGrupo(grupoEventoAsistenteParaEliminar.CodigoEvento);

                if (!codigoPersonaGrupo.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo persona del grupo dueño de este evento, BUUUUUUGGG!..");
                }

                NoticiasRepository noticiasRepo = new NoticiasRepository(context);
                Notificaciones notificacion = new Notificaciones
                {
                    CodigoEvento = grupoEventoAsistenteParaEliminar.CodigoEvento,
                    CodigoPersonaDestinoAccion = codigoPersonaGrupo.Value,
                    CodigoPersonaOrigenAccion = grupoEventoAsistenteParaEliminar.CodigoPersona,
                    CodigoTipoNotificacion = (int)TipoNotificacionEnum.DesuscripcionEventoUsuario,
                    Creacion = DateTime.Now
                };
                noticiasRepo.CrearNotificacion(notificacion);

                WrapperSimpleTypesDTO wrapperEliminarGrupoEventoAsistente = new WrapperSimpleTypesDTO();
                TimeLineNotificaciones timeLineNotificacion = null;

                wrapperEliminarGrupoEventoAsistente.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarGrupoEventoAsistente.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarGrupoEventoAsistente.Exitoso = true;

                    if (notificacion.Consecutivo > 0)
                    {
                        timeLineNotificacion = new TimeLineNotificaciones(await noticiasRepo.BuscarNotificacion(notificacion));
                    }
                }

                return Tuple.Create(wrapperEliminarGrupoEventoAsistente, timeLineNotificacion);
            }
        }


        #endregion


    }
}
