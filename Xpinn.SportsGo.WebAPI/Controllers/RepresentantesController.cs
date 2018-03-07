using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class RepresentantesController : ApiController
    {
        RepresentantesBusiness _representanteBusiness;

        public RepresentantesController()
        {
            _representanteBusiness = new RepresentantesBusiness();
        }


        #region Metodos Representantes


        public async Task<IHttpActionResult> CrearRepresentante(Representantes representanteParaCrear)
        {
            if (representanteParaCrear == null || representanteParaCrear.Personas == null || representanteParaCrear.Personas.Usuarios == null | representanteParaCrear.CategoriasRepresentantes == null)
            {
                return BadRequest("representanteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Nombres) || representanteParaCrear.Personas.CodigoPais <= 0 || representanteParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                     || representanteParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.CiudadResidencia))
            {
                return BadRequest("Persona de representanteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Email))
            {
                return BadRequest("Usuario de representanteParaCrear vacio y/o invalido!.");
            }
            else if (representanteParaCrear.CategoriasRepresentantes.Count <= 0 || !representanteParaCrear.CategoriasRepresentantes.All(x => x.CodigoCategoria > 0))
            {
                return BadRequest("Categorias de representanteParaCrear vacio y/o invalido!.");
            }

            try
            {
                string urlLogo = Url.Content("~/Content/Images/LogoSportsGo.png");
                string urlBanner = Url.Content("~/Content/Images/BannerSportsGo.png");

                WrapperSimpleTypesDTO wrapperCrearGrupo = await _representanteBusiness.CrearRepresentante(representanteParaCrear, urlLogo, urlBanner);

                return Ok(wrapperCrearGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarRepresentantePorCodigoPersona(Representantes representanteParaBuscar)
        {
            if (representanteParaBuscar == null || representanteParaBuscar.Personas == null || representanteParaBuscar.Personas.Consecutivo <= 0)
            {
                return BadRequest("representanteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Representantes representanteBuscado = await _representanteBusiness.BuscarRepresentantePorCodigoPersona(representanteParaBuscar);

                return Ok(representanteBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarRepresentantePorCodigoRepresentante(Representantes representanteParaBuscar)
        {
            if (representanteParaBuscar == null || representanteParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("representanteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Representantes representanteBuscado = await _representanteBusiness.BuscarRepresentantePorCodigoRepresentante(representanteParaBuscar);

                return Ok(representanteBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarRepresentantes(Representantes representanteParaListar)
        {
            if (representanteParaListar == null || representanteParaListar.SkipIndexBase < 0 || representanteParaListar.TakeIndexBase <= 0)
            {
                return BadRequest("representanteParaListar vacio y/o invalido!.");
            }

            try
            {
                List<RepresentantesDTO> listaInformacionRepresentante = await _representanteBusiness.ListarRepresentantes(representanteParaListar);

                return Ok(listaInformacionRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarInformacionRepresentante(Representantes representanteParaModificar)
        {
            if (representanteParaModificar == null || representanteParaModificar.Consecutivo <= 0)
            {
                return BadRequest("representanteParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarInformacionRepresentante = await _representanteBusiness.ModificarInformacionRepresentante(representanteParaModificar);

                return Ok(wrapperModificarInformacionRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
