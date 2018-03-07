using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;

namespace Xpinn.SportsGo.Repositories
{
    public class RepresentantesRepository
    {
        SportsGoEntities _context;

        public RepresentantesRepository(SportsGoEntities context)
        {
            _context = context;
        }


        #region Metodos Representantes


        public void CrearRepresentante(Representantes representanteParaCrear)
        {
            representanteParaCrear.NumeroIdentificacion = !string.IsNullOrWhiteSpace(representanteParaCrear.NumeroIdentificacion) ? representanteParaCrear.NumeroIdentificacion.Trim() : string.Empty;

            representanteParaCrear.Personas.Nombres = representanteParaCrear.Personas.Nombres.Trim();
            representanteParaCrear.Personas.CiudadResidencia = representanteParaCrear.Personas.CiudadResidencia.Trim();
            representanteParaCrear.Personas.Telefono = representanteParaCrear.Personas.Telefono.Trim();
            representanteParaCrear.Personas.Skype = !string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Skype) ? representanteParaCrear.Personas.Skype.Trim() : string.Empty;

            representanteParaCrear.Personas.Usuarios.Usuario = representanteParaCrear.Personas.Usuarios.Usuario.Trim();
            representanteParaCrear.Personas.Usuarios.Clave = representanteParaCrear.Personas.Usuarios.Clave.Trim();
            representanteParaCrear.Personas.Usuarios.Email = representanteParaCrear.Personas.Usuarios.Email.Trim();
            representanteParaCrear.Personas.Usuarios.Creacion = DateTime.Now;

            _context.Representantes.Add(representanteParaCrear);
        }

        public async Task<Representantes> BuscarRepresentantePorCodigoPersona(Representantes representanteParaBuscar)
        {
            Representantes informacionRepresentante = await (from representante in _context.Representantes
                                                             where representante.CodigoPersona == representanteParaBuscar.Personas.Consecutivo
                                                             select representante).Include(x => x.Personas)
                                                                                  .Include(x => x.CategoriasRepresentantes)
                                                                                  .AsNoTracking()
                                                                                  .FirstOrDefaultAsync();

            return informacionRepresentante;
        }

        public async Task<Representantes> BuscarRepresentantePorCodigoRepresentante(Representantes representanteParaBuscar)
        {
            Representantes informacionRepresentante = await (from representante in _context.Representantes
                                                             where representante.Consecutivo == representanteParaBuscar.Consecutivo
                                                             select representante).Include(x => x.Personas)
                                                                                  .Include(x => x.CategoriasRepresentantes)
                                                                                  .AsNoTracking()
                                                                                  .FirstOrDefaultAsync();

            return informacionRepresentante;
        }

        public async Task<List<RepresentantesDTO>> ListarRepresentantes(Representantes candidatoParaListar)
        {
            IQueryable<Representantes> queryListaRepresentantes = _context.Representantes.AsQueryable();

            int queryContador = await queryListaRepresentantes.CountAsync();

            List<RepresentantesDTO> listaRepresentantes = await queryListaRepresentantes
                .Select(x => new RepresentantesDTO
                {
                    Consecutivo = x.Consecutivo,
                    CodigoPersona = x.CodigoPersona,
                    NumeroIdentificacion = x.NumeroIdentificacion,
                    NumeroRegistrosExistentes = queryContador,
                    Personas = new PersonasDTO
                    {
                        Consecutivo = x.Personas.Consecutivo,
                        Nombres = x.Personas.Nombres,
                        CodigoIdioma = x.Personas.CodigoIdioma,
                        CodigoPais = x.Personas.CodigoPais,
                        CodigoArchivoImagenPerfil = x.Personas.CodigoArchivoImagenPerfil,
                        Skype = x.Personas.Skype,
                        CiudadResidencia = x.Personas.CiudadResidencia,
                        Telefono = x.Personas.Telefono,
                    }
                })
                .OrderBy(x => x.Personas.Nombres)
                .Skip(() => candidatoParaListar.SkipIndexBase)
                .Take(() => candidatoParaListar.TakeIndexBase)
                .ToListAsync();

            return listaRepresentantes;
        }

        public async Task<Representantes> ModificarInformacionRepresentante(Representantes representanteParaModificar)
        {
            Representantes representanteExistente = await _context.Representantes.Where(x => x.Consecutivo == representanteParaModificar.Consecutivo).FirstOrDefaultAsync();

            representanteExistente.NumeroIdentificacion = !string.IsNullOrWhiteSpace(representanteParaModificar.NumeroIdentificacion) ? representanteParaModificar.NumeroIdentificacion.Trim() : string.Empty;

            return representanteExistente;
        }


        #endregion


    }
}