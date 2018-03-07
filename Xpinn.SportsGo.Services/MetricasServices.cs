using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.Enums;
using System.Linq;
using Xpinn.SportsGo.Util.Portable.BaseClasses;

namespace Xpinn.SportsGo.Services
{
    public class MetricasServices : BaseService
    {


        #region Metodos Metricas Usuarios


        public async Task<WrapperSimpleTypesDTO> NumeroDescargasMoviles()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroDescargasMoviles = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroDescargasMoviles");

            return wrapperNumeroDescargasMoviles;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVentasUltimoMes()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroVentasUltimoMes = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroVentasUltimoMes");

            return wrapperNumeroVentasUltimoMes;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistrados()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistrados = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroUsuariosRegistrados");

            return wrapperNumeroUsuariosRegistrados;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistradosUltimoMes()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistradosUltimoMes = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroUsuariosRegistradosUltimoMes");

            return wrapperNumeroUsuariosRegistradosUltimoMes;
        }

        public async Task<MetricasDTO> MetricasUsuarios(MetricasDTO metricasParaBuscar)
        {
            IHttpClient client = ConfigurarHttpClient();

            MetricasDTO metricasUsuarios = await client.PostAsync("Metricas/MetricasUsuarios", metricasParaBuscar);

            return metricasUsuarios;
        }

        public async Task<List<PersonasDTO>> ListarUsuariosMetricas(MetricasDTO metricasParaBuscar)
        {
            if (metricasParaBuscar == null) throw new ArgumentNullException("No puedes listar los usuario si metricasParaBuscar es nulo!.");
            if (metricasParaBuscar.TakeIndexBase <= 0 || metricasParaBuscar.SkipIndexBase < 0 || metricasParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                throw new ArgumentException("metricasParaBuscar vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<PersonasDTO> listaUsuarioMetricas = await client.PostAsync<MetricasDTO, List<PersonasDTO>>("Metricas/ListarUsuariosMetricas", metricasParaBuscar);

            return listaUsuarioMetricas;
        }


        #endregion


        #region Metodos Metricas Anunciantes


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistrados()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistrados = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroAnunciantesRegistrados");

            return wrapperNumeroAnunciantesRegistrados;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistradosUltimoMes()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistradosUltimoMes = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroAnunciantesRegistradosUltimoMes");

            return wrapperNumeroAnunciantesRegistradosUltimoMes;
        }


        #endregion


        #region Metodos Metricas Anuncios


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistrados(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistrados = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroAnunciosRegistrados", metricasParaBuscar);

            return wrapperNumeroAnunciosRegistrados;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistradosUltimoMes(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistradosUltimoMes = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroAnunciosRegistradosUltimoMes", metricasParaBuscar);

            return wrapperNumeroAnunciosRegistradosUltimoMes;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeados(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroVecesClickeados = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroVecesClickeados", metricasParaBuscar);

            return wrapperNumeroVecesClickeados;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeadosUltimoMes(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroVecesClickeadosUltimoMes = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroVecesClickeadosUltimoMes", metricasParaBuscar);

            return wrapperNumeroVecesClickeadosUltimoMes;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistos(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroVecesVistos = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroVecesVistos", metricasParaBuscar);

            return wrapperNumeroVecesVistos;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistosUltimoMes(MetricasDTO metricasParaBuscar = null)
        {
            IHttpClient client = ConfigurarHttpClient();

            if (metricasParaBuscar == null)
            {
                metricasParaBuscar = new MetricasDTO();
            }

            WrapperSimpleTypesDTO wrapperNumeroVecesVistosUltimoMes = await client.PostAsync<MetricasDTO, WrapperSimpleTypesDTO>("Metricas/NumeroVecesVistosUltimoMes", metricasParaBuscar);

            return wrapperNumeroVecesVistosUltimoMes;
        }

        public async Task<MetricasDTO> MetricasAnuncios(MetricasDTO metricasParaBuscar)
        {
            IHttpClient client = ConfigurarHttpClient();

            MetricasDTO metricasAnuncios = await client.PostAsync("Metricas/MetricasAnuncios", metricasParaBuscar);

            return metricasAnuncios;
        }


        #endregion


        #region Metodos Metricas Eventos


        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistrados()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroEventosRegistrados = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroEventosRegistrados");

            return wrapperNumeroEventosRegistrados;
        }

        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistradosUltimoMes()
        {
            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperNumeroEventosRegistradosUltimoMes = await client.PostAsync<WrapperSimpleTypesDTO>("Metricas/NumeroEventosRegistradosUltimoMes");

            return wrapperNumeroEventosRegistradosUltimoMes;
        }

        public async Task<MetricasDTO> MetricasEventos(MetricasDTO metricasParaBuscar)
        {
            IHttpClient client = ConfigurarHttpClient();

            MetricasDTO metricasEventos = await client.PostAsync("Metricas/MetricasEventos", metricasParaBuscar);

            return metricasEventos;
        }


        #endregion


    }
}
