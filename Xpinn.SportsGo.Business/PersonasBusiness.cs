using System.Collections.Generic;
using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Enums;
using System;

namespace Xpinn.SportsGo.Business
{
    public class PersonasBusiness
    {


        public async Task<PersonasDTO> BuscarPersona(Personas personaParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PersonasRepository personaRepo = new PersonasRepository(context);
                PersonasDTO personaBuscada = await personaRepo.BuscarPersona(personaParaBuscar);

                if (personaBuscada.CandidatoDeLaPersona != null)
                {
                    if (personaBuscada.CandidatoDeLaPersona.CodigoResponsable.HasValue)
                    {
                        CandidatosRepository candidatoRepo = new CandidatosRepository(context);

                        CandidatosResponsables candidatoReponsable = new CandidatosResponsables
                        {
                            Consecutivo = personaBuscada.CandidatoDeLaPersona.CodigoResponsable.Value
                        };

                        personaBuscada.CandidatoDeLaPersona.CandidatosResponsables = await candidatoRepo.BuscarSoloCandidatoResponsableDTO(candidatoReponsable);
                    }
                }

                return personaBuscada;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPersona(Personas personaParaModificar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PersonasRepository personaRepo = new PersonasRepository(context);
                Personas personaExistente = await personaRepo.ModificarPersona(personaParaModificar);

                WrapperSimpleTypesDTO wrapperModificarPersona = new WrapperSimpleTypesDTO();

                wrapperModificarPersona.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperModificarPersona.NumeroRegistrosAfectados > 0) wrapperModificarPersona.Exitoso = true;

                return wrapperModificarPersona;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfil(Personas personaParaAsignarImagenPerfil)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PersonasRepository personaRepo = new PersonasRepository(context);
                int? codigoImagenPerfil = await personaRepo.BuscarCodigoImagenPerfil(personaParaAsignarImagenPerfil);
                personaParaAsignarImagenPerfil.ArchivosPerfil.CodigoTipoArchivo = (int)TipoArchivo.Imagen;

                if (codigoImagenPerfil.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    personaParaAsignarImagenPerfil.ArchivosPerfil.Consecutivo = codigoImagenPerfil.Value;
                    archivoRepo.ModificarArchivo(personaParaAsignarImagenPerfil.ArchivosPerfil);
                }
                else
                {
                    Personas personaExistente = await personaRepo.AsignarImagenPerfil(personaParaAsignarImagenPerfil);
                }

                WrapperSimpleTypesDTO wrapperCrearImagenPerfil = new WrapperSimpleTypesDTO();

                wrapperCrearImagenPerfil.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearImagenPerfil.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearImagenPerfil.Exitoso = true;
                    wrapperCrearImagenPerfil.ConsecutivoArchivoCreado = Convert.ToInt64(personaParaAsignarImagenPerfil.CodigoArchivoImagenPerfil);
                }

                return wrapperCrearImagenPerfil;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarImagenBanner(Personas personaParaAsignarImagenBanner)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PersonasRepository personaRepo = new PersonasRepository(context);
                int? codigoImagenBanner = await personaRepo.BuscarCodigoImagenBanner(personaParaAsignarImagenBanner);
                personaParaAsignarImagenBanner.ArchivosBanner.CodigoTipoArchivo = (int)TipoArchivo.Imagen;

                if (codigoImagenBanner.HasValue)
                {
                    ArchivosRepository archivoRepo = new ArchivosRepository(context);
                    personaParaAsignarImagenBanner.ArchivosBanner.Consecutivo = codigoImagenBanner.Value;
                    archivoRepo.ModificarArchivo(personaParaAsignarImagenBanner.ArchivosBanner);
                }
                else
                {
                    Personas personaExistente = await personaRepo.AsignarImagenBanner(personaParaAsignarImagenBanner);
                }

                WrapperSimpleTypesDTO wrapperCrearImagenBanner = new WrapperSimpleTypesDTO();

                wrapperCrearImagenBanner.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperCrearImagenBanner.NumeroRegistrosAfectados > 0)
                {
                    wrapperCrearImagenBanner.Exitoso = true;
                    wrapperCrearImagenBanner.ConsecutivoArchivoCreado = Convert.ToInt64(personaParaAsignarImagenBanner.CodigoArchivoImagenBanner);
                }

                return wrapperCrearImagenBanner;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarImagenPerfil(Personas personaImagenPerfilParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                PersonasRepository personaRepo = new PersonasRepository(context);
                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = personaImagenPerfilParaBorrar.CodigoArchivoImagenPerfil.Value,
                };

                Personas personaExistente = await personaRepo.DesasignarImagenPerfil(personaImagenPerfilParaBorrar);

                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarImagenPerfil = new WrapperSimpleTypesDTO();

                wrapperEliminarImagenPerfil.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarImagenPerfil.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarImagenPerfil.Exitoso = true;
                }

                return wrapperEliminarImagenPerfil;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarImagenBanner(Personas personaImagenBannerParaBorrar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                Archivos archivoParaEliminar = new Archivos
                {
                    Consecutivo = personaImagenBannerParaBorrar.CodigoArchivoImagenBanner.Value,
                };

                PersonasRepository personaRepo = new PersonasRepository(context);
                Personas personaExistente = await personaRepo.DesasignarImagenBanner(personaImagenBannerParaBorrar);

                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarImagenPerfil = new WrapperSimpleTypesDTO();

                wrapperEliminarImagenPerfil.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarImagenPerfil.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarImagenPerfil.Exitoso = true;
                }

                return wrapperEliminarImagenPerfil;
            }
        }
    }
}
