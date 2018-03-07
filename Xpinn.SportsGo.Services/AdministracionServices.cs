using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class AdministracionServices : BaseService
    {

        #region Metodos Administrar Usuario


        public async Task<WrapperSimpleTypesDTO> ModificarUsuario(UsuariosDTO usuarioParamodificar)
        {
            if (usuarioParamodificar == null || usuarioParamodificar.PlanesUsuarios == null) throw new ArgumentNullException("No puedes modificar un usuario si usuarioParamodificar es nulo!.");
            if (usuarioParamodificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(usuarioParamodificar.Usuario) || string.IsNullOrWhiteSpace(usuarioParamodificar.Email) 
                || usuarioParamodificar.PlanesUsuarios.Consecutivo <= 0 || usuarioParamodificar.PlanesUsuarios.CodigoPlanDeseado <= 0)
            {
                throw new ArgumentException("usuarioParamodificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarUsuario = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Administracion/ModificarUsuario", usuarioParamodificar);

            return wrapperModificarUsuario;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarUsuario(UsuariosDTO usuarioParaEliminar)
        {
            if (usuarioParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un usuario si usuarioParaEliminar es nulo!.");
            if (usuarioParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("usuarioParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarUsuario = await client.PostAsync<UsuariosDTO, WrapperSimpleTypesDTO>("Administracion/EliminarUsuario", usuarioParaEliminar);

            return wrapperEliminarUsuario;
        }


        #endregion


        #region Metodos TerminosCondiciones


        public async Task<WrapperSimpleTypesDTO> AsignarTerminosCondiciones(TerminosCondicionesDTO terminosCondicionesParaAsignar)
        {
            if (terminosCondicionesParaAsignar == null) throw new ArgumentNullException("No puedes asignar los terminos condiciones si terminosCondicionesParaAsignar es nula!.");
            if (string.IsNullOrWhiteSpace(terminosCondicionesParaAsignar.Texto) || terminosCondicionesParaAsignar.IdiomaDeLosTerminos == Idioma.SinIdioma)
            {
                throw new ArgumentException("terminosCondicionesParaAsignar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarTerminosCondiciones = await client.PostAsync<TerminosCondicionesDTO, WrapperSimpleTypesDTO>("Administracion/AsignarTerminosCondiciones", terminosCondicionesParaAsignar);

            return wrapperAsignarTerminosCondiciones;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarTerminosCondicionesLista(List<TerminosCondicionesDTO> terminosCondicionesParaAsignar)
        {
            if (terminosCondicionesParaAsignar == null) throw new ArgumentNullException("No puedes asignar los terminos condiciones si terminosCondicionesParaAsignar es nula!.");
            if (terminosCondicionesParaAsignar.Count <= 0 || !terminosCondicionesParaAsignar.TrueForAll(x => !string.IsNullOrWhiteSpace(x.Texto) && x.IdiomaDeLosTerminos != Idioma.SinIdioma))
            {
                throw new ArgumentException("terminosCondicionesParaAsignar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarTerminosCondicionesLista = await client.PostAsync<List<TerminosCondicionesDTO>, WrapperSimpleTypesDTO>("Administracion/AsignarTerminosCondicionesLista", terminosCondicionesParaAsignar);

            return wrapperAsignarTerminosCondicionesLista;
        }

        public async Task<TerminosCondicionesDTO> BuscarTerminosCondiciones(TerminosCondicionesDTO terminosCondicionesParaBuscar)
        {
            if (terminosCondicionesParaBuscar == null) throw new ArgumentNullException("No puedes buscar los terminos condiciones si terminosCondicionesParaBuscar es nula!.");
            if (terminosCondicionesParaBuscar.IdiomaDeLosTerminos == Idioma.SinIdioma)
            {
                throw new ArgumentException("terminosCondicionesParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            TerminosCondicionesDTO terminosCondicionesBuscados = await client.PostAsync("Administracion/BuscarTerminosCondiciones", terminosCondicionesParaBuscar);

            return terminosCondicionesBuscados;
        }

        public async Task<List<TerminosCondicionesDTO>> ListarTerminosCondiciones()
        {
            IHttpClient client = ConfigurarHttpClient();

            List<TerminosCondicionesDTO> terminosCondicionesBuscados = await client.PostAsync<List<TerminosCondicionesDTO>>("Administracion/ListarTerminosCondiciones");

            return terminosCondicionesBuscados;
        }


        #endregion


        #region Metodos ImagenesPerfilAdministradores


        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfilAdministrador(int codigoUsuario, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes crear un archivo si streamSource es nulo!.");
            if (codigoUsuario <= 0) throw new ArgumentException("No puedes crear un archivo si codigoUsuario es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarImagenPerfilAdministrador = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Administracion/AsignarImagenPerfilAdministrador/" + codigoUsuario, streamSource);

            return wrapperAsignarImagenPerfilAdministrador;
        }


        #endregion


        #region Metodos Paises


        public async Task<WrapperSimpleTypesDTO> CrearPais(PaisesDTO paisParaCrear)
        {
            if (paisParaCrear == null || paisParaCrear.PaisesContenidos == null) throw new ArgumentNullException("No puedes crear un pais si paisParaCrear o su contenido es nula!.");
            if (paisParaCrear.CodigoMoneda <= 0 || paisParaCrear.CodigoIdioma <= 0 || paisParaCrear.PaisesContenidos.Count <= 0 || !paisParaCrear.PaisesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0)
                || paisParaCrear.Archivos == null || paisParaCrear.Archivos.ArchivoContenido == null)
            {
                throw new ArgumentException("paisParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearPais = await client.PostAsync<PaisesDTO, WrapperSimpleTypesDTO>("Administracion/CrearPais", paisParaCrear);

            return wrapperCrearPais;
        }

        public async Task<PaisesDTO> BuscarPais(PaisesDTO paisParaBuscar)
        {
            if (paisParaBuscar == null) throw new ArgumentNullException("No puedes buscar un pais si paisParaBuscar es nulo!.");
            if (paisParaBuscar.Consecutivo <= 0) throw new ArgumentException("paisParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            PaisesDTO paisBuscado = await client.PostAsync("Administracion/BuscarPais", paisParaBuscar);

            return paisBuscado;
        }

        public async Task<List<PaisesDTO>> ListarPaisesPorIdioma(PaisesDTO paisParaListar)
        {
            if (paisParaListar == null) throw new ArgumentNullException("No puedes listar un pais si paisParaListar es nulo!.");
            if (paisParaListar.IdiomaBase == Idioma.SinIdioma) throw new ArgumentException("paisParaListar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            List<PaisesDTO> listaPaises = await client.PostAsync<PaisesDTO, List<PaisesDTO>>("Administracion/ListarPaisesPorIdioma", paisParaListar);

            return listaPaises;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoPais(PaisesDTO paisParaModificar)
        {
            if (paisParaModificar == null) throw new ArgumentNullException("No puedes modificar el archivo del pais si paisParaModificar es nulo!.");
            if (paisParaModificar.CodigoArchivo <= 0 || paisParaModificar.ArchivoContenido == null)
            {
                throw new ArgumentException("paisParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarArchivoPais = await client.PostAsync<PaisesDTO, WrapperSimpleTypesDTO>("Administracion/ModificarArchivoPais", paisParaModificar);

            return wrapperModificarArchivoPais;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPais(PaisesDTO paisParaModificar)
        {
            if (paisParaModificar == null) throw new ArgumentNullException("No puedes modificar un pais si paisParaModificar es nulo!.");
            if (paisParaModificar.Consecutivo <= 0 || paisParaModificar.CodigoIdioma <= 0 || paisParaModificar.CodigoMoneda <= 0
                || paisParaModificar.PaisesContenidos == null || paisParaModificar.PaisesContenidos.Count <= 0
                || !paisParaModificar.PaisesContenidos.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.Consecutivo > 0))
            {
                throw new ArgumentException("paisParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarPais = await client.PostAsync<PaisesDTO, WrapperSimpleTypesDTO>("Administracion/ModificarPais", paisParaModificar);

            return wrapperModificarPais;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPais(PaisesDTO paisParaEliminar)
        {
            if (paisParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un pais si paisParaEliminar es nulo!.");
            if (paisParaEliminar.Consecutivo <= 0 || paisParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("paisParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarPais = await client.PostAsync<PaisesDTO, WrapperSimpleTypesDTO>("Administracion/EliminarPais", paisParaEliminar);

            return wrapperEliminarPais;
        }


        #endregion


        #region Metodos PaisesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearPaisesContenidos(List<PaisesContenidosDTO> paisContenidoParaCrear)
        {
            if (paisContenidoParaCrear == null) throw new ArgumentNullException("No puedes crear un paisContenido si paisContenidoParaCrear o su contenido es nula!.");
            if (paisContenidoParaCrear.Count <= 0 ||
                !paisContenidoParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoPais > 0))
            {
                throw new ArgumentException("paisContenidoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearPaisesContenidos = await client.PostAsync<List<PaisesContenidosDTO>, WrapperSimpleTypesDTO>("Administracion/CrearPaisesContenidos", paisContenidoParaCrear);

            return wrapperCrearPaisesContenidos;
        }

        public async Task<PaisesContenidosDTO> BuscarPaisContenido(PaisesContenidosDTO paisContenidoParaBuscar)
        {
            if (paisContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una paisContenido si paisContenidoParaBuscar es nulo!.");
            if (paisContenidoParaBuscar.Consecutivo <= 0) throw new ArgumentException("paisContenidoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            PaisesContenidosDTO paisContenidoBuscado = await client.PostAsync("Administracion/BuscarPaisContenido", paisContenidoParaBuscar);

            return paisContenidoBuscado;
        }

        public async Task<List<PaisesContenidosDTO>> ListarContenidoDeUnPais(PaisesContenidosDTO paisContenidoParaListar)
        {
            if (paisContenidoParaListar == null) throw new ArgumentNullException("No puedes listar una paisContenido si paisContenidoParaListar es nulo!.");
            if (paisContenidoParaListar.CodigoPais <= 0) throw new ArgumentException("paisContenidoParaListar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            List<PaisesContenidosDTO> listaPaisContenido = await client.PostAsync<PaisesContenidosDTO, List<PaisesContenidosDTO>>("Administracion/ListarContenidoDeUnPais", paisContenidoParaListar);

            return listaPaisContenido;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPaisContenido(PaisesContenidosDTO paisContenidoParaModificar)
        {
            if (paisContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar una paisContenido si paisContenidoParaModificar es nulo!.");
            if (paisContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(paisContenidoParaModificar.Descripcion))
            {
                throw new ArgumentException("paisContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarPaisContenido = await client.PostAsync<PaisesContenidosDTO, WrapperSimpleTypesDTO>("Administracion/ModificarPaisContenido", paisContenidoParaModificar);

            return wrapperModificarPaisContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPaisContenido(PaisesContenidosDTO paisContenidoParaEliminar)
        {
            if (paisContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una paisContenido si paisContenidoParaEliminar es nulo!.");
            if (paisContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("paisContenidoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarPaisContenido = await client.PostAsync<PaisesContenidosDTO, WrapperSimpleTypesDTO>("Administracion/EliminarPaisContenido", paisContenidoParaEliminar);

            return wrapperEliminarPaisContenido;
        }


        #endregion


    }
}
