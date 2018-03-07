using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class RepresentantesBusiness
    {


        #region Metodos Representantes


        public async Task<WrapperSimpleTypesDTO> CrearRepresentante(Representantes representanteParaCrear, string urlLogo, string urlBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanDefault = await planRepo.BuscarCodigoPlanDefault(TipoPerfil.Representante);

                if (!codigoPlanDefault.HasValue)
                {
                    throw new InvalidOperationException("No existe un plan default para los representante!.");
                }

                PlanesUsuarios planUsuarioDefault = new PlanesUsuarios
                {
                    CodigoPlan = codigoPlanDefault.Value,
                    Adquisicion = DateTime.Now,
                    Vencimiento = DateTime.MaxValue
                };

                representanteParaCrear.Personas.Usuarios.CuentaActiva = 0;
                representanteParaCrear.Personas.Usuarios.PlanesUsuarios = planUsuarioDefault;
                representanteParaCrear.Personas.Usuarios.TipoPerfil = representanteParaCrear.Personas.TipoPerfil;

                representanteParaCrear.Personas.Candidatos = null;
                representanteParaCrear.Personas.Paises = null;
                representanteParaCrear.Personas.Idiomas = null;
                representanteParaCrear.Personas.Anunciantes = null;
                representanteParaCrear.Personas.Grupos = null;
                representanteParaCrear.Personas.Representantes = null;
                representanteParaCrear.Personas.Usuarios.Personas = null;
                representanteParaCrear.Personas.Paises = null;
                representanteParaCrear.Personas.Idiomas = null;

                foreach (var categoriaRepresentante in representanteParaCrear.CategoriasRepresentantes)
                {
                    categoriaRepresentante.Categorias = null;
                }

                RepresentantesRepository representanteRepo = new RepresentantesRepository(context);
                representanteRepo.CrearRepresentante(representanteParaCrear);

                WrapperSimpleTypesDTO wrapperCrearRepresentante = new WrapperSimpleTypesDTO();

                wrapperCrearRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearRepresentante.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearRepresentante.Exitoso = true;
                    wrapperCrearRepresentante.ConsecutivoCreado = representanteParaCrear.Consecutivo;
                    wrapperCrearRepresentante.ConsecutivoPersonaCreado = representanteParaCrear.Personas.Consecutivo;
                    wrapperCrearRepresentante.ConsecutivoUsuarioCreado = representanteParaCrear.Personas.Usuarios.Consecutivo;

                    AuthenticateRepository authenticateRepo = new AuthenticateRepository(context);
                    string formatoEmail = await authenticateRepo.BuscarFormatoCorreoPorCodigoIdioma(representanteParaCrear.Personas.CodigoIdioma, TipoFormatosEnum.ConfirmacionCuenta);

                    if (!string.IsNullOrWhiteSpace(formatoEmail))
                    {
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderNombre, representanteParaCrear.Personas.Nombres);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenLogo, urlLogo);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderImagenBanner, urlBanner);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlWeb, URL.UrlWeb);

                        string urlConfirmacionFormated = string.Format(URL.UrlWeb + @"Authenticate/ConfirmationOfRegistration?ID={0}&Language={1}", representanteParaCrear.Personas.Usuarios.Consecutivo, representanteParaCrear.Personas.CodigoIdioma);
                        formatoEmail = formatoEmail.Replace(AppConstants.PlaceHolderUrlPaginaConfirmacion, urlConfirmacionFormated);

                        string tema = string.Empty;
                        switch (representanteParaCrear.Personas.IdiomaDeLaPersona)
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
                        CorreoHelper correoHelper = new CorreoHelper(representanteParaCrear.Personas.Usuarios.Email.Trim(), AppConstants.CorreoAplicacion, AppConstants.ClaveCorreoAplicacion);
                        wrapperCrearRepresentante.Exitoso = correoHelper.EnviarCorreoConHTML(formatoEmail, Correo.Gmail, tema, "SportsGo");
                    }
                    else
                    {
                        throw new InvalidOperationException("No hay formatos parametrizados para la confirmacion de la clave");
                    }
                }

                return wrapperCrearRepresentante;
            }
        }

        public async Task<Representantes> BuscarRepresentantePorCodigoPersona(Representantes representanteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                RepresentantesRepository representanteRepo = new RepresentantesRepository(context);
                Representantes informacionRepresentante = await representanteRepo.BuscarRepresentantePorCodigoPersona(representanteParaBuscar);

                return informacionRepresentante;
            }
        }

        public async Task<Representantes> BuscarRepresentantePorCodigoRepresentante(Representantes representanteParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                RepresentantesRepository representanteRepo = new RepresentantesRepository(context);
                Representantes informacionRepresentante = await representanteRepo.BuscarRepresentantePorCodigoRepresentante(representanteParaBuscar);

                return informacionRepresentante;
            }
        }

        public async Task<List<RepresentantesDTO>> ListarRepresentantes(Representantes representanteParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                RepresentantesRepository representanteRepo = new RepresentantesRepository(context);
                List<RepresentantesDTO> listarInformacionRepresentante = await representanteRepo.ListarRepresentantes(representanteParaListar);

                return listarInformacionRepresentante;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionRepresentante(Representantes representanteParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                RepresentantesRepository representanteRepo = new RepresentantesRepository(context);
                Representantes representanteExistente = await representanteRepo.ModificarInformacionRepresentante(representanteParaModificar);

                WrapperSimpleTypesDTO wrapperModificarInformacionRepresentante = new WrapperSimpleTypesDTO();

                wrapperModificarInformacionRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarInformacionRepresentante.NumeroRegistrosAfectados > 0) wrapperModificarInformacionRepresentante.Exitoso = true;

                return wrapperModificarInformacionRepresentante;
            }
        }

        #endregion


    }
}
