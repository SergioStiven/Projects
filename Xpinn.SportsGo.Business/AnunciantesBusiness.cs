using System;
using System.Collections.Generic;
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
    public class AnunciantesBusiness
    {


        #region Metodos Anunciantes


        public async Task<WrapperSimpleTypesDTO> CrearAnunciante(Anunciantes anuncianteParaCrear, string urlLogo, string urlBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanDefault = await planRepo.BuscarCodigoPlanDefault(TipoPerfil.Anunciante);

                if (!codigoPlanDefault.HasValue)
                {
                    throw new InvalidOperationException("No existe un plan default para los anunciantes!.");
                }

                PlanesUsuarios planUsuarioDefault = new PlanesUsuarios
                {
                    CodigoPlan = codigoPlanDefault.Value,
                    Adquisicion = DateTime.Now,
                    Vencimiento = DateTime.MaxValue
                };

                anuncianteParaCrear.Personas.Anunciantes = null;
                anuncianteParaCrear.Personas.Usuarios.Personas = null;

                anuncianteParaCrear.Personas.Usuarios.CuentaActiva = 0;
                anuncianteParaCrear.Personas.Usuarios.PlanesUsuarios = planUsuarioDefault;
                anuncianteParaCrear.Personas.Usuarios.TipoPerfil = anuncianteParaCrear.Personas.TipoPerfil;

                anuncianteParaCrear.Personas.Candidatos = null;
                anuncianteParaCrear.Personas.Paises = null;
                anuncianteParaCrear.Personas.Idiomas = null;
                anuncianteParaCrear.Personas.Anunciantes = null;
                anuncianteParaCrear.Personas.Grupos = null;
                anuncianteParaCrear.Personas.Representantes = null;
                anuncianteParaCrear.Personas.Usuarios.Personas = null;

                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                anuncianteRepo.CrearAnunciante(anuncianteParaCrear);

                WrapperSimpleTypesDTO wrapperCrearAnunciante = new WrapperSimpleTypesDTO();

                wrapperCrearAnunciante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearAnunciante.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearAnunciante.ConsecutivoCreado = anuncianteParaCrear.Consecutivo;
                    wrapperCrearAnunciante.ConsecutivoPersonaCreado = anuncianteParaCrear.Personas.Consecutivo;
                    wrapperCrearAnunciante.ConsecutivoUsuarioCreado = anuncianteParaCrear.Personas.Usuarios.Consecutivo;

                    AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                    string formatoEmail = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(anuncianteParaCrear.Personas.CodigoIdioma, TipoFormatosEnum.ConfirmacionCuenta);

                    if (!string.IsNullOrWhiteSpace(formatoEmail))
                    {
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderNombre, anuncianteParaCrear.Personas.Nombres);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenLogo, urlLogo);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenBanner, urlBanner);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlWeb, URL.UrlWeb);

                        string urlConfirmacionFormated = string.Format(URL.UrlWeb + @"Authenticate/ConfirmationOfRegistration?ID={0}&Language={1}", anuncianteParaCrear.Personas.Usuarios.Consecutivo, anuncianteParaCrear.Personas.CodigoIdioma);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlPaginaConfirmacion, urlConfirmacionFormated);

                        string tema = string.Empty;
                        switch (anuncianteParaCrear.Personas.IdiomaDeLaPersona)
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
                        CorreoHelper correoHelper = new CorreoHelper(anuncianteParaCrear.Personas.Usuarios.Email.Trim(), AppConstants.CorreoAplicacion, AppConstants.ClaveCorreoAplicacion);
                        wrapperCrearAnunciante.Exitoso = correoHelper.EnviarCorreoConHTML(formatoEmail, Correo.Gmail, tema, "SportsGo");
                    }
                    else
                    {
                        throw new InvalidOperationException("No hay formatos parametrizados para la confirmacion de la clave");
                    }
                }

                return wrapperCrearAnunciante;
            }
        }

        public async Task<Anunciantes> BuscarAnunciantePorConsecutivo(Anunciantes anuncianteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anunciantes anuncianteBuscado = await anuncianteRepo.BuscarAnunciantePorConsecutivo(anuncianteParaBuscar);

                return anuncianteBuscado;
            }
        }

        public async Task<Anunciantes> BuscarAnunciantePorCodigoPersona(Anunciantes anuncianteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anunciantes anuncianteBuscado = await anuncianteRepo.BuscarAnunciantePorCodigoPersona(anuncianteParaBuscar);

                return anuncianteBuscado;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionAnunciante(Anunciantes anuncianteParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anunciantes anuncianteExistente = await anuncianteRepo.ModificarInformacionAnunciante(anuncianteParaModificar);

                WrapperSimpleTypesDTO wrapperModificarAnunciante = new WrapperSimpleTypesDTO();

                wrapperModificarAnunciante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarAnunciante.NumeroRegistrosAfectados > 0) wrapperModificarAnunciante.Exitoso = true;

                return wrapperModificarAnunciante;
            }
        }


        #endregion


        #region Metodos Anuncios


        public async Task<WrapperSimpleTypesDTO> CrearAnuncio(Anuncios anuncioParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);

                PlanesDTO configuracionPlan = await anuncianteRepo.BuscarConfiguracionAnuncioPorPlanAnunciante(anuncioParaCrear.CodigoAnunciante);

                anuncioParaCrear.Vencimiento = DateTimeHelper.SumarDiasSegunTipoCalendario(anuncioParaCrear.FechaInicio, configuracionPlan.NumeroDiasVigenciaAnuncio);
                anuncioParaCrear.NumeroApariciones = configuracionPlan.NumeroAparicionesAnuncio;

                anuncianteRepo.CrearAnuncio(anuncioParaCrear);

                WrapperSimpleTypesDTO wrapperCrearAnuncio = new WrapperSimpleTypesDTO();

                wrapperCrearAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearAnuncio.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearAnuncio.Exitoso = true;
                    wrapperCrearAnuncio.ConsecutivoCreado = anuncioParaCrear.Consecutivo;
                }

                return wrapperCrearAnuncio;
            }
        }

        public async Task<Anuncios> BuscarAnuncioPorConsecutivo(Anuncios anuncioParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anuncios anuncioBuscado = await anuncianteRepo.BuscarAnuncioPorConsecutivo(anuncioParaBuscar);

                if (anuncioBuscado != null)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    anuncioBuscado.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncioBuscado.Creacion);
                    anuncioBuscado.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncioBuscado.FechaInicio);

                    if (anuncioBuscado.Vencimiento.HasValue)
                    {
                        anuncioBuscado.Vencimiento = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncioBuscado.Vencimiento.Value);
                    }
                }

                return anuncioBuscado;
            }
        }

        public async Task<List<AnunciosDTO>> ListarAnunciosDeUnAnunciante(Anuncios anuncioParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                List<AnunciosDTO> listarInformacionAnuncios = await anuncianteRepo.ListarAnunciosDeUnAnunciante(anuncioParaListar);

                if (listarInformacionAnuncios != null && listarInformacionAnuncios.Count > 0) 
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    foreach (var anuncios in listarInformacionAnuncios)
                    {
                        anuncios.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.Creacion);
                        anuncios.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.FechaInicio);

                        if (anuncios.Vencimiento.HasValue)
                        {
                            anuncios.Vencimiento = helper.ConvertDateTimeFromAnotherTimeZone(anuncioParaListar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.Vencimiento.Value);
                        }
                    }
                }

                return listarInformacionAnuncios;
            }
        }

        public async Task<List<AnunciosDTO>> ListarAnuncios(BuscadorDTO buscador)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();

                if (buscador.FechaInicio != DateTime.MinValue)
                {
                    buscador.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, buscador.FechaInicio);
                }

                List<AnunciosDTO> listarInformacionAnuncios = await anuncianteRepo.ListarAnuncios(buscador);

                if (listarInformacionAnuncios != null && listarInformacionAnuncios.Count > 0)
                {
                    
                    foreach (var anuncios in listarInformacionAnuncios)
                    {
                        anuncios.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.Creacion);
                        anuncios.FechaInicio = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.FechaInicio);

                        if (anuncios.Vencimiento.HasValue)
                        {
                            anuncios.Vencimiento = helper.ConvertDateTimeFromAnotherTimeZone(buscador.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, anuncios.Vencimiento.Value);
                        }
                    }
                }

                return listarInformacionAnuncios;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarAnuncio(Anuncios anuncioParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anuncios anuncioExistente = await anuncianteRepo.ModificarAnuncio(anuncioParaModificar);

                if (anuncioParaModificar.AnunciosContenidos != null && anuncioParaModificar.AnunciosContenidos.Count > 0)
                {
                    foreach (AnunciosContenidos anuncioContenido in anuncioParaModificar.AnunciosContenidos)
                    {
                        AnunciosContenidos anuncioContenidoExistente = await anuncianteRepo.ModificarAnuncioContenido(anuncioContenido);
                    }
                }

                if (anuncioParaModificar.AnunciosPaises != null && anuncioParaModificar.AnunciosPaises.Count > 0)
                {
                    AnunciosPaises anuncioPaisParaBorrar = new AnunciosPaises
                    {
                        CodigoAnuncio = anuncioParaModificar.Consecutivo
                    };

                    anuncianteRepo.EliminarMultiplesAnuncioPais(anuncioPaisParaBorrar);
                    anuncianteRepo.CrearAnunciosPaises(anuncioParaModificar.AnunciosPaises);
                }

                if (anuncioParaModificar.CategoriasAnuncios != null && anuncioParaModificar.CategoriasAnuncios.Count > 0)
                {
                    CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                    CategoriasAnuncios categoriaAnuncioParaBorrar = new CategoriasAnuncios
                    {
                        CodigoAnuncio = anuncioParaModificar.Consecutivo
                    };

                    categoriasRepo.EliminarMultiplesCategoriasAnuncios(categoriaAnuncioParaBorrar);
                    categoriasRepo.CrearListaCategoriaAnuncios(anuncioParaModificar.CategoriasAnuncios);
                }

                WrapperSimpleTypesDTO wrapperModificarAnuncio = new WrapperSimpleTypesDTO();

                wrapperModificarAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarAnuncio.NumeroRegistrosAfectados > 0) wrapperModificarAnuncio.Exitoso = true;

                return wrapperModificarAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AumentarContadorClickDeUnAnuncio(Anuncios anuncioParaAumentar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anuncios anuncioExistente = await anuncianteRepo.AumentarContadorClickDeUnAnuncio(anuncioParaAumentar);

                WrapperSimpleTypesDTO wrapperAumentarContadorClickDeUnAnuncio = new WrapperSimpleTypesDTO();

                wrapperAumentarContadorClickDeUnAnuncio.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                if (wrapperAumentarContadorClickDeUnAnuncio.NumeroRegistrosAfectados > 0)
                {
                    wrapperAumentarContadorClickDeUnAnuncio.Exitoso = true;
                }

                return wrapperAumentarContadorClickDeUnAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncio(Anuncios anuncioParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                AnunciosContenidos anuncioContenidoParaBorrar = new AnunciosContenidos
                {
                    CodigoAnuncio = anuncioParaEliminar.Consecutivo
                };
                anuncianteRepo.EliminarMultiplesContenidosAnuncios(anuncioContenidoParaBorrar);

                AnunciosPaises anuncioPaisesParaBorrar = new AnunciosPaises
                {
                    CodigoAnuncio = anuncioParaEliminar.Consecutivo
                };
                anuncianteRepo.EliminarMultiplesAnuncioPais(anuncioPaisesParaBorrar);

                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasAnuncios categoriasAnunciosParaBorrar = new CategoriasAnuncios
                {
                    CodigoAnuncio = anuncioParaEliminar.Consecutivo
                };
                categoriasRepo.EliminarMultiplesCategoriasAnuncios(categoriasAnunciosParaBorrar);

                int? codigoArchivoDeAnuncio = await anuncianteRepo.BuscarArchivoDeUnAnuncio(anuncioParaEliminar);
                anuncianteRepo.EliminarAnuncio(anuncioParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarAnuncio = new WrapperSimpleTypesDTO();

                wrapperEliminarAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (codigoArchivoDeAnuncio.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    Archivos archivoParaEliminar = new Archivos
                    {
                        Consecutivo = codigoArchivoDeAnuncio.Value,
                    };
                    archivoRepo.EliminarArchivo(archivoParaEliminar);
                }

                wrapperEliminarAnuncio.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                if (wrapperEliminarAnuncio.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarAnuncio.Exitoso = true;
                }

                return wrapperEliminarAnuncio;
            }
        }

        public async Task<int> CalcularDuracionPermitidaVideoParaUnAnuncio(int codigoAnuncio)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                int? duracionVideoPermitida = await anuncianteRepo.ConsultarDuracionVideoParaElPlanDeEsteAnuncio(codigoAnuncio);

                if (!duracionVideoPermitida.HasValue)
                {
                    throw new InvalidOperationException("No se encontro la duracion del video permitida del anunciante de este anuncio!.");
                }

                return duracionVideoPermitida.Value;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoAnuncio(Anuncios anuncioArchivoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = anuncioArchivoParaEliminar.CodigoArchivo.Value,
                };

                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                Anuncios anuncioExistente = await anuncianteRepo.DesasignarArchivoAnuncio(anuncioArchivoParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperDesasignarArchivoAnuncio = new WrapperSimpleTypesDTO();

                wrapperDesasignarArchivoAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperDesasignarArchivoAnuncio.NumeroRegistrosAfectados > 0)
                {
                    wrapperDesasignarArchivoAnuncio.Exitoso = true;
                }

                return wrapperDesasignarArchivoAnuncio;
            }
        }


        #endregion


        #region Metodos AnunciosContenidos


        public async Task<WrapperSimpleTypesDTO> CrearAnunciosContenidos(List<AnunciosContenidos> anuncioContenidoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                anuncianteRepo.CrearAnunciosContenidos(anuncioContenidoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearAnunciosContenido = new WrapperSimpleTypesDTO();

                wrapperCrearAnunciosContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearAnunciosContenido.NumeroRegistrosAfectados > 0) wrapperCrearAnunciosContenido.Exitoso = true;

                return wrapperCrearAnunciosContenido;
            }
        }

        public async Task<AnunciosContenidos> BuscarAnuncioContenidoPorConsecutivo(AnunciosContenidos anuncioContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                AnunciosContenidos anuncioContenidoBuscado = await anuncianteRepo.BuscarAnuncioContenidoPorConsecutivo(anuncioContenidoParaBuscar);

                return anuncioContenidoBuscado;
            }
        }

        public async Task<List<AnunciosContenidos>> ListarAnunciosContenidosDeUnAnuncio(AnunciosContenidos anuncioContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                List<AnunciosContenidos> listaContenidoDeUnAnuncio = await anuncianteRepo.ListarAnunciosContenidosDeUnAnuncio(anuncioContenidoParaListar);

                return listaContenidoDeUnAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarAnuncioContenido(AnunciosContenidos anuncioContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                AnunciosContenidos anuncioContenidoExistente = await anuncianteRepo.ModificarAnuncioContenido(anuncioContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarAnuncioContenido = new WrapperSimpleTypesDTO();

                wrapperModificarAnuncioContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarAnuncioContenido.NumeroRegistrosAfectados > 0) wrapperModificarAnuncioContenido.Exitoso = true;

                return wrapperModificarAnuncioContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMultiplesAnuncioContenido(List<AnunciosContenidos> anuncioContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);

                foreach (var anuncioContenido in anuncioContenidoParaModificar)
                {
                    AnunciosContenidos anuncioContenidoExistente = await anuncianteRepo.ModificarAnuncioContenido(anuncioContenido);
                }

                WrapperSimpleTypesDTO wrapperModificarMultiplesAnuncioContenido = new WrapperSimpleTypesDTO();

                wrapperModificarMultiplesAnuncioContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarMultiplesAnuncioContenido.NumeroRegistrosAfectados > 0) wrapperModificarMultiplesAnuncioContenido.Exitoso = true;

                return wrapperModificarMultiplesAnuncioContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncioContenido(AnunciosContenidos anuncioContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                anuncianteRepo.EliminarAnuncioContenido(anuncioContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarAnuncioContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarAnuncioContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarAnuncioContenido.NumeroRegistrosAfectados > 0) wrapperEliminarAnuncioContenido.Exitoso = true;

                return wrapperEliminarAnuncioContenido;
            }
        }


        #endregion


        #region Metodos AnunciosPaises


        public async Task<WrapperSimpleTypesDTO> CrearAnunciosPaises(List<AnunciosPaises> anuncioPaisParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                anuncianteRepo.CrearAnunciosPaises(anuncioPaisParaCrear);

                WrapperSimpleTypesDTO wrapperCrearAnunciosPaises = new WrapperSimpleTypesDTO();

                wrapperCrearAnunciosPaises.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearAnunciosPaises.NumeroRegistrosAfectados > 0) wrapperCrearAnunciosPaises.Exitoso = true;

                return wrapperCrearAnunciosPaises;
            }
        }

        public async Task<AnunciosPaises> BuscaAnuncioPaisPorConsecutivo(AnunciosPaises anuncioPaisParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                AnunciosPaises anuncioPaisBuscado = await anuncianteRepo.BuscaAnuncioPaisPorConsecutivo(anuncioPaisParaBuscar);

                return anuncioPaisBuscado;
            }
        }

        public async Task<List<AnunciosPaises>> ListarAnunciosPaisesDeUnAnuncio(AnunciosPaises anuncioPaisesParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                List<AnunciosPaises> listaPaisesDeUnAnuncio = await anuncianteRepo.ListarAnunciosPaisesDeUnAnuncio(anuncioPaisesParaListar);

                return listaPaisesDeUnAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncioPais(AnunciosPaises anuncioPaisParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                AnunciantesRepository anuncianteRepo = new AnunciantesRepository(context);
                anuncianteRepo.EliminarAnuncioPais(anuncioPaisParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarAnuncioPais = new WrapperSimpleTypesDTO();

                wrapperEliminarAnuncioPais.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarAnuncioPais.NumeroRegistrosAfectados > 0) wrapperEliminarAnuncioPais.Exitoso = true;

                return wrapperEliminarAnuncioPais;
            }
        }


        #endregion


    }
}