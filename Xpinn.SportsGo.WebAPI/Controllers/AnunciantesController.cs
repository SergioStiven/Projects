using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using System;
using Xpinn.SportsGo.DomainEntities;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class AnunciantesController : ApiController
    {
        AnunciantesBusiness _anuncianteBusiness;

        public AnunciantesController()
        {
            _anuncianteBusiness = new AnunciantesBusiness();
        }


        #region Metodos Anunciantes


        public async Task<IHttpActionResult> CrearAnunciante(Anunciantes anuncianteParaCrear)
        {
            if (anuncianteParaCrear == null || anuncianteParaCrear.Personas == null || anuncianteParaCrear.Personas.Usuarios == null)
            {
                return BadRequest("anuncianteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Nombres) || anuncianteParaCrear.Personas.CodigoPais <= 0 || anuncianteParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                || anuncianteParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.CiudadResidencia))
            {
                return BadRequest("Persona de anuncianteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Email))
            {
                return BadRequest("Usuario de anuncianteParaCrear vacio y/o invalido!.");
            }

            try
            {
                string urlLogo = Url.Content("~/Content/Images/LogoSportsGo.png");
                string urlBanner = Url.Content("~/Content/Images/BannerSportsGo.png");

                WrapperSimpleTypesDTO wrapperCrearAnunciante = await _anuncianteBusiness.CrearAnunciante(anuncianteParaCrear, urlLogo, urlBanner);

                return Ok(wrapperCrearAnunciante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarAnunciantePorConsecutivo(Anunciantes anuncianteParaBuscar)
        {
            if (anuncianteParaBuscar == null || anuncianteParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("anuncianteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Anunciantes anuncianteBuscado = await _anuncianteBusiness.BuscarAnunciantePorConsecutivo(anuncianteParaBuscar);

                return Ok(anuncianteBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarAnunciantePorCodigoPersona(Anunciantes anuncianteParaBuscar)
        {
            if (anuncianteParaBuscar == null || anuncianteParaBuscar.CodigoPersona <= 0)
            {
                return BadRequest("anuncianteParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Anunciantes anuncianteBuscado = await _anuncianteBusiness.BuscarAnunciantePorCodigoPersona(anuncianteParaBuscar);

                return Ok(anuncianteBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarInformacionAnunciante(Anunciantes anuncianteParaModificar)
        {
            if (anuncianteParaModificar == null || anuncianteParaModificar.Consecutivo <= 0)
            {
                return BadRequest("anuncianteParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarAnunciante = await _anuncianteBusiness.ModificarInformacionAnunciante(anuncianteParaModificar);

                return Ok(wrapperModificarAnunciante);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Anuncios


        public async Task<IHttpActionResult> CrearAnuncio(Anuncios anuncioParaCrear)
        {
            if (anuncioParaCrear == null || anuncioParaCrear.CodigoAnunciante <= 0 || anuncioParaCrear.CodigoTipoAnuncio <= 0 || anuncioParaCrear.FechaInicio == DateTime.MinValue)
            {
                return BadRequest("anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.AnunciosContenidos == null || anuncioParaCrear.AnunciosContenidos.Count <= 0
                  || !anuncioParaCrear.AnunciosContenidos.All(x => x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.Titulo)))
            {
                return BadRequest("AnuncioContenido del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.AnunciosPaises == null || anuncioParaCrear.AnunciosPaises.Count <= 0
                  || !anuncioParaCrear.AnunciosPaises.All(x => x.CodigoPais > 0))
            {
                return BadRequest("AnuncioPais del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.CategoriasAnuncios == null || anuncioParaCrear.CategoriasAnuncios.Count <= 0
                  || !anuncioParaCrear.CategoriasAnuncios.All(x => x.CodigoCategoria > 0))
            {
                return BadRequest("CategoriasAnuncio del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.Archivos != null)
            {
                return BadRequest("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearAnuncio = await _anuncianteBusiness.CrearAnuncio(anuncioParaCrear);

                return Ok(wrapperCrearAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarAnuncioPorConsecutivo(Anuncios anuncioParaBuscar)
        {
            if (anuncioParaBuscar == null || anuncioParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("anuncioParaBuscar vacio y/o invalido!.");
            }

            try
            {
                Anuncios anuncioBuscado = await _anuncianteBusiness.BuscarAnuncioPorConsecutivo(anuncioParaBuscar);

                return Ok(anuncioBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarAnunciosDeUnAnunciante(Anuncios anuncioParaListar)
        {
            if (anuncioParaListar == null || anuncioParaListar.SkipIndexBase < 0 || anuncioParaListar.TakeIndexBase <= 0 || anuncioParaListar.CodigoAnunciante <= 0 || anuncioParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("anuncioParaListar vacio y/o invalido!.");
            }

            try
            {
                List<AnunciosDTO> listarInformacionAnuncios = await _anuncianteBusiness.ListarAnunciosDeUnAnunciante(anuncioParaListar);

                return Ok(listarInformacionAnuncios);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarAnuncios(BuscadorDTO buscador)
        {
            if (buscador == null || buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("buscador vacio y/o invalido!.");
            }

            try
            {
                List<AnunciosDTO> listarInformacionAnuncios = await _anuncianteBusiness.ListarAnuncios(buscador);

                return Ok(listarInformacionAnuncios);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarAnuncio(Anuncios anuncioParaModificar)
        {
            if (anuncioParaModificar == null || anuncioParaModificar.Consecutivo <= 0 || anuncioParaModificar.FechaInicio == DateTime.MinValue)
            {
                return BadRequest("anuncioParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarAnuncio = await _anuncianteBusiness.ModificarAnuncio(anuncioParaModificar);

                return Ok(wrapperModificarAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> AumentarContadorClickDeUnAnuncio(Anuncios anuncioParaAumentar)
        {
            if (anuncioParaAumentar == null || anuncioParaAumentar.Consecutivo <= 0)
            {
                return BadRequest("anuncioParaAumentar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperAumentarContadorClickDeUnAnuncio = await _anuncianteBusiness.AumentarContadorClickDeUnAnuncio(anuncioParaAumentar);

                return Ok(wrapperAumentarContadorClickDeUnAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarAnuncio(Anuncios anuncioParaEliminar)
        {
            if (anuncioParaEliminar == null || anuncioParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("anuncioParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarAnuncio = await _anuncianteBusiness.EliminarAnuncio(anuncioParaEliminar);

                return Ok(wrapperEliminarAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarArchivoAnuncio(Anuncios anuncioArchivoParaEliminar)
        {
            if (anuncioArchivoParaEliminar == null || anuncioArchivoParaEliminar.Consecutivo <= 0 || anuncioArchivoParaEliminar.CodigoArchivo <= 0)
            {
                return BadRequest("anuncioArchivoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarArchivoAnuncio = await _anuncianteBusiness.EliminarArchivoAnuncio(anuncioArchivoParaEliminar);

                return Ok(wrapperEliminarArchivoAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos AnunciosContenidos


        public async Task<IHttpActionResult> CrearAnunciosContenidos(List<AnunciosContenidos> anuncioContenidoParaCrear)
        {
            if (anuncioContenidoParaCrear == null || anuncioContenidoParaCrear.Count <= 0 
                || !anuncioContenidoParaCrear.All( x => x.CodigoAnuncio > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                return BadRequest("anuncioContenidoParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearAnunciosContenidos = await _anuncianteBusiness.CrearAnunciosContenidos(anuncioContenidoParaCrear);

                return Ok(wrapperCrearAnunciosContenidos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscarAnuncioContenidoPorConsecutivo(AnunciosContenidos anuncioContenidoParaBuscar)
        {
            if (anuncioContenidoParaBuscar == null || anuncioContenidoParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("anuncioContenidoParaBuscar vacio y/o invalido!.");
            }

            try
            {
                AnunciosContenidos anuncioContenidoBuscado = await _anuncianteBusiness.BuscarAnuncioContenidoPorConsecutivo(anuncioContenidoParaBuscar);

                return Ok(anuncioContenidoBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarAnunciosContenidosDeUnAnuncio(AnunciosContenidos anuncioContenidoParaListar)
        {
            if (anuncioContenidoParaListar == null || anuncioContenidoParaListar.CodigoAnuncio <= 0)
            {
                return BadRequest("anuncioContenidoParaListar vacio y/o invalido!.");
            }

            try
            {
                List<AnunciosContenidos> listaContenidoDeUnAnuncio = await _anuncianteBusiness.ListarAnunciosContenidosDeUnAnuncio(anuncioContenidoParaListar);

                return Ok(listaContenidoDeUnAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarAnuncioContenido(AnunciosContenidos anuncioContenidoParaModificar)
        {
            if (anuncioContenidoParaModificar == null || anuncioContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(anuncioContenidoParaModificar.Titulo))
            {
                return BadRequest("anuncioContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarAnuncioContenido = await _anuncianteBusiness.ModificarAnuncioContenido(anuncioContenidoParaModificar);

                return Ok(wrapperModificarAnuncioContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ModificarMultiplesAnuncioContenido(List<AnunciosContenidos> anuncioContenidoParaModificar)
        {
            if (anuncioContenidoParaModificar == null || anuncioContenidoParaModificar.Count > 0
                || anuncioContenidoParaModificar.All(x => x.Consecutivo > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                return BadRequest("anuncioContenidoParaModificar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperModificarMultiplesAnuncioContenido = await _anuncianteBusiness.ModificarMultiplesAnuncioContenido(anuncioContenidoParaModificar);

                return Ok(wrapperModificarMultiplesAnuncioContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarAnuncioContenido(AnunciosContenidos anuncioContenidoParaEliminar)
        {
            if (anuncioContenidoParaEliminar == null || anuncioContenidoParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("anuncioContenidoParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarAnuncioContenido = await _anuncianteBusiness.EliminarAnuncioContenido(anuncioContenidoParaEliminar);

                return Ok(wrapperEliminarAnuncioContenido);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos AnunciosPaises


        public async Task<IHttpActionResult> CrearAnunciosPaises(List<AnunciosPaises> anuncioPaisParaCrear)
        {
            if (anuncioPaisParaCrear == null || anuncioPaisParaCrear.Count <= 0
                || !anuncioPaisParaCrear.All(x => x.CodigoAnuncio > 0 && x.CodigoPais > 0))
            {
                return BadRequest("anuncioPaisParaCrear vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperCrearAnunciosPaises = await _anuncianteBusiness.CrearAnunciosPaises(anuncioPaisParaCrear);

                return Ok(wrapperCrearAnunciosPaises);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> BuscaAnuncioPaisPorConsecutivo(AnunciosPaises anuncioPaisParaBuscar)
        {
            if (anuncioPaisParaBuscar == null || anuncioPaisParaBuscar.Consecutivo <= 0)
            {
                return BadRequest("anuncioPaisParaBuscar vacio y/o invalido!.");
            }

            try
            {
                AnunciosPaises anuncioPaisBuscado = await _anuncianteBusiness.BuscaAnuncioPaisPorConsecutivo(anuncioPaisParaBuscar);

                return Ok(anuncioPaisBuscado);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarAnunciosPaisesDeUnAnuncio(AnunciosPaises anuncioPaisesParaListar)
        {
            if (anuncioPaisesParaListar == null || anuncioPaisesParaListar.CodigoAnuncio <= 0)
            {
                return BadRequest("anuncioPaisesParaListar vacio y/o invalido!.");
            }

            try
            {
                List<AnunciosPaises> listaPaisesDeUnAnuncio = await _anuncianteBusiness.ListarAnunciosPaisesDeUnAnuncio(anuncioPaisesParaListar);

                return Ok(listaPaisesDeUnAnuncio);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> EliminarAnuncioPais(AnunciosPaises anuncioPaisParaEliminar)
        {
            if (anuncioPaisParaEliminar == null || anuncioPaisParaEliminar.Consecutivo <= 0)
            {
                return BadRequest("anuncioPaisParaEliminar vacio y/o invalido!.");
            }

            try
            {
                WrapperSimpleTypesDTO wrapperEliminarAnuncioPais = await _anuncianteBusiness.EliminarAnuncioPais(anuncioPaisParaEliminar);

                return Ok(wrapperEliminarAnuncioPais);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion

    }
}
