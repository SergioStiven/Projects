using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Services
{
    public class CategoriasServices : BaseService
    {


        #region Metodos Categorias


        public async Task<WrapperSimpleTypesDTO> CrearCategoria(CategoriasDTO categoriaParaCrear)
        {
            if (categoriaParaCrear == null || categoriaParaCrear.CategoriasContenidos == null) throw new ArgumentNullException("No puedes crear una categoria si categoriaParaCrear o su contenido es nula!.");
            if (categoriaParaCrear.CategoriasContenidos.Count <= 0 || !categoriaParaCrear.CategoriasContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0)
                || categoriaParaCrear.Archivos == null || categoriaParaCrear.Archivos.ArchivoContenido == null)
            {
                throw new ArgumentException("categoriaParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoria = await client.PostAsync<CategoriasDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoria", categoriaParaCrear);

            return wrapperCrearCategoria;
        }

        public async Task<CategoriasDTO> BuscarCategoria(CategoriasDTO categoriaParaBuscar)
        {
            if (categoriaParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoria si categoriaParaBuscar es nulo!.");
            if (categoriaParaBuscar.Consecutivo <= 0) throw new ArgumentException("categoriaParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CategoriasDTO categoriaBuscada = await client.PostAsync("Categorias/BuscarCategoria", categoriaParaBuscar);

            return categoriaBuscada;
        }

        public async Task<List<CategoriasDTO>> ListarCategoriasPorIdioma(CategoriasDTO categoriaParaListar)
        {
            if (categoriaParaListar == null) throw new ArgumentNullException("No puedes listar una categoria si categoriaParaListar es nulo!.");
            if (categoriaParaListar.IdiomaBase == Idioma.SinIdioma) throw new ArgumentException("categoriaParaListar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasDTO> listaCategorias = await client.PostAsync<CategoriasDTO, List<CategoriasDTO>>("Categorias/ListarCategoriasPorIdioma", categoriaParaListar);

            return listaCategorias;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoCategoria(CategoriasDTO categoriaParaModificar)
        {
            if (categoriaParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoria si categoriaParaModificar es nulo!.");
            if (categoriaParaModificar.CodigoArchivo <= 0 || categoriaParaModificar.ArchivoContenido == null)
            {
                throw new ArgumentException("categoriaParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarArchivoCategoria = await client.PostAsync<CategoriasDTO, WrapperSimpleTypesDTO>("Categorias/ModificarArchivoCategoria", categoriaParaModificar);

            return wrapperModificarArchivoCategoria;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoria(CategoriasDTO categoriaParaEliminar)
        {
            if (categoriaParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una categoria si categoriaParaEliminar es nulo!.");
            if (categoriaParaEliminar.Consecutivo <= 0 || categoriaParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("categoriaParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoria = await client.PostAsync<CategoriasDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoria", categoriaParaEliminar);

            return wrapperEliminarCategoria;
        }


        #endregion


        #region Metodos CategoriasContenidos


        public async Task<WrapperSimpleTypesDTO> CrearCategoriasContenidos(List<CategoriasContenidosDTO> categoriaContenidoParaCrear)
        {
            if (categoriaContenidoParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaContenido si categoriaContenidoParaCrear o su contenido es nula!.");
            if (categoriaContenidoParaCrear.Count <= 0 ||
                !categoriaContenidoParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoCategoria > 0))
            {
                throw new ArgumentException("categoriaContenidoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaContenido = await client.PostAsync<List<CategoriasContenidosDTO>, WrapperSimpleTypesDTO>("Categorias/CrearCategoriasContenidos", categoriaContenidoParaCrear);

            return wrapperCrearCategoriaContenido;
        }

        public async Task<CategoriasContenidosDTO> BuscarCategoriaContenido(CategoriasContenidosDTO categoriaContenidoParaBuscar)
        {
            if (categoriaContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaContenido si categoriaContenidoParaBuscar es nulo!.");
            if (categoriaContenidoParaBuscar.Consecutivo <= 0) throw new ArgumentException("categoriaContenidoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CategoriasContenidosDTO categoriaContenidoBuscada = await client.PostAsync("Categorias/BuscarCategoriaContenido", categoriaContenidoParaBuscar);

            return categoriaContenidoBuscada;
        }

        public async Task<List<CategoriasContenidosDTO>> ListarContenidoDeUnaCategoria(CategoriasContenidosDTO categoriaContenidoParaListar)
        {
            if (categoriaContenidoParaListar == null) throw new ArgumentNullException("No puedes listar una categoriaContenido si categoriaContenidoParaListar es nulo!.");
            if (categoriaContenidoParaListar.CodigoCategoria <= 0) throw new ArgumentException("categoriaContenidoParaListar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasContenidosDTO> listaCategoriasContenido = await client.PostAsync<CategoriasContenidosDTO, List<CategoriasContenidosDTO>>("Categorias/ListarContenidoDeUnaCategoria", categoriaContenidoParaListar);

            return listaCategoriasContenido;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaContenido(CategoriasContenidosDTO categoriaContenidoParaModificar)
        {
            if (categoriaContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoriaContenido si categoriaContenidoParaModificar es nulo!.");
            if (categoriaContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(categoriaContenidoParaModificar.Descripcion))
            {
                throw new ArgumentException("categoriaContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaContenido = await client.PostAsync<CategoriasContenidosDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaContenido", categoriaContenidoParaModificar);

            return wrapperModificarCategoriaContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaContenido(CategoriasContenidosDTO categoriaContenidoParaEliminar)
        {
            if (categoriaContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una categoriaContenido si categoriaContenidoParaEliminar es nulo!.");
            if (categoriaContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("categoriaContenidoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaContenido = await client.PostAsync<CategoriasContenidosDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaContenido", categoriaContenidoParaEliminar);

            return wrapperEliminarCategoriaContenido;
        }


        #endregion


        #region Metodos CategoriasAnuncios


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaAnuncios(List<CategoriasAnunciosDTO> categoriaAnuncioParaCrear)
        {
            if (categoriaAnuncioParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaAnuncio si categoriaAnuncioParaCrear es nula!.");
            if (categoriaAnuncioParaCrear.Count <= 0 || !categoriaAnuncioParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoAnuncio > 0))
            {
                throw new ArgumentException("categoriaAnuncioParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearListaCategoriaAnuncios = await client.PostAsync<List<CategoriasAnunciosDTO>, WrapperSimpleTypesDTO>("Categorias/CrearListaCategoriaAnuncios", categoriaAnuncioParaCrear);

            return wrapperCrearListaCategoriaAnuncios;
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaAnuncios(CategoriasAnunciosDTO categoriaAnuncioParaCrear)
        {
            if (categoriaAnuncioParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaAnuncio si categoriaAnuncioParaCrear es nula!.");
            if (categoriaAnuncioParaCrear.CodigoCategoria > 0 || categoriaAnuncioParaCrear.CodigoAnuncio > 0)
            {
                throw new ArgumentException("categoriaAnuncioParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaAnuncio = await client.PostAsync<CategoriasAnunciosDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoriaAnuncios", categoriaAnuncioParaCrear);

            return wrapperCrearCategoriaAnuncio;
        }

        public async Task<CategoriasAnunciosDTO> BuscarCategoriaAnuncioPorConsecutivoAndIdioma(CategoriasAnunciosDTO categoriaAnuncioParaBuscar)
        {
            if (categoriaAnuncioParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaAnuncio si categoriaParaBuscar es nulo!.");
            if (categoriaAnuncioParaBuscar.Consecutivo <= 0 || categoriaAnuncioParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            CategoriasAnunciosDTO categoriaAnuncioBuscada = await client.PostAsync("Categorias/BuscarCategoriaAnuncioPorConsecutivoAndIdioma", categoriaAnuncioParaBuscar);

            return categoriaAnuncioBuscada;
        }

        public async Task<List<CategoriasDTO>> ListarCategoriasDeUnAnuncioPorIdioma(CategoriasAnunciosDTO categoriaAnuncioParaListar)
        {
            if (categoriaAnuncioParaListar == null) throw new ArgumentNullException("No puedes buscar una categoriaAnuncio si categoriaAnuncioParaListar es nulo!.");
            if (categoriaAnuncioParaListar.CodigoAnuncio <= 0 || categoriaAnuncioParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaAnuncioParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasDTO> listaCategoriasDeUnAnuncio = await client.PostAsync<CategoriasAnunciosDTO, List<CategoriasDTO>>("Categorias/ListarCategoriasDeUnAnuncioPorIdioma", categoriaAnuncioParaListar);

            return listaCategoriasDeUnAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaAnuncio(CategoriasAnunciosDTO categoriaAnuncioParaModificar)
        {
            if (categoriaAnuncioParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoriaAnuncio si categoriaAnuncioParaModificar es nula!.");
            if (categoriaAnuncioParaModificar.Consecutivo <= 0 || categoriaAnuncioParaModificar.CodigoCategoria <= 0)
            {
                throw new ArgumentException("categoriaAnuncioParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaAnuncio = await client.PostAsync<CategoriasAnunciosDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaAnuncio", categoriaAnuncioParaModificar);

            return wrapperModificarCategoriaAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaAnuncio(CategoriasAnunciosDTO categoriaAnuncioParaBorrar)
        {
            if (categoriaAnuncioParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una categoriaAnuncio si categoriaAnuncioParaBorrar es nula!.");
            if (categoriaAnuncioParaBorrar.Consecutivo <= 0) throw new ArgumentException("categoriaAnuncioParaBorrar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaAnuncio = await client.PostAsync<CategoriasAnunciosDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaAnuncio", categoriaAnuncioParaBorrar);

            return wrapperEliminarCategoriaAnuncio;
        }


        #endregion


        #region Metodos CategoriasCandidatos


        public async Task<WrapperSimpleTypesDTO> CrearCategoriaCandidatos(CategoriasCandidatosDTO categoriaCandidatoParaCrear)
        {
            if (categoriaCandidatoParaCrear == null || categoriaCandidatoParaCrear.HabilidadesCandidatos == null)
            {
                throw new ArgumentNullException("No puedes crear una categoriaCandidato si categoriaCandidatoParaCrear es nula!.");
            }
            if (categoriaCandidatoParaCrear.CodigoCategoria <= 0 || categoriaCandidatoParaCrear.CodigoCandidato <= 0
                || categoriaCandidatoParaCrear.HabilidadesCandidatos.Count <= 0
                || !categoriaCandidatoParaCrear.HabilidadesCandidatos.All(x => x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                throw new ArgumentException("categoriaCandidatoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaCandidato = await client.PostAsync<CategoriasCandidatosDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoriaCandidatos", categoriaCandidatoParaCrear);

            return wrapperCrearCategoriaCandidato;
        }

        public async Task<CategoriasCandidatosDTO> BuscarCategoriaCandidatoPorConsecutivoAndIdioma(CategoriasCandidatosDTO categoriaCandidatoParaBuscar)
        {
            if (categoriaCandidatoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaCandidato si categoriaCandidatoParaBuscar es nulo!.");
            if (categoriaCandidatoParaBuscar.Consecutivo <= 0 || categoriaCandidatoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaCandidatoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            CategoriasCandidatosDTO categoriaCandidatoBuscada = await client.PostAsync("Categorias/BuscarCategoriaCandidatoPorConsecutivoAndIdioma", categoriaCandidatoParaBuscar);

            return categoriaCandidatoBuscada;
        }

        public async Task<List<CategoriasCandidatosDTO>> ListarCategoriasDeUnCandidatoPorIdioma(CategoriasCandidatosDTO categoriaCandidatoParaListar)
        {
            if (categoriaCandidatoParaListar == null) throw new ArgumentNullException("No puedes buscar una categoriaCandidato si categoriaCandidatoParaListar es nulo!.");
            if (categoriaCandidatoParaListar.CodigoCandidato <= 0 || categoriaCandidatoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaCandidatoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasCandidatosDTO> listaCategoriasCandidatos = await client.PostAsync<CategoriasCandidatosDTO, List<CategoriasCandidatosDTO>>("Categorias/ListarCategoriasDeUnCandidatoPorIdioma", categoriaCandidatoParaListar);

            return listaCategoriasCandidatos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaCandidato(CategoriasCandidatosDTO categoriaCandidatoParaModificar)
        {
            if (categoriaCandidatoParaModificar == null || categoriaCandidatoParaModificar.HabilidadesCandidatos == null)
            {
                throw new ArgumentNullException("No puedes modificar una categoriaCandidato si categoriaCandidatoParaModificar es nula!.");
            }
            if (categoriaCandidatoParaModificar.Consecutivo <= 0 || categoriaCandidatoParaModificar.CodigoCategoria <= 0 || categoriaCandidatoParaModificar.CodigoCandidato <= 0
                || categoriaCandidatoParaModificar.HabilidadesCandidatos.Count <= 0
                || !categoriaCandidatoParaModificar.HabilidadesCandidatos.All(x => x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                throw new ArgumentException("categoriaCandidatoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaCandidato = await client.PostAsync<CategoriasCandidatosDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaCandidato", categoriaCandidatoParaModificar);

            return wrapperModificarCategoriaCandidato;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaCandidato(CategoriasCandidatosDTO categoriaCandidatoParaBorrar)
        {
            if (categoriaCandidatoParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una categoriaCandidato si categoriaCandidatoParaBorrar es nula!.");
            if (categoriaCandidatoParaBorrar.Consecutivo <= 0)
            {
                throw new ArgumentException("categoriaCandidatoParaBorrar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaCandidato = await client.PostAsync<CategoriasCandidatosDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaCandidato", categoriaCandidatoParaBorrar);

            return wrapperEliminarCategoriaCandidato;
        }


        #endregion


        #region Metodos CategoriasEventos


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaEventos(List<CategoriasEventosDTO> categoriaEventoParaCrear)
        {
            if (categoriaEventoParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaEvento si categoriaEventoParaCrear es nula!.");
            if (categoriaEventoParaCrear.Count <= 0 || !categoriaEventoParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoEvento > 0))
            {
                throw new ArgumentException("categoriaEventoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearListaCategoriaEventos = await client.PostAsync<List<CategoriasEventosDTO>, WrapperSimpleTypesDTO>("Categorias/CrearListaCategoriaEventos", categoriaEventoParaCrear);

            return wrapperCrearListaCategoriaEventos;
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaEventos(CategoriasEventosDTO categoriaEventoParaCrear)
        {
            if (categoriaEventoParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaEvento si categoriaEventoParaCrear es nula!.");
            if (categoriaEventoParaCrear.CodigoCategoria > 0 || categoriaEventoParaCrear.CodigoEvento > 0)
            {
                throw new ArgumentException("categoriaEventoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaEvento = await client.PostAsync<CategoriasEventosDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoriaEventos", categoriaEventoParaCrear);

            return wrapperCrearCategoriaEvento;
        }

        public async Task<CategoriasEventosDTO> BuscarCategoriaEventoPorConsecutivoAndIdioma(CategoriasEventosDTO categoriaEventoParaBuscar)
        {
            if (categoriaEventoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaEvento si categoriaEventoParaBuscar es nulo!.");
            if (categoriaEventoParaBuscar.Consecutivo <= 0 || categoriaEventoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaEventoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            CategoriasEventosDTO categoriaEventoBuscada = await client.PostAsync("Categorias/BuscarCategoriaEventoPorConsecutivoAndIdioma", categoriaEventoParaBuscar);

            return categoriaEventoBuscada;
        }

        public async Task<List<CategoriasEventosDTO>> ListarCategoriasDeUnEventoPorIdioma(CategoriasEventosDTO categoriaEventoParaListar)
        {
            if (categoriaEventoParaListar == null) throw new ArgumentNullException("No puedes buscar una categoriaEvento si categoriaEventoParaListar es nulo!.");
            if (categoriaEventoParaListar.CodigoEvento <= 0 || categoriaEventoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaEventoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasEventosDTO> listaCategoriasEventos = await client.PostAsync<CategoriasEventosDTO, List<CategoriasEventosDTO>>("Categorias/ListarCategoriasDeUnEventoPorIdioma", categoriaEventoParaListar);

            return listaCategoriasEventos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaEvento(CategoriasEventosDTO categoriaEventoParaModificar)
        {
            if (categoriaEventoParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoriaEvento si categoriaEventoParaModificar es nula!.");
            if (categoriaEventoParaModificar.Consecutivo <= 0 || categoriaEventoParaModificar.CodigoCategoria <= 0)
            {
                throw new ArgumentException("categoriaEventoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaEvento = await client.PostAsync<CategoriasEventosDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaEvento", categoriaEventoParaModificar);

            return wrapperModificarCategoriaEvento;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaEvento(CategoriasEventosDTO categoriaEventoParaBorrar)
        {
            if (categoriaEventoParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una categoriaEvento si categoriaEventoParaBorrar es nula!.");
            if (categoriaEventoParaBorrar.Consecutivo <= 0) throw new ArgumentException("categoriaEventoParaBorrar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaEvento = await client.PostAsync<CategoriasEventosDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaEvento", categoriaEventoParaBorrar);

            return wrapperEliminarCategoriaEvento;
        }


        #endregion


        #region Metodos CategoriasGrupos


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaGrupos(List<CategoriasGruposDTO> categoriaGrupoParaCrear)
        {
            if (categoriaGrupoParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaGrupo si categoriaGrupoParaCrear es nula!.");
            if (categoriaGrupoParaCrear.Count <= 0 || !categoriaGrupoParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoGrupo > 0))
            {
                throw new ArgumentException("categoriaGrupoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearListaCategoriaGrupos = await client.PostAsync<List<CategoriasGruposDTO>, WrapperSimpleTypesDTO>("Categorias/CrearListaCategoriaGrupos", categoriaGrupoParaCrear);

            return wrapperCrearListaCategoriaGrupos;
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaGrupos(CategoriasGruposDTO categoriaGrupoParaCrear)
        {
            if (categoriaGrupoParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaGrupo si categoriaGrupoParaCrear es nula!.");
            if (categoriaGrupoParaCrear.CodigoCategoria <= 0 || categoriaGrupoParaCrear.CodigoGrupo <= 0)
            {
                throw new ArgumentException("categoriaGrupoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaGrupo = await client.PostAsync<CategoriasGruposDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoriaGrupos", categoriaGrupoParaCrear);

            return wrapperCrearCategoriaGrupo;
        }

        public async Task<CategoriasGruposDTO> BuscarCategoriaGrupoPorConsecutivoAndIdioma(CategoriasGruposDTO categoriaGrupoParaBuscar)
        {
            if (categoriaGrupoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaGrupo si categoriaGrupoParaBuscar es nulo!.");
            if (categoriaGrupoParaBuscar.Consecutivo <= 0 || categoriaGrupoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaGrupoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            CategoriasGruposDTO categoriaGrupoBuscada = await client.PostAsync("Categorias/BuscarCategoriaGrupoPorConsecutivoAndIdioma", categoriaGrupoParaBuscar);

            return categoriaGrupoBuscada;
        }

        public async Task<List<CategoriasGruposDTO>> ListarCategoriasDeUnGrupoPorIdioma(CategoriasGruposDTO categoriaGrupoParaListar)
        {
            if (categoriaGrupoParaListar == null) throw new ArgumentNullException("No puedes buscar una categoriaGrupo si categoriaGrupoParaListar es nulo!.");
            if (categoriaGrupoParaListar.CodigoGrupo <= 0 || categoriaGrupoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaGrupoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasGruposDTO> listaCategoriaGrupo = await client.PostAsync<CategoriasGruposDTO, List<CategoriasGruposDTO>>("Categorias/ListarCategoriasDeUnGrupoPorIdioma", categoriaGrupoParaListar);

            return listaCategoriaGrupo;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaGrupo(CategoriasGruposDTO categoriaGrupoParaModificar)
        {
            if (categoriaGrupoParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoriaGrupo si categoriaGrupoParaModificar es nula!.");
            if (categoriaGrupoParaModificar.Consecutivo <= 0 || categoriaGrupoParaModificar.CodigoCategoria <= 0)
            {
                throw new ArgumentException("categoriaGrupoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaGrupo = await client.PostAsync<CategoriasGruposDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaGrupo", categoriaGrupoParaModificar);

            return wrapperModificarCategoriaGrupo;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaGrupo(CategoriasGruposDTO categoriaGrupoParaBorrar)
        {
            if (categoriaGrupoParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una categoriaGrupo si categoriaGrupoParaBorrar es nula!.");
            if (categoriaGrupoParaBorrar.Consecutivo <= 0) throw new ArgumentException("categoriaGrupoParaBorrar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaGrupo = await client.PostAsync<CategoriasGruposDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaGrupo", categoriaGrupoParaBorrar);

            return wrapperEliminarCategoriaGrupo;
        }


        #endregion


        #region Metodos CategoriasRepresentantes


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaRepresentante(List<CategoriasRepresentantesDTO> categoriaRepresentanteParaCrear)
        {
            if (categoriaRepresentanteParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaRepresentante si categoriaRepresentanteParaCrear es nula!.");
            if (categoriaRepresentanteParaCrear.Count <= 0 || !categoriaRepresentanteParaCrear.TrueForAll(x => x.CodigoCategoria > 0 && x.CodigoRepresentante > 0))
            {
                throw new ArgumentException("categoriaRepresentanteParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearListaCategoriaRepresentante = await client.PostAsync<List<CategoriasRepresentantesDTO>, WrapperSimpleTypesDTO>("Categorias/CrearListaCategoriaRepresentante", categoriaRepresentanteParaCrear);

            return wrapperCrearListaCategoriaRepresentante;
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaRepresentante(CategoriasRepresentantesDTO categoriaRepresentanteParaCrear)
        {
            if (categoriaRepresentanteParaCrear == null) throw new ArgumentNullException("No puedes crear una categoriaRepresentante si categoriaRepresentanteParaCrear es nula!.");
            if (categoriaRepresentanteParaCrear.CodigoCategoria > 0 || categoriaRepresentanteParaCrear.CodigoRepresentante > 0)
            {
                throw new ArgumentException("categoriaRepresentanteParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCategoriaRepresentante = await client.PostAsync<CategoriasRepresentantesDTO, WrapperSimpleTypesDTO>("Categorias/CrearCategoriaRepresentante", categoriaRepresentanteParaCrear);

            return wrapperCrearCategoriaRepresentante;
        }

        public async Task<CategoriasRepresentantesDTO> BuscarCategoriaRepresentantePorConsecutivoAndIdioma(CategoriasRepresentantesDTO categoriaRepresentantesParaBuscar)
        {
            if (categoriaRepresentantesParaBuscar == null) throw new ArgumentNullException("No puedes buscar una categoriaRepresentante si categoriaRepresentantesParaBuscar es nulo!.");
            if (categoriaRepresentantesParaBuscar.Consecutivo <= 0 || categoriaRepresentantesParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaRepresentantesParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            CategoriasRepresentantesDTO categoriaEventoBuscada = await client.PostAsync("Categorias/BuscarCategoriaRepresentantePorConsecutivoAndIdioma", categoriaRepresentantesParaBuscar);

            return categoriaEventoBuscada;
        }

        public async Task<List<CategoriasRepresentantesDTO>> ListarCategoriasDeUnRepresentante(CategoriasRepresentantesDTO categoriaRepresentanteParaListar)
        {
            if (categoriaRepresentanteParaListar == null) throw new ArgumentNullException("No puedes buscar una categoriaRepresentante si categoriaRepresentanteParaListar es nulo!.");
            if (categoriaRepresentanteParaListar.CodigoRepresentante <= 0 || categoriaRepresentanteParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("categoriaRepresentanteParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CategoriasRepresentantesDTO> listaCategoriasRepresentantes = await client.PostAsync<CategoriasRepresentantesDTO, List<CategoriasRepresentantesDTO>>("Categorias/ListarCategoriasDeUnRepresentante", categoriaRepresentanteParaListar);

            return listaCategoriasRepresentantes;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaRepresentante(CategoriasRepresentantesDTO categoriaReprensentateParaModificar)
        {
            if (categoriaReprensentateParaModificar == null) throw new ArgumentNullException("No puedes modificar una categoriaRepresentante si categoriaReprensentateParaModificar es nula!.");
            if (categoriaReprensentateParaModificar.Consecutivo <= 0 || categoriaReprensentateParaModificar.CodigoCategoria <= 0)
            {
                throw new ArgumentException("categoriaReprensentateParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCategoriaRepresentante = await client.PostAsync<CategoriasRepresentantesDTO, WrapperSimpleTypesDTO>("Categorias/ModificarCategoriaRepresentante", categoriaReprensentateParaModificar);

            return wrapperModificarCategoriaRepresentante;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaRepresentante(CategoriasRepresentantesDTO categoriaRepresanteParaBorrar)
        {
            if (categoriaRepresanteParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una categoriaRepresentante si categoriaRepresanteParaBorrar es nula!.");
            if (categoriaRepresanteParaBorrar.Consecutivo <= 0) throw new ArgumentException("categoriaRepresanteParaBorrar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCategoriaRepresentante = await client.PostAsync<CategoriasRepresentantesDTO, WrapperSimpleTypesDTO>("Categorias/EliminarCategoriaRepresentante", categoriaRepresanteParaBorrar);

            return wrapperEliminarCategoriaRepresentante;
        }


        #endregion


    }
}