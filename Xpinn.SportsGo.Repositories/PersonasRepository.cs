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
    public class PersonasRepository
    {
        SportsGoEntities _context;

        public PersonasRepository(SportsGoEntities context)
        {
            _context = context;
        }


        public async Task<PersonasDTO> BuscarPersona(Personas personaParaBuscar)
        {
            PersonasDTO personaBuscada = await (from persona in _context.Personas
                                             where persona.Consecutivo == personaParaBuscar.Consecutivo
                                             select new PersonasDTO
                                             {
                                                 Consecutivo = persona.Consecutivo,
                                                 Nombres = persona.Nombres,
                                                 Apellidos = persona.Apellidos,
                                                 CodigoIdioma = persona.CodigoIdioma,
                                                 CodigoUsuario = persona.CodigoUsuario,
                                                 CodigoArchivoImagenPerfil = persona.CodigoArchivoImagenPerfil,
                                                 CodigoArchivoImagenBanner = persona.CodigoArchivoImagenBanner,
                                                 CodigoTipoPerfil = persona.CodigoTipoPerfil,
                                                 CiudadResidencia = persona.CiudadResidencia,
                                                 CodigoPais = persona.CodigoPais,
                                                 Telefono = persona.Telefono,
                                                 Skype = persona.Skype,
                                                 YaEstaAgregadaContactos = _context.Contactos.Where(x => x.CodigoPersonaOwner == personaParaBuscar.ConsecutivoViendoPersona && x.CodigoPersonaContacto == persona.Consecutivo).Any(),
                                                 ConsecutivoContacto = _context.Contactos.Where(x => x.CodigoPersonaOwner == personaParaBuscar.ConsecutivoViendoPersona && x.CodigoPersonaContacto == persona.Consecutivo).Select(x => x.Consecutivo).FirstOrDefault(),
                                                 Paises = new PaisesDTO
                                                 {
                                                     Consecutivo = persona.Paises.Consecutivo,
                                                     CodigoIdioma = persona.Paises.CodigoIdioma,
                                                     CodigoArchivo = persona.Paises.CodigoArchivo,
                                                     CodigoMoneda = persona.Paises.CodigoMoneda,
                                                     DescripcionIdiomaBuscado = persona.Paises.PaisesContenidos.Where(z => z.CodigoIdioma == personaParaBuscar.CodigoIdioma).Select(z => z.Descripcion).FirstOrDefault(),
                                                     Monedas = new MonedasDTO
                                                     {
                                                         Consecutivo = persona.Paises.Monedas.Consecutivo,
                                                         AbreviaturaMoneda = persona.Paises.Monedas.AbreviaturaMoneda
                                                     }
                                                 },
                                                 Usuarios = new UsuariosDTO
                                                 {
                                                     Consecutivo = persona.Usuarios.Consecutivo,
                                                     Email = persona.Usuarios.Email
                                                 },
                                                 Candidatos = persona.Candidatos.Where(y => y.CodigoPersona == personaParaBuscar.Consecutivo)
                                                                                           .Select(y => new CandidatosDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                               CodigoPersona = y.CodigoPersona,
                                                                                               CodigoResponsable = y.CodigoResponsable,
                                                                                               CodigoGenero = y.CodigoGenero,
                                                                                               Estatura = y.Estatura,
                                                                                               Peso = y.Peso,
                                                                                               Biografia = y.Biografia,
                                                                                               FechaNacimiento = y.FechaNacimiento,
                                                                                               Alias = y.Alias,
                                                                                               CategoriasCandidatos = y.CategoriasCandidatos.Where(z => z.CodigoCandidato == y.Consecutivo)
                                                                                                                       .Select(z => new CategoriasCandidatosDTO
                                                                                                                       {
                                                                                                                           Consecutivo = z.Consecutivo,
                                                                                                                           CodigoCandidato = z.CodigoCandidato,
                                                                                                                           CodigoCategoria = z.CodigoCategoria,
                                                                                                                           PosicionCampo = z.PosicionCampo,
                                                                                                                           Categorias = new CategoriasDTO
                                                                                                                           {
                                                                                                                               Consecutivo = z.Categorias.Consecutivo,
                                                                                                                               CodigoArchivo = z.Categorias.CodigoArchivo,
                                                                                                                               DescripcionIdiomaBuscado = z.Categorias.CategoriasContenidos.Where(h => h.CodigoIdioma == personaParaBuscar.CodigoIdioma)
                                                                                                                                                           .Select(h => h.Descripcion).FirstOrDefault()
                                                                                                                           } 
                                                                                                                       }).ToList(),
                                                                                           }).ToList(),
                                                 Grupos = persona.Grupos.Where(y => y.CodigoPersona == persona.Consecutivo)
                                                                                           .Select(y => new GruposDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                               NombreContacto = y.NombreContacto,
                                                                                               CategoriasGrupos = y.CategoriasGrupos.Where(z => z.CodigoGrupo == y.Consecutivo)
                                                                                                                   .Select(z => new CategoriasGruposDTO
                                                                                                                   {
                                                                                                                       Consecutivo = z.Consecutivo,
                                                                                                                       CodigoGrupo = z.CodigoGrupo,
                                                                                                                       CodigoCategoria = z.CodigoCategoria,
                                                                                                                       Categorias = new CategoriasDTO
                                                                                                                       {
                                                                                                                           Consecutivo = z.Categorias.Consecutivo,
                                                                                                                           CodigoArchivo = z.Categorias.CodigoArchivo,
                                                                                                                           DescripcionIdiomaBuscado = z.Categorias.CategoriasContenidos.Where(h => h.CodigoIdioma == personaParaBuscar.CodigoIdioma)
                                                                                                                                                       .Select(h => h.Descripcion).FirstOrDefault()
                                                                                                                       }
                                                                                                                   }).ToList()
                                                                                           }).ToList(),
                                                 Anunciantes = persona.Anunciantes.Where(y => y.CodigoPersona == persona.Consecutivo)
                                                                                           .Select(y => new AnunciantesDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                               NumeroIdentificacion = y.NumeroIdentificacion,
                                                                                               Empresa = y.Empresa
                                                                                           }).ToList(),
                                                 Representantes = persona.Representantes.Where(y => y.CodigoPersona == persona.Consecutivo)
                                                                                           .Select(y => new RepresentantesDTO
                                                                                           {
                                                                                               Consecutivo = y.Consecutivo,
                                                                                               NumeroIdentificacion = y.NumeroIdentificacion,
                                                                                               CategoriasRepresentantes = y.CategoriasRepresentantes.Where(z => z.CodigoRepresentante == y.Consecutivo)
                                                                                                                           .Select(z => new CategoriasRepresentantesDTO
                                                                                                                           {
                                                                                                                               Consecutivo = z.Consecutivo,
                                                                                                                               CodigoRepresentante = z.CodigoRepresentante,
                                                                                                                               CodigoCategoria = z.CodigoCategoria,
                                                                                                                               Categorias = new CategoriasDTO
                                                                                                                               {
                                                                                                                                   Consecutivo = z.Categorias.Consecutivo,
                                                                                                                                   CodigoArchivo = z.Categorias.CodigoArchivo,
                                                                                                                                   DescripcionIdiomaBuscado = z.Categorias.CategoriasContenidos.Where(h => h.CodigoIdioma == personaParaBuscar.CodigoIdioma)
                                                                                                                                                               .Select(h => h.Descripcion).FirstOrDefault()
                                                                                                                               }
                                                                                                                           }).ToList()
                                                                                           }).ToList(),
                                             })
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync();

            return personaBuscada;
        }

        public async Task<Personas> ModificarPersona(Personas personaParaModificar)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaParaModificar.Consecutivo).FirstOrDefaultAsync();

            personaExistente.Nombres = personaParaModificar.Nombres.Trim();
            personaExistente.Apellidos = !string.IsNullOrWhiteSpace(personaParaModificar.Apellidos) ? personaParaModificar.Apellidos.Trim() : string.Empty;
            personaExistente.CodigoIdioma = personaParaModificar.CodigoIdioma;
            personaExistente.CodigoPais = personaParaModificar.CodigoPais;
            personaExistente.CiudadResidencia = personaParaModificar.CiudadResidencia;
            personaExistente.Telefono = personaParaModificar.Telefono;
            personaExistente.Skype = !string.IsNullOrWhiteSpace(personaParaModificar.Skype) ? personaParaModificar.Skype.Trim() : string.Empty; 

            return personaExistente;
        }

        public async Task<int> BuscarCodigoIdiomaDeLaPersona(int codigoPersona)
        {
            int codigoIdioma = await _context.Personas.Where(x => x.Consecutivo == codigoPersona).Select(x => x.CodigoIdioma).FirstOrDefaultAsync();

            return codigoIdioma;
        }

        public async Task<Personas> AsignarImagenPerfil(Personas personaParaAsignarImagenPerfil)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaParaAsignarImagenPerfil.Consecutivo).FirstOrDefaultAsync();

            personaExistente.ArchivosPerfil = personaParaAsignarImagenPerfil.ArchivosPerfil;
            personaExistente.ArchivosPerfil.CodigoTipoArchivo = personaParaAsignarImagenPerfil.ArchivosPerfil.CodigoTipoArchivo;

            return personaExistente;
        }

        public async Task<Personas> AsignarImagenBanner(Personas personaParaAsignarImagenBanner)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaParaAsignarImagenBanner.Consecutivo).FirstOrDefaultAsync();

            personaExistente.ArchivosBanner = personaParaAsignarImagenBanner.ArchivosBanner;
            personaExistente.ArchivosBanner.CodigoTipoArchivo = personaParaAsignarImagenBanner.ArchivosBanner.CodigoTipoArchivo;

            return personaExistente;
        }

        public async Task<Personas> AsignarCodigoImagenPerfil(Personas personaParaAsignarImagenPerfil)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaParaAsignarImagenPerfil.Consecutivo).FirstOrDefaultAsync();

            personaExistente.CodigoArchivoImagenPerfil = personaParaAsignarImagenPerfil.CodigoArchivoImagenPerfil;

            return personaExistente;
        }

        public async Task<Personas> AsignarCodigoImagenBanner(Personas personaParaAsignarImagenBanner)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaParaAsignarImagenBanner.Consecutivo).FirstOrDefaultAsync();

            personaExistente.CodigoArchivoImagenBanner = personaParaAsignarImagenBanner.CodigoArchivoImagenBanner;

            return personaExistente;
        }

        public async Task<int?> BuscarCodigoImagenPerfil(Personas personaImagenPerfilParaBuscar)
        {
            int? codigoImagenPerfil = await (from persona in _context.Personas
                                             where persona.Consecutivo == personaImagenPerfilParaBuscar.Consecutivo
                                             select persona.CodigoArchivoImagenPerfil).FirstOrDefaultAsync();

            return codigoImagenPerfil;
        }

        public async Task<int?> BuscarCodigoImagenBanner(Personas personaImagenBannerParaBuscar)
        {
            int? codigoImagenBanner = await (from persona in _context.Personas
                                             where persona.Consecutivo == personaImagenBannerParaBuscar.Consecutivo
                                             select persona.CodigoArchivoImagenBanner).FirstOrDefaultAsync();

            return codigoImagenBanner;
        }

        public async Task<Personas> DesasignarImagenPerfil(Personas personaImagenPerfilParaEliminar)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == personaImagenPerfilParaEliminar.Consecutivo).FirstOrDefaultAsync();

            personaExistente.CodigoArchivoImagenPerfil = null;

            return personaExistente;
        }

        public async Task<Personas> DesasignarImagenBanner(Personas PersonaImagenBannerParaBorrar)
        {
            Personas personaExistente = await _context.Personas.Where(x => x.Consecutivo == PersonaImagenBannerParaBorrar.Consecutivo).FirstOrDefaultAsync();

            personaExistente.CodigoArchivoImagenBanner = null;

            return personaExistente;
        }
    }
}
