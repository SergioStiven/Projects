using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Services
{
    public class PlanesServices : BaseService
    {


        #region Metodos Planes


        public async Task<WrapperSimpleTypesDTO> CrearPlan(PlanesDTO planParaCrear)
        {
            if (planParaCrear == null || planParaCrear.Archivos == null) throw new ArgumentNullException("No puedes crear un plan si planParaCrear es nulo!.");
            if (planParaCrear.Precio < 0 || planParaCrear.CodigoPeriodicidad <= 0
                || (planParaCrear.VideosPerfil < 0 || planParaCrear.VideosPerfil > 1)
                || (planParaCrear.ServiciosChat < 0 | planParaCrear.ServiciosChat > 1)
                || (planParaCrear.ConsultaCandidatos < 0 || planParaCrear.ConsultaCandidatos > 1)
                || (planParaCrear.DetalleCandidatos < 0 || planParaCrear.DetalleCandidatos > 1)
                || (planParaCrear.ConsultaGrupos < 0 || planParaCrear.ConsultaGrupos > 1)
                || (planParaCrear.DetalleGrupos < 0 || planParaCrear.DetalleGrupos > 1)
                || (planParaCrear.ConsultaEventos < 0 || planParaCrear.ConsultaEventos > 1)
                || (planParaCrear.CreacionAnuncios < 0 || planParaCrear.CreacionAnuncios > 1)
                || (planParaCrear.EstadisticasAnuncios < 0 || planParaCrear.EstadisticasAnuncios > 1)
                || (planParaCrear.TiempoPermitidoVideo < AppConstants.MinimoSegundos || planParaCrear.TiempoPermitidoVideo > AppConstants.MaximoSegundos)
                || planParaCrear.NumeroCategoriasPermisibles <= 0 || planParaCrear.Archivos.ArchivoContenido == null
                || planParaCrear.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                throw new ArgumentException("planParaCrear vacio y/o invalido!.");
            }
            else if (planParaCrear.TipoPerfil == TipoPerfil.Anunciante && (planParaCrear.NumeroDiasVigenciaAnuncio <= 0 && planParaCrear.NumeroAparicionesAnuncio <= 0))
            {
                throw new ArgumentException("planParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearPlan = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/CrearPlan", planParaCrear);

            return wrapperCrearPlan;
        }

        public async Task<PlanesDTO> BuscarPlan(PlanesDTO planParaBuscar)
        {
            if (planParaBuscar == null) throw new ArgumentNullException("No puedes buscar un plan si planParaBuscar es nulo!.");
            if (planParaBuscar.Consecutivo <= 0 )
            {
                throw new ArgumentException("planParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            PlanesDTO planBuscado = await client.PostAsync("Planes/BuscarPlan", planParaBuscar);

            return planBuscado;
        }

        public async Task<PlanesDTO> BuscarPlanDefaultDeUnPerfil(PlanesDTO planParaBuscar)
        {
            if (planParaBuscar == null) throw new ArgumentNullException("No puedes buscar un plan si planParaBuscar es nulo!.");
            if (planParaBuscar.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                throw new ArgumentException("planParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            PlanesDTO planBuscado = await client.PostAsync("Planes/BuscarPlanDefaultDeUnPerfil", planParaBuscar);

            return planBuscado;
        }

        public async Task<List<PlanesDTO>> ListarPlanesAdministrador(PlanesDTO planParaListar)
        {
            if (planParaListar == null) throw new ArgumentNullException("No puedes listar los planes si planParaListar es nulo!.");
            if (planParaListar.IdiomaBase == Idioma.SinIdioma || planParaListar.SkipIndexBase < 0 || planParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("planParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<PlanesDTO> planBuscado = await client.PostAsync<PlanesDTO, List<PlanesDTO>>("Planes/ListarPlanesAdministrador", planParaListar);

            return planBuscado;
        }


        public async Task<List<PlanesDTO>> ListarPlanesPorIdioma(PlanesDTO planParaListar)
        {
            if (planParaListar == null) throw new ArgumentNullException("No puedes listar los planes si planParaListar es nulo!.");
            if (planParaListar.IdiomaBase == Idioma.SinIdioma || planParaListar.CodigoPaisParaBuscarMoneda <= 0
                || planParaListar.SkipIndexBase < 0 || planParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("planParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<PlanesDTO> planBuscado = await client.PostAsync<PlanesDTO, List<PlanesDTO>>("Planes/ListarPlanesPorIdioma", planParaListar);

            return planBuscado;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPlan(PlanesDTO planParaModificar)
        {
            if (planParaModificar == null) throw new ArgumentNullException("No puedes modificar un plan si planParaModificar es nulo!.");
            if (planParaModificar == null || planParaModificar.Precio < 0 || planParaModificar.CodigoPeriodicidad <= 0 || planParaModificar.Consecutivo <= 0
                || (planParaModificar.VideosPerfil < 0 || planParaModificar.VideosPerfil > 1)
                || (planParaModificar.ServiciosChat < 0 | planParaModificar.ServiciosChat > 1)
                || (planParaModificar.ConsultaCandidatos < 0 || planParaModificar.ConsultaCandidatos > 1)
                || (planParaModificar.DetalleCandidatos < 0 || planParaModificar.DetalleCandidatos > 1)
                || (planParaModificar.ConsultaGrupos < 0 || planParaModificar.ConsultaGrupos > 1)
                || (planParaModificar.DetalleGrupos < 0 || planParaModificar.DetalleGrupos > 1)
                || (planParaModificar.ConsultaEventos < 0 || planParaModificar.ConsultaEventos > 1)
                || (planParaModificar.CreacionAnuncios < 0 || planParaModificar.CreacionAnuncios > 1)
                || (planParaModificar.EstadisticasAnuncios < 0 || planParaModificar.EstadisticasAnuncios > 1)
                || (planParaModificar.TiempoPermitidoVideo < AppConstants.MinimoSegundos || planParaModificar.TiempoPermitidoVideo > AppConstants.MaximoSegundos)
                || planParaModificar.NumeroCategoriasPermisibles <= 0 || planParaModificar.TipoPerfil == TipoPerfil.SinTipoPerfil)
            {
                throw new ArgumentException("planParaModificar vacio y/o invalido!.");
            }
            else if (planParaModificar.TipoPerfil == TipoPerfil.Anunciante && (planParaModificar.NumeroDiasVigenciaAnuncio <= 0 && planParaModificar.NumeroAparicionesAnuncio <= 0))
            {
                throw new ArgumentException("planParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarPlan = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/ModificarPlan", planParaModificar);

            return wrapperModificarPlan;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarPlanDefault(PlanesDTO planParaAsignar)
        {
            if (planParaAsignar == null) throw new ArgumentNullException("No puedes asignar un plan default si planParaAsignar es nulo!.");
            if (planParaAsignar.Consecutivo <= 0)
            {
                throw new ArgumentException("planParaAsignar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarPlanDefault = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/AsignarPlanDefault", planParaAsignar);

            return wrapperAsignarPlanDefault;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoPlan(PlanesDTO planArchivoParaModificar)
        {
            if (planArchivoParaModificar == null) throw new ArgumentNullException("No puedes modificar el archivo del plan si planArchivoParaModificar es nulo!.");
            if (planArchivoParaModificar.CodigoArchivo <= 0 || planArchivoParaModificar.ArchivoContenido == null)
            {
                throw new ArgumentException("planArchivoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarArchivoPlan = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/ModificarArchivoPlan", planArchivoParaModificar);

            return wrapperModificarArchivoPlan;
        }

        public async Task<WrapperSimpleTypesDTO> DesasignarPlanDefault(PlanesDTO planParaDesasignar)
        {
            if (planParaDesasignar == null) throw new ArgumentNullException("No puedes desasignar un plan default si planParaDesasignar es nulo!.");
            if (planParaDesasignar.Consecutivo <= 0)
            {
                throw new ArgumentException("planParaDesasignar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperDesasignarPlanDefault = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/DesasignarPlanDefault", planParaDesasignar);

            return wrapperDesasignarPlanDefault;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPlan(PlanesDTO planParaEliminar)
        {
            if (planParaEliminar == null) throw new ArgumentNullException("No puedes eliminar un plan si planParaEliminar es nulo!.");
            if (planParaEliminar.Consecutivo <= 0 || planParaEliminar.CodigoArchivo <= 0)
            {
                throw new ArgumentException("planParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperPlanParaEliminar = await client.PostAsync<PlanesDTO, WrapperSimpleTypesDTO>("Planes/EliminarPlan", planParaEliminar);

            return wrapperPlanParaEliminar;
        }


        #endregion


        #region Metodos PlanesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearPlanesContenidos(List<PlanesContenidosDTO> planContenidoParaCrear)
        {
            if (planContenidoParaCrear == null) throw new ArgumentNullException("No puedes crear una planContenido si planContenidoParaCrear o su contenido es nula!.");
            if (planContenidoParaCrear.Count <= 0 ||
                !planContenidoParaCrear.All(x => !string.IsNullOrWhiteSpace(x.Descripcion) && x.CodigoIdioma > 0 && x.CodigoPlan > 0))
            {
                throw new ArgumentException("planContenidoParaCrear vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearPlanesContenidos = await client.PostAsync<List<PlanesContenidosDTO>, WrapperSimpleTypesDTO>("Planes/CrearPlanesContenidos", planContenidoParaCrear);

            return wrapperCrearPlanesContenidos;
        }

        public async Task<PlanesContenidosDTO> BuscarPlanContenido(PlanesContenidosDTO planContenidoParaBuscar)
        {
            if (planContenidoParaBuscar == null) throw new ArgumentNullException("No puedes buscar una planContenido si planContenidoParaBuscar es nulo!.");
            if (planContenidoParaBuscar.Consecutivo <= 0) throw new ArgumentException("planContenidoParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            PlanesContenidosDTO planContenidoBuscado = await client.PostAsync("Planes/BuscarPlanContenido", planContenidoParaBuscar);

            return planContenidoBuscado;
        }

        public async Task<List<PlanesContenidosDTO>> ListarContenidoDeUnPlan(PlanesContenidosDTO planContenidoParaListar)
        {
            if (planContenidoParaListar == null) throw new ArgumentNullException("No puedes listar una planContenido si planContenidoParaListar es nulo!.");
            if (planContenidoParaListar.CodigoPlan <= 0) throw new ArgumentException("planContenidoParaListar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            List<PlanesContenidosDTO> listaPlanesContenidos = await client.PostAsync<PlanesContenidosDTO, List<PlanesContenidosDTO>>("Planes/ListarContenidoDeUnPlan", planContenidoParaListar);

            return listaPlanesContenidos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPlanContenido(PlanesContenidosDTO planContenidoParaModificar)
        {
            if (planContenidoParaModificar == null) throw new ArgumentNullException("No puedes modificar una planContenido si planContenidoParaModificar es nulo!.");
            if (planContenidoParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(planContenidoParaModificar.Descripcion))
            {
                throw new ArgumentException("planContenidoParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarPlanContenido = await client.PostAsync<PlanesContenidosDTO, WrapperSimpleTypesDTO>("Planes/ModificarPlanContenido", planContenidoParaModificar);

            return wrapperModificarPlanContenido;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarPlanContenido(PlanesContenidosDTO planContenidoParaEliminar)
        {
            if (planContenidoParaEliminar == null) throw new ArgumentNullException("No puedes eliminar una planContenido si planContenidoParaEliminar es nulo!.");
            if (planContenidoParaEliminar.Consecutivo <= 0)
            {
                throw new ArgumentException("planContenidoParaEliminar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarPlanContenido = await client.PostAsync<PlanesContenidosDTO, WrapperSimpleTypesDTO>("Planes/EliminarPlanContenido", planContenidoParaEliminar);

            return wrapperEliminarPlanContenido;
        }


        #endregion


        #region Metodos PlanesUsuarios


        public async Task<PlanesUsuariosDTO> BuscarPlanUsuario(PlanesUsuariosDTO planUsuarioParaBuscar)
        {
            if (planUsuarioParaBuscar == null) throw new ArgumentNullException("No puedes buscar un planUsuario si planUsuarioParaBuscar es nulo!.");
            if (planUsuarioParaBuscar.Consecutivo <= 0 || planUsuarioParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("planUsuarioParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            PlanesUsuariosDTO planUsuarioBuscado = await client.PostAsync("Planes/BuscarPlanUsuario", planUsuarioParaBuscar);

            return planUsuarioBuscado;
        }

        public async Task<WrapperSimpleTypesDTO> VerificarSiPlanSoportaLaOperacion(PlanesUsuariosDTO planUsuarioParaValidar)
        {
            if (planUsuarioParaValidar == null) throw new ArgumentNullException("No puedes validar un planUsuario si planUsuarioParaValidar es nulo!.");
            if (planUsuarioParaValidar.Consecutivo <= 0 || planUsuarioParaValidar.TipoOperacionBase == TipoOperacion.SinOperacion)
            {
                throw new ArgumentException("planUsuarioParaValidar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperVerificarPlanOperacion = await client.PostAsync<PlanesUsuariosDTO, WrapperSimpleTypesDTO>("Planes/VerificarSiPlanSoportaLaOperacion", planUsuarioParaValidar);

            return wrapperVerificarPlanOperacion;
        }

        public async Task<WrapperSimpleTypesDTO> CambiarDePlanUsuario(PlanesUsuariosDTO planParaCambiar)
        {
            if (planParaCambiar == null) throw new ArgumentNullException("No puedes cambiar un planUsuario si planParaCambiar es nulo!.");
            if (planParaCambiar.Consecutivo <= 0 || planParaCambiar.CodigoPlanDeseado <= 0)
            {
                throw new ArgumentException("planParaCambiar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCambiarDePlanUsuario = await client.PostAsync<PlanesUsuariosDTO, WrapperSimpleTypesDTO>("Planes/CambiarDePlanUsuario", planParaCambiar);

            return wrapperCambiarDePlanUsuario;
        }


        #endregion


    }
}
