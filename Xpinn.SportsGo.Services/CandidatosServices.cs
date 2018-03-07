using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using System.IO;
using System.Linq;

namespace Xpinn.SportsGo.Services
{
    public class CandidatosServices : BaseService
    {


        #region Metodos Candidatos


        public async Task<WrapperSimpleTypesDTO> CrearCandidato(CandidatosDTO candidatoParaCrear)
        {
            if (candidatoParaCrear == null || candidatoParaCrear.Personas == null || candidatoParaCrear.Personas.Usuarios == null || candidatoParaCrear.CategoriasCandidatos == null)
            {
                throw new ArgumentNullException("No puedes crear un candidato si candidatoParaCrear o la persona del candidato es nulo!.");
            }
            if (candidatoParaCrear.CodigoGenero <= 0
                || candidatoParaCrear.Estatura <= 0 || candidatoParaCrear.Peso <= 0 || candidatoParaCrear.FechaNacimiento == DateTime.MinValue)
            {
                throw new ArgumentException("candidatoParaCrear vacia y/o invalida!.");
            }
            else if (string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Nombres) || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Apellidos) || candidatoParaCrear.Personas.CodigoPais <= 0 
                    || candidatoParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil || candidatoParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.CiudadResidencia))
            {
                throw new ArgumentException("Persona de candidatoParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(candidatoParaCrear.Personas.Usuarios.Email))
            {
                throw new ArgumentException("Usuario de candidatoParaCrear vacio y/o invalido!.");
            }
            else if (candidatoParaCrear.CategoriasCandidatos.Count <= 0 
                     || !candidatoParaCrear.CategoriasCandidatos.All(x => x.CodigoCategoria > 0 && (x.HabilidadesCandidatos.Count > 0 && x.HabilidadesCandidatos.All(y => y.CodigoHabilidad > 0 && y.NumeroEstrellas >= 0 && y.NumeroEstrellas <= 5))))
            {
                throw new ArgumentException("Categorias o Habilidades de candidatoParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCandidato = await client.PostAsync<CandidatosDTO, WrapperSimpleTypesDTO>("Candidatos/CrearCandidato", candidatoParaCrear);

            return wrapperCrearCandidato;
        }

        public async Task<CandidatosDTO> BuscarCandidatoPorCodigoPersona(CandidatosDTO candidatoParaBuscar)
        {
            if (candidatoParaBuscar == null || candidatoParaBuscar.Personas == null) throw new ArgumentNullException("No puedes buscar un candidato si candidatoParaBuscar es nulo!.");
            if (candidatoParaBuscar.Personas.Consecutivo <= 0) throw new ArgumentException("candidatoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CandidatosDTO informacionCandidato = await client.PostAsync("Candidatos/BuscarCandidatoPorCodigoPersona", candidatoParaBuscar);

            return informacionCandidato;
        }

        public async Task<CandidatosDTO> BuscarCandidatoPorCodigoCandidato(CandidatosDTO candidatoParaBuscar)
        {
            if (candidatoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un candidato si candidatoParaBuscar es nulo!.");
            if (candidatoParaBuscar.Consecutivo <= 0) throw new ArgumentException("candidatoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CandidatosDTO informacionCandidato = await client.PostAsync("Candidatos/BuscarCandidatoPorCodigoCandidato", candidatoParaBuscar);

            return informacionCandidato;
        }

        public async Task<List<CandidatosDTO>> ListarCandidatos(BuscadorDTO buscador)
        {
            if (buscador == null) throw new ArgumentNullException("No puedes listar los candidatos si buscador es nulo!.");
            if (buscador.SkipIndexBase < 0 || buscador.TakeIndexBase <= 0 || buscador.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("buscador vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CandidatosDTO> listaInformacionCandidatos = await client.PostAsync<BuscadorDTO, List<CandidatosDTO>>("Candidatos/ListarCandidatos", buscador);

            return listaInformacionCandidatos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionCandidato(CandidatosDTO candidatoParaModificar)
        {
            if (candidatoParaModificar == null) throw new ArgumentNullException("No puedes modificar un candidato si categoriaParaModificar es nulo!.");
            if (candidatoParaModificar.Consecutivo <= 0
                || candidatoParaModificar.CodigoGenero <= 0 || candidatoParaModificar.Estatura <= 0 || candidatoParaModificar.Peso <= 0
                || candidatoParaModificar.FechaNacimiento == DateTime.MinValue)
            {
                throw new ArgumentException("candidatoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCandidato = await client.PostAsync<CandidatosDTO, WrapperSimpleTypesDTO>("Candidatos/ModificarInformacionCandidato", candidatoParaModificar);

            return wrapperModificarCandidato;
        }

        #endregion


        #region Metodos CandidatosVideos


        public async Task<WrapperSimpleTypesDTO> CrearCandidatoVideo(CandidatosVideosDTO candidatoVideoParaCrear)
        {
            if (candidatoVideoParaCrear == null) throw new ArgumentNullException("No puedes crear un candidatoVideo si candidatoVideoParaCrear es nulo!.");
            if (string.IsNullOrWhiteSpace(candidatoVideoParaCrear.Titulo) || candidatoVideoParaCrear.CodigoCandidato <= 0 || candidatoVideoParaCrear.CodigoArchivo <= 0)
            {
                throw new ArgumentException("candidatoVideoParaCrear vacia y/o invalida!.");
            }
            else if (candidatoVideoParaCrear.Archivos != null)
            {
                throw new ArgumentException("Usa CrearArchivoStream en ArchivosService para crear el archivo o mataras la memoria del servidor!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCandidatoVideo = await client.PostAsync<CandidatosVideosDTO, WrapperSimpleTypesDTO>("Candidatos/CrearCandidatoVideo", candidatoVideoParaCrear);

            return wrapperCrearCandidatoVideo;
        }

        public async Task<CandidatosVideosDTO> BuscarCandidatoVideo(CandidatosVideosDTO candidatoVideoParaBuscar)
        {
            if (candidatoVideoParaBuscar == null) throw new ArgumentNullException("No puedes buscar un candidatoVideo si candidatoVideoParaBuscar es nulo!.");
            if (candidatoVideoParaBuscar.Consecutivo <= 0) throw new ArgumentException("candidatoVideoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CandidatosVideosDTO candidatoVideoBuscado = await client.PostAsync("Candidatos/BuscarCandidatoVideo", candidatoVideoParaBuscar);

            return candidatoVideoBuscado;
        }

        public async Task<List<CandidatosVideosDTO>> ListarCandidatosVideosDeUnCandidato(CandidatosVideosDTO candidatoVideoParaListar)
        {
            if (candidatoVideoParaListar == null) throw new ArgumentNullException("No puedes buscar un candidatoVideo si candidatoVideoParaListar es nulo!.");
            if (candidatoVideoParaListar.CodigoCandidato <= 0 || candidatoVideoParaListar.SkipIndexBase < 0 || candidatoVideoParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("candidatoVideoParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<CandidatosVideosDTO> listaInformacionCandidatos = await client.PostAsync<CandidatosVideosDTO, List<CandidatosVideosDTO>>("Candidatos/ListarCandidatosVideosDeUnCandidato", candidatoVideoParaListar);

            return listaInformacionCandidatos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCandidatoVideo(CandidatosVideosDTO candidatoVideoParaModificar)
        {
            if (candidatoVideoParaModificar == null) throw new ArgumentNullException("No puedes modificar un candidatoVideo si candidatoVideoParaModificar es nulo!.");
            if (candidatoVideoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(candidatoVideoParaModificar.Titulo))
            {
                throw new ArgumentException("candidatoVideoParaModificar vacio y/o invalido!.");
            }
            if (candidatoVideoParaModificar.ArchivoContenido != null)
            {
                throw new ArgumentException("El archivo debe modificarse por su controller!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCandidatoVideo = await client.PostAsync<CandidatosVideosDTO, WrapperSimpleTypesDTO>("Candidatos/ModificarCandidatoVideo", candidatoVideoParaModificar);

            return wrapperModificarCandidatoVideo;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCandidatoVideo(CandidatosVideosDTO candidatoVideoParaEliminar)
        {
            if (candidatoVideoParaEliminar == null) throw new ArgumentNullException("No puedes modificar un candidatoVideo si candidatoVideoParaEliminar es nulo!.");
            if (candidatoVideoParaEliminar.Consecutivo <= 0 || candidatoVideoParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("candidatoVideoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCandidatoVideo = await client.PostAsync<CandidatosVideosDTO, WrapperSimpleTypesDTO>("Candidatos/EliminarCandidatoVideo", candidatoVideoParaEliminar);

            return wrapperEliminarCandidatoVideo;
        }


        #endregion


        #region Metodos Responsables


        public async Task<WrapperSimpleTypesDTO> CrearCandidatoResponsable(CandidatosResponsablesDTO candidatoResponsableParaCrear)
        {
            if (candidatoResponsableParaCrear == null) throw new ArgumentNullException("No puedes crear un candidatoResponsable si candidatoResponsableParaCrear es nulo!.");
            if (string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.TelefonoMovil)
                || string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaCrear.Nombres))
            {
                throw new ArgumentException("candidatoResponsableParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearCandidatoResponsable = await client.PostAsync<CandidatosResponsablesDTO, WrapperSimpleTypesDTO>("Candidatos/CrearCandidatoResponsable", candidatoResponsableParaCrear);

            return wrapperCrearCandidatoResponsable;
        }

        public async Task<CandidatosResponsablesDTO> BuscarCandidatoResponsable(CandidatosResponsablesDTO candidatoResponsableParaBuscar)
        {
            if (candidatoResponsableParaBuscar == null) throw new ArgumentNullException("No puedes buscar un candidatoResponsable si candidatoResponsableParaBuscar es nulo!.");
            if (candidatoResponsableParaBuscar.Consecutivo <= 0) throw new ArgumentException("candidatoResponsableParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            CandidatosResponsablesDTO candidatoResponsableBuscado = await client.PostAsync("Candidatos/BuscarCandidatoResponsable", candidatoResponsableParaBuscar);

            return candidatoResponsableBuscado;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarCandidatoResponsable(CandidatosResponsablesDTO candidatoResponsableParaAsignar)
        {
            if (candidatoResponsableParaAsignar == null) throw new ArgumentNullException("No puedes asignar un candidatoResponsable si candidatoResponsableParaAsignar es nulo!.");
            if (string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.TelefonoMovil) || candidatoResponsableParaAsignar.CodigoCandidato <= 0
                || string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaAsignar.Nombres))
            {
                throw new ArgumentException("candidatoResponsableParaAsignar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarCandidatoResponsable = await client.PostAsync<CandidatosResponsablesDTO, WrapperSimpleTypesDTO>("Candidatos/AsignarCandidatoResponsable", candidatoResponsableParaAsignar);

            return wrapperAsignarCandidatoResponsable;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCandidatoResponsable(CandidatosResponsablesDTO candidatoResponsableParaModificar)
        {
            if (candidatoResponsableParaModificar == null) throw new ArgumentNullException("No puedes modificar un candidatoResponsable si candidatoResponsableParaModificar es nulo!.");
            if (candidatoResponsableParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.TelefonoMovil)
                || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Email) || string.IsNullOrWhiteSpace(candidatoResponsableParaModificar.Nombres))
            {
                throw new ArgumentException("candidatoResponsableParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarCandidatoResponsable = await client.PostAsync<CandidatosResponsablesDTO, WrapperSimpleTypesDTO>("Candidatos/ModificarCandidatoResponsable", candidatoResponsableParaModificar);

            return wrapperModificarCandidatoResponsable;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCandidatoResponsable(CandidatosDTO candidatoResponsableParaBorrar)
        {
            if (candidatoResponsableParaBorrar == null) throw new ArgumentNullException("No puedes modificar un candidatoResponsable si candidatoResponsableParaBorrar es nula!.");
            if (candidatoResponsableParaBorrar.Consecutivo <= 0 || candidatoResponsableParaBorrar.CodigoResponsable <= 0)
            {
                throw new ArgumentException("candidatoResponsableParaBorrar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarCandidatoResponsable = await client.PostAsync<CandidatosDTO, WrapperSimpleTypesDTO>("Candidatos/EliminarCandidatoVideo", candidatoResponsableParaBorrar);

            return wrapperEliminarCandidatoResponsable;
        }


        #endregion


    }
}
