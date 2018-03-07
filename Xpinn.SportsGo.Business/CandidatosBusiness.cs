using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using System.IO;
using Xpinn.SportsGo.Util.Portable.HelperClasses;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.HelperClasses;
using System.Text.RegularExpressions;

namespace Xpinn.SportsGo.Business
{
    public class CandidatosBusiness
    {


        #region Metodos Candidatos


        public async Task<WrapperSimpleTypesDTO> CrearCandidato(Candidatos candidatoParaCrear, string urlLogo, string urlBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanDefault = await planRepo.BuscarCodigoPlanDefault(TipoPerfil.Candidato);

                if (!codigoPlanDefault.HasValue)
                {
                    throw new InvalidOperationException("No existe un plan default para los candidatos!.");
                }

                PlanesUsuarios planUsuarioDefault = new PlanesUsuarios
                {
                    CodigoPlan = codigoPlanDefault.Value,
                    Adquisicion = DateTime.Now,
                    Vencimiento = DateTime.MaxValue
                };

                candidatoParaCrear.Personas.Usuarios.CuentaActiva = 0;
                candidatoParaCrear.Personas.Usuarios.PlanesUsuarios = planUsuarioDefault;
                candidatoParaCrear.Personas.Usuarios.TipoPerfil = candidatoParaCrear.Personas.TipoPerfil;

                bool soyMenorDeEdad = DateTimeHelper.DiferenciaEntreDosFechasAños(DateTime.Now, candidatoParaCrear.FechaNacimiento) < AppConstants.MayoriaEdad;

                // Si soy menor de edad y no tengo tutor o el tutor no tiene email, explota
                if (soyMenorDeEdad && (candidatoParaCrear.CandidatosResponsables == null || string.IsNullOrWhiteSpace(candidatoParaCrear.CandidatosResponsables.Email)))
                {
                    throw new InvalidOperationException("Falta informacion para el tutor, esta persona es menor de edad!.");
                }

                // Si no requiero tutor no lo guardo
                if (soyMenorDeEdad)
                {
                    candidatoParaCrear.CandidatosResponsables.Candidatos = null;
                }
                else
                {
                    candidatoParaCrear.CandidatosResponsables = null;
                }

                candidatoParaCrear.Personas.Candidatos = null;
                candidatoParaCrear.Personas.Paises = null;
                candidatoParaCrear.Personas.Idiomas = null;
                candidatoParaCrear.Generos = null;
                candidatoParaCrear.Personas.Anunciantes = null;
                candidatoParaCrear.Personas.Grupos = null;
                candidatoParaCrear.Personas.Representantes = null;
                candidatoParaCrear.Personas.Usuarios.Personas = null;

                foreach (var categoriaCandidato in candidatoParaCrear.CategoriasCandidatos)
                {
                    categoriaCandidato.Categorias = null;
                    categoriaCandidato.Candidatos = null;

                    foreach (var habilidadCandidato in categoriaCandidato.HabilidadesCandidatos)
                    {
                        habilidadCandidato.Habilidades = null;
                    }
                }

                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                candidatosRepo.CrearCandidato(candidatoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCandidato = new WrapperSimpleTypesDTO();

                wrapperCrearCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCandidato.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCandidato.ConsecutivoCreado = candidatoParaCrear.Consecutivo;
                    wrapperCrearCandidato.ConsecutivoPersonaCreado = candidatoParaCrear.Personas.Consecutivo;
                    wrapperCrearCandidato.ConsecutivoUsuarioCreado = candidatoParaCrear.Personas.Usuarios.Consecutivo;

                    AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                    string formatoEmail = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(candidatoParaCrear.Personas.CodigoIdioma, TipoFormatosEnum.ConfirmacionCuenta);

                    if (!string.IsNullOrWhiteSpace(formatoEmail))
                    {
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderNombre, candidatoParaCrear.Personas.Nombres);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenLogo, urlLogo);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenBanner, urlBanner);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlWeb, URL.UrlWeb);

                        string urlConfirmacionFormated = string.Format(URL.UrlWeb + @"Authenticate/ConfirmationOfRegistration?ID={0}&Language={1}", candidatoParaCrear.Personas.Usuarios.Consecutivo, candidatoParaCrear.Personas.CodigoIdioma);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlPaginaConfirmacion, urlConfirmacionFormated);

                        string tema = string.Empty;
                        switch (candidatoParaCrear.Personas.IdiomaDeLaPersona)
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

                        string emailParaEnviarConfirmacion = string.Empty;

                        // Si soy menor de edad el correo de confirmacion va hacia el tutor, si no va normal como siempre
                        if (soyMenorDeEdad)
                        {
                            emailParaEnviarConfirmacion = candidatoParaCrear.CandidatosResponsables.Email.Trim();
                        }
                        else
                        {
                            emailParaEnviarConfirmacion = candidatoParaCrear.Personas.Usuarios.Email.Trim();
                        }

                        // Recordar configurar la cuenta Gmail en este caso para que permita el logeo de manera insegura y poder mandar correos
                        // https://myaccount.google.com/lesssecureapps?pli=1
                        CorreoHelper correoHelper = new CorreoHelper(emailParaEnviarConfirmacion, AppConstants.CorreoAplicacion, AppConstants.ClaveCorreoAplicacion);
                        wrapperCrearCandidato.Exitoso = correoHelper.EnviarCorreoConHTML(formatoEmail, Correo.Gmail, tema, "SportsGo");
                    }
                    else
                    {
                        throw new InvalidOperationException("No hay formatos parametrizados para la confirmacion de la clave");
                    }
                }

                return wrapperCrearCandidato;
            }
        }

        public async Task<Candidatos> BuscarCandidatoPorCodigoPersona(Candidatos candidatoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);

                Candidatos informacionCandidato = await candidatosRepo.BuscarCandidatoPorCodigoPersona(candidatoParaBuscar);

                return informacionCandidato;
            }
        }

        public async Task<Candidatos> BuscarCandidatoPorCodigoCandidato(Candidatos candidatoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);

                Candidatos informacionCandidato = await candidatosRepo.BuscarCandidatoPorCodigoCandidato(candidatoParaBuscar);

                return informacionCandidato;
            }
        }

        public async Task<List<CandidatosDTO>> ListarCandidatos(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                List<CandidatosDTO> listaInformacionCandidatos = await candidatosRepo.ListarCandidatos(buscador);

                return listaInformacionCandidatos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionCandidato(Candidatos candidatoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                Candidatos candidatoExistente = await candidatosRepo.ModificarInformacionCandidato(candidatoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarInformacionCandidato = new WrapperSimpleTypesDTO();

                wrapperModificarInformacionCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarInformacionCandidato.NumeroRegistrosAfectados > 0) wrapperModificarInformacionCandidato.Exitoso = true;

                return wrapperModificarInformacionCandidato;
            }
        }


        #endregion


        #region Metodos CandidatosVideos


        public async Task<WrapperSimpleTypesDTO> CrearCandidatoVideo(CandidatosVideos candidatoVideoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                candidatosRepo.CrearCandidatoVideo(candidatoVideoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearVideoCandidato = new WrapperSimpleTypesDTO();

                wrapperCrearVideoCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearVideoCandidato.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearVideoCandidato.Exitoso = true;
                    wrapperCrearVideoCandidato.ConsecutivoCreado = candidatoVideoParaCrear.Consecutivo;
                }

                return wrapperCrearVideoCandidato;
            }
        }

        public async Task<CandidatosVideos> BuscarCandidatoVideo(CandidatosVideos candidatoVideoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                CandidatosVideos candidatoVideoBuscado = await candidatosRepo.BuscarCandidatoVideo(candidatoVideoParaBuscar);

                if (candidatoVideoBuscado != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    candidatoVideoBuscado.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(candidatoVideoParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, candidatoVideoBuscado.Creacion);
                }

                return candidatoVideoBuscado;
            }
        }

        public async Task<int> CalcularDuracionPermitidaVideoParaUnaPublicacionCandidato(int codigoCandidatoVideo)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                int? duracionVideoPermitida = await candidatosRepo.ConsultarDuracionVideoParaElPlanDeEstaPublicacionCandidato(codigoCandidatoVideo);

                if (!duracionVideoPermitida.HasValue)
                {
                    throw new InvalidOperationException("No se encontro la duracion del video permitida del candidato para esta publicacion!.");
                }

                return duracionVideoPermitida.Value;
            }
        }

        public async Task<List<CandidatosVideosDTO>> ListarCandidatosVideosDeUnCandidato(CandidatosVideos candidatoVideoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (candidatoVideoParaListar.FechaFiltroBase != DateTime.MinValue)
                {
                    candidatoVideoParaListar.FechaFiltroBase = helper.ConvertDateTimeFromAnotherTimeZone(candidatoVideoParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, candidatoVideoParaListar.FechaFiltroBase);
                }

                List<CandidatosVideosDTO> listaVideosCandidato = await candidatosRepo.ListarCandidatosVideosDeUnCandidato(candidatoVideoParaListar);

                if (listaVideosCandidato != null && listaVideosCandidato.Count > 0)
                {
                    foreach (var videoCandidato in listaVideosCandidato)
                    {
                        videoCandidato.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(candidatoVideoParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, videoCandidato.Creacion);
                    }
                }

                return listaVideosCandidato;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCandidatoVideo(CandidatosVideos candidatoVideoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                CandidatosVideos candidatoVideoExistente = await candidatosRepo.ModificarCandidatoVideo(candidatoVideoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCandidatoVideo = new WrapperSimpleTypesDTO();

                wrapperModificarCandidatoVideo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCandidatoVideo.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarCandidatoVideo.Exitoso = true;
                }

                return wrapperModificarCandidatoVideo;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCandidatoVideo(CandidatosVideos candidatoVideosParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = candidatoVideosParaEliminar.CodigoArchivo,
                };

                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                candidatosRepo.EliminarCandidatoVideo(candidatoVideosParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarCandidatoVideo = new WrapperSimpleTypesDTO();

                wrapperEliminarCandidatoVideo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCandidatoVideo.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarCandidatoVideo.Exitoso = true;
                }

                return wrapperEliminarCandidatoVideo;
            }
        }


        #endregion


        #region Metodos Responsables


        public async Task<WrapperSimpleTypesDTO> CrearCandidatoResponsable(CandidatosResponsables candidatoResponsableParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                candidatosRepo.CrearCandidatoResponsable(candidatoResponsableParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCandidatoResponsable = new WrapperSimpleTypesDTO();

                wrapperCrearCandidatoResponsable.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCandidatoResponsable.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCandidatoResponsable.Exitoso = true;
                    wrapperCrearCandidatoResponsable.ConsecutivoCreado = candidatoResponsableParaCrear.Consecutivo;
                }

                return wrapperCrearCandidatoResponsable;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaAsignar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);

                Candidatos candidatoExistente = await candidatosRepo.BuscarCandidatoParaAsignar(candidatoResponsableParaAsignar);

                candidatoResponsableParaAsignar.Consecutivo = candidatoExistente.CodigoResponsable.HasValue ? candidatoExistente.CodigoResponsable.Value : 0;

                if (candidatoExistente.CodigoResponsable.HasValue)
                {
                    CandidatosResponsables candidatoResponsableExistente = await candidatosRepo.ModificarCandidatoResponsable(candidatoResponsableParaAsignar);
                }
                else
                {
                    candidatoExistente.CandidatosResponsables = candidatoResponsableParaAsignar;
                }

                WrapperSimpleTypesDTO wrapperCrearCandidatoResponsable = new WrapperSimpleTypesDTO();

                wrapperCrearCandidatoResponsable.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCandidatoResponsable.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCandidatoResponsable.Exitoso = true;
                    wrapperCrearCandidatoResponsable.ConsecutivoCreado = candidatoExistente.CodigoResponsable.Value;
                }

                return wrapperCrearCandidatoResponsable;
            }
        }

        public async Task<CandidatosResponsables> BuscarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                CandidatosResponsables candidatoResponsableBuscado = await candidatosRepo.BuscarCandidatoResponsable(candidatoResponsableParaBuscar);

                return candidatoResponsableBuscado;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                CandidatosResponsables candidatoResponsableExistente = await candidatosRepo.ModificarCandidatoResponsable(candidatoResponsableParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCandidatoResponsable = new WrapperSimpleTypesDTO();

                wrapperModificarCandidatoResponsable.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCandidatoResponsable.NumeroRegistrosAfectados > 0) wrapperModificarCandidatoResponsable.Exitoso = true;

                return wrapperModificarCandidatoResponsable;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCandidatoResponsable(Candidatos candidatoResponsableParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CandidatosRepository candidatosRepo = new CandidatosRepository(context);
                Candidatos candidatoExistente = await candidatosRepo.DesasignarResponsableDeUnCandidato(candidatoResponsableParaBorrar);

                CandidatosResponsables responsableABorrar = new CandidatosResponsables
                {
                    Consecutivo = candidatoResponsableParaBorrar.CodigoResponsable.Value
                };

                candidatosRepo.EliminarCandidatoResponsable(responsableABorrar);

                WrapperSimpleTypesDTO wrapperEliminarCandidatoResponsable = new WrapperSimpleTypesDTO();

                wrapperEliminarCandidatoResponsable.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCandidatoResponsable.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarCandidatoResponsable.Exitoso = true;
                }

                return wrapperEliminarCandidatoResponsable;
            }
        }


        #endregion


    }
}
