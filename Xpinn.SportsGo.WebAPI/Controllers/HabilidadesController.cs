using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class HabilidadesController : ApiController
    {
        HabilidadesBusiness _habilidadesBusiness;

        public HabilidadesController()
        {
            _habilidadesBusiness = new HabilidadesBusiness();
        }


        #region Metodos Habilidades


        public async Task<IHttpActionResult> CrearHabilidad(Habilidades habilidadParaCrear)
        {
            if (habilidadParaCrear == null || habilidadParaCrear.CodigoCategoria <= 0 || habilidadParaCrear.TipoHabilidad == TipoHabilidad.SinTipoHabilidad 
                || habilidadParaCrear.HabilidadesContenidos == null || habilidadParaCrear.HabilidadesContenidos.Count <= 0
                || !habilidadParaCrear.HabilidadesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0))
            {
                return BadRequest("habilidadParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearHabilidad = await _habilidadesBusiness.CrearHabilidad(habilidadParaCrear);

                return Ok(wrapperCrearHabilidad);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarHabilidad(Habilidades habilidadParaBuscar)
        {
            if (habilidadParaBuscar == null || habilidadParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("habilidadParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Habilidades habilidadBuscada = await _habilidadesBusiness.BuscarHabilidad(habilidadParaBuscar);

                return Ok(habilidadBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarHabilidadesPorIdioma(Habilidades habilidadParaListar)
        {
            if (habilidadParaListar == null || habilidadParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("habilidadParaListar buscar vacio y/o invalido!.");
            }

            try
            {
                List<HabilidadesDTO> listaHabilidades = await _habilidadesBusiness.ListarHabilidadesPorIdioma(habilidadParaListar);

                return Ok(listaHabilidades);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarHabilidadesPorCodigoCategoriaAndIdioma(Habilidades habilidadParaListar)
        {
            if (habilidadParaListar == null || habilidadParaListar.IdiomaBase == Idioma.SinIdioma || habilidadParaListar.CodigoCategoria <= 0)
            {
                return BadRequest("habilidadParaListar buscar vacio y/o invalido!.");
            }

            try
            {
                List<HabilidadesDTO> listaHabilidadesPorCategoria = await _habilidadesBusiness.ListarHabilidadesPorCodigoCategoriaAndIdioma(habilidadParaListar);

                return Ok(listaHabilidadesPorCategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarHabilidad(Habilidades habilidadParaModificar)
        {
            if (habilidadParaModificar == null || habilidadParaModificar.Consecutivo <= 0 || habilidadParaModificar.TipoHabilidad == TipoHabilidad.SinTipoHabilidad)
            {
                return BadRequest("habilidadParaModificar vacio y/o invalido!.");
            }
            if (habilidadParaModificar.HabilidadesContenidos != null && !habilidadParaModificar.HabilidadesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion)))
            {
                return BadRequest("Habilidad contenido para modificar descripcion vacia y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarHabilidad = await _habilidadesBusiness.ModificarHabilidad(habilidadParaModificar);

                return Ok(wrapperModificarHabilidad);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarHabilidad(Habilidades habilidadParaEliminar)
        {
            if (habilidadParaEliminar == null || habilidadParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("habilidadParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarHabilidad = await _habilidadesBusiness.EliminarHabilidad(habilidadParaEliminar);

                return Ok(wrapperEliminarHabilidad);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos HabilidadesCandidatos


        public async Task<IHttpActionResult> CrearHabilidadesCandidato(List<HabilidadesCandidatos> habilidadCandidatoParaCrear)
        {
            if (habilidadCandidatoParaCrear == null || habilidadCandidatoParaCrear.Count <= 0
                || !habilidadCandidatoParaCrear.All(x => x.CodigoCategoriaCandidato > 0 && x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                return BadRequest("habilidadCandidatoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearHabilidadesCandidato = await _habilidadesBusiness.CrearHabilidadesCandidato(habilidadCandidatoParaCrear);

                return Ok(wrapperCrearHabilidadesCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarHabilidadCandidatoPorIdioma(HabilidadesCandidatos habilidadCandidatoParaBuscar)
        {
            if (habilidadCandidatoParaBuscar == null || habilidadCandidatoParaBuscar.Consecutivo <= 0 || habilidadCandidatoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("habilidadCandidatoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                HabilidadesCandidatosDTO habilidadCandidatoBuscada = await _habilidadesBusiness.BuscarHabilidadCandidatoPorIdioma(habilidadCandidatoParaBuscar);

                return Ok(habilidadCandidatoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(HabilidadesCandidatos habilidadCandidatoParaListar)
        {
            if (habilidadCandidatoParaListar == null || habilidadCandidatoParaListar.CodigoCategoriaCandidato <= 0 || habilidadCandidatoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("habilidadCandidatoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<HabilidadesCandidatosDTO> listaHabilidadesCandidato = await _habilidadesBusiness.ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(habilidadCandidatoParaListar);

                return Ok(listaHabilidadesCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarHabilidadCandidato(HabilidadesCandidatos habilidadCandidatoParaBorrar)
        {
            if (habilidadCandidatoParaBorrar == null || habilidadCandidatoParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("habilidadCandidatoParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarHabilidadCandidato = await _habilidadesBusiness.EliminarHabilidadCandidato(habilidadCandidatoParaBorrar);

                return Ok(wrapperEliminarHabilidadCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos HabilidadesContenido


        public async Task<IHttpActionResult> CrearHabilidadesContenidos(List<HabilidadesContenidos> habilidadesContenidosParaCrear)
        {
            if (habilidadesContenidosParaCrear == null || habilidadesContenidosParaCrear.Count <= 0
                || !habilidadesContenidosParaCrear.All(x => x.CodigoHabilidad > 0 && !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0))
            {
                return BadRequest("habilidadesContenidosParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearHabilidadesContenido = await _habilidadesBusiness.CrearHabilidadesContenidos(habilidadesContenidosParaCrear);

                return Ok(wrapperCrearHabilidadesContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CrearHabilidadesContenidosIndividual(HabilidadesContenidos habilidadesContenidosParaCrear)
        {
            if (habilidadesContenidosParaCrear == null
                || habilidadesContenidosParaCrear.CodigoHabilidad > 0 || !string.IsNullOrWhiteSpace(habilidadesContenidosParaCrear.Descripcion)
                || habilidadesContenidosParaCrear.CodigoIdioma > 0)
            {
                return BadRequest("habilidadesContenidosParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearHabilidadesContenidosIndividual = await _habilidadesBusiness.CrearHabilidadesContenidosIndividual(habilidadesContenidosParaCrear);

                return Ok(wrapperCrearHabilidadesContenidosIndividual);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaBuscar)
        {
            if (habilidadContenidoParaBuscar == null || habilidadContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("habilidadContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                HabilidadesContenidos habilidadesContenidoBuscada = await _habilidadesBusiness.BuscarHabilidadContenido(habilidadContenidoParaBuscar);

                return Ok(habilidadesContenidoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarContenidoDeUnaHabilidad(HabilidadesContenidos habilidadContenidoParaListar)
        {
            if (habilidadContenidoParaListar == null || habilidadContenidoParaListar.CodigoHabilidad <= 0)
            {
                return BadRequest("habilidadContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<HabilidadesContenidos> listaHabilidadesContenidos = await _habilidadesBusiness.ListarContenidoDeUnaHabilidad(habilidadContenidoParaListar);

                return Ok(listaHabilidadesContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaModificar)
        {
            if (habilidadContenidoParaModificar == null || habilidadContenidoParaModificar.Consecutivo <= 0
                || string.IsNullOrWhiteSpace(habilidadContenidoParaModificar.Descripcion))
            {
                return BadRequest("habilidadContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarHabilidadContenido = await _habilidadesBusiness.ModificarHabilidadContenido(habilidadContenidoParaModificar);

                return Ok(wrapperModificarHabilidadContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaEliminar)
        {
            if (habilidadContenidoParaEliminar == null || habilidadContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("habilidadContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarHabilidadContenido = await _habilidadesBusiness.EliminarHabilidadContenido(habilidadContenidoParaEliminar);

                return Ok(wrapperEliminarHabilidadContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
