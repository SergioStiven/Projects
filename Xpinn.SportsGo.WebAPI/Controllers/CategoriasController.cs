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
    public class CategoriasController : ApiController
    {
        CategoriasBusiness _categoriasBusiness;

        public CategoriasController()
        {
            _categoriasBusiness = new CategoriasBusiness();
        }


        #region Metodos Categorias


        public async Task<IHttpActionResult> CrearCategoria(Categorias categoriaParaCrear)
        {
            if (categoriaParaCrear == null || categoriaParaCrear.CategoriasContenidos == null ||categoriaParaCrear.CategoriasContenidos.Count <= 0
                ||!categoriaParaCrear.CategoriasContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0)
                || categoriaParaCrear.Archivos == null || categoriaParaCrear.Archivos.ArchivoContenido == null)
            {
                return BadRequest("categoriaParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoria = await _categoriasBusiness.CrearCategoria(categoriaParaCrear);

                return Ok(wrapperCrearCategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoria(Categorias categoriaParaBuscar)
        {
            if (categoriaParaBuscar == null || categoriaParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("categoriaParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Categorias categoriaBuscada = await _categoriasBusiness.BuscarCategoria(categoriaParaBuscar);

                return Ok(categoriaBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasPorIdioma(Categorias categoriaParaListar)
        {
            if (categoriaParaListar == null || categoriaParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaParaListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasDTO> listaCategorias = await _categoriasBusiness.ListarCategoriasPorIdioma(categoriaParaListar);

                return Ok(listaCategorias);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarArchivoCategoria(Categorias categoriaParaModificar)
        {
            if (categoriaParaModificar == null || categoriaParaModificar.CodigoArchivo <= 0 || categoriaParaModificar.ArchivoContenido == null)
            {
                return BadRequest("categoriaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoria = await _categoriasBusiness.ModificarArchivoCategoria(categoriaParaModificar);

                return Ok(wrapperModificarCategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoria(Categorias categoriaParaEliminar)
        {
            if (categoriaParaEliminar == null || categoriaParaEliminar.Consecutivo <= 0 || categoriaParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("categoriaParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoria = await _categoriasBusiness.EliminarCategoria(categoriaParaEliminar);

                return Ok(wrapperEliminarCategoria);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasContenido


        public async Task<IHttpActionResult> CrearCategoriasContenidos(List<CategoriasContenidos> categoriaContenidoParaCrear)
        {
            if (categoriaContenidoParaCrear == null || categoriaContenidoParaCrear.Count <= 0 || 
                !categoriaContenidoParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoCategoria > 0))
            {
                return BadRequest("categoriaContenidoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriasContenidos = await _categoriasBusiness.CrearCategoriasContenidos(categoriaContenidoParaCrear);

                return Ok(wrapperCrearCategoriasContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaContenido(CategoriasContenidos categoriaContenidoParaBuscar)
        {
            if (categoriaContenidoParaBuscar == null || categoriaContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("categoriaContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasContenidos categoriaContenidoBuscada = await _categoriasBusiness.BuscarCategoriaContenido(categoriaContenidoParaBuscar);

                return Ok(categoriaContenidoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarContenidoDeUnaCategoria(CategoriasContenidos categoriaContenidoParaListar)
        {
            if (categoriaContenidoParaListar == null || categoriaContenidoParaListar.CodigoCategoria <= 0)
            {
                return BadRequest("categoriaContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasContenidos> listaCategoriasContenido = await _categoriasBusiness.ListarContenidoDeUnaCategoria(categoriaContenidoParaListar);

                return Ok(listaCategoriasContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaContenido(CategoriasContenidos categoriaContenidoParaModificar)
        {
            if (categoriaContenidoParaModificar == null || categoriaContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(categoriaContenidoParaModificar.Descripcion))
            {
                return BadRequest("categoriaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaContenido = await _categoriasBusiness.ModificarCategoriaContenido(categoriaContenidoParaModificar);

                return Ok(wrapperModificarCategoriaContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaContenido(CategoriasContenidos categoriaContenidoParaEliminar)
        {
            if (categoriaContenidoParaEliminar == null || categoriaContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("categoriaContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaContenido = await _categoriasBusiness.EliminarCategoriaContenido(categoriaContenidoParaEliminar);

                return Ok(wrapperEliminarCategoriaContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasAnuncios


        public async Task<IHttpActionResult> CrearListaCategoriaAnuncios(List<CategoriasAnuncios> categoriaAnuncioParaCrear)
        {
            if (categoriaAnuncioParaCrear == null || categoriaAnuncioParaCrear.Count <= 0 || !categoriaAnuncioParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoAnuncio > 0))
            {
                return BadRequest("categoriaAnuncioParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaAnuncio = await _categoriasBusiness.CrearListaCategoriaAnuncios(categoriaAnuncioParaCrear);

                return Ok(wrapperCrearCategoriaAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CrearCategoriaAnuncios(CategoriasAnuncios categoriaAnuncioParaCrear)
        {
            if (categoriaAnuncioParaCrear == null || categoriaAnuncioParaCrear.CodigoCategoria > 0 || categoriaAnuncioParaCrear.CodigoAnuncio > 0)
            {
                return BadRequest("categoriaAnuncioParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaAnuncio = await _categoriasBusiness.CrearCategoriaAnuncios(categoriaAnuncioParaCrear);

                return Ok(wrapperCrearCategoriaAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaAnuncioPorConsecutivoAndIdioma(CategoriasAnuncios categoriaAnuncioParaBuscar)
        {
            if (categoriaAnuncioParaBuscar == null || categoriaAnuncioParaBuscar.Consecutivo <= 0 || categoriaAnuncioParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaAnuncioParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasAnunciosDTO categoriaAnuncioBuscada = await _categoriasBusiness.BuscarCategoriaAnuncioPorConsecutivoAndIdioma(categoriaAnuncioParaBuscar);

                return Ok(categoriaAnuncioBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasDeUnAnuncioPorIdioma(CategoriasAnuncios categoriaAnuncioParaListar)
        {
            if (categoriaAnuncioParaListar == null || categoriaAnuncioParaListar.CodigoAnuncio <= 0 || categoriaAnuncioParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaAnuncioAListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasAnunciosDTO> listaCategoriasDeUnAnuncio = await _categoriasBusiness.ListarCategoriasDeUnAnuncioPorIdioma(categoriaAnuncioParaListar);

                return Ok(listaCategoriasDeUnAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaModificar)
        {
            if (categoriaAnuncioParaModificar == null || categoriaAnuncioParaModificar.Consecutivo <= 0 || categoriaAnuncioParaModificar.CodigoCategoria <= 0)
            {
                return BadRequest("categoriaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaAnuncio = await _categoriasBusiness.ModificarCategoriaAnuncio(categoriaAnuncioParaModificar);

                return Ok(wrapperModificarCategoriaAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaBorrar)
        {
            if (categoriaAnuncioParaBorrar == null || categoriaAnuncioParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("categoriaAnuncioParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaAnuncio = await _categoriasBusiness.EliminarCategoriaAnuncio(categoriaAnuncioParaBorrar);

                return Ok(wrapperEliminarCategoriaAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasCandidatos


        public async Task<IHttpActionResult> CrearCategoriaCandidatos(CategoriasCandidatos categoriaCandidatoParaCrear)
        {
            if (categoriaCandidatoParaCrear == null || categoriaCandidatoParaCrear.HabilidadesCandidatos == null
                || categoriaCandidatoParaCrear.CodigoCategoria <= 0 || categoriaCandidatoParaCrear.CodigoCandidato <= 0
                || categoriaCandidatoParaCrear.HabilidadesCandidatos.Count <= 0
                || !categoriaCandidatoParaCrear.HabilidadesCandidatos.All(x => x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                return BadRequest("categoriaCandidatoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaCandidatos = await _categoriasBusiness.CrearCategoriaCandidatos(categoriaCandidatoParaCrear);

                return Ok(wrapperCrearCategoriaCandidatos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaCandidatoPorConsecutivoAndIdioma(CategoriasCandidatos categoriaCandidatoParaBuscar)
        {
            if (categoriaCandidatoParaBuscar == null || categoriaCandidatoParaBuscar.Consecutivo <= 0 || categoriaCandidatoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaAnuncioParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasCandidatosDTO categoriaCandidatoBuscada = await _categoriasBusiness.BuscarCategoriaCandidatoPorConsecutivoAndIdioma(categoriaCandidatoParaBuscar);

                return Ok(categoriaCandidatoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasDeUnCandidatoPorIdioma(CategoriasCandidatos categoriaCandidatoParaListar)
        {
            if (categoriaCandidatoParaListar == null || categoriaCandidatoParaListar.CodigoCandidato <= 0 || categoriaCandidatoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaCandidatoAListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasCandidatosDTO> listaCategoriasCandidatos = await _categoriasBusiness.ListarCategoriasDeUnCandidatoPorIdioma(categoriaCandidatoParaListar);

                return Ok(listaCategoriasCandidatos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaModificar)
        {
            if (categoriaCandidatoParaModificar == null|| categoriaCandidatoParaModificar.HabilidadesCandidatos == null
                || categoriaCandidatoParaModificar.Consecutivo <= 0 || categoriaCandidatoParaModificar.CodigoCategoria <= 0  || categoriaCandidatoParaModificar.CodigoCandidato <= 0
                || categoriaCandidatoParaModificar.HabilidadesCandidatos.Count <= 0
                || !categoriaCandidatoParaModificar.HabilidadesCandidatos.All(x => x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                return BadRequest("categoriaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaCandidato = await _categoriasBusiness.ModificarCategoriaCandidato(categoriaCandidatoParaModificar);

                return Ok(wrapperModificarCategoriaCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaBorrar)
        {
            if (categoriaCandidatoParaBorrar == null || categoriaCandidatoParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("categoriaCandidatoParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaCandidato = await _categoriasBusiness.EliminarCategoriaCandidato(categoriaCandidatoParaBorrar);

                return Ok(wrapperEliminarCategoriaCandidato);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasEventos


        public async Task<IHttpActionResult> CrearListaCategoriaEventos(List<CategoriasEventos> categoriaEventoParaCrear)
        {
            if (categoriaEventoParaCrear == null || categoriaEventoParaCrear.Count <= 0 || !categoriaEventoParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoEvento > 0))
            {
                return BadRequest("categoriaCandidatoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaEventos = await _categoriasBusiness.CrearListaCategoriaEventos(categoriaEventoParaCrear);

                return Ok(wrapperCrearCategoriaEventos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CrearCategoriaEventos(CategoriasEventos categoriaEventoParaCrear)
        {
            if (categoriaEventoParaCrear == null || categoriaEventoParaCrear.CodigoCategoria > 0 || categoriaEventoParaCrear.CodigoEvento > 0)
            {
                return BadRequest("categoriaCandidatoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaEventos = await _categoriasBusiness.CrearCategoriaEventos(categoriaEventoParaCrear);

                return Ok(wrapperCrearCategoriaEventos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaEventoPorConsecutivoAndIdioma(CategoriasEventos categoriaEventoParaBuscar)
        {
            if (categoriaEventoParaBuscar == null || categoriaEventoParaBuscar.Consecutivo <= 0 || categoriaEventoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaEventoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasEventosDTO categoriaEventoBuscada = await _categoriasBusiness.BuscarCategoriaEventoPorConsecutivoAndIdioma(categoriaEventoParaBuscar);

                return Ok(categoriaEventoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasDeUnEventoPorIdioma(CategoriasEventos categoriaEventoParaListar)
        {
            if (categoriaEventoParaListar == null || categoriaEventoParaListar.CodigoEvento <= 0 || categoriaEventoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaEventoAListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasEventosDTO> listaCategoriasEventos = await _categoriasBusiness.ListarCategoriasDeUnEventoPorIdioma(categoriaEventoParaListar);

                return Ok(listaCategoriasEventos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaEvento(CategoriasEventos categoriaEventoParaModificar)
        {
            if (categoriaEventoParaModificar == null || categoriaEventoParaModificar.Consecutivo <= 0 || categoriaEventoParaModificar.CodigoCategoria <= 0)
            {
                return BadRequest("categoriaParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaEvento = await _categoriasBusiness.ModificarCategoriaEvento(categoriaEventoParaModificar);

                return Ok(wrapperModificarCategoriaEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaEvento(CategoriasEventos categoriaEventoParaBorrar)
        {
            if (categoriaEventoParaBorrar == null || categoriaEventoParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("categoriaEventoParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaEvento = await _categoriasBusiness.EliminarCategoriaEvento(categoriaEventoParaBorrar);

                return Ok(wrapperEliminarCategoriaEvento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasGrupos


        public async Task<IHttpActionResult> CrearListaCategoriaGrupos(List<CategoriasGrupos> categoriaGrupoParaCrear)
        {
            if (categoriaGrupoParaCrear == null || categoriaGrupoParaCrear.Count <= 0 || !categoriaGrupoParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoGrupo > 0))
            {
                return BadRequest("categoriaGrupoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaGrupos = await _categoriasBusiness.CrearListaCategoriaGrupos(categoriaGrupoParaCrear);

                return Ok(wrapperCrearCategoriaGrupos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CrearCategoriaGrupos(CategoriasGrupos categoriaGrupoParaCrear)
        {
            if (categoriaGrupoParaCrear == null || categoriaGrupoParaCrear.CodigoCategoria <= 0 || categoriaGrupoParaCrear.CodigoGrupo <= 0)
            {
                return BadRequest("categoriaGrupoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaGrupos = await _categoriasBusiness.CrearCategoriaGrupos(categoriaGrupoParaCrear);

                return Ok(wrapperCrearCategoriaGrupos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaGrupoPorConsecutivoAndIdioma(CategoriasGrupos categoriaGrupoParaBuscar)
        {
            if (categoriaGrupoParaBuscar == null || categoriaGrupoParaBuscar.Consecutivo <= 0 || categoriaGrupoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaGrupoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasGruposDTO categoriaGrupoBuscada = await _categoriasBusiness.BuscarCategoriaGrupoPorConsecutivoAndIdioma(categoriaGrupoParaBuscar);

                return Ok(categoriaGrupoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasDeUnGrupoPorIdioma(CategoriasGrupos categoriaGrupoParaListar)
        {
            if (categoriaGrupoParaListar == null || categoriaGrupoParaListar.CodigoGrupo <= 0 || categoriaGrupoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaGrupoAListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasGruposDTO> listaCategoriaGrupo = await _categoriasBusiness.ListarCategoriasDeUnGrupoPorIdioma(categoriaGrupoParaListar);

                return Ok(listaCategoriaGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaModificar)
        {
            if (categoriaGrupoParaModificar == null || categoriaGrupoParaModificar.Consecutivo <= 0 || categoriaGrupoParaModificar.CodigoCategoria <= 0)
            {
                return BadRequest("categoriaGrupoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaGrupo = await _categoriasBusiness.ModificarCategoriaGrupo(categoriaGrupoParaModificar);

                return Ok(wrapperModificarCategoriaGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaBorrar)
        {
            if (categoriaGrupoParaBorrar == null || categoriaGrupoParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("categoriaGrupoParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaGrupo = await _categoriasBusiness.EliminarCategoriaGrupo(categoriaGrupoParaBorrar);

                return Ok(wrapperEliminarCategoriaGrupo);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos CategoriasRepresentantes


        public async Task<IHttpActionResult> CrearListaCategoriaRepresentante(List<CategoriasRepresentantes> categoriaRepresentanteParaCrear)
        {
            if (categoriaRepresentanteParaCrear == null || categoriaRepresentanteParaCrear.Count <= 0 || !categoriaRepresentanteParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoRepresentante > 0))
            {
                return BadRequest("categoriaRepresentanteParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaRepresentante = await _categoriasBusiness.CrearListaCategoriaRepresentante(categoriaRepresentanteParaCrear);

                return Ok(wrapperCrearCategoriaRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> CrearCategoriaRepresentante(CategoriasRepresentantes categoriaRepresentanteParaCrear)
        {
            if (categoriaRepresentanteParaCrear == null || categoriaRepresentanteParaCrear.CodigoCategoria > 0 || categoriaRepresentanteParaCrear.CodigoRepresentante > 0)
            {
                return BadRequest("categoriaRepresentanteParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearCategoriaRepresentante = await _categoriasBusiness.CrearCategoriaRepresentante(categoriaRepresentanteParaCrear);

                return Ok(wrapperCrearCategoriaRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarCategoriaRepresentantePorConsecutivoAndIdioma(CategoriasRepresentantes categoriaRepresentantesParaBuscar)
        {
            if (categoriaRepresentantesParaBuscar == null || categoriaRepresentantesParaBuscar.Consecutivo <= 0 || categoriaRepresentantesParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaRepresentantesParaBuscar vacio y/o invalido!.");
            }

            try
            {
                CategoriasRepresentantesDTO categoriaEventoBuscada = await _categoriasBusiness.BuscarCategoriaRepresentantePorConsecutivoAndIdioma(categoriaRepresentantesParaBuscar);

                return Ok(categoriaEventoBuscada);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarCategoriasDeUnRepresentante(CategoriasRepresentantes categoriaRepresentanteParaListar)
        {
            if (categoriaRepresentanteParaListar == null || categoriaRepresentanteParaListar.CodigoRepresentante <= 0 || categoriaRepresentanteParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("categoriaRepresentanteAListar vacio y/o invalido!.");
            }

            try
            {
                List<CategoriasRepresentantesDTO> listaCategoriasRepresentantes = await _categoriasBusiness.ListarCategoriasDeUnRepresentantePorIdioma(categoriaRepresentanteParaListar);

                return Ok(listaCategoriasRepresentantes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarCategoriaRepresentante(CategoriasRepresentantes categoriaReprensentateParaModificar)
        {
            if (categoriaReprensentateParaModificar == null || categoriaReprensentateParaModificar.Consecutivo <= 0 || categoriaReprensentateParaModificar.CodigoCategoria <= 0)
            {
                return BadRequest("categoriaReprensentateParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarCategoriaRepresentante = await _categoriasBusiness.ModificarCategoriaRepresentante(categoriaReprensentateParaModificar);

                return Ok(wrapperModificarCategoriaRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarCategoriaRepresentante(CategoriasRepresentantes categoriaRepresanteParaBorrar)
        {
            if (categoriaRepresanteParaBorrar == null || categoriaRepresanteParaBorrar.Consecutivo <= 0)
            {
                return BadRequest("categoriaRepresanteParaBorrar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarCategoriaRepresentante = await _categoriasBusiness.EliminarCategoriaRepresentante(categoriaRepresanteParaBorrar);

                return Ok(wrapperEliminarCategoriaRepresentante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}
