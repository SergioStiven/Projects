using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class PersonasController : ApiController
    {
        PersonasBusiness _personasBusiness;

        public PersonasController()
        {
            _personasBusiness = new PersonasBusiness();
        }

        public async Task<IHttpActionResult> BuscarPersona(Personas personaParaBuscar)
        {
            if (personaParaBuscar == null || personaParaBuscar.Consecutivo <= 0 || personaParaBuscar.IdiomaDeLaPersona == Idioma.SinIdioma)
            {
                return BadRequest("personaParaBuscar vacio y/o invalido!.");
            }

            try
            {
                PersonasDTO personaBuscada = await _personasBusiness.BuscarPersona(personaParaBuscar);

                return Ok(personaBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarPersona(Personas personaParaModificar)
        {
            if (personaParaModificar == null || personaParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(personaParaModificar.Nombres)
                || personaParaModificar.CodigoIdioma <= 0 || personaParaModificar.CodigoPais <= 0 || string.IsNullOrWhiteSpace(personaParaModificar.Telefono)
                || string.IsNullOrWhiteSpace(personaParaModificar.CiudadResidencia))
            {
                return BadRequest("personaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarPersona = await _personasBusiness.ModificarPersona(personaParaModificar);

                return Ok(wrapperModificarPersona);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarImagenPerfil(Personas personaParaAsignarImagenPerfil)
        {
            if (personaParaAsignarImagenPerfil == null || personaParaAsignarImagenPerfil.Consecutivo <= 0
                || personaParaAsignarImagenPerfil.ArchivosPerfil == null || personaParaAsignarImagenPerfil.ArchivosPerfil.ArchivoContenido == null)
            {
                return BadRequest("personaParaAsignarImagenPerfil vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarImagenPerfil = await _personasBusiness.AsignarImagenPerfil(personaParaAsignarImagenPerfil);

                return Ok(wrapperAsignarImagenPerfil);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AsignarImagenBanner(Personas personaParaAsignarImagenBanner)
        {
            if (personaParaAsignarImagenBanner == null || personaParaAsignarImagenBanner.Consecutivo <= 0
                || personaParaAsignarImagenBanner.ArchivosBanner == null || personaParaAsignarImagenBanner.ArchivosBanner.ArchivoContenido == null)
            {
                return BadRequest("personaParaAsignarImagenBanner vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAsignarImagenBanner = await _personasBusiness.AsignarImagenBanner(personaParaAsignarImagenBanner);

                return Ok(wrapperAsignarImagenBanner);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarImagenPerfil(Personas personaParaEliminarImagenPerfil)
        {
            if (personaParaEliminarImagenPerfil == null || !personaParaEliminarImagenPerfil.CodigoArchivoImagenPerfil.HasValue || personaParaEliminarImagenPerfil.CodigoArchivoImagenPerfil <= 0 || personaParaEliminarImagenPerfil.Consecutivo <= 0)
            {
                return BadRequest("personaParaEliminarImagenPerfil vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarImagenPerfil = await _personasBusiness.EliminarImagenPerfil(personaParaEliminarImagenPerfil);

                return Ok(wrapperEliminarImagenPerfil);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarImagenBanner(Personas personaParaEliminarImagenBanner)
        {
            if (personaParaEliminarImagenBanner == null || !personaParaEliminarImagenBanner.CodigoArchivoImagenBanner.HasValue || personaParaEliminarImagenBanner.CodigoArchivoImagenBanner <= 0 || personaParaEliminarImagenBanner.Consecutivo <= 0)
            {
                return BadRequest("personaParaEliminarImagenBanner vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarImagenBanner = await _personasBusiness.EliminarImagenBanner(personaParaEliminarImagenBanner);

                return Ok(wrapperEliminarImagenBanner);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
