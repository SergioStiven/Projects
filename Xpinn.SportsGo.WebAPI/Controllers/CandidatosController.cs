using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;
using System.IO;
using System.Linq;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class CandidatosController : ApiController
    {
        CandidatosBusiness _candidatosBusiness;

        public CandidatosController()
        {
            _candidatosBusiness = new CandidatosBusiness();
        }


        #region Metodos Candidatos


        public async Task<IHttpActionResult> CrearCandidato(Candidatos candidatoParaCrear)
        {
            if (candidatoParaCrear == null || candidatoParaCrear.CodigoGenero <= 0
                || candidatoParaCrear.Estatura <= 0 || candidatoParaCrear.Peso <= 0 || candidatoParaCrear.FechaNacimiento == DateTime.MinValue)
            {
                return BadRequest("candidatoParaCrear vacio y/o invalido!.");
            }
            else if (candidatoParaCrear.Personas == null || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Nombres) || candidatoParaCrear.Personas.CodigoPais <= 0 || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Apellidos) 
                || candidatoParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil || candidatoParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.CiudadResidencia))
            {
                return BadRequest("Persona de candidatoParaCrear vacio y/o invalido!.");
            }
            else if (candidatoParaCrear.Personas.Usuarios == null || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Email))
            {
                return BadRequest("Usuario de candidatoParaCrear vacio y/o invalido!.");
            }
            else if (candidatoParaCrear.CategoriasCandidatos.Count <= 0 
                || !candidatoParaCrear.CategoriasCandidatos.All(x => x.CodigoCategoria > 0 && (x.HabilidadesCandidatos.Count > 0 && x.HabilidadesCandidatos.All(y => y.CodigoHabilidad > 0 && y.NumeroEstrellas >= 0 && y.NumeroEstrellas <= 5))))
            {
                return BadRequest("Categorias o Habilidades de candidatoParaCrear vacio y/o invalido!.");
            }

            try
            {
                string urlLogo = Url.Content("~/Content/Images/LogoSportsGo.png");
                string urlBanner = Url.Content("~/Content/Images/BannerSportsGo.png");

                WrapperSimpleTypesDTO wrapperCrearCandidato = await _candidatosBusiness.CrearCandidato(candidatoParaCrear, urlLogo, urlBanner);

                return Ok(wrapperCrearCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCandidatoPorCodigoPersona(Candidatos candidatoParaBuscar)
        {
            if (candidatoParaBuscar == null || candidatoParaBuscar.Personas == null || candidatoParaBuscar.Personas.Consecutivo <= 0)
            {
                return BadRequest("candidatoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Candidatos informacionCandidato = await _candidatosBusiness.BuscarCandidatoPorCodigoPersona(candidatoParaBuscar);

                return Ok(informacionCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCandidatoPorCodigoCandidato(Candidatos candidatoParaBuscar)
        {
            if (candidatoParaBuscar == null ||  candidatoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("candidatoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Candidatos informacionCandidato = await _candidatosBusiness.BuscarCandidatoPorCodigoCandidato(candidatoParaBuscar);

                return Ok(informacionCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCandidatos(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<CandidatosDTO> listaInformacionCandidatos = await _candidatosBusiness.ListarCandidatos(buscador);

                return Ok(listaInformacionCandidatos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarInformacionCandidato(Candidatos candidatoParaModificar)
        {
            if (candidatoParaModificar == null || candidatoParaModificar.Consecutivo <= 0
                || candidatoParaModificar.CodigoGenero <= 0 || candidatoParaModificar.Estatura <= 0 || candidatoParaModificar.Peso <= 0
                || candidatoParaModificar.FechaNacimiento == DateTime.MinValue)
            {
                return BadRequest("candidatoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarInformacionCandidato = await _candidatosBusiness.ModificarInformacionCandidato(candidatoParaModificar);

                return Ok(wrapperModificarInformacionCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CandidatosVideos


        public async Task<IHttpActionResult> CrearCandidatoVideo(CandidatosVideos candidatoVideoParaCrear)
        {
            if (candidatoVideoParaCrear == null || string.IsNullOrWhiteSpace(candidatoVideoParaCrear.Titulo) || candidatoVideoParaCrear.CodigoCandidato <= 0)
            {
                return BadRequest("candidatoVideoParaCrear vacio y/o invalido!.");
            }
            else if (candidatoVideoParaCrear.Archivos != null)
            {
                return BadRequest("Usa CrearArchivoCandidadoVideo para crear el archivo o mataras la memoria del servidor!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCandidatoVideo = await _candidatosBusiness.CrearCandidatoVideo(candidatoVideoParaCrear);

                return Ok(wrapperCrearCandidatoVideo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCandidatoVideo(CandidatosVideos candidatoVideoParaBuscar)
        {
            if (candidatoVideoParaBuscar == null || candidatoVideoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("candidatoVideoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CandidatosVideos candidatoVideoBuscado = await _candidatosBusiness.BuscarCandidatoVideo(candidatoVideoParaBuscar);

                return Ok(candidatoVideoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCandidatosVideosDeUnCandidato(CandidatosVideos candidatoVideoParaListar)
        {
            if (candidatoVideoParaListar == null || candidatoVideoParaListar.CodigoCandidato <= 0 || candidatoVideoParaListar.SkipIndexBase < 0 || candidatoVideoParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("candidatoVideoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<CandidatosVideosDTO> listaInformacionCandidatos = await _candidatosBusiness.ListarCandidatosVideosDeUnCandidato(candidatoVideoParaListar);

                return Ok(listaInformacionCandidatos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCandidatoVideo(CandidatosVideos candidatoVideoParaModificar)
        {
            if (candidatoVideoParaModificar == null || candidatoVideoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(candidatoVideoParaModificar.Titulo))
            {
                return BadRequest("candidatoVideoParaModificar vacio y/o invalido!.");
            }
            if (candidatoVideoParaModificar.ArchivoContenido != null)
            {
                return BadRequest("El archivo debe modificarse por su controller!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCandidatoVideo = await _candidatosBusiness.ModificarCandidatoVideo(candidatoVideoParaModificar);

                return Ok(wrapperModificarCandidatoVideo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCandidatoVideo(CandidatosVideos candidatoVideoParaEliminar)
        {
            if (candidatoVideoParaEliminar == null || candidatoVideoParaEliminar.Consecutivo <= 0 || candidatoVideoParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("candidatoVideoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCandidatoVideo = await _candidatosBusiness.EliminarCandidatoVideo(candidatoVideoParaEliminar);

                return Ok(wrapperEliminarCandidatoVideo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Responsables


        public async Task<IHttpActionResult> CrearCandidatoResponsable(CandidatosResponsables candidatoResponsableParaCrear)
        {
            if (candidatoResponsableParaCrear == null  || string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.TelefonoMovil) 
                || string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Nombres))
            {
                return BadRequest("candidatoVideoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCandidatoResponsable = await _candidatosBusiness.CrearCandidatoResponsable(candidatoResponsableParaCrear);

                return Ok(wrapperCrearCandidatoResponsable);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaAsignar)
        {
            if (candidatoResponsableParaAsignar == null || string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.TelefonoMovil) || candidatoResponsableParaAsignar.CodigoCandidato <= 0
                || string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.Nombres))
            {
                return BadRequest("candidatoVideoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCandidatoResponsable = await _candidatosBusiness.AsignarCandidatoResponsable(candidatoResponsableParaAsignar);

                return Ok(wrapperCrearCandidatoResponsable);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaBuscar)
        {
            if (candidatoResponsableParaBuscar == null || candidatoResponsableParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("candidatoResponsableParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CandidatosResponsables candidatoResponsableBuscado = await _candidatosBusiness.BuscarCandidatoResponsable(candidatoResponsableParaBuscar);

                return Ok(candidatoResponsableBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCandidatoResponsable(CandidatosResponsables candidatoResponsableParaModificar)
        {
            if (candidatoResponsableParaModificar == null || candidatoResponsableParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.TelefonoMovil)
                || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Nombres))
            { 
                return BadRequest("candidatoResponsableParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCandidatoResponsable = await _candidatosBusiness.ModificarCandidatoResponsable(candidatoResponsableParaModificar);

                return Ok(wrapperModificarCandidatoResponsable);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCandidatoResponsable(Candidatos candidatoResponsableParaBorrar)
        {
            if (candidatoResponsableParaBorrar == null || candidatoResponsableParaBorrar.Consecutivo <= 0 || candidatoResponsableParaBorrar.CodigoResponsable <= 0)
            {
                return BadRequest("candidatoVideoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCandidatoResponsable = await _candidatosBusiness.EliminarCandidatoResponsable(candidatoResponsableParaBorrar);

                return Ok(wrapperEliminarCandidatoResponsable);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
