using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using System;
using Xpinn.SportsGo.Util.Portable.HelperClasses;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class MetricasBusiness
    {


        #region Metodos Metricas Usuarios


        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistrados()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistrados = await metricasRepo.NumeroUsuariosRegistrados();

                return wrapperNumeroUsuariosRegistrados;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroUsuariosRegistradosUltimoMes()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroUsuariosRegistradosUltimoMes = await metricasRepo.NumeroUsuariosRegistradosUltimoMes();

                return wrapperNumeroUsuariosRegistradosUltimoMes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVentasUltimoMes()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);

                WrapperSimpleTypesDTO wrapperNumeroVentasUltimoMes = new WrapperSimpleTypesDTO();

                wrapperNumeroVentasUltimoMes.NumeroRegistrosExistentes = await metricasRepo.NumeroVentasUltimoMes();

                return wrapperNumeroVentasUltimoMes;
            }
        }

        public async Task<MetricasDTO> MetricasUsuarios(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);

                MetricasDTO metricasBuscada = new MetricasDTO();

                metricasBuscada.NumeroCandidatos = await metricasRepo.MetricasCandidatos(metricasParaBuscar);

                metricasBuscada.NumeroGrupos = await metricasRepo.MetricasGrupos(metricasParaBuscar);

                metricasBuscada.NumeroRepresentantes = await metricasRepo.MetricasRepresentantes(metricasParaBuscar);

                return metricasBuscada;
            }
        }

        public async Task<List<PersonasDTO>> ListarUsuariosMetricas(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                List<PersonasDTO> listaUsuarioMetricas = await metricasRepo.ListarUsuariosMetricas(metricasParaBuscar);

                if (listaUsuarioMetricas != null && listaUsuarioMetricas.Count > 0)
                {
                    DateTimeHelperNoPortable helper = new DateTimeHelperNoPortable();
                    foreach (var persona in listaUsuarioMetricas)
                    {
                        persona.Usuarios.Creacion = helper.ConvertDateTimeFromAnotherTimeZone(metricasParaBuscar.ZonaHorariaGMTBase, helper.DifferenceBetweenGMTAndLocalTimeZone, persona.Usuarios.Creacion);
                    }
                }

                return listaUsuarioMetricas;
            }
        }


        #endregion


        #region Metodos Metricas Anunciantes


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistrados()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistrados = await metricasRepo.NumeroAnunciantesRegistrados();

                return wrapperNumeroAnunciantesRegistrados;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciantesRegistradosUltimoMes()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroAnunciantesRegistradosUltimoMes = await metricasRepo.NumeroAnunciantesRegistradosUltimoMes();

                return wrapperNumeroAnunciantesRegistradosUltimoMes;
            }
        }


        #endregion


        #region Metodos Metricas Anuncios


        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistrados(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistrados = await metricasRepo.NumeroAnunciosRegistrados(metricasParaBuscar);

                return wrapperNumeroAnunciosRegistrados;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroAnunciosRegistradosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroAnunciosRegistradosUltimoMes = await metricasRepo.NumeroAnunciosRegistradosUltimoMes(metricasParaBuscar);

                return wrapperNumeroAnunciosRegistradosUltimoMes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeados(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroVecesClickeados = await metricasRepo.NumeroVecesClickeados(metricasParaBuscar);

                return wrapperNumeroVecesClickeados;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesClickeadosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroVecesClickeadosUltimoMes = await metricasRepo.NumeroVecesClickeadosUltimoMes(metricasParaBuscar);

                return wrapperNumeroVecesClickeadosUltimoMes;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistos(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroVecesVistos = await metricasRepo.NumeroVecesVistos(metricasParaBuscar);

                return wrapperNumeroVecesVistos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroVecesVistosUltimoMes(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroVecesVistosUltimoMes = await metricasRepo.NumeroVecesVistosUltimoMes(metricasParaBuscar);

                return wrapperNumeroVecesVistosUltimoMes;
            }
        }

        public async Task<MetricasDTO> MetricasAnuncios(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);

                MetricasDTO metricasBuscada = new MetricasDTO();

                metricasBuscada.NumeroAnuncios = await metricasRepo.MetricasAnuncios(metricasParaBuscar);

                return metricasBuscada;
            }
        }


        #endregion


        #region Metodos Metricas Eventos


        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistrados()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroEventosRegistrados = await metricasRepo.NumeroEventosRegistrados();

                return wrapperNumeroEventosRegistrados;
            }
        }

        public async Task<WrapperSimpleTypesDTO> NumeroEventosRegistradosUltimoMes()
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);
                WrapperSimpleTypesDTO wrapperNumeroEventosRegistradosUltimoMes = await metricasRepo.NumeroEventosRegistradosUltimoMes();

                return wrapperNumeroEventosRegistradosUltimoMes;
            }
        }

        public async Task<MetricasDTO> MetricasEventos(MetricasDTO metricasParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                MetricasRepository metricasRepo = new MetricasRepository(context);

                MetricasDTO metricasBuscada = new MetricasDTO();

                metricasBuscada.NumeroEventos = await metricasRepo.MetricasEventos(metricasParaBuscar);

                return metricasBuscada;
            }
        }


        #endregion


    }
}
