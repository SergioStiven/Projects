using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using System.Linq;

namespace Xpinn.SportsGo.Services
{
    public class AnunciantesServices : BaseService
    {


        #region Metodos Anunciantes


        public async Task<WrapperSimpleTypesDTO> CrearAnunciante(AnunciantesDTO anuncianteParaCrear)
        {
            if (anuncianteParaCrear == null || anuncianteParaCrear.Personas == null || anuncianteParaCrear.Personas.Usuarios == null)
            {
                throw new ArgumentNullException("No puedes crear un anunciante si anuncianteParaCrear, la persona o el usuario del anunciante es nulo!.");
            }
            else if (string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Nombres) || anuncianteParaCrear.Personas.CodigoPais <= 0 || anuncianteParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                || anuncianteParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.CiudadResidencia))
            {
                throw new ArgumentException("Persona de anuncianteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(anuncianteParaCrear.Personas.Usuarios.Email))
            {
                throw new ArgumentException("Usuario de anuncianteParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearAnunciante = await client.PostAsync<AnunciantesDTO, WrapperSimpleTypesDTO>("Anunciantes/CrearAnunciante", anuncianteParaCrear);

            return wrapperCrearAnunciante;
        }

        public async Task<AnunciantesDTO> BuscarAnunciantePorConsecutivo(AnunciantesDTO anuncianteParaBuscar)
        {
            if (anuncianteParaBuscar == null) throw new ArgumentNullException("No puedes buscar un anunciante si anuncianteParaBuscar es nulo!.");
            if (anuncianteParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncianteParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            AnunciantesDTO anuncianteBuscado = await client.PostAsync("Anunciantes/BuscarAnunciantePorConsecutivo", anuncianteParaBuscar);

            return anuncianteBuscado;
        }

        public async Task<AnunciantesDTO> BuscarAnunciantePorCodigoPersona(AnunciantesDTO anuncianteParaBuscar)
        {
            if (anuncianteParaBuscar == null) throw new ArgumentNullException("No puedes buscar un anunciante si anuncianteParaBuscar es nulo!.");
            if (anuncianteParaBuscar.CodigoPersona <= 0)
            {
                throw new ArgumentException("anuncianteParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            AnunciantesDTO anuncianteBuscado = await client.PostAsync("Anunciantes/BuscarAnunciantePorCodigoPersona", anuncianteParaBuscar);

            return anuncianteBuscado;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionAnunciante(AnunciantesDTO anuncianteParaModificar)
        {
            if (anuncianteParaModificar == null) throw new ArgumentNullException("No puedes modificar un anunciante si anuncianteParaModificar es nulo!.");
            if (anuncianteParaModificar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncianteParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarAnunciante = await client.PostAsync<AnunciantesDTO, WrapperSimpleTypesDTO>("Anunciantes/ModificarInformacionAnunciante", anuncianteParaModificar);

            return wrapperModificarAnunciante;
        }


        #endregion


        #region Metodos Anuncios


        public async Task<WrapperSimpleTypesDTO> CrearAnuncio(AnunciosDTO anuncioParaCrear)
        {
            if (anuncioParaCrear == null || anuncioParaCrear.AnunciosContenidos == null || anuncioParaCrear.AnunciosPaises == null || anuncioParaCrear.CategoriasAnuncios == null)
            {
                throw new ArgumentNullException("No puedes crear un anuncio si anuncioParaCrear, AnunciosContenidos, AnunciosPaises o CategoriasAnuncios son nulos!.");
            }
            if (anuncioParaCrear.CodigoAnunciante <= 0 || anuncioParaCrear.CodigoTipoAnuncio <= 0 || anuncioParaCrear.FechaInicio == DateTime.MinValue)
            {
                throw new ArgumentException("anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.AnunciosContenidos.Count <= 0
                  || !anuncioParaCrear.AnunciosContenidos.All(x => x.CodigoIdioma > 0 && !string.IsNullOrWhiteSpace(x.Titulo)))
            {
                throw new ArgumentException("AnuncioContenido del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.AnunciosPaises.Count <= 0
                  || !anuncioParaCrear.AnunciosPaises.All(x => x.CodigoPais > 0))
            {
                throw new ArgumentException("AnuncioPais del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.CategoriasAnuncios.Count <= 0
                  || !anuncioParaCrear.CategoriasAnuncios.All(x => x.CodigoCategoria > 0))
            {
                throw new ArgumentException("CategoriasAnuncio del anuncioParaCrear vacio y/o invalido!.");
            }
            else if (anuncioParaCrear.Archivos != null)
            {
                throw new ArgumentException("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearAnuncio = await client.PostAsync<AnunciosDTO, WrapperSimpleTypesDTO>("Anunciantes/CrearAnuncio", anuncioParaCrear);

            return wrapperCrearAnuncio;
        }

        public async Task<AnunciosDTO> BuscarAnuncioPorConsecutivo(AnunciosDTO anuncioParaBuscar)
        {
            if (anuncioParaBuscar == null) throw new ArgumentNullException("No puedes buscar un anuncio si anuncioParaBuscar es nulo!.");
            if (anuncioParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            AnunciosDTO anuncioBuscado = await client.PostAsync("Anunciantes/BuscarAnuncioPorConsecutivo", anuncioParaBuscar);

            return anuncioBuscado;
        }

        public async Task<List<AnunciosDTO>> ListarAnunciosDeUnAnunciante(AnunciosDTO anuncioParaListar)
        {
            if (anuncioParaListar == null) throw new ArgumentNullException("No puedes listar los anuncios si anuncioParaListar es nulo!.");
            if (anuncioParaListar.SkipIndexBase < 0 || anuncioParaListar.TakeIndexBase <= 0 || anuncioParaListar.CodigoAnunciante <= 0 || anuncioParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("anuncioParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<AnunciosDTO> listarInformacionAnuncios = await client.PostAsync<AnunciosDTO, List<AnunciosDTO>>("Anunciantes/ListarAnunciosDeUnAnunciante", anuncioParaListar);

            return listarInformacionAnuncios;
        }

        public async Task<List<AnunciosDTO>> ListarAnuncios(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los anuncios si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0  || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<AnunciosDTO> listarInformacionAnuncios = await client.PostAsync<BuscadorDTO, List<AnunciosDTO>>("Anunciantes/ListarAnuncios", buscador);

            return listarInformacionAnuncios;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarAnuncio(AnunciosDTO anuncioParaModificar)
        {
            if (anuncioParaModificar == null) throw new ArgumentNullException("No puedes modificar un anuncio si anuncioParaModificar es nulo!.");
            if (anuncioParaModificar.Consecutivo <= 0 || anuncioParaModificar.FechaInicio == DateTime.MinValue)
            {
                throw new ArgumentException("anuncioParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarAnuncio = await client.PostAsync<AnunciosDTO, WrapperSimpleTypesDTO>("Anunciantes/ModificarAnuncio", anuncioParaModificar);

            return wrapperModificarAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> AumentarContadorClickDeUnAnuncio(AnunciosDTO anuncioParaAumentar)
        {
            if (anuncioParaAumentar == null) throw new ArgumentNullException("No puedes aumentar el contador de un anuncio si anuncioParaAumentar es nulo!.");
            if (anuncioParaAumentar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioParaAumentar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAumentarContadorClickDeUnAnuncio = await client.PostAsync<AnunciosDTO, WrapperSimpleTypesDTO>("Anunciantes/AumentarContadorClickDeUnAnuncio", anuncioParaAumentar);

            return wrapperAumentarContadorClickDeUnAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncio(AnunciosDTO anuncioParaEliminar)
        {
            if (anuncioParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un anuncio si anuncioParaEliminar es nulo!.");
            if (anuncioParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarAnuncio = await client.PostAsync<AnunciosDTO, WrapperSimpleTypesDTO>("Anunciantes/EliminarAnuncio", anuncioParaEliminar);

            return wrapperEliminarAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivoAnuncio(AnunciosDTO anuncioArchivoParaEliminar)
        {
            if (anuncioArchivoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un archivoAnuncio si anuncioArchivoParaEliminar es nulo!.");
            if (anuncioArchivoParaEliminar.Consecutivo <= 0 || anuncioArchivoParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("anuncioArchivoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarArchivoAnuncio = await client.PostAsync<AnunciosDTO, WrapperSimpleTypesDTO>("Anunciantes/EliminarArchivoAnuncio", anuncioArchivoParaEliminar);

            return wrapperEliminarArchivoAnuncio;
        }


        #endregion


        #region Metodos AnunciosContenidos


        public async Task<WrapperSimpleTypesDTO> CrearAnunciosContenidos(List<AnunciosContenidosDTO> anuncioContenidoParaCrear)
        {
            if (anuncioContenidoParaCrear == null) throw new ArgumentNullException("No puedes crear un anuncioContenido si anuncioContenidoParaCrear es nulo!.");
            if (anuncioContenidoParaCrear.Count <= 0
                || !anuncioContenidoParaCrear.All(x => x.CodigoAnuncio > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                throw new ArgumentException("anuncioContenidoParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearAnunciosContenidos = await client.PostAsync<List<AnunciosContenidosDTO>, WrapperSimpleTypesDTO>("Anunciantes/CrearAnunciosContenidos", anuncioContenidoParaCrear);

            return wrapperCrearAnunciosContenidos;
        }

        public async Task<AnunciosContenidosDTO> BuscarAnuncioContenidoPorConsecutivo(AnunciosContenidosDTO anuncioContenidoParaBuscar)
        {
            if (anuncioContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un anuncioContenido si anuncioContenidoParaBuscar es nulo!.");
            if (anuncioContenidoParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioContenidoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            AnunciosContenidosDTO anuncioContenidoBuscado = await client.PostAsync("Anunciantes/BuscarAnuncioContenidoPorConsecutivo", anuncioContenidoParaBuscar);

            return anuncioContenidoBuscado;
        }

        public async Task<List<AnunciosContenidosDTO>> ListarAnunciosContenidosDeUnAnuncio(AnunciosContenidosDTO anuncioContenidoParaListar)
        {
            if (anuncioContenidoParaListar == null) throw new ArgumentNullException("No puedes listar los anunciosContenidos si anuncioContenidoParaListar es nulo!.");
            if (anuncioContenidoParaListar.CodigoAnuncio <= 0)
            {
                throw new ArgumentException("anuncioContenidoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<AnunciosContenidosDTO> listaContenidoDeUnAnuncio = await client.PostAsync<AnunciosContenidosDTO, List<AnunciosContenidosDTO>>("Anunciantes/ListarAnunciosContenidosDeUnAnuncio", anuncioContenidoParaListar);

            return listaContenidoDeUnAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarAnuncioContenido(AnunciosContenidosDTO anuncioContenidoParaModificar)
        {
            if (anuncioContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar un anuncioContenido si anuncioContenidoParaModificar es nulo!.");
            if (anuncioContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(anuncioContenidoParaModificar.Titulo))
            {
                throw new ArgumentException("anuncioContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarAnuncioContenido = await client.PostAsync<AnunciosContenidosDTO, WrapperSimpleTypesDTO>("Anunciantes/ModificarAnuncioContenido", anuncioContenidoParaModificar);

            return wrapperModificarAnuncioContenido;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarMultiplesAnuncioContenido(List<AnunciosContenidosDTO> anuncioContenidoParaModificar)
        {
            if (anuncioContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar un anuncioContenido si anuncioContenidoParaModificar es nulo!.");
            if (anuncioContenidoParaModificar.Count > 0 && anuncioContenidoParaModificar.All(x =>  x.Consecutivo > 0 && !string.IsNullOrWhiteSpace(x.Titulo) && x.CodigoIdioma > 0))
            {
                throw new ArgumentException("anuncioContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarMultiplesAnuncioContenido = await client.PostAsync<List<AnunciosContenidosDTO>, WrapperSimpleTypesDTO>("Anunciantes/ModificarMultiplesAnuncioContenido", anuncioContenidoParaModificar);

            return wrapperModificarMultiplesAnuncioContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncioContenido(AnunciosContenidosDTO anuncioContenidoParaEliminar)
        {
            if (anuncioContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un anuncioContenido si anuncioContenidoParaEliminar es nulo!.");
            if (anuncioContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioContenidoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarAnuncioContenido = await client.PostAsync<AnunciosContenidosDTO, WrapperSimpleTypesDTO>("Anunciantes/EliminarAnuncioContenido", anuncioContenidoParaEliminar);

            return wrapperEliminarAnuncioContenido;
        }


        #endregion


        #region Metodos AnunciosPaises


        public async Task<WrapperSimpleTypesDTO> CrearAnunciosPaises(List<AnunciosPaisesDTO> anuncioPaisParaCrear)
        {
            if (anuncioPaisParaCrear == null) throw new ArgumentNullException("No puedes crear un anuncioPais si anuncioPaisParaCrear es nulo!.");
            if (anuncioPaisParaCrear.Count <= 0
                || !anuncioPaisParaCrear.All(x => x.CodigoAnuncio > 0 && x.CodigoPais > 0))
            {
                throw new ArgumentException("anuncioPaisParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearAnunciosPaises = await client.PostAsync<List<AnunciosPaisesDTO>, WrapperSimpleTypesDTO>("Anunciantes/CrearAnunciosPaises", anuncioPaisParaCrear);

            return wrapperCrearAnunciosPaises;
        }

        public async Task<AnunciosPaisesDTO> BuscaAnuncioPaisPorConsecutivo(AnunciosPaisesDTO anuncioPaisParaBuscar)
        {
            if (anuncioPaisParaBuscar == null) throw new ArgumentNullException("No puedes buscar un anuncioPais si anuncioPaisParaBuscar es nulo!.");
            if (anuncioPaisParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioPaisParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            AnunciosPaisesDTO anuncioPaisBuscado = await client.PostAsync("Anunciantes/BuscaAnuncioPaisPorConsecutivo", anuncioPaisParaBuscar);

            return anuncioPaisBuscado;
        }

        public async Task<List<AnunciosPaisesDTO>> ListarAnunciosPaisesDeUnAnuncio(AnunciosPaisesDTO anuncioPaisesParaListar)
        {
            if (anuncioPaisesParaListar == null) throw new ArgumentNullException("No puedes listar los anuncioPais si anuncioPaisesParaListar es nulo!.");
            if (anuncioPaisesParaListar.CodigoAnuncio <= 0)
            {
                throw new ArgumentException("anuncioPaisesParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<AnunciosPaisesDTO> listaPaisesDeUnAnuncio = await client.PostAsync<AnunciosPaisesDTO, List<AnunciosPaisesDTO>>("Anunciantes/ListarAnunciosPaisesDeUnAnuncio", anuncioPaisesParaListar);

            return listaPaisesDeUnAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarAnuncioPais(AnunciosPaisesDTO anuncioPaisParaEliminar)
        {
            if (anuncioPaisParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un anuncioPais si anuncioPaisParaEliminar es nulo!.");
            if (anuncioPaisParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("anuncioPaisParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarAnuncioPais = await client.PostAsync<AnunciosPaisesDTO, WrapperSimpleTypesDTO>("Anunciantes/EliminarAnuncioPais", anuncioPaisParaEliminar);

            return wrapperEliminarAnuncioPais;
        }


        #endregion


    }
}
