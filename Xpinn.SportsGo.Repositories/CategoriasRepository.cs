using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class CategoriasRepository
    {
        SportsGoEntities _context;

        public CategoriasRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Categorias


        public void CrearCategoria(Categorias categoriaParaCrear)
        {
            _context.Categorias.Add(categoriaParaCrear);
        }

        public async Task<Categorias> BuscarCategoria(Categorias categoriaParaBuscar)
        {
            Categorias categoriaBuscada = await (from categoria in _context.Categorias
                                                 where categoria.Consecutivo == categoriaParaBuscar.Consecutivo
                                                 select categoria).Include(x => x.CategoriasContenidos)
                                                                  .Include(x => x.Habilidades)
                                                                  .AsNoTracking()
                                                                  .FirstOrDefaultAsync();

            return categoriaBuscada;
        }

        public async Task<List<CategoriasDTO>> ListarCategoriasPorIdioma(Categorias categoriaParaListar)
        {
            List<CategoriasDTO> listaCategorias = await _context.Categorias
                .Select(x => new CategoriasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoArchivo = x.CodigoArchivo,
                    DescripcionIdiomaBuscado = x.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    CategoriasContenidos = x.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaParaListar.CodigoIdiomaUsuarioBase)
                                            .Select(z => new CategoriasContenidosDTO
                                            {
                                                Consecutivo = z.Consecutivo,
                                                CodigoIdioma = z.CodigoIdioma,
                                                CodigoCategoria = z.CodigoCategoria,
                                                Descripcion = z.Descripcion
                                            })
                                            .ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategorias;
        }

        public void EliminarCategoria(Categorias categoriaParaEliminar)
        {
            _context.Categorias.Attach(categoriaParaEliminar);
            _context.Categorias.Remove(categoriaParaEliminar);
        }


        #endregion


        #region Metodos CategoriasContenidos


        public void CrearCategoriasContenidos(ICollection<CategoriasContenidos> categoriaContenidoParaCrear)
        {
            categoriaContenidoParaCrear.ForEach(x => x.Descripcion.Trim());
            _context.CategoriasContenidos.AddRange(categoriaContenidoParaCrear);
        }

        public async Task<CategoriasContenidos> BuscarCategoriaContenido(CategoriasContenidos categoriaContenidoParaBuscar)
        {
            CategoriasContenidos categoriaContenidoBuscada = await (from categoriaContenido in _context.CategoriasContenidos
                                                                    where categoriaContenido.Consecutivo == categoriaContenidoParaBuscar.Consecutivo
                                                                    select categoriaContenido).Include(x => x.Idiomas)
                                                                                              .Include(x => x.Categorias)
                                                                                              .AsNoTracking()
                                                                                              .FirstOrDefaultAsync();

            return categoriaContenidoBuscada;
        }

        public async Task<List<CategoriasContenidos>> ListarContenidoDeUnaCategoria(CategoriasContenidos categoriaContenidoParaListar)
        {
            List<CategoriasContenidos> listaCategoriasContenidos = await (from categoriaContenido in _context.CategoriasContenidos
                                                                          where categoriaContenido.CodigoCategoria == categoriaContenidoParaListar.CodigoCategoria
                                                                          select categoriaContenido).Include(x => x.Idiomas)
                                                                                                    .AsNoTracking()
                                                                                                    .ToListAsync();

            return listaCategoriasContenidos;
        }

        public async Task<CategoriasContenidos> ModificarCategoriaContenido(CategoriasContenidos categoriaContenidoParaModificar)
        {
            CategoriasContenidos categoriaContenidoExistente = await _context.CategoriasContenidos.Where(x => x.Consecutivo == categoriaContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaContenidoExistente.Descripcion = categoriaContenidoParaModificar.Descripcion.Trim();

            return categoriaContenidoExistente;
        }

        public void EliminarCategoriaContenido(CategoriasContenidos categoriaContenidoParaEliminar)
        {
            _context.CategoriasContenidos.Attach(categoriaContenidoParaEliminar);
            _context.CategoriasContenidos.Remove(categoriaContenidoParaEliminar);
        }

        public void EliminarMultiplesCategoriasContenidos(CategoriasContenidos categoriaContenidoParaBorrar)
        {
            _context.CategoriasContenidos.RemoveRange(_context.CategoriasContenidos.Where(x => x.CodigoCategoria == categoriaContenidoParaBorrar.CodigoCategoria));
        }


        #endregion


        #region Metodo CategoriasAnuncios


        public void CrearListaCategoriaAnuncios(ICollection<CategoriasAnuncios> categoriaAnuncioParaCrear)
        {
            _context.CategoriasAnuncios.AddRange(categoriaAnuncioParaCrear);
        }

        public void CrearCategoriaAnuncios(CategoriasAnuncios categoriaAnuncioParaCrear)
        {
            _context.CategoriasAnuncios.Add(categoriaAnuncioParaCrear);
        }

        public async Task<CategoriasAnunciosDTO> BuscarCategoriaAnuncioPorConsecutivoAndIdioma(CategoriasAnuncios categoriaAnuncioParaBuscar)
        {
            CategoriasAnunciosDTO categoriaAnuncioBuscada = await _context.CategoriasAnuncios.Where(x => x.Consecutivo == categoriaAnuncioParaBuscar.Consecutivo)
                .Select(x => new CategoriasAnunciosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoAnuncio = x.CodigoAnuncio,
                    CodigoCategoria = x.CodigoCategoria,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaAnuncioParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaAnuncioParaBuscar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaAnuncioBuscada;
        }

        public async Task<List<CategoriasAnunciosDTO>> ListarCategoriasDeUnAnuncioPorIdioma(CategoriasAnuncios categoriaAnuncioParaListar)
        {
            IQueryable<CategoriasAnuncios> queryAnuncios = _context.CategoriasAnuncios.Where(x => x.CodigoAnuncio == categoriaAnuncioParaListar.CodigoAnuncio).AsQueryable();

            List<CategoriasAnunciosDTO> listaCategoriasDeUnAnuncio = await queryAnuncios
                .Select(x => new CategoriasAnunciosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoAnuncio = x.CodigoAnuncio,
                    CodigoCategoria = x.CodigoCategoria,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaAnuncioParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaAnuncioParaListar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategoriasDeUnAnuncio;
        }

        public async Task<CategoriasAnuncios> ModificarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaModificar)
        {
            CategoriasAnuncios categoriaAnuncioExistente = await _context.CategoriasAnuncios.Where(x => x.Consecutivo == categoriaAnuncioParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaAnuncioExistente.CodigoCategoria = categoriaAnuncioParaModificar.CodigoCategoria;

            return categoriaAnuncioExistente;
        }

        public void EliminarCategoriaAnuncio(CategoriasAnuncios categoriaAnuncioParaBorrar)
        {
            _context.CategoriasAnuncios.Attach(categoriaAnuncioParaBorrar);
            _context.CategoriasAnuncios.Remove(categoriaAnuncioParaBorrar);
        }

        public void EliminarMultiplesCategoriasAnuncios(CategoriasAnuncios categoriaAnuncioParaBorrar)
        {
            _context.CategoriasAnuncios.RemoveRange(_context.CategoriasAnuncios.Where(x => x.CodigoAnuncio == categoriaAnuncioParaBorrar.CodigoAnuncio));
        }

        #endregion


        #region Metodos CategoriasCandidatos


        public void CrearCategoriaCandidatos(CategoriasCandidatos categoriaCandidatoParaCrear)
        {
            _context.CategoriasCandidatos.Add(categoriaCandidatoParaCrear);
        }

        public async Task<CategoriasCandidatosDTO> BuscarCategoriaCandidatoPorConsecutivoAndIdioma(CategoriasCandidatos categoriaCandidatoParaBuscar)
        {
            IQueryable<CategoriasCandidatos> queryCandidato = _context.CategoriasCandidatos.Where(x => x.Consecutivo == categoriaCandidatoParaBuscar.Consecutivo).AsQueryable();

            CategoriasCandidatosDTO categoriaCandidatoBuscada = await queryCandidato
                .Select(x => new CategoriasCandidatosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCandidato = x.CodigoCandidato,
                    CodigoCategoria = x.CodigoCategoria,
                    PosicionCampo = x.PosicionCampo,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaCandidatoParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaCandidatoParaBuscar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaCandidatoBuscada;
        }

        public async Task<int?> BuscarCodigoCandidatoDeUnaCategoriaCandidato(int consecutivoCategoriaCandidato)
        {
            int? codigoCandidato = await _context.CategoriasCandidatos.Where(x => x.Consecutivo == consecutivoCategoriaCandidato)
                                                                      .Select(x => x.CodigoCandidato).FirstOrDefaultAsync();

            return codigoCandidato;
        }

        public async Task<List<CategoriasCandidatosDTO>> ListarCategoriasDeUnCandidatoPorIdioma(CategoriasCandidatos categoriaCandidatoParaListar)
        {
            IQueryable<CategoriasCandidatos> queryCandidatos = _context.CategoriasCandidatos.Where(x => x.CodigoCandidato == categoriaCandidatoParaListar.CodigoCandidato).AsQueryable();

            List<CategoriasCandidatosDTO> listaCategoriasCandidatos = await queryCandidatos
                .Select(x => new CategoriasCandidatosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCandidato = x.CodigoCandidato,
                    CodigoCategoria = x.CodigoCategoria,
                    PosicionCampo = x.PosicionCampo,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaCandidatoParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaCandidatoParaListar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategoriasCandidatos;
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnCandidato(int codigoPersona)
        {
            CandidatosDTO candidato = await _context.Candidatos.Where(x => x.CodigoPersona == codigoPersona)
                                                               .Select(x => new CandidatosDTO
                                                               {
                                                                   CategoriasCandidatos = x.CategoriasCandidatos.Where(z => z.CodigoCandidato == x.Consecutivo)
                                                                                           .Select(z => new CategoriasCandidatosDTO
                                                                                           {
                                                                                               CodigoCategoria = z.CodigoCategoria,
                                                                                           }).ToList(),
                                                               })
                                                               .FirstOrDefaultAsync();

            return candidato.CategoriasCandidatos.Select(x => x.CodigoCategoria).ToList();
        }

        public async Task<CategoriasCandidatos> ModificarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaModificar)
        {
            CategoriasCandidatos categoriaCandidatoExistente = await _context.CategoriasCandidatos.Where(x => x.Consecutivo == categoriaCandidatoParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaCandidatoExistente.CodigoCategoria = categoriaCandidatoParaModificar.CodigoCategoria;
            categoriaCandidatoExistente.PosicionCampo = categoriaCandidatoParaModificar.PosicionCampo;

            return categoriaCandidatoExistente;
        }

        public void EliminarCategoriaCandidato(CategoriasCandidatos categoriaCandidatoParaBorrar)
        {
            _context.CategoriasCandidatos.Attach(categoriaCandidatoParaBorrar);
            _context.CategoriasCandidatos.Remove(categoriaCandidatoParaBorrar);
        }


        #endregion


        #region Metodos CategoriasEventos


        public void CrearListaCategoriaEventos(ICollection<CategoriasEventos> categoriaEventoParaCrear)
        {
            _context.CategoriasEventos.AddRange(categoriaEventoParaCrear);
        }

        public void CrearCategoriaEventos(CategoriasEventos categoriaEventoParaCrear)
        {
            _context.CategoriasEventos.Add(categoriaEventoParaCrear);
        }

        public async Task<CategoriasEventosDTO> BuscarCategoriaEventoPorConsecutivoAndIdioma(CategoriasEventos categoriaEventoParaBuscar)
        {
            IQueryable<CategoriasEventos> queryCategoria = _context.CategoriasEventos.Where(x => x.Consecutivo == categoriaEventoParaBuscar.Consecutivo).AsQueryable();

            CategoriasEventosDTO categoriaEventoBuscada = await queryCategoria
                .Select(x => new CategoriasEventosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoEvento = x.CodigoEvento,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaEventoParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaEventoParaBuscar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaEventoBuscada;
        }

        public async Task<List<CategoriasEventosDTO>> ListarCategoriasDeUnEventoPorIdioma(CategoriasEventos categoriaEventoParaListar)
        {
            IQueryable<CategoriasEventos> queryEventos = _context.CategoriasEventos.Where(x => x.CodigoEvento == categoriaEventoParaListar.CodigoEvento).AsQueryable();

            List<CategoriasEventosDTO> listaCategoriaEvento = await queryEventos
                .Select(x => new CategoriasEventosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoEvento = x.CodigoEvento,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaEventoParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaEventoParaListar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategoriaEvento;
        }

        public async Task<CategoriasEventos> ModificarCategoriaEvento(CategoriasEventos categoriaEventoParaModificar)
        {
            CategoriasEventos categoriaEventoExistente = await _context.CategoriasEventos.Where(x => x.Consecutivo == categoriaEventoParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaEventoExistente.CodigoCategoria = categoriaEventoParaModificar.CodigoCategoria;

            return categoriaEventoExistente;
        }

        public void EliminarCategoriaEvento(CategoriasEventos categoriaEventoParaBorrar)
        {
            _context.CategoriasEventos.Attach(categoriaEventoParaBorrar);
            _context.CategoriasEventos.Remove(categoriaEventoParaBorrar);
        }

        public void EliminarMultiplesCategoriasEventos(CategoriasEventos categoriaEventoParaBorrar)
        {
            _context.CategoriasEventos.RemoveRange(_context.CategoriasEventos.Where(x => x.CodigoEvento == categoriaEventoParaBorrar.CodigoEvento));
        }


        #endregion


        #region Metodos CategoriasGrupos


        public void CrearListaCategoriaGrupos(ICollection<CategoriasGrupos> categoriaGrupoParaCrear)
        {
            _context.CategoriasGrupos.AddRange(categoriaGrupoParaCrear);
        }

        public void CrearCategoriaGrupos(CategoriasGrupos categoriaGrupoParaCrear)
        {
            _context.CategoriasGrupos.Add(categoriaGrupoParaCrear);
        }

        public async Task<CategoriasGruposDTO> BuscarCategoriaGrupoPorConsecutivoAndIdioma(CategoriasGrupos categoriaGrupoParaBuscar)
        {
            IQueryable<CategoriasGrupos> queryCategoriasGrupo = _context.CategoriasGrupos.Where(x => x.Consecutivo == categoriaGrupoParaBuscar.Consecutivo).AsQueryable();

            CategoriasGruposDTO categoriaGrupoBuscada = await queryCategoriasGrupo
                .Select(x => new CategoriasGruposDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoGrupo = x.CodigoGrupo,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaGrupoParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaGrupoParaBuscar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaGrupoBuscada;
        }

        public async Task<List<CategoriasGruposDTO>> ListarCategoriasDeUnGrupoPorIdioma(CategoriasGrupos categoriaGrupoParaListar)
        {
            IQueryable<CategoriasGrupos> queryCategoriaGrupo = _context.CategoriasGrupos.Where(x => x.CodigoGrupo == categoriaGrupoParaListar.CodigoGrupo).AsQueryable();

            List<CategoriasGruposDTO> listaCategoriaGrupo = await queryCategoriaGrupo
                .Select(x => new CategoriasGruposDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoGrupo = x.CodigoGrupo,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaGrupoParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaGrupoParaListar.CodigoIdiomaUsuarioBase)
                                                .Select(z => new CategoriasContenidosDTO
                                                {
                                                    Consecutivo = z.Consecutivo,
                                                    Descripcion = z.Descripcion,
                                                    CodigoIdioma = z.CodigoIdioma
                                                }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();


            return listaCategoriaGrupo;
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnGrupo(int codigoPersona)
        {
            GruposDTO grupo = await _context.Grupos.Where(x => x.CodigoPersona == codigoPersona)
                                                   .Select(x => new GruposDTO
                                                   {
                                                       CategoriasGrupos = x.CategoriasGrupos.Where(z => z.CodigoGrupo == x.Consecutivo)
                                                                               .Select(z => new CategoriasGruposDTO
                                                                               {
                                                                                   CodigoCategoria = z.CodigoCategoria,
                                                                               }).ToList(),
                                                   })
                                                   .FirstOrDefaultAsync();

            return grupo.CategoriasGrupos.Select(x => x.CodigoCategoria).ToList();
        }

        public async Task<CategoriasGrupos> ModificarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaModificar)
        {
            CategoriasGrupos categoriaGrupoExistente = await _context.CategoriasGrupos.Where(x => x.Consecutivo == categoriaGrupoParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaGrupoExistente.CodigoCategoria = categoriaGrupoParaModificar.CodigoCategoria;

            return categoriaGrupoExistente;
        }

        public void EliminarCategoriaGrupo(CategoriasGrupos categoriaGrupoParaBorrar)
        {
            _context.CategoriasGrupos.Attach(categoriaGrupoParaBorrar);
            _context.CategoriasGrupos.Remove(categoriaGrupoParaBorrar);
        }


        #endregion


        #region Metodos CategoriasRepresentantes


        public void CrearListaCategoriaRepresentante(ICollection<CategoriasRepresentantes> categoriaRepresentanteParaCrear)
        {
            _context.CategoriasRepresentantes.AddRange(categoriaRepresentanteParaCrear);
        }

        public void CrearCategoriaRepresentante(CategoriasRepresentantes categoriaRepresentanteParaCrear)
        {
            _context.CategoriasRepresentantes.Add(categoriaRepresentanteParaCrear);
        }

        public async Task<int?> BuscarCodigoCandidatoDeUnaCategoriaGrupo(int consecutivoCategoriaGrupo)
        {
            int? codigoGrupo = await _context.CategoriasGrupos.Where(x => x.Consecutivo == consecutivoCategoriaGrupo)
                                                              .Select(x => x.CodigoGrupo).FirstOrDefaultAsync();

            return codigoGrupo;
        }

        public async Task<CategoriasRepresentantesDTO> BuscarCategoriaRepresentantePorConsecutivoAndIdioma(CategoriasRepresentantes categoriaRepresentantesParaBuscar)
        {
            IQueryable<CategoriasRepresentantes> queryRepresentante = _context.CategoriasRepresentantes.Where(x => x.Consecutivo == categoriaRepresentantesParaBuscar.Consecutivo).AsQueryable();

            CategoriasRepresentantesDTO categoriaRepresentanteBuscada = await queryRepresentante
                .Select(x => new CategoriasRepresentantesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoRepresentante = x.CodigoRepresentante,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaRepresentantesParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaRepresentantesParaBuscar.CodigoIdiomaUsuarioBase)
                            .Select(z => new CategoriasContenidosDTO
                            {
                                Consecutivo = z.Consecutivo,
                                Descripcion = z.Descripcion,
                                CodigoIdioma = z.CodigoIdioma
                            }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaRepresentanteBuscada;
        }

        public async Task<List<CategoriasRepresentantesDTO>> ListarCategoriasDeUnRepresentantePorIdioma(CategoriasRepresentantes categoriaRepresentanteParaListar)
        {
            IQueryable<CategoriasRepresentantes> queryRepresentantes = _context.CategoriasRepresentantes.Where(x => x.CodigoRepresentante == categoriaRepresentanteParaListar.CodigoRepresentante).AsQueryable();

            List<CategoriasRepresentantesDTO> listaCategoriasRepresentantes = await queryRepresentantes
                .Select(x => new CategoriasRepresentantesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoRepresentante = x.CodigoRepresentante,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaRepresentanteParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaRepresentanteParaListar.CodigoIdiomaUsuarioBase)
                            .Select(z => new CategoriasContenidosDTO
                            {
                                Consecutivo = z.Consecutivo,
                                Descripcion = z.Descripcion,
                                CodigoIdioma = z.CodigoIdioma
                            }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategoriasRepresentantes;
        }

        public async Task<List<int>> ListarCodigoCategoriasDeUnRepresentante(int codigoPersona)
        {
            RepresentantesDTO representante = await _context.Representantes.Where(x => x.CodigoPersona == codigoPersona)
                                                               .Select(x => new RepresentantesDTO
                                                               {
                                                                   CategoriasRepresentantes = x.CategoriasRepresentantes.Where(z => z.CodigoRepresentante == x.Consecutivo)
                                                                                           .Select(z => new CategoriasRepresentantesDTO
                                                                                           {
                                                                                               CodigoCategoria = z.CodigoCategoria,
                                                                                           }).ToList(),
                                                               })
                                                               .FirstOrDefaultAsync();

            return representante.CategoriasRepresentantes.Select(x => x.CodigoCategoria).ToList();
        }

        public async Task<CategoriasRepresentantes> ModificarCategoriaRepresentante(CategoriasRepresentantes categoriaReprensentateParaModificar)
        {
            CategoriasRepresentantes categoriaRepresentanteExistente = await _context.CategoriasRepresentantes.Where(x => x.Consecutivo == categoriaReprensentateParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaRepresentanteExistente.CodigoCategoria = categoriaReprensentateParaModificar.CodigoCategoria;

            return categoriaRepresentanteExistente;
        }

        public void EliminarCategoriaRepresentante(CategoriasRepresentantes categoriaRepresanteParaBorrar)
        {
            _context.CategoriasRepresentantes.Attach(categoriaRepresanteParaBorrar);
            _context.CategoriasRepresentantes.Remove(categoriaRepresanteParaBorrar);
        }


        #endregion


        #region Metodos CategoriasNoticias


        public void CrearListaCategoriaNoticias(ICollection<CategoriasNoticias> categoriaNoticiasParaCrear)
        {
            _context.CategoriasNoticias.AddRange(categoriaNoticiasParaCrear);
        }

        public void CrearCategoriaNoticias(CategoriasNoticias categoriaNoticiasParaCrear)
        {
            _context.CategoriasNoticias.Add(categoriaNoticiasParaCrear);
        }

        public async Task<CategoriasNoticiasDTO> BuscarCategoriasNoticiasPorConsecutivoAndIdioma(CategoriasNoticias categoriaNoticiasParaBuscar)
        {
            IQueryable<CategoriasNoticias> queryRepresentante = _context.CategoriasNoticias.Where(x => x.Consecutivo == categoriaNoticiasParaBuscar.Consecutivo).AsQueryable();

            CategoriasNoticiasDTO categoriaRepresentanteBuscada = await queryRepresentante
                .Select(x => new CategoriasNoticiasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoNoticia = x.CodigoNoticia,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaNoticiasParaBuscar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaNoticiasParaBuscar.CodigoIdiomaUsuarioBase)
                            .Select(z => new CategoriasContenidosDTO
                            {
                                Consecutivo = z.Consecutivo,
                                Descripcion = z.Descripcion,
                                CodigoIdioma = z.CodigoIdioma
                            }).ToList()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoriaRepresentanteBuscada;
        }

        public async Task<List<CategoriasNoticiasDTO>> ListarCategoriasDeUnaNoticiaPorIdioma(CategoriasNoticias categoriaNoticiaParaListar)
        {
            IQueryable<CategoriasNoticias> queryNoticias = _context.CategoriasNoticias.Where(x => x.CodigoNoticia == categoriaNoticiaParaListar.CodigoNoticia).AsQueryable();

            List<CategoriasNoticiasDTO> listaCategoriasRepresentantes = await queryNoticias
                .Select(x => new CategoriasNoticiasDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoNoticia = x.CodigoNoticia,
                    Categorias = new CategoriasDTO
                    {
                        Consecutivo = x.Categorias.Consecutivo,
                        CodigoArchivo = x.Categorias.CodigoArchivo,
                        DescripcionIdiomaBuscado = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaNoticiaParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                        CategoriasContenidos = x.Categorias.CategoriasContenidos.Where(y => y.CodigoIdioma == categoriaNoticiaParaListar.CodigoIdiomaUsuarioBase)
                            .Select(z => new CategoriasContenidosDTO
                            {
                                Consecutivo = z.Consecutivo,
                                Descripcion = z.Descripcion,
                                CodigoIdioma = z.CodigoIdioma
                            }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaCategoriasRepresentantes;
        }

        public async Task<int?> BuscarCodigoCandidatoDeUnaCategoriaRepresentante(int consecutivoCategoriaRepresentante)
        {
            int? codigoRepresentante = await _context.CategoriasGrupos.Where(x => x.Consecutivo == consecutivoCategoriaRepresentante)
                                                                      .Select(x => x.CodigoGrupo).FirstOrDefaultAsync();

            return codigoRepresentante;
        }

        public async Task<CategoriasNoticias> ModificarCategoriaNoticia(CategoriasNoticias categoriaNoticiaParaModificar)
        {
            CategoriasNoticias categoriaNoticiaExistente = await _context.CategoriasNoticias.Where(x => x.Consecutivo == categoriaNoticiaParaModificar.Consecutivo).FirstOrDefaultAsync();

            categoriaNoticiaExistente.CodigoCategoria = categoriaNoticiaParaModificar.CodigoCategoria;

            return categoriaNoticiaExistente;
        }

        public void EliminarCategoriaNoticia(CategoriasNoticias categoriaNoticiaParaBorrar)
        {
            _context.CategoriasNoticias.Attach(categoriaNoticiaParaBorrar);
            _context.CategoriasNoticias.Remove(categoriaNoticiaParaBorrar);
        }

        public void EliminarMultiplesCategoriasNoticias(CategoriasNoticias categoriaNoticiaParaBorrar)
        {
            _context.CategoriasNoticias.RemoveRange(_context.CategoriasNoticias.Where(x => x.CodigoNoticia == categoriaNoticiaParaBorrar.CodigoNoticia));
        }


        #endregion


    }
}
