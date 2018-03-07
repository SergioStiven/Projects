using System.Threading.Tasks;
using Xpinn.SportsGo.Repositories;
using Xpinn.SportsGo.DomainEntities;
using System.Data.Entity;
using Xpinn.SportsGo.Entities;
using System.IO;
using Xpinn.SportsGo.Util.HelperClasses;
using Xpinn.SportsGo.Util.Portable.Enums;
using System;

namespace Xpinn.SportsGo.Business
{
    public class ArchivosBusiness
    {
        public async Task StreamArchivoRange(Archivos archivoParaBuscar, Stream outputStream, long start, long end)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                using (Stream source = await archivoRepo.StreamArchivo(archivoParaBuscar))
                {
                    MediaStreamHelper.CreatePartialContent(source, outputStream, start, end);
                }
            }
        }

        public async Task StreamArchivoFull(Archivos archivoParaBuscar, Stream outputStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                using (Stream source = await archivoRepo.StreamArchivo(archivoParaBuscar))
                {
                    await source.CopyToAsync(outputStream);
                }
            }
        }

        public async Task<long> StreamArchivoGetLenght(Archivos archivoParaBuscar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);
                return await archivoRepo.StreamArchivoGetLength(archivoParaBuscar);
            }
        }

        public async Task<WrapperSimpleTypesDTO> CrearArchivoStream(int codigoTipoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                transaction.Commit();

                if (wrapper.NumeroRegistrosAfectados > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;
                }

                return wrapper;
            }
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoStream(int consecutivoArchivo, int codigoTipoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoTipoParaModificar = new Archivos
                {
                    Consecutivo = consecutivoArchivo,
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.ModificarCodigoTipoArchivoDeUnArchivo(archivoTipoParaModificar);

                await archivoRepo.ModificarArchivoContenidoStream(consecutivoArchivo, sourceStream);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                transaction.Commit();

                if (wrapper.NumeroRegistrosAfectados > 0)
                {
                    wrapper.Exitoso = true;
                }

                return wrapper;
            }
        }

        public async Task<WrapperSimpleTypesDTO> EliminarArchivo(Archivos archivoParaEliminar)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                archivoRepo.EliminarArchivo(archivoParaEliminar);

                WrapperSimpleTypesDTO wrapperEliminarArchivo = new WrapperSimpleTypesDTO();

                wrapperEliminarArchivo.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                if (wrapperEliminarArchivo.NumeroRegistrosAfectados > 0)
                {
                    wrapperEliminarArchivo.Exitoso = true;
                }

                return wrapperEliminarArchivo;
            }
        }


        #region Metodos Imagenes Perfil/Banner Persona


        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfilPersona(int codigoPersona, int codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    PersonasRepository personaRepo = new PersonasRepository(context);
                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    Personas persona = new Personas
                    {
                        Consecutivo = codigoPersona,
                        CodigoArchivoImagenPerfil = archivoParaCrear.Consecutivo
                    };

                    Personas personaExistente = await personaRepo.AsignarCodigoImagenPerfil(persona);

                    // Elimino el viejo archivo
                    if (codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }

        public async Task<WrapperSimpleTypesDTO> AsignarImagenBannerPersona(int codigoPersona, int codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    PersonasRepository personaRepo = new PersonasRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    Personas persona = new Personas
                    {
                        Consecutivo = codigoPersona,
                        CodigoArchivoImagenBanner = archivoParaCrear.Consecutivo
                    };

                    Personas personaExistente = await personaRepo.AsignarCodigoImagenBanner(persona);

                    // Elimino el viejo archivo
                    if (codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


        #region Metodos Archivo Recibo Pago


        public async Task<WrapperSimpleTypesDTO> AsignarArchivoReciboPago(int codigoHistorialPago, int codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = (int)TipoArchivo.Imagen,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    PagosRepository pagosRepo = new PagosRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    HistorialPagosPersonas pagoParaAsignar = new HistorialPagosPersonas
                    {
                        Consecutivo = codigoHistorialPago,
                        CodigoArchivo = archivoParaCrear.Consecutivo
                    };

                    HistorialPagosPersonas pagoExistente = await pagosRepo.AsignarArchivoHistorialPago(pagoParaAsignar);

                    // Elimino el viejo archivo
                    if (codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


        #region Metodos Archivos Anuncios


        public async Task<WrapperSimpleTypesDTO> ModificarArchivoAnuncio(int codigoTipoArchivo, int codigoAnuncio, int? codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    AnunciantesRepository anuncioRepo = new AnunciantesRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    Anuncios anuncio = new Anuncios
                    {
                        Consecutivo = codigoAnuncio,
                        CodigoArchivo = archivoParaCrear.Consecutivo
                    };

                    Anuncios anuncioExistente = await anuncioRepo.ModificarCodigoArchivoAnuncio(anuncio);

                    // Elimino el viejo archivo
                    if (codigoArchivo.HasValue && codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo.Value
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


        #region Metodos Archivos Noticias


        public async Task<WrapperSimpleTypesDTO> ModificarArchivoNoticia(int codigoTipoArchivo, int codigoNoticia, int? codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    NoticiasRepository noticiasRepo = new NoticiasRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    Noticias noticia = new Noticias
                    {
                        Consecutivo = codigoNoticia,
                        CodigoArchivo = archivoParaCrear.Consecutivo
                    };

                    Noticias noticiaExistente = await noticiasRepo.ModificarCodigoArchivoNoticia(noticia);

                    // Elimino el viejo archivo
                    if (codigoArchivo.HasValue && codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo.Value
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


        #region Metodos Archivos Eventos


        public async Task<WrapperSimpleTypesDTO> ModificarArchivoEventos(int codigoTipoArchivo, int codigoEventos, int? codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    GruposRepository grupoRepo = new GruposRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    GruposEventos grupoEvento = new GruposEventos
                    {
                        Consecutivo = codigoEventos,
                        CodigoArchivo = archivoParaCrear.Consecutivo
                    };

                    GruposEventos grupoEventoExistente = await grupoRepo.ModificarCodigoArchivoGrupoEvento(grupoEvento);

                    // Elimino el viejo archivo
                    if (codigoArchivo.HasValue && codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo.Value
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


        #region Metodos Archivos CandidatoVideo


        public async Task<WrapperSimpleTypesDTO> ModificarArchivoCandidatoVideos(int codigoTipoArchivo, int codigoCandidatoVideo, int codigoArchivo, Stream sourceStream)
        {
            using (SportsGoEntities context = new SportsGoEntities(false))
            using (DbContextTransaction transaction = context.Database.BeginTransaction())
            {
                ArchivosRepository archivoRepo = new ArchivosRepository(context);

                Archivos archivoParaCrear = new Archivos
                {
                    CodigoTipoArchivo = codigoTipoArchivo,
                    ArchivoContenido = new byte[] { 0, 1, 2 }
                };

                archivoRepo.CrearArchivo(archivoParaCrear);

                WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();
                wrapper.NumeroRegistrosAfectados = await context.SaveChangesAsync();

                await archivoRepo.ModificarArchivoContenidoStream(archivoParaCrear.Consecutivo, sourceStream);

                if (wrapper.NumeroRegistrosAfectados > 0 || archivoParaCrear.Consecutivo > 0)
                {
                    wrapper.Exitoso = true;
                    wrapper.ConsecutivoCreado = archivoParaCrear.Consecutivo;
                    wrapper.ConsecutivoArchivoCreado = archivoParaCrear.Consecutivo;

                    CandidatosRepository candidatosRepo = new CandidatosRepository(context);

                    // Meto el consecutivo del archivo generado por el EF por la creacion
                    CandidatosVideos candidatoVideo = new CandidatosVideos
                    {
                        Consecutivo = codigoCandidatoVideo,
                        CodigoArchivo = archivoParaCrear.Consecutivo
                    };

                    CandidatosVideos candidatoVideoExistente = await candidatosRepo.ModificarCodigoArchivoCandidatoVideo(candidatoVideo);

                    // Elimino el viejo archivo
                    if (codigoArchivo > 0)
                    {
                        Archivos archivoParaBorrar = new Archivos
                        {
                            Consecutivo = codigoArchivo
                        };

                        archivoRepo.EliminarArchivo(archivoParaBorrar);
                    }

                    wrapper.NumeroRegistrosAfectados += await context.SaveChangesAsync();

                    transaction.Commit();
                }

                return wrapper;
            }
        }


        #endregion


    }
}