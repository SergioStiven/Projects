using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Services
{
    public class HabilidadesServices : BaseService
    {


        #region Metodos Habilidades


        public async Task<WrapperSimpleTypesDTO> CrearHabilidad(HabilidadesDTO habilidadParaCrear)
        {
            if (habilidadParaCrear == null || habilidadParaCrear.HabilidadesContenidos == null) throw new ArgumentNullException("No puedes crear una habilidad si habilidadParaCrear es nula!.");
            if (habilidadParaCrear.CodigoCategoria <= 0 || habilidadParaCrear.TipoHabilidad == TipoHabilidad.SinTipoHabilidad 
                || habilidadParaCrear.HabilidadesContenidos.Count <= 0
                || !habilidadParaCrear.HabilidadesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0))
            {
                throw new ArgumentException("habilidadParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearHabilidad = await client.PostAsync<HabilidadesDTO, WrapperSimpleTypesDTO>("Habilidades/CrearHabilidad", habilidadParaCrear);

            return wrapperCrearHabilidad;
        }

        public async Task<HabilidadesDTO> BuscarHabilidad(HabilidadesDTO habilidadParaBuscar)
        {
            if (habilidadParaBuscar == null) throw new ArgumentNullException("No puedes buscar una habilidad si habilidadParaBuscar es nulo!.");
            if (habilidadParaBuscar.Consecutivo <= 0) throw new ArgumentException("habilidadParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            HabilidadesDTO habilidadBuscada = await client.PostAsync("Habilidades/BuscarHabilidad", habilidadParaBuscar);

            return habilidadBuscada;
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorIdioma(HabilidadesDTO habilidadParaListar)
        {
            if (habilidadParaListar == null) throw new ArgumentNullException("No puedes listar una habilidad si habilidadParaListar es nulo!.");
            if (habilidadParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("habilidadParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<HabilidadesDTO> listaHabilidades = await client.PostAsync<HabilidadesDTO,List<HabilidadesDTO>>("Habilidades/ListarHabilidadesPorIdioma", habilidadParaListar);

            return listaHabilidades;
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorCodigoCategoriaAndIdioma(HabilidadesDTO habilidadParaListar)
        {
            if (habilidadParaListar == null)  throw new ArgumentNullException("habilidadParaListar vacia y/o invalida!.");
            else if (habilidadParaListar.IdiomaBase == Idioma.SinIdioma || habilidadParaListar.CodigoCategoria <= 0)
            {
                throw new ArgumentException("habilidadParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<HabilidadesDTO> listaHabilidadesPorCategoria = await client.PostAsync<HabilidadesDTO, List<HabilidadesDTO>>("Habilidades/ListarHabilidadesPorCodigoCategoriaAndIdioma", habilidadParaListar);

            return listaHabilidadesPorCategoria;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarHabilidad(HabilidadesDTO habilidadParaModificar)
        {
            if (habilidadParaModificar == null) throw new ArgumentNullException("habilidadParaModificar vacia y/o invalida!.");
            if (habilidadParaModificar.Consecutivo <= 0 || habilidadParaModificar.TipoHabilidad == TipoHabilidad.SinTipoHabilidad)
            {
                throw new ArgumentException("habilidadParaModificar vacio y/o invalido!.");
            }
            if (habilidadParaModificar.HabilidadesContenidos != null && !habilidadParaModificar.HabilidadesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion)))
            {
                throw new ArgumentException("Habilidad contenido para modificar descripcion vacia y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarHabilidad = await client.PostAsync<HabilidadesDTO, WrapperSimpleTypesDTO>("Habilidades/ModificarHabilidad", habilidadParaModificar);

            return wrapperModificarHabilidad;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidad(HabilidadesDTO habilidadParaEliminar)
        {
            if (habilidadParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una habilidad si habilidadParaEliminar es nula!.");
            if (habilidadParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("habilidadParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarHabilidad = await client.PostAsync<HabilidadesDTO, WrapperSimpleTypesDTO>("Habilidades/EliminarHabilidad", habilidadParaEliminar);

            return wrapperEliminarHabilidad;
        }


        #endregion


        #region Metodos HabilidadesCandidatos


        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesCandidato(List<HabilidadesCandidatosDTO> habilidadCandidatoParaCrear)
        {
            if (habilidadCandidatoParaCrear == null) throw new ArgumentNullException("No puedes crear una HabilidadCandidato si habilidadCandidatoParaCrear es nula!.");
            if (habilidadCandidatoParaCrear.Count <= 0
                || !habilidadCandidatoParaCrear.All(x => x.CodigoCategoriaCandidato > 0 && x.CodigoHabilidad > 0 && x.NumeroEstrellas >= 0 && x.NumeroEstrellas <= 5))
            {
                throw new ArgumentException("habilidadCandidatoParaCrear para crear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearHabilidadesCandidato = await client.PostAsync<List<HabilidadesCandidatosDTO>, WrapperSimpleTypesDTO>("Habilidades/CrearHabilidadesCandidato", habilidadCandidatoParaCrear);

            return wrapperCrearHabilidadesCandidato;
        }

        public async Task<HabilidadesCandidatosDTO> BuscarHabilidadCandidatoPorIdioma(HabilidadesCandidatosDTO habilidadCandidatoParaBuscar)
        {
            if (habilidadCandidatoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una habilidadCandidato si habilidadCandidatoParaBuscar es nulo!.");
            if (habilidadCandidatoParaBuscar.Consecutivo <= 0 || habilidadCandidatoParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("habilidadCandidatoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            HabilidadesCandidatosDTO habilidadCandidatoBuscada = await client.PostAsync("Habilidades/BuscarHabilidadCandidatoPorIdioma", habilidadCandidatoParaBuscar);
            
            return habilidadCandidatoBuscada;
        }

        public async Task<List<HabilidadesCandidatosDTO>> ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(HabilidadesCandidatosDTO habilidadCandidatoParaListar)
        {
            if (habilidadCandidatoParaListar == null) throw new ArgumentNullException("No puedes listar las habilidadesCandidato si el habilidadCandidatoParaListar es nulo!.");
            if (habilidadCandidatoParaListar.CodigoCategoriaCandidato <= 0 || habilidadCandidatoParaListar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentNullException("habilidadCandidatoParaListar para listar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient(); 

            List<HabilidadesCandidatosDTO> listaHabilidadesCandidato = await client.PostAsync<HabilidadesCandidatosDTO, List<HabilidadesCandidatosDTO>>("Habilidades/ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma", habilidadCandidatoParaListar);

            return listaHabilidadesCandidato;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidadCandidato(HabilidadesCandidatosDTO habilidadCandidatoParaBorrar)
        {
            if (habilidadCandidatoParaBorrar == null) throw new ArgumentNullException("No puedes eliminar una habilidadCandidato si habilidadCandidatoParaBorrar es nulo!.");
            if (habilidadCandidatoParaBorrar.Consecutivo <= 0)
            {
                throw new ArgumentException("habilidadCandidatoParaBorrar para buscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarHabilidadCandidato = await client.PostAsync<HabilidadesCandidatosDTO, WrapperSimpleTypesDTO>("Habilidades/EliminarHabilidadCandidato", habilidadCandidatoParaBorrar);

            return wrapperEliminarHabilidadCandidato;
        }


        #endregion


        #region Metodos HabilidadesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesContenidos(List<HabilidadesContenidosDTO> habilidadesContenidosParaCrear)
        {
            if (habilidadesContenidosParaCrear == null) throw new ArgumentNullException("No puedes crear una habilidadContenido si habilidadesContenidosParaCrear es nula!.");
            if (habilidadesContenidosParaCrear.Count <= 0
                || !habilidadesContenidosParaCrear.All(x => x.CodigoIdioma > 0 && x.CodigoHabilidad > 0 && !string.IsNullOrWhiteSpace(x.Descripcion)))
            {
                throw new ArgumentException("habilidadesContenidosParaCrear para crear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearHabilidadesContenido = await client.PostAsync<List<HabilidadesContenidosDTO>, WrapperSimpleTypesDTO>("Habilidades/CrearHabilidadesContenidos", habilidadesContenidosParaCrear);

            return wrapperCrearHabilidadesContenido;
        }

        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesContenidosIndividual(HabilidadesContenidosDTO habilidadesContenidosParaCrear)
        {
            if (habilidadesContenidosParaCrear == null) throw new ArgumentNullException("No puedes crear una habilidadContenido si habilidadesContenidosParaCrear es nula!.");
            if (habilidadesContenidosParaCrear.CodigoHabilidad > 0 || !string.IsNullOrWhiteSpace(habilidadesContenidosParaCrear.Descripcion)
                || habilidadesContenidosParaCrear.CodigoIdioma > 0)
            {
                throw new ArgumentException("habilidadesContenidosParaCrear para crear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearHabilidadesContenidosIndividual = await client.PostAsync<HabilidadesContenidosDTO, WrapperSimpleTypesDTO>("Habilidades/CrearHabilidadesContenidosIndividual", habilidadesContenidosParaCrear);

            return wrapperCrearHabilidadesContenidosIndividual;
        }

        public async Task<HabilidadesContenidosDTO> BuscarHabilidadContenido(HabilidadesContenidosDTO habilidadContenidoParaBuscar)
        {
            if (habilidadContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una habilidadContenido si habilidadContenidoParaBuscar es nulo!.");
            if (habilidadContenidoParaBuscar.Consecutivo <= 0)
            {
                throw new ArgumentException("habilidadContenidoParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            HabilidadesContenidosDTO habilidadesContenidoBuscada = await client.PostAsync("Habilidades/BuscarHabilidadContenido", habilidadContenidoParaBuscar);

            return habilidadesContenidoBuscada;
        }

        public async Task<List<HabilidadesContenidosDTO>> ListarContenidoDeUnaHabilidad(HabilidadesContenidosDTO habilidadContenidoParaListar)
        {
            if (habilidadContenidoParaListar == null) throw new ArgumentNullException("No puedes listar las habilidadContenido si el habilidadContenidoParaListar es nulo!.");
            if (habilidadContenidoParaListar.CodigoHabilidad <= 0)
            {
                throw new ArgumentNullException("habilidadContenidoParaListar para listar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<HabilidadesContenidosDTO> listaHabilidadesContenidos = await client.PostAsync<HabilidadesContenidosDTO, List<HabilidadesContenidosDTO>>("Habilidades/ListarContenidoDeUnaHabilidad", habilidadContenidoParaListar);

            return listaHabilidadesContenidos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarHabilidadContenido(HabilidadesContenidosDTO habilidadContenidoParaModificar)
        {
            if (habilidadContenidoParaModificar == null) throw new ArgumentNullException("No puedes eliminar una habilidadContenido si habilidadContenidoParaModificar es nulo!.");
            if (habilidadContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(habilidadContenidoParaModificar.Descripcion))
            {
                throw new ArgumentException("habilidadContenidoParaModificar para buscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarHabilidadContenido = await client.PostAsync<HabilidadesContenidosDTO, WrapperSimpleTypesDTO>("Habilidades/ModificarHabilidadContenido", habilidadContenidoParaModificar);

            return wrapperModificarHabilidadContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidadContenido(HabilidadesContenidosDTO habilidadContenidoParaEliminar)
        {
            if (habilidadContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una habilidadContenido si habilidadContenidoParaEliminar es nulo!.");
            if (habilidadContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("habilidadContenidoParaEliminar para buscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarHabilidadContenido = await client.PostAsync<HabilidadesContenidosDTO, WrapperSimpleTypesDTO>("Habilidades/EliminarHabilidadContenido", habilidadContenidoParaEliminar);

            return wrapperEliminarHabilidadContenido;
        }


        #endregion


    }
}
