using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.HelperClasses;

namespace Xpinn.SportsGo.Repositories
{
    public class HabilidadesRepository
    {
        SportsGoEntities _context;

        public HabilidadesRepository(SportsGoEntities context)
        {
            _context = context;
        }
        

        #region Metodos Habilidades


        public void CrearHabilidad(Habilidades habilidadParaCrear)
        {
            _context.Habilidades.Add(habilidadParaCrear);
        }

        public async Task<Habilidades> BuscarHabilidad(Habilidades habilidadParaBuscar)
        {
            Habilidades habilidadBuscada = await (from habilidad in _context.Habilidades
                                                  where habilidad.Consecutivo == habilidadParaBuscar.Consecutivo
                                                  select habilidad).Include(x => x.Categorias)
                                                                   .Include(x => x.Categorias.CategoriasContenidos)
                                                                   .Include(x => x.HabilidadesContenidos)
                                                                   .AsNoTracking()
                                                                   .FirstOrDefaultAsync();

            return habilidadBuscada;
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorIdioma(Habilidades habilidadParaListar)
        {
            IQueryable<Habilidades> queryHabilidadesPorCategoria = _context.Habilidades.AsQueryable();

            List<HabilidadesDTO> listaHabilidades = await queryHabilidadesPorCategoria
                .Select(x => new HabilidadesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoTipoHabilidad = x.CodigoTipoHabilidad,
                    DescripcionIdiomaBuscado = x.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    HabilidadesContenidos = x.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadParaListar.CodigoIdiomaUsuarioBase)
                                             .Select(z => new HabilidadesContenidosDTO
                                             {
                                                 Consecutivo = z.Consecutivo,
                                                 CodigoIdioma = z.CodigoIdioma,
                                                 Descripcion = z.Descripcion
                                             }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return listaHabilidades;
        }

        public async Task<List<HabilidadesDTO>> ListarHabilidadesPorCodigoCategoriaAndIdioma(Habilidades habilidadParaListar)
        {
            IQueryable<Habilidades> queryHabilidadesPorCategoria = _context.Habilidades.Where(x => x.CodigoCategoria == habilidadParaListar.CodigoCategoria).AsQueryable();

            List<HabilidadesDTO> listaHabilidadesPorCategoria = await queryHabilidadesPorCategoria
                .Select(x => new HabilidadesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoCategoria = x.CodigoCategoria,
                    CodigoTipoHabilidad = x.CodigoTipoHabilidad,
                    DescripcionIdiomaBuscado = x.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadParaListar.CodigoIdiomaUsuarioBase).Select(y => y.Descripcion).FirstOrDefault(),
                    HabilidadesContenidos = x.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadParaListar.CodigoIdiomaUsuarioBase)
                                             .Select(z => new HabilidadesContenidosDTO
                                             {
                                                 Consecutivo = z.Consecutivo,
                                                 CodigoIdioma = z.CodigoIdioma,
                                                 CodigoHabilidad = z.CodigoHabilidad,
                                                 Descripcion = z.Descripcion
                                             }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            return listaHabilidadesPorCategoria;
        }

        public async Task<List<int>> ListarCodigoHabilidadesPorCategoria(Habilidades habilidadParaListar)
        {
            List<int> listacodigos = await _context.Habilidades.Where(x => x.CodigoCategoria == habilidadParaListar.CodigoCategoria)
                                                               .Select(x => x.Consecutivo)
                                                               .ToListAsync();

            return listacodigos;
        }

        public async Task<Habilidades> ModificarHabilidad(Habilidades habilidadParaModificar)
        {
            Habilidades habilidadExistente = await _context.Habilidades.Where(x => x.Consecutivo == habilidadParaModificar.Consecutivo).FirstOrDefaultAsync();

            habilidadExistente.CodigoTipoHabilidad = habilidadParaModificar.CodigoTipoHabilidad;

            return habilidadExistente;
        }

        public void EliminarHabilidad(Habilidades habilidadParaEliminar)
        {
            _context.Habilidades.Attach(habilidadParaEliminar);
            _context.Habilidades.Remove(habilidadParaEliminar);
        }

        public void EliminarMultiplesHabilidadesPorCodigoCategoria(Habilidades habilidadParaBorrar)
        {
            _context.Habilidades.RemoveRange(_context.Habilidades.Where(x => x.CodigoCategoria == habilidadParaBorrar.CodigoCategoria));
        }


        #endregion


        #region Metodos HabilidadesCandidatos


        public void CrearHabilidadesCandidato(ICollection<HabilidadesCandidatos> habilidadCandidatoParaCrear)
        {
            _context.HabilidadesCandidatos.AddRange(habilidadCandidatoParaCrear);
        }

        public async Task<HabilidadesCandidatosDTO> BuscarHabilidadCandidatoPorIdioma(HabilidadesCandidatos habilidadCandidatoParaBuscar)
        {
            IQueryable<HabilidadesCandidatos> queryHabilidadesCandidatos = _context.HabilidadesCandidatos.Where(x => x.Consecutivo == habilidadCandidatoParaBuscar.Consecutivo).AsQueryable();

            HabilidadesCandidatosDTO habilidadesCandidatos = await queryHabilidadesCandidatos
                .Select(x => new HabilidadesCandidatosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoHabilidad = x.CodigoHabilidad,
                    CodigoCategoriaCandidato = x.CodigoCategoriaCandidato,
                    NumeroEstrellas = x.NumeroEstrellas,
                    Habilidades = new HabilidadesDTO
                    {
                        Consecutivo = x.Habilidades.Consecutivo,
                        CodigoCategoria = x.Habilidades.CodigoCategoria,
                        CodigoTipoHabilidad = x.Habilidades.CodigoTipoHabilidad,
                        DescripcionIdiomaBuscado = x.Habilidades.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadCandidatoParaBuscar.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                    }
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return habilidadesCandidatos;
        }

        public async Task<List<HabilidadesCandidatosDTO>> ListarHabilidadesCandidatoPorCategoriaCandidatoAndIdioma(HabilidadesCandidatos habilidadCandidatoParaListar)
        {
            IQueryable<HabilidadesCandidatos> queryHabilidadesCandidatos = _context.HabilidadesCandidatos.Where(x => x.CodigoCategoriaCandidato == habilidadCandidatoParaListar.CodigoCategoriaCandidato).AsQueryable();

            List<HabilidadesCandidatosDTO> listaHabilidadesCandidatos = await queryHabilidadesCandidatos
                .Select(x => new HabilidadesCandidatosDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoHabilidad = x.CodigoHabilidad,
                    CodigoCategoriaCandidato = x.CodigoCategoriaCandidato,
                    NumeroEstrellas = x.NumeroEstrellas,
                    Habilidades = new HabilidadesDTO
                    {
                        Consecutivo = x.Habilidades.Consecutivo,
                        CodigoCategoria = x.Habilidades.CodigoCategoria,
                        CodigoTipoHabilidad = x.Habilidades.CodigoTipoHabilidad,
                        DescripcionIdiomaBuscado = x.Habilidades.HabilidadesContenidos.Where(y => y.CodigoIdioma == habilidadCandidatoParaListar.CodigoIdiomaUsuarioBase).Select(z => z.Descripcion).FirstOrDefault()
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return listaHabilidadesCandidatos;
        }

        public void EliminarHabilidadCandidato(HabilidadesCandidatos habilidadCandidatoParaBorrar)
        {
            _context.HabilidadesCandidatos.Attach(habilidadCandidatoParaBorrar);
            _context.HabilidadesCandidatos.Remove(habilidadCandidatoParaBorrar);
        }

        public void EliminarMultiplesHabilidadesCandidatosPorCodigoCandidato(HabilidadesCandidatos habilidadCandidatoParaBorrar)
        {
            _context.HabilidadesCandidatos.RemoveRange(_context.HabilidadesCandidatos.Where(x => x.CodigoCategoriaCandidato == habilidadCandidatoParaBorrar.CodigoCategoriaCandidato));
        }


        #endregion


        #region Metodos HabilidadesContenidos


        public void CrearHabilidadesContenidos(ICollection<HabilidadesContenidos> habilidadContenidoParaCrear)
        {
            habilidadContenidoParaCrear.ForEach(x => x.Descripcion.Trim());
            _context.HabilidadesContenidos.AddRange(habilidadContenidoParaCrear);
        }

        public void CrearHabilidadesContenidosIndividual(HabilidadesContenidos habilidadContenidoParaCrear)
        {
            _context.HabilidadesContenidos.Add(habilidadContenidoParaCrear);
        }

        public async Task<HabilidadesContenidos> BuscarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaBuscar)
        {
            HabilidadesContenidos habilidadesContenidoBuscada = await (from habilidadContenido in _context.HabilidadesContenidos
                                                                       where habilidadContenido.Consecutivo == habilidadContenidoParaBuscar.Consecutivo
                                                                       select habilidadContenido).Include(x => x.Idiomas)
                                                                                                 .Include(x => x.Habilidades)
                                                                                                 .AsNoTracking()
                                                                                                 .FirstOrDefaultAsync();

            return habilidadesContenidoBuscada;
        }

        public async Task<List<HabilidadesContenidos>> ListarContenidoDeUnaHabilidad(HabilidadesContenidos habilidadContenidoParaListar)
        {
            List<HabilidadesContenidos> listaHabilidadesContenidos = await (from habilidadContenido in _context.HabilidadesContenidos
                                                                            where habilidadContenido.CodigoHabilidad == habilidadContenidoParaListar.CodigoHabilidad
                                                                            select habilidadContenido).Include(x => x.Idiomas)
                                                                                                      .AsNoTracking()
                                                                                                      .ToListAsync();

            return listaHabilidadesContenidos;
        }

        public async Task<HabilidadesContenidos> ModificarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaModificar)
        {
            HabilidadesContenidos habilidadContenidoExistente = await _context.HabilidadesContenidos.Where(x => x.Consecutivo == habilidadContenidoParaModificar.Consecutivo).FirstOrDefaultAsync();

            habilidadContenidoExistente.Descripcion = habilidadContenidoParaModificar.Descripcion.Trim();

            return habilidadContenidoExistente;
        }

        public void EliminarHabilidadContenido(HabilidadesContenidos habilidadContenidoParaEliminar)
        {
            _context.HabilidadesContenidos.Attach(habilidadContenidoParaEliminar);
            _context.HabilidadesContenidos.Remove(habilidadContenidoParaEliminar);
        }

        public void EliminarMultiplesHabilidadesContenidosPorCodigoHabilidad(HabilidadesContenidos habilidadesContenidosParaBorrar)
        {
            EliminarMultiplesHabilidadesContenidosPorCodigoHabilidad(habilidadesContenidosParaBorrar.CodigoHabilidad);
        }

        public void EliminarMultiplesHabilidadesContenidosPorCodigoHabilidad(int codigoHabilidadParaBorrar)
        {
            _context.HabilidadesContenidos.RemoveRange(_context.HabilidadesContenidos.Where(x => x.CodigoHabilidad == codigoHabilidadParaBorrar));
        }

        #endregion


    }
}