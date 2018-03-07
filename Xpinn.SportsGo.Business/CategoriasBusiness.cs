using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Business
{
    public class CategoriasBusiness
    {


        #region Metodos Categorias


        public async Task<WrapperSimpleTypesDTO> CrearCategoria(Categorias categoriaParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);

                categoriaParaCrear.Archivos.CodigoTipoArchivo = (int)TipoArchivo.Imagen;
                categoriasRepo.CrearCategoria(categoriaParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCategoria = new WrapperSimpleTypesDTO();

                wrapperCrearCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCategoria.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCategoria.Exitoso = true;
                    wrapperCrearCategoria.ConsecutivoCreado = categoriaParaCrear.Consecutivo;
                    wrapperCrearCategoria.ConsecutivoArchivoCreado = categoriaParaCrear.CodigoArchivo;
                }

                return wrapperCrearCategoria;
            }
        }

        public async Task<Categorias> BuscarCategoria(Categorias categoriaParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                Categorias categoriaBuscada = await categoriasRepo.BuscarCategoria(categoriaParaBuscar);

                return categoriaBuscada;
            }
        }

        public async Task<List<CategoriasDTO>> ListarCategoriasPorIdioma(Categorias categoriaParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasDTO> listaCategorias = await categoriasRepo.ListarCategoriasPorIdioma(categoriaParaListar);

                return listaCategorias;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoCategoria(Categorias categoriaParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivo = new Archivos
                {
                    Consecutivo = categoriaParaModificar.CodigoArchivo,
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = categoriaParaModificar.ArchivoContenido
                };

                archivoRepo.ModificarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperModificarArchivoCategoria = new WrapperSimpleTypesDTO();

                wrapperModificarArchivoCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarArchivoCategoria.NumeroRegistrosAfectados > 0)
                {
                    wrapperModificarArchivoCategoria.Exitoso = true;
                }

                return wrapperModificarArchivoCategoria;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoria(Categorias categoriaParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                if (categoriaParaEliminar.Consecutivo == 3 && categoriaParaEliminar.Consecutivo == 5 
                    && categoriaParaEliminar.Consecutivo == 6 && categoriaParaEliminar.Consecutivo == 7)
                {
                    throw new InvalidOperationException("Estas categorias deben existir y si o si ya que son las definidas para la posicion");
                }

                Archivos archivo = new Archivos
                {
                    Consecutivo = categoriaParaEliminar.CodigoArchivo
                };

                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                Habilidades habilidadesParaBorrar = new Habilidades
                {
                    CodigoCategoria = categoriaParaEliminar.Consecutivo
                };

                List<int> listaCodigoHabilidades = await habilidadesRepo.ListarCodigoHabilidadesPorCategoria(habilidadesParaBorrar);

                foreach (var codigo in listaCodigoHabilidades)
                {
                    habilidadesRepo.EliminarMultiplesHabilidadesContenidosPorCodigoHabilidad(codigo);
                }
                
                habilidadesRepo.EliminarMultiplesHabilidadesPorCodigoCategoria(habilidadesParaBorrar);

                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasContenidos categoriaContenidoParaBorrar = new CategoriasContenidos
                {
                    CodigoCategoria = categoriaParaEliminar.Consecutivo
                };

                categoriasRepo.EliminarMultiplesCategoriasContenidos(categoriaContenidoParaBorrar);
                categoriasRepo.EliminarCategoria(categoriaParaEliminar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivo);

                WrapperSimpleTypesDTO wrapperEliminarCategoria = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoria.NumeroRegistrosAfectados > 0) wrapperEliminarCategoria.Exitoso = true;

                return wrapperEliminarCategoria;
            }
        }


        #endregion


        #region Metodos CategoriasContenidos


        public async Task<WrapperSimpleTypesDTO> CrearCategoriasContenidos(List<CategoriasContenidos> categoriaContenidoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriasContenidos(categoriaContenidoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCategoriaContenido = new WrapperSimpleTypesDTO();

                wrapperCrearCategoriaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCategoriaContenido.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCategoriaContenido.Exitoso = true;
                }

                return wrapperCrearCategoriaContenido;
            }
        }

        public async Task<CategoriasContenidos> BuscarCategoriaContenido(CategoriasContenidos categoriaContenidoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasContenidos categoriaContenidoBuscada = await categoriasRepo.BuscarCategoriaContenido(categoriaContenidoParaBuscar);

                return categoriaContenidoBuscada;
            }
        }

        public async Task<List<CategoriasContenidos>> ListarContenidoDeUnaCategoria(CategoriasContenidos categoriaContenidoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasContenidos> listaCategorias = await categoriasRepo.ListarContenidoDeUnaCategoria(categoriaContenidoParaListar);

                return listaCategorias;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaContenido(CategoriasContenidos categoriaContenidoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasContenidos categoriaContenidoExistente = await categoriasRepo.ModificarCategoriaContenido(categoriaContenidoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCategoriaContenido = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaContenido.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaContenido.Exitoso = true;

                return wrapperModificarCategoriaContenido;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaContenido(CategoriasContenidos categoriaContenidoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.EliminarCategoriaContenido(categoriaContenidoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaContenido = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaContenido.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaContenido.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaContenido.Exitoso = true;

                return wrapperEliminarCategoriaContenido;
            }
        }


        #endregion


        #region Metodos CategoriasAnuncios


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaAnuncios(List<CategoriasAnuncios> categoriaAnuncioParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearListaCategoriaAnuncios(categoriaAnuncioParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCategoria = new WrapperSimpleTypesDTO();

                wrapperCrearCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCategoria.NumeroRegistrosAfectados > 0) wrapperCrearCategoria.Exitoso = true;

                return wrapperCrearCategoria;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaAnuncios(CategoriasAnuncios categoriaAnuncioParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriaAnuncios(categoriaAnuncioParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCategoria = new WrapperSimpleTypesDTO();

                wrapperCrearCategoria.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCategoria.NumeroRegistrosAfectados > 0) wrapperCrearCategoria.Exitoso = true;

                return wrapperCrearCategoria;
            }
        }

        public async Task<CategoriasAnunciosDTO> BuscarCategoriaAnuncioPorConsecutivoAndIdioma(CategoriasAnuncios categoriaAnuncioParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasAnunciosDTO categoriaAnuncioBuscada = await categoriasRepo.BuscarCategoriaAnuncioPorConsecutivoAndIdioma(categoriaAnuncioParaBuscar);

                return categoriaAnuncioBuscada;
            }
        }

        public async Task<List<CategoriasAnunciosDTO>> ListarCategoriasDeUnAnuncioPorIdioma(CategoriasAnuncios categoriaAnuncioParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasAnunciosDTO> listaCategoriasDeUnAnuncio = await categoriasRepo.ListarCategoriasDeUnAnuncioPorIdioma(categoriaAnuncioParaListar);

                return listaCategoriasDeUnAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasAnuncios categoriaAnuncioExistente = await categoriasRepo.ModificarCategoriaAnuncio(categoriaAnuncioParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCategoriaAnuncio = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaAnuncio.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaAnuncio.Exitoso = true;

                return wrapperModificarCategoriaAnuncio;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.EliminarCategoriaAnuncio(categoriaAnuncioParaBorrar);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaAnuncio = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaAnuncio.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaAnuncio.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaAnuncio.Exitoso = true;

                return wrapperEliminarCategoriaAnuncio;
            }
        }


        #endregion


        #region Metodos CategoriasCandidatos    


        public async Task<WrapperSimpleTypesDTO> CrearCategoriaCandidatos(CategoriasCandidatos categoriaCandidatoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanExistente = await planRepo.BuscarCodigoPlanUsuarioPorCodigoCandidato(categoriaCandidatoParaCrear.CodigoCandidato);

                if (!codigoPlanExistente.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el plan del candidato para crear la categoria para el!.");
                }

                // Se "SUBE" el contador de categorias 1
                PlanesUsuarios planUsuarioExistente = await planRepo.ModificarNumeroCategoriasUsadas(codigoPlanExistente.Value, 1);

                if (categoriaCandidatoParaCrear.HabilidadesCandidatos != null && categoriaCandidatoParaCrear.HabilidadesCandidatos.Count > 0)
                {
                    categoriaCandidatoParaCrear.HabilidadesCandidatos = categoriaCandidatoParaCrear.HabilidadesCandidatos.Where(x => x.NumeroEstrellas > 0).ToList();
                }

                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriaCandidatos(categoriaCandidatoParaCrear);

                WrapperSimpleTypesDTO wrapperCrearCategoriaCandidato = new WrapperSimpleTypesDTO();

                wrapperCrearCategoriaCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearCategoriaCandidato.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearCategoriaCandidato.Exitoso = true;
                    wrapperCrearCategoriaCandidato.ConsecutivoCreado = categoriaCandidatoParaCrear.Consecutivo;
                }

                return wrapperCrearCategoriaCandidato;
            }
        }

        public async Task<CategoriasCandidatosDTO> BuscarCategoriaCandidatoPorConsecutivoAndIdioma(CategoriasCandidatos categoriaCandidatoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasCandidatosDTO categoriaCandidatoBuscada = await categoriasRepo.BuscarCategoriaCandidatoPorConsecutivoAndIdioma(categoriaCandidatoParaBuscar);

                return categoriaCandidatoBuscada;
            }
        }

        public async Task<List<CategoriasCandidatosDTO>> ListarCategoriasDeUnCandidatoPorIdioma(CategoriasCandidatos categoriaCandidatoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasCandidatosDTO> listaCategoriasCandidatos = await categoriasRepo.ListarCategoriasDeUnCandidatoPorIdioma(categoriaCandidatoParaListar);

                return listaCategoriasCandidatos;
            }
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnCandidato(int codigoPersona)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<int> listaCategoriasCandidatos = await categoriasRepo.ListarCodigoCategoriasDeUnCandidato(codigoPersona);

                return listaCategoriasCandidatos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasCandidatos categoriaCandidatoExistente = await categoriasRepo.ModificarCategoriaCandidato(categoriaCandidatoParaModificar);

                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesCandidatos habilidadParaBorrar = new HabilidadesCandidatos
                {
                    CodigoCategoriaCandidato = categoriaCandidatoParaModificar.Consecutivo
                };
                habilidadesRepo.EliminarMultiplesHabilidadesCandidatosPorCodigoCandidato(habilidadParaBorrar);

                if (categoriaCandidatoParaModificar.HabilidadesCandidatos != null && categoriaCandidatoParaModificar.HabilidadesCandidatos.Count > 0)
                {
                    categoriaCandidatoParaModificar.HabilidadesCandidatos = categoriaCandidatoParaModificar.HabilidadesCandidatos.Where(x => x.NumeroEstrellas > 0).ToList();
                }

                categoriaCandidatoParaModificar.HabilidadesCandidatos.ForEach(x =>
                {
                    x.CodigoCategoriaCandidato = categoriaCandidatoParaModificar.Consecutivo;
                    x.Habilidades = null;
                });
                habilidadesRepo.CrearHabilidadesCandidato(categoriaCandidatoParaModificar.HabilidadesCandidatos);

                WrapperSimpleTypesDTO wrapperModificarCategoriaCandidato = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaCandidato.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaCandidato.Exitoso = true;

                return wrapperModificarCategoriaCandidato;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                HabilidadesRepository habilidadesRepo = new HabilidadesRepository(context);
                HabilidadesCandidatos habilidadParaBorrar = new HabilidadesCandidatos
                {
                    CodigoCategoriaCandidato = categoriaCandidatoParaBorrar.Consecutivo
                };
                habilidadesRepo.EliminarMultiplesHabilidadesCandidatosPorCodigoCandidato(habilidadParaBorrar);

                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                int? codigoCandidato = await categoriasRepo.BuscarCodigoCandidatoDeUnaCategoriaCandidato(categoriaCandidatoParaBorrar.Consecutivo);

                if (!codigoCandidato.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo del candidato para borrar la categoria y modificar el plan!.");
                }

                categoriasRepo.EliminarCategoriaCandidato(categoriaCandidatoParaBorrar);

                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanExistente = await planRepo.BuscarCodigoPlanUsuarioPorCodigoCandidato(codigoCandidato.Value);

                if (!codigoPlanExistente.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el plan del candidato para crear la categoria para el!.");
                }

                // Se "BAJA" el contador de categorias 1
                PlanesUsuarios planUsuarioExistente = await planRepo.ModificarNumeroCategoriasUsadas(codigoPlanExistente.Value, -1);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaCandidato = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaCandidato.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaCandidato.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaCandidato.Exitoso = true;

                return wrapperEliminarCategoriaCandidato;
            }
        }


        #endregion


        #region Metodos CategoriasEventos


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaEventos(List<CategoriasEventos> categoriaEventoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearListaCategoriaEventos(categoriaEventoParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaEventos = new WrapperSimpleTypesDTO();

                wrapperCategoriaEventos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaEventos.NumeroRegistrosAfectados > 0) wrapperCategoriaEventos.Exitoso = true;

                return wrapperCategoriaEventos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaEventos(CategoriasEventos categoriaEventoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriaEventos(categoriaEventoParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaEventos = new WrapperSimpleTypesDTO();

                wrapperCategoriaEventos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaEventos.NumeroRegistrosAfectados > 0)
                {
                    wrapperCategoriaEventos.Exitoso = true;
                    wrapperCategoriaEventos.ConsecutivoCreado = categoriaEventoParaCrear.Consecutivo;
                }

                return wrapperCategoriaEventos;
            }
        }

        public async Task<CategoriasEventosDTO> BuscarCategoriaEventoPorConsecutivoAndIdioma(CategoriasEventos categoriaEventoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasEventosDTO categoriaEventoBuscada = await categoriasRepo.BuscarCategoriaEventoPorConsecutivoAndIdioma(categoriaEventoParaBuscar);

                return categoriaEventoBuscada;
            }
        }

        public async Task<List<CategoriasEventosDTO>> ListarCategoriasDeUnEventoPorIdioma(CategoriasEventos categoriaEventoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasEventosDTO> listaCategoriaEvento = await categoriasRepo.ListarCategoriasDeUnEventoPorIdioma(categoriaEventoParaListar);

                return listaCategoriaEvento;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaEvento(CategoriasEventos categoriaEventoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasEventos categoriaEventoExistente = await categoriasRepo.ModificarCategoriaEvento(categoriaEventoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCategoriaEvento = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaEvento.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaEvento.Exitoso = true;

                return wrapperModificarCategoriaEvento;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaEvento(CategoriasEventos categoriaEventoParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.EliminarCategoriaEvento(categoriaEventoParaBorrar);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaEvento = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaEvento.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaEvento.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaEvento.Exitoso = true;

                return wrapperEliminarCategoriaEvento;
            }
        }


        #endregion


        #region Metodos CategoriasGrupos


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaGrupos(List<CategoriasGrupos> categoriaGrupoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearListaCategoriaGrupos(categoriaGrupoParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaEventos = new WrapperSimpleTypesDTO();

                wrapperCategoriaEventos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaEventos.NumeroRegistrosAfectados > 0) wrapperCategoriaEventos.Exitoso = true;

                return wrapperCategoriaEventos;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaGrupos(CategoriasGrupos categoriaGrupoParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriaGrupos(categoriaGrupoParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaEventos = new WrapperSimpleTypesDTO();

                wrapperCategoriaEventos.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaEventos.NumeroRegistrosAfectados > 0)
                {
                    wrapperCategoriaEventos.Exitoso = true;
                    wrapperCategoriaEventos.ConsecutivoCreado = categoriaGrupoParaCrear.Consecutivo;
                }

                return wrapperCategoriaEventos;
            }
        }

        public async Task<CategoriasGruposDTO> BuscarCategoriaGrupoPorConsecutivoAndIdioma(CategoriasGrupos categoriaGrupoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasGruposDTO categoriaEventoBuscada = await categoriasRepo.BuscarCategoriaGrupoPorConsecutivoAndIdioma(categoriaGrupoParaBuscar);

                return categoriaEventoBuscada;
            }
        }

        public async Task<List<CategoriasGruposDTO>> ListarCategoriasDeUnGrupoPorIdioma(CategoriasGrupos categoriaGrupoParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasGruposDTO> listaCategoriaEvento = await categoriasRepo.ListarCategoriasDeUnGrupoPorIdioma(categoriaGrupoParaListar);

                return listaCategoriaEvento;
            }
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnGrupo(int codigoPersona)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<int> listaCategoriasGrupo = await categoriasRepo.ListarCodigoCategoriasDeUnGrupo(codigoPersona);

                return listaCategoriasGrupo;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasGrupos categoriaGrupoExistente = await categoriasRepo.ModificarCategoriaGrupo(categoriaGrupoParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCategoriaGrupo = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaGrupo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaGrupo.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaGrupo.Exitoso = true;

                return wrapperModificarCategoriaGrupo;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                int? codigoGrupo = await categoriasRepo.BuscarCodigoCandidatoDeUnaCategoriaGrupo(categoriaGrupoParaBorrar.Consecutivo);

                if (!codigoGrupo.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo del grupo para borrar la categoria y modificar el plan!.");
                }

                categoriasRepo.EliminarCategoriaGrupo(categoriaGrupoParaBorrar);

                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanExistente = await planRepo.BuscarCodigoPlanUsuarioPorCodigoGrupo(codigoGrupo.Value);

                if (!codigoPlanExistente.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el plan del grupo para crear la categoria para el!.");
                }

                // Se "BAJA" el contador de categorias 1
                PlanesUsuarios planUsuarioExistente = await planRepo.ModificarNumeroCategoriasUsadas(codigoPlanExistente.Value, -1);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaGrupo = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaGrupo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaGrupo.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaGrupo.Exitoso = true;

                return wrapperEliminarCategoriaGrupo;
            }
        }


        #endregion


        #region CategoriasRepresentantes


        public async Task<WrapperSimpleTypesDTO> CrearListaCategoriaRepresentante(List<CategoriasRepresentantes> categoriaRepresentanteParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearListaCategoriaRepresentante(categoriaRepresentanteParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaRepresentante = new WrapperSimpleTypesDTO();

                wrapperCategoriaRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaRepresentante.NumeroRegistrosAfectados > 0) wrapperCategoriaRepresentante.Exitoso = true;

                return wrapperCategoriaRepresentante;
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearCategoriaRepresentante(CategoriasRepresentantes categoriaRepresentanteParaCrear)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                categoriasRepo.CrearCategoriaRepresentante(categoriaRepresentanteParaCrear);

                WrapperSimpleTypesDTO wrapperCategoriaRepresentante = new WrapperSimpleTypesDTO();

                wrapperCategoriaRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCategoriaRepresentante.NumeroRegistrosAfectados > 0)
                {
                    wrapperCategoriaRepresentante.Exitoso = true;
                    wrapperCategoriaRepresentante.ConsecutivoCreado = categoriaRepresentanteParaCrear.Consecutivo;
                }

                return wrapperCategoriaRepresentante;
            }
        }

        public async Task<CategoriasRepresentantesDTO> BuscarCategoriaRepresentantePorConsecutivoAndIdioma(CategoriasRepresentantes categoriaRepresentantesParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasRepresentantesDTO categoriaRepresentanteBuscada = await categoriasRepo.BuscarCategoriaRepresentantePorConsecutivoAndIdioma(categoriaRepresentantesParaBuscar);

                return categoriaRepresentanteBuscada;
            }
        }

        public async Task<List<CategoriasRepresentantesDTO>> ListarCategoriasDeUnRepresentantePorIdioma(CategoriasRepresentantes categoriaRepresentanteParaListar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<CategoriasRepresentantesDTO> listaCategoriasRepresentantes = await categoriasRepo.ListarCategoriasDeUnRepresentantePorIdioma(categoriaRepresentanteParaListar);

                return listaCategoriasRepresentantes;
            }
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnRepresentante(int codigoPersona)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                List<int> listaCategoriasRepresentante = await categoriasRepo.ListarCodigoCategoriasDeUnRepresentante(codigoPersona);

                return listaCategoriasRepresentante;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarCategoriaRepresentante(CategoriasRepresentantes categoriaReprensentateParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                CategoriasRepresentantes categoriaRepresentanteExistente = await categoriasRepo.ModificarCategoriaRepresentante(categoriaReprensentateParaModificar);

                WrapperSimpleTypesDTO wrapperModificarCategoriaRepresentante = new WrapperSimpleTypesDTO();

                wrapperModificarCategoriaRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarCategoriaRepresentante.NumeroRegistrosAfectados > 0) wrapperModificarCategoriaRepresentante.Exitoso = true;

                return wrapperModificarCategoriaRepresentante;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarCategoriaRepresentante(CategoriasRepresentantes categoriaRepresanteParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                CategoriasRepository categoriasRepo = new CategoriasRepository(context);
                int? codigoRepresentante = await categoriasRepo.BuscarCodigoCandidatoDeUnaCategoriaRepresentante(categoriaRepresanteParaBorrar.Consecutivo);

                if (!codigoRepresentante.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el codigo del representante para borrar la categoria y modificar el plan!.");
                }

                categoriasRepo.EliminarCategoriaRepresentante(categoriaRepresanteParaBorrar);

                PlanesRepository planRepo = new PlanesRepository(context);
                int? codigoPlanExistente = await planRepo.BuscarCodigoPlanUsuarioPorCodigoRepresentante(codigoRepresentante.Value);

                if (!codigoPlanExistente.HasValue)
                {
                    throw new InvalidOperationException("No se pudo hallar el plan del representante para crear la categoria para el!.");
                }

                // Se "BAJA" el contador de categorias 1
                PlanesUsuarios planUsuarioExistente = await planRepo.ModificarNumeroCategoriasUsadas(codigoPlanExistente.Value, -1);

                WrapperSimpleTypesDTO wrapperEliminarCategoriaRepresentante = new WrapperSimpleTypesDTO();

                wrapperEliminarCategoriaRepresentante.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarCategoriaRepresentante.NumeroRegistrosAfectados > 0) wrapperEliminarCategoriaRepresentante.Exitoso = true;

                return wrapperEliminarCategoriaRepresentante;
            }
        }


        #endregion


    }
}
