using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.MVC.Models;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Controllers
{
    public class ProfileController : BaseController
    {
        #region Views

        public async Task<ActionResult> Index()
        {
            if (GetPersonToSearch() != null && GetPersonToSearch().Consecutivo != 0)
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                switch (GetPersonToSearch().TipoPerfil)
                {
                    case TipoPerfil.Candidato:
                        if (!await ValidateIfOperationIsSupportedByPlan(TipoOperacion.DetalleCandidatos))
                            return View("Unauthorized");
                        break;
                    case TipoPerfil.Grupo:
                        if (!await ValidateIfOperationIsSupportedByPlan(TipoOperacion.DetalleGrupos))
                            return View("Unauthorized");
                        break;
                    default:
                        break;
                }
            }
            return View();
        }

        public ActionResult MyProfile()
        {
            SetPersonToSearch(null);
            return RedirectToAction("Index", "Profile");
        }

        public ActionResult Posts()
        {
            return View();
        }

        public ActionResult GeneralInformation()
        {
            return View();
        }

        public ActionResult InformationByProfile()
        {
            return View();
        }

        #endregion

        #region General
        [HttpGet]
        public async Task<JsonResult> GetInfoPersonLoggedIn(PersonasDTO person)
        {
            Result<PersonasDTO> result = new Result<PersonasDTO>();
            try
            {
                if (GetPersonToSearch() != null && GetPersonToSearch().Consecutivo != 0)
                {
                    PersonasServices personToSearchService = new PersonasServices();
                    GetPersonToSearch().ConsecutivoViendoPersona = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                    GetPersonToSearch().IdiomaDeLaPersona = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona;
                    result.obj = await personToSearchService.BuscarPersona(GetPersonToSearch());
                    if (result.obj != null)
                    {
                        result.obj.Consecutivo = result.obj.Consecutivo == UserLoggedIn().PersonaDelUsuario.Consecutivo ? -2 : -1;
                        result.obj.CodigoIdioma = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                else if (UserLoggedIn().PersonaDelUsuario.Consecutivo == 0)
                {
                    result.Success = false;
                    result.obj = new PersonasDTO { TipoPerfil = UserLoggedIn().TipoPerfil, Usuarios = UserLoggedIn(), IdiomaDeLaPersona = UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                JsonResult res = await getInfoPerson();
                return res;
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSesion(), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> getInfoPerson()
        {
            Result<PersonasDTO> result = new Result<PersonasDTO>();

            try
            {
                PersonasServices personService = new PersonasServices();
                PersonasDTO personToSearch = new PersonasDTO();
                personToSearch.Consecutivo = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                personToSearch.CodigoIdioma = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.obj = await personService.BuscarPersona(personToSearch);
                if (result.obj != null)
                    return Json(result, JsonRequestBehavior.AllowGet);

                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                result.Success = false;
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetProfile()
        {
            Result<UsuariosDTO> result = new Result<UsuariosDTO>();
            try
            {
                result.obj = UserLoggedIn();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices personService = new ArchivosServices();
                int ConsecutivoArchivo = int.Parse(Request.Form["ConsecutivoArchivo"].ToString());
                if (ConsecutivoArchivo != 0)
                    result.obj = await personService.ModificarArchivoStream(ConsecutivoArchivo, Helper.getFileType(Request.Files[0].FileName), Request.Files[0].InputStream);
                else
                    result.obj = await personService.CrearArchivoStream(Helper.getFileType(Request.Files[0].FileName), Request.Files[0].InputStream);

                if (result.obj == null || !result.obj.Exitoso)
                    return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UploadVideoToControlDuration()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();

                int typeFileId = Helper.getFileType(Request.Files[0].FileName);

                if(typeFileId == 2)
                    result.obj = await fileService.CrearArchivoStreamYControlarDuracionVideo(typeFileId, UserLoggedIn().PlanesUsuarios.Planes.TiempoPermitidoVideo, Request.Files[0].InputStream);
                else
                    result.obj = await fileService.CrearArchivoStream(typeFileId, Request.Files[0].InputStream);

                if (result.obj == null || !result.obj.Exitoso)
                    return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Person
        [HttpPost]
        public async Task<JsonResult> UpdatePerson(PersonasDTO person)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                switch (person.TipoPerfil)
                {
                    case TipoPerfil.Candidato:
                        CandidatosDTO candidateToUpdate = new CandidatosDTO();
                        candidateToUpdate = person.CandidatoDeLaPersona;
                        person.CandidatoDeLaPersona = null;
                        candidateToUpdate.Personas = person;
                        return await CreateCandidate(candidateToUpdate);

                    case TipoPerfil.Grupo:
                        GruposDTO groupToUpdate = new GruposDTO();
                        groupToUpdate = person.GrupoDeLaPersona;
                        person.GrupoDeLaPersona = null;
                        groupToUpdate.Personas = person;
                        return await CreateGroup(groupToUpdate);

                    case TipoPerfil.Representante:
                        RepresentantesDTO agentToUpdate = new RepresentantesDTO();
                        agentToUpdate = person.RepresentanteDeLaPersona;
                        person.RepresentanteDeLaPersona = null;
                        agentToUpdate.Personas = person;
                        return await CreateAgent(agentToUpdate);

                    default:
                        result.Success = false;
                        result.Message = "No es un perfíl válido";
                        return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> uploadImageProfile()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();
                if (UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenPerfil == null)
                    UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenPerfil = 0;

                result.obj = await fileService.AsignarImagenPerfilPersona(UserLoggedIn().PersonaDelUsuario.Consecutivo, (int)UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenPerfil, Request.Files[0].InputStream);
                if (result.obj != null && result.obj.Exitoso)
                {
                    UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenPerfil = (int)result.obj.ConsecutivoArchivoCreado;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> uploadImageBanner()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();
                if (UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenBanner == null)
                    UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenBanner = 0;

                result.obj = await fileService.AsignarImagenBannerPersona(UserLoggedIn().PersonaDelUsuario.Consecutivo, (int)UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenBanner, Request.Files[0].InputStream);
                if (result.obj != null && result.obj.Exitoso)
                {
                    UserLoggedIn().PersonaDelUsuario.CodigoArchivoImagenBanner = (int)result.obj.ConsecutivoArchivoCreado;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public async Task<JsonResult> validateOperationByPlan()
        {
            if (await ValidateIfOperationIsSupportedByPlan(TipoOperacion.VideosPerfil))
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            return Json(Helper.returnErrorUnauthorizedByPlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Candidates
        [HttpPost]
        public async Task<JsonResult> CreateCandidate(CandidatosDTO candidato)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                string newEmail = candidato.Personas.Usuarios.Email;
                candidato.Personas.CodigoUsuario = UserLoggedIn().Consecutivo;
                candidato.Personas.Usuarios = UserLoggedIn();
                candidato.Personas.Usuarios.Email = newEmail;
                
                CandidatosServices canditateService = new CandidatosServices();
                PersonasServices personService = new PersonasServices();
                AuthenticateServices usuarioService = new AuthenticateServices();
                if (candidato.Consecutivo != 0)
                {
                    candidato.CategoriasCandidatos = null;
                    result.obj = await personService.ModificarPersona(candidato.Personas); // Update person
                    if (result.obj != null)
                        result.obj = await canditateService.ModificarInformacionCandidato(candidato); // Update candidate
                    if (result.obj != null)
                        result.obj = await usuarioService.ModificarEmailUsuario(candidato.Personas.Usuarios); // Update email user
                    if (result.obj != null)
                        UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona = candidato.Personas.IdiomaDeLaPersona;
                }
                else
                {
                    candidato.CategoriasCandidatos.ToList().ForEach(c => c.Categorias = null);
                    result.obj = await canditateService.CrearCandidato(candidato); // Create a new candidate
                    if (result.obj != null)
                    {
                        AuthenticateServices service = new AuthenticateServices();
                        UsuariosDTO userToValidate = UserLoggedIn();
                        userToValidate.Personas = null;
                        userToValidate.TiposPerfiles = null;
                        userToValidate.PlanesUsuarios = null;
                        var user = await service.VerificarUsuario(userToValidate);
                        if (user != null)
                        {
                            setUserLogin(user);
                        }
                    }
                }
                    

                if (result.obj == null)
                {
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                result.Message = "La información se ha guardado con éxito";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> CreateCandidateSkills(CategoriasCandidatosDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                category.Categorias = null;
                category.HabilidadesCandidatos.ToList().ForEach(c => { c.Habilidades = null; c.Consecutivo = 0; c.CodigoCategoriaCandidato = 0; });
                CategoriasServices categoryService = new CategoriasServices();
                if (category.Consecutivo != 0)
                    result.obj = await categoryService.ModificarCategoriaCandidato(category);
                else
                {
                    result.obj = await categoryService.CrearCategoriaCandidatos(category);
                }
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteCandidateSkills(CategoriasCandidatosDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoryService = new CategoriasServices();
                result.obj = await categoryService.EliminarCategoriaCandidato(category);
                if (result.obj == null)
                    return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorDelete(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetListAbilitiesByCandidate(HabilidadesCandidatosDTO ability)
        {
            Result<HabilidadesCandidatosDTO> result = new Result<HabilidadesCandidatosDTO>();
            try
            {
                HabilidadesServices categoryService = new HabilidadesServices();
                ability.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await categoryService.ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(ability);
                if (result.list == null || result.list.Count == 0)
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> GetListPostsByCandidate(CandidatosVideosDTO posts)
        {
            if (posts.CodigoCandidato == 0)
                return Json(Helper.returnNoDataList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            Result<CandidatosVideosDTO> result = new Result<CandidatosVideosDTO>();
            try
            {
                CandidatosServices categoryService = new CandidatosServices();
                result.list = await categoryService.ListarCandidatosVideosDeUnCandidato(posts);
                if (result.list == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> CreatePostsCandidate(CandidatosVideosDTO post)
        {
            if (!await ValidateIfOperationIsSupportedByPlan(TipoOperacion.VideosPerfil))
                return Json(Helper.returnErrorUnauthorizedByPlan(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CandidatosServices categoryService = new CandidatosServices();
                if(post.Consecutivo != 0)
                {
                    post.ArchivoContenido = null;
                    result.obj = await categoryService.ModificarCandidatoVideo(post);
                }
                    
                else
                    result.obj = await categoryService.CrearCandidatoVideo(post);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeletePostsCandidate(CandidatosVideosDTO post)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CandidatosServices categoryService = new CandidatosServices();
                result.obj = await categoryService.EliminarCandidatoVideo(post);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> CreateTutorCandidate(CandidatosResponsablesDTO tutor)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CandidatosServices canditateService = new CandidatosServices();
                if (tutor.Consecutivo != 0)
                    result.obj = await canditateService.ModificarCandidatoResponsable(tutor); // Update tutor
                else
                    result.obj = await canditateService.AsignarCandidatoResponsable(tutor); // Create tutor

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(Helper.returnSuccessObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateVideoCandidate()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ArchivosServices fileService = new ArchivosServices();

                int ConsecutivoArchivo = int.Parse(Request.Form["ConsecutivoArchivo"].ToString());
                int CodigoCandidatoVideo = int.Parse(Request.Form["CodigoCandidatoVideo"].ToString());
                int typeFileId = Helper.getFileType(Request.Files[0].FileName);
                
                result.obj = await fileService.ModificarArchivoCandidatoVideos(typeFileId, CodigoCandidatoVideo, ConsecutivoArchivo, Request.Files[0].InputStream);
               
                if (result.obj == null || !result.obj.Exitoso)
                    return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorFile(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Groups
        [HttpPost]
        public async Task<JsonResult> CreateGroup(GruposDTO group)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                string newEmail = group.Personas.Usuarios.Email;
                group.Personas.CodigoUsuario = UserLoggedIn().Consecutivo;
                group.Personas.Usuarios = UserLoggedIn();
                group.Personas.Usuarios.Email = newEmail;

                GruposServices groupService = new GruposServices();
                PersonasServices personService = new PersonasServices();
                AuthenticateServices usuarioService = new AuthenticateServices();
                if (group.Consecutivo != 0)
                {
                    group.CategoriasGrupos = null;
                    result.obj = await personService.ModificarPersona(group.Personas); // Update person
                    if (result.obj != null)
                        result.obj = await groupService.ModificarInformacionGrupo(group); // Update group
                    if (result.obj != null)
                        result.obj = await usuarioService.ModificarEmailUsuario(group.Personas.Usuarios); // Update email user
                    if (result.obj != null)
                        UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona = group.Personas.IdiomaDeLaPersona;
                }
                else
                {
                    group.CategoriasGrupos.ToList().ForEach(c => c.Categorias = null);
                    result.obj = await groupService.CrearGrupo(group); // Create a new group
                    if (result.obj != null)
                    {
                        AuthenticateServices service = new AuthenticateServices();
                        UsuariosDTO userToValidate = UserLoggedIn();
                        userToValidate.Personas = null;
                        userToValidate.TiposPerfiles = null;
                        userToValidate.PlanesUsuarios = null;
                        var userAgent = await service.VerificarUsuario(userToValidate);
                        if (userAgent != null)
                        {
                            setUserLogin(userAgent);
                        }
                    }
                }
                
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                result.Message = "La información se ha guardado con éxito";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> CreateGroupSkills(CategoriasGruposDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                category.Categorias = null;
                category.Grupos = null;
                CategoriasServices categoryService = new CategoriasServices();
                result.obj = await categoryService.CrearCategoriaGrupos(category);
                if (result.obj == null)
                {
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteGroupSkills(CategoriasGruposDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoryService = new CategoriasServices();
                result.obj = await categoryService.EliminarCategoriaGrupo(category);
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetListPostsByGroup(BuscadorDTO filter)
        {
            Result<GruposEventosDTO> result = new Result<GruposEventosDTO>();
            try
            {
                GruposServices categoryService = new GruposServices();
                result.list = await categoryService.ListarEventosDeUnGrupo(filter);
                if (result.list == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> CreatePostsGroup(GruposEventosDTO post)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                GruposServices categoryService = new GruposServices();
                
                if (post.Consecutivo != 0)
                    result.obj = await categoryService.ModificarInformacionGrupoEvento(post);
                else
                    result.obj = await categoryService.CrearGrupoEvento(post);

                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Agents
        [HttpPost]
        public async Task<JsonResult> CreateAgent(RepresentantesDTO agent)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                string newEmail = agent.Personas.Usuarios.Email;
                agent.Personas.CodigoUsuario = UserLoggedIn().Consecutivo;
                agent.Personas.Usuarios = UserLoggedIn();
                agent.Personas.Usuarios.Email = newEmail;
                
                RepresentantesServices agentService = new RepresentantesServices();
                PersonasServices personService = new PersonasServices();
                AuthenticateServices usuarioService = new AuthenticateServices();
                if (agent.Consecutivo != 0)
                {
                    agent.CategoriasRepresentantes = null;
                    result.obj = await personService.ModificarPersona(agent.Personas); // Update person
                    if (result.obj != null)
                        result.obj = await agentService.ModificarInformacionRepresentante(agent); // Update agent
                    if (result.obj != null)
                        result.obj = await usuarioService.ModificarEmailUsuario(agent.Personas.Usuarios); // Update email user
                    if (result.obj != null)
                        UserLoggedIn().PersonaDelUsuario.IdiomaDeLaPersona = agent.Personas.IdiomaDeLaPersona;
                }
                else
                {
                    agent.CategoriasRepresentantes.ToList().ForEach(c => c.Categorias = null);
                    result.obj = await agentService.CrearRepresentante(agent); // Create a new agent
                    if (result.obj != null)
                    {
                        AuthenticateServices service = new AuthenticateServices();
                        UsuariosDTO userToValidate = UserLoggedIn();
                        userToValidate.Personas = null;
                        userToValidate.TiposPerfiles = null;
                        userToValidate.PlanesUsuarios = null;
                        var userAgent = await service.VerificarUsuario(userToValidate);
                        if (userAgent != null)
                        {
                            setUserLogin(userAgent);
                        }
                    }
                }
                
                if (result.obj == null)
                    return Json(Helper.returnErrorObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                result.Message = "La información se ha guardado con éxito";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }

        }
        [HttpPost]
        public async Task<JsonResult> CreateAgentSkills(CategoriasRepresentantesDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                category.Categorias = null;
                CategoriasServices categoryService = new CategoriasServices();
                result.obj = await categoryService.CrearCategoriaRepresentante(category);
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteAgentSkills(CategoriasRepresentantesDTO category)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                CategoriasServices categoryService = new CategoriasServices();
                result.obj = await categoryService.EliminarCategoriaRepresentante(category);
                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Contacs
        [HttpPost]
        public async Task<JsonResult> CreateContact()
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                ContactosDTO contact = new ContactosDTO();
                contact.CodigoPersonaContacto = GetPersonToSearchTemp().Consecutivo;
                contact.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;
                result.obj = await chatService.CrearContacto(contact);

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> DeleteContact(ContactosDTO contact)
        {
            Result<WrapperSimpleTypesDTO> result = new Result<WrapperSimpleTypesDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                result.obj = await chatService.EliminarContacto(contact);

                if (result.obj == null)
                    return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);

                return Json(Helper.returnSuccessDeleteObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorSaveObj(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public async Task<JsonResult> GetMyContacts(ContactosDTO contact)
        {
            Result<ContactosDTO> result = new Result<ContactosDTO>();
            try
            {
                ChatsServices chatService = new ChatsServices();
                if (GetPersonToSearch() != null && GetPersonToSearch().Consecutivo != 0)
                    contact.CodigoPersonaOwner = GetPersonToSearch().Consecutivo;
                else
                    contact.CodigoPersonaOwner = UserLoggedIn().PersonaDelUsuario.Consecutivo;

                contact.CodigoIdiomaUsuarioBase = UserLoggedIn().PersonaDelUsuario.CodigoIdioma;
                result.list = await chatService.ListarContactos(contact);

                if (result.list == null)
                    return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
                
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(Helper.returnErrorList(UserLoggedIn().PersonaDelUsuario.CodigoIdioma), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}