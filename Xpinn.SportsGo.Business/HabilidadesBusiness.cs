using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;

namespace Xpinn.SportsGo.Business
{
    public class HabilidadesBusiness
    {


        #region Metodos Habilidades


        public async Task<WrapperSimpleTypesDTO> CrearHabilidad(Habilidades habilidadParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);

                habilidadesRepo.CrearHabilidad(habilidadParaCrear);

                WrapperSimpleTypesDTO wrapperCrearHabilidad = new WrapperSimpleTypesDTO();

                wrapperCrearHabilidad.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearHabilidad.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearHabilidad.Exitoso = true;
                    wrapperCrearHabilidad.ConsecutivoCreado = habilidadParaCrear.Consecutivo;
                }

                return wrapperCrearHabilidad;
            }
        }

        public async Task<Habilidades> BuscarHabilidad(Habilidades habilidadParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                Habilidades habilidadBuscada = await habilidadesRepo.BuscarHabilidad(habilidadParaBuscar);

                return habilidadBuscada;
            }
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorIdioma(Habilidades habilidadParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                List<HabilidadesDTO> listaHabilidades = await habilidadesRepo.ListarHabilidadesPorIdioma(habilidadParaListar);

                return listaHabilidades;
            }
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorCodigoCategoriaAndIdioma(Habilidades habilidadParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                List<HabilidadesDTO> listaHabilidadesPorCategoria = await habilidadesRepo.ListarHabilidadesPorCodigoCategoriaAndIdioma(habilidadParaListar);

                return listaHabilidadesPorCategoria;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarHabilidad(Habilidades habilidadParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);

                habilidadParaModificar.CodigoTipoHabilidad = (int)habilidadParaModificar.TipoHabilidad;
                Habilidades habilidadExistente = await habilidadesRepo.ModificarHabilidad(habilidadParaModificar);

                if (habilidadParaModificar.HabilidadesContenidos != null && habilidadParaModificar.HabilidadesContenidos.Count > 0)
                {
                    foreach (var item in habilidadParaModificar.HabilidadesContenidos)
                    {
                        await habilidadesRepo.ModificarHabilidadContenido(item);
                    }
                }

                WrapperSimpleTypesDTO wrapperModificarHabilidad = new WrapperSimpleTypesDTO();

                wrapperModificarHabilidad.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarHabilidad.NumeroRegistrosAfectados > 0) wrapperModificarHabilidad.Exitoso = true;

                return wrapperModificarHabilidad;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidad(Habilidades habilidadParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesContenidos habilidadContenidosParaBorrar = new HabilidadesContenidos
                {
                    CodigoHabilidad = habilidadParaEliminar.Consecutivo
                };

                habilidadesRepo.EliminarMultiplesHabilidadesContenidosPorCodigoHabilidad(habilidadContenidosParaBorrar);
                habilidadesRepo.EliminarHabilidad(habilidadParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarHabilidad = new WrapperSimpleTypesDTO();

                wrapperEliminarHabilidad.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarHabilidad.NumeroRegistrosAfectados > 0) wrapperEliminarHabilidad.Exitoso = true;

                return wrapperEliminarHabilidad;
            }
        }


        #endregion


        #region Metodos HabilidadesCandidatos


        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesCandidato(List<HabilidadesCandidatos> habilidadCandidatoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);

                // Elimina todas las habilidades para recrearlas luega
                habilidadesRepo.EliminarMultiplesHabilidadesCandidatosPorCodigoCandidato(habilidadCandidatoParaCrear[0]);

                // Evitar que si se le manda una instancia la vuelva a crear
                habilidadCandidatoParaCrear.ForEach(x => x.Habilidades = null);

                habilidadesRepo.CrearHabilidadesCandidato(habilidadCandidatoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearHabilidadesCandidato = new WrapperSimpleTypesDTO();

                wrapperCrearHabilidadesCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearHabilidadesCandidato.NumeroRegistrosAfectados > 0) wrapperCrearHabilidadesCandidato.Exitoso = true;

                return wrapperCrearHabilidadesCandidato;
            }
        }

        public async Task<HabilidadesCandidatosDTO> BuscarHabilidadCandidatoPorIdioma(HabilidadesCandidatos habilidadCandidatoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesCandidatosDTO habilidadCandidatoBuscada = await habilidadesRepo.BuscarHabilidadCandidatoPorIdioma(habilidadCandidatoParaBuscar);

                return habilidadCandidatoBuscada;
            }
        }

        public async Task<List<HabilidadesCandidatosDTO>> ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(HabilidadesCandidatos habilidadCandidatoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                List<HabilidadesCandidatosDTO> listaHabilidadesCandidato = await habilidadesRepo.ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(habilidadCandidatoParaListar);

                return listaHabilidadesCandidato;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidadCandidato(HabilidadesCandidatos habilidadCandidatoParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                habilidadesRepo.EliminarHabilidadCandidato(habilidadCandidatoParaBorrar);

                WrapperSimpleTypesDTO wrapperEliminarHabilidadCandidato = new WrapperSimpleTypesDTO();

                wrapperEliminarHabilidadCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarHabilidadCandidato.NumeroRegistrosAfectados > 0) wrapperEliminarHabilidadCandidato.Exitoso = true;

                return wrapperEliminarHabilidadCandidato;
            }
        }


        #endregion


        #region Metodos HabilidadesContenidos


        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesContenidos(List<HabilidadesContenidos> habilidadesContenidosParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                habilidadesRepo.CrearHabilidadesContenidos(habilidadesContenidosParaCrear);

                WrapperSimpleTypesDTO wrapperCrearHabilidadesContenido = new WrapperSimpleTypesDTO();

                wrapperCrearHabilidadesContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearHabilidadesContenido.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearHabilidadesContenido.Exitoso = true;
                }

                return wrapperCrearHabilidadesContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearHabilidadesContenidosIndividual(HabilidadesContenidos habilidadesContenidosParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                habilidadesRepo.CrearHabilidadesContenidosIndividual(habilidadesContenidosParaCrear);

                WrapperSimpleTypesDTO wrapperCrearHabilidadesContenidosIndividual = new WrapperSimpleTypesDTO();

                wrapperCrearHabilidadesContenidosIndividual.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearHabilidadesContenidosIndividual.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearHabilidadesContenidosIndividual.Exitoso = true;
                }

                return wrapperCrearHabilidadesContenidosIndividual;
            }
        }

        public async Task<HabilidadesContenidos> BuscarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesContenidos habilidadesContenidoBuscada = await habilidadesRepo.BuscarHabilidadContenido(habilidadContenidoParaBuscar);

                return habilidadesContenidoBuscada;
            }
        }

        public async Task<List<HabilidadesContenidos>> ListarContenidoDeUnaHabilidad(HabilidadesContenidos habilidadContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                List<HabilidadesContenidos> listaHabilidadesContenidos = await habilidadesRepo.ListarContenidoDeUnaHabilidad(habilidadContenidoParaListar);

                return listaHabilidadesContenidos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesContenidos habilidadContenidoExistente = await habilidadesRepo.ModificarHabilidadContenido(habilidadContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarHabilidadContenido = new WrapperSimpleTypesDTO();

                wrapperModificarHabilidadContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarHabilidadContenido.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarHabilidadContenido.Exitoso = true;
                }

                return wrapperModificarHabilidadContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                habilidadesRepo.EliminarHabilidadContenido(habilidadContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarHabilidadContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarHabilidadContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarHabilidadContenido.NumeroRegistrosAfectados > 0) wrapperEliminarHabilidadContenido.Exitoso = true;

                return wrapperEliminarHabilidadContenido;
            }
        }


        #endregion


    }
}