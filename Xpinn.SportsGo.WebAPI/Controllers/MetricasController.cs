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
    public class MetricasController : ApiController
    {
        MetricasBusiness _metricasBusiness;

        public MetricasController()
        {
            _metricasBusiness = new MetricasBusiness();
        }


        #region Metodos Metricas Usuarios


        public IHttpActionResult NumeroDescargasMoviles()
        {
            try
            {
                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO
                {
                    NumeroRegistrosExistentes = 100
                };

                return Ok(wrapper);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroVentasUltimoMes()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroVentasUltimoMes = await _metricasBusiness.NumeroVentasUltimoMes();

                return Ok(wrapperNumeroVentasUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroUsuariosRegistrados()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistrados = await _metricasBusiness.NumeroUsuariosRegistrados();

                return Ok(wrapperNumeroUsuariosRegistrados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroUsuariosRegistradosUltimoMes()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistradosUltimoMes = await _metricasBusiness.NumeroUsuariosRegistradosUltimoMes();

                return Ok(wrapperNumeroUsuariosRegistradosUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> MetricasUsuarios(MetricasDTO metricasParaBuscar)
        {
            try
            {
                MetricasDTO metricasUsuarios = await _metricasBusiness.MetricasUsuarios(metricasParaBuscar);

                return Ok(metricasUsuarios);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> ListarUsuariosMetricas(MetricasDTO metricasParaBuscar)
        {
            if (metricasParaBuscar == null || metricasParaBuscar.TakeIndexBase <= 0 || metricasParaBuscar.SkipIndexBase < 0 || metricasParaBuscar.IdiomaBase == Idioma.SinIdioma)
            {
                return BadRequest("metricasParaBuscar vacio y/o invalido!.");
            }

            try
            {
                List<PersonasDTO> listaUsuarioMetricas = await _metricasBusiness.ListarUsuariosMetricas(metricasParaBuscar);

                return Ok(listaUsuarioMetricas);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Metricas Anunciantes


        public async Task<IHttpActionResult> NumeroAnunciantesRegistrados()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistrados = await _metricasBusiness.NumeroAnunciantesRegistrados();

                return Ok(wrapperNumeroAnunciantesRegistrados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroAnunciantesRegistradosUltimoMes()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistradosUltimoMes = await _metricasBusiness.NumeroAnunciantesRegistradosUltimoMes();

                return Ok(wrapperNumeroAnunciantesRegistradosUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


        #region Metodos Metricas Anuncios


        public async Task<IHttpActionResult> NumeroAnunciosRegistrados(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistrados = await _metricasBusiness.NumeroAnunciosRegistrados(metricasParaBuscar);

                return Ok(wrapperNumeroAnunciosRegistrados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroAnunciosRegistradosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistradosUltimoMes = await _metricasBusiness.NumeroAnunciosRegistradosUltimoMes(metricasParaBuscar);

                return Ok(wrapperNumeroAnunciosRegistradosUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroVecesClickeados(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroVecesClickeados = await _metricasBusiness.NumeroVecesClickeados(metricasParaBuscar);

                return Ok(wrapperNumeroVecesClickeados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroVecesClickeadosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroVecesClickeados = await _metricasBusiness.NumeroVecesClickeadosUltimoMes(metricasParaBuscar);

                return Ok(wrapperNumeroVecesClickeados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroVecesVistos(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroVecesVistos = await _metricasBusiness.NumeroVecesVistos(metricasParaBuscar);

                return Ok(wrapperNumeroVecesVistos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroVecesVistosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroVecesVistosUltimoMes = await _metricasBusiness.NumeroVecesVistosUltimoMes(metricasParaBuscar);

                return Ok(wrapperNumeroVecesVistosUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> MetricasAnuncios(MetricasDTO metricasParaBuscar)
        {
            try
            {
                MetricasDTO metricasAnuncios = await _metricasBusiness.MetricasAnuncios(metricasParaBuscar);

                return Ok(metricasAnuncios);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        #endregion


        #region Metodos Metricas Eventos


        public async Task<IHttpActionResult> NumeroEventosRegistrados()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroEventosRegistrados = await _metricasBusiness.NumeroEventosRegistrados();

                return Ok(wrapperNumeroEventosRegistrados);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> NumeroEventosRegistradosUltimoMes()
        {
            try
            {
                WrapperSimpleTypesDTO wrapperNumeroEventosRegistradosUltimoMes = await _metricasBusiness.NumeroEventosRegistradosUltimoMes();

                return Ok(wrapperNumeroEventosRegistradosUltimoMes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public async Task<IHttpActionResult> MetricasEventos(MetricasDTO metricasParaBuscar)
        {
            try
            {
                MetricasDTO metricasEventos = await _metricasBusiness.MetricasEventos(metricasParaBuscar);

                return Ok(metricasEventos);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        #endregion


    }
}