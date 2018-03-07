using NReco.VideoConverter;
using NReco.VideoInfo;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.WebAPI.Infraestructure;

namespace Xpinn.SportsGo.WebAPI.Controllers
{
    public class ArchivosController : ApiController
    {
        ArchivosBusiness _archivoBusiness;

        public ArchivosController()
        {
            _archivoBusiness = new ArchivosBusiness();
        }

        [HttpGet]
        [Route("api/Archivos/BuscarArchivo/{id}/{tipoArchivo}")]
        public IHttpActionResult BuscarArchivo(int id, int tipoArchivo)
        {
            if (id <= 0) return BadRequest("Id del archivo invalido!.");
            if (tipoArchivo <= 0) return BadRequest("tipoArchivo del archivo invalido!.");

            try
            {
                Archivos archivoParaStremear = new Archivos
                {
                    Consecutivo = id,
                    CodigoTipoArchivo = tipoArchivo
                };

                return new FileStreamAPI(_archivoBusiness, archivoParaStremear, Request.Headers.Range);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/CrearArchivoStream/{codigoTipoArchivo}")]
        public async Task<IHttpActionResult> CrearArchivoStream(int codigoTipoArchivo)
        {
            if (codigoTipoArchivo <= 0)
            {
                return BadRequest("codigoTipoArchivo nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();
                        
                        FFMpegConverter ffMpeg = new FFMpegConverter();
                        ffMpeg.ConvertMedia(fullFileNameToVerify, fullFileNameToCopy, Format.mp4);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperCrearArchivoStream = await _archivoBusiness.CrearArchivoStream(codigoTipoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperCrearArchivoStream);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperCrearArchivoStream = await _archivoBusiness.CrearArchivoStream(codigoTipoArchivo, sourceStream);

                        return Ok(wrapperCrearArchivoStream);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/CrearArchivoStreamYControlarDuracionVideo/{codigoTipoArchivo}/{duracionVideoPermitida}")]
        public async Task<IHttpActionResult> CrearArchivoStreamYControlarDuracionVideo(int codigoTipoArchivo, int duracionVideoPermitida)
        {
            if (codigoTipoArchivo <= 0 || duracionVideoPermitida < AppConstants.MinimoSegundos || duracionVideoPermitida > AppConstants.MaximoSegundos)
            {
                return BadRequest("codigoTipoArchivo nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();

                        ConvertSettings outputSettings = new ConvertSettings
                        {
                            MaxDuration = duracionVideoPermitida
                        };

                        ffMpeg.ConvertMedia(fullFileNameToVerify, null, fullFileNameToCopy, Format.mp4, outputSettings);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperCrearArchivoStream = await _archivoBusiness.CrearArchivoStream(codigoTipoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperCrearArchivoStream);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperCrearArchivoStream = await _archivoBusiness.CrearArchivoStream(codigoTipoArchivo, sourceStream);

                        return Ok(wrapperCrearArchivoStream);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/ModificarArchivoStream/{consecutivoArchivo}/{codigoTipoArchivo}")]
        public async Task<IHttpActionResult> ModificarArchivoStream(int consecutivoArchivo, int codigoTipoArchivo)
        {
            if (consecutivoArchivo <= 0 || codigoTipoArchivo <= 0)
            {
                return BadRequest("consecutivoArchivo o codigoTipoArchivo nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();
                        ffMpeg.ConvertMedia(fullFileNameToVerify, fullFileNameToCopy, Format.mp4);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperModificarArchivoStream = await _archivoBusiness.ModificarArchivoStream(consecutivoArchivo, codigoTipoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperModificarArchivoStream);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperModificarArchivoStream = await _archivoBusiness.ModificarArchivoStream(consecutivoArchivo, codigoTipoArchivo, sourceStream);

                        return Ok(wrapperModificarArchivoStream);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/AsignarImagenPerfilPersona/{codigoPersona}/{codigoArchivo}")]
        public async Task<IHttpActionResult> AsignarImagenPerfilPersona(int codigoPersona, int codigoArchivo)
        {
            if (codigoPersona <= 0)
            {
                return BadRequest("codigoPersona nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    WrapperSimpleTypesDTO wrapperAsignarImagenPerfilPersona = await _archivoBusiness.AsignarImagenPerfilPersona(codigoPersona, codigoArchivo, sourceStream);

                    return Ok(wrapperAsignarImagenPerfilPersona);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/AsignarImagenBannerPersona/{codigoPersona}/{codigoArchivo}")]
        public async Task<IHttpActionResult> AsignarImagenBannerPersona(int codigoPersona, int codigoArchivo)
        {
            if (codigoPersona <= 0)
            {
                return BadRequest("codigoPersona nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    WrapperSimpleTypesDTO wrapperAsignarImagenBannerPersona = await _archivoBusiness.AsignarImagenBannerPersona(codigoPersona, codigoArchivo, sourceStream);

                    return Ok(wrapperAsignarImagenBannerPersona);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/ModificarArchivoAnuncio/{codigoTipoArchivo}/{codigoAnuncio}/{codigoArchivo}")]
        public async Task<IHttpActionResult> ModificarArchivoAnuncio(int codigoTipoArchivo, int codigoAnuncio, int? codigoArchivo)
        {
            if (codigoAnuncio <= 0 && codigoTipoArchivo <= 0)
            {
                return BadRequest("codigoAnuncio nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        AnunciantesBusiness anuncianteBuss = new AnunciantesBusiness();
                        int duracionPermitidaParaEsteAnuncio = await anuncianteBuss.CalcularDuracionPermitidaVideoParaUnAnuncio(codigoAnuncio);

                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();

                        ConvertSettings outputSettings = new ConvertSettings
                        {
                            MaxDuration = duracionPermitidaParaEsteAnuncio
                        };

                        ffMpeg.ConvertMedia(fullFileNameToVerify, null, fullFileNameToCopy, Format.mp4, outputSettings);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperModificarArchivoAnuncio = await _archivoBusiness.ModificarArchivoAnuncio(codigoTipoArchivo, codigoAnuncio, codigoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperModificarArchivoAnuncio);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperModificarArchivoAnuncio = await _archivoBusiness.ModificarArchivoAnuncio(codigoTipoArchivo, codigoAnuncio, codigoArchivo, sourceStream);

                        return Ok(wrapperModificarArchivoAnuncio);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/ModificarArchivoNoticia/{codigoTipoArchivo}/{codigoNoticia}/{codigoArchivo}")]
        public async Task<IHttpActionResult> ModificarArchivoNoticia(int codigoTipoArchivo, int codigoNoticia, int? codigoArchivo)
        {
            if (codigoNoticia <= 0 && codigoTipoArchivo <= 0)
            {
                return BadRequest("codigoNoticia nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();
                        ffMpeg.ConvertMedia(fullFileNameToVerify, fullFileNameToCopy, Format.mp4);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperModificarArchivoNoticia = await _archivoBusiness.ModificarArchivoNoticia(codigoTipoArchivo, codigoNoticia, codigoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperModificarArchivoNoticia);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperModificarArchivoNoticia = await _archivoBusiness.ModificarArchivoNoticia(codigoTipoArchivo, codigoNoticia, codigoArchivo, sourceStream);

                        return Ok(wrapperModificarArchivoNoticia);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/ModificarArchivoEventos/{codigoTipoArchivo}/{codigoEvento}/{codigoArchivo}")]
        public async Task<IHttpActionResult> ModificarArchivoEventos(int codigoTipoArchivo, int codigoEvento, int? codigoArchivo)
        {
            if (codigoEvento <= 0 && codigoTipoArchivo <= 0)
            {
                return BadRequest("codigoEvento nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        GruposBusiness grupoBusiness = new GruposBusiness();
                        int duracionVideoPermitidaParaEsteEvento = await grupoBusiness.CalcularDuracionPermitidaVideoParaUnEvento(codigoEvento);

                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();

                        ConvertSettings outputSettings = new ConvertSettings
                        {
                            MaxDuration = duracionVideoPermitidaParaEsteEvento
                        };

                        ffMpeg.ConvertMedia(fullFileNameToVerify, null, fullFileNameToCopy, Format.mp4, outputSettings);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperModificarArchivoEventos = await _archivoBusiness.ModificarArchivoEventos(codigoTipoArchivo, codigoEvento, codigoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperModificarArchivoEventos);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperModificarArchivoEventos = await _archivoBusiness.ModificarArchivoEventos(codigoTipoArchivo, codigoEvento, codigoArchivo, sourceStream);

                        return Ok(wrapperModificarArchivoEventos);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/ModificarArchivoCandidatoVideos/{codigoTipoArchivo}/{codigoCandidatoVideo}/{codigoArchivo}")]
        public async Task<IHttpActionResult> ModificarArchivoCandidatoVideos(int codigoTipoArchivo, int codigoCandidatoVideo, int codigoArchivo)
        {
            if (codigoCandidatoVideo <= 0 && codigoTipoArchivo <= 0)
            {
                return BadRequest("codigoCandidatoVideo nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    if (codigoTipoArchivo == (int)TipoArchivo.Video)
                    {
                        CandidatosBusiness candidatosBuss = new CandidatosBusiness();
                        int duracionVideoPermitidaParaEstaPublicacion = await candidatosBuss.CalcularDuracionPermitidaVideoParaUnaPublicacionCandidato(codigoCandidatoVideo);

                        string fileNameToVerify = Guid.NewGuid().ToString();
                        string fullFileNameToVerify = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToVerify));

                        string fileNameToCopy = Guid.NewGuid().ToString();
                        string fullFileNameToCopy = HttpContext.Current.Server.MapPath(string.Format("~/App_Data/{0}.mp4", fileNameToCopy));

                        using (FileStream fileToVerify = new FileStream(fullFileNameToVerify, FileMode.Create))
                        {
                            await sourceStream.CopyToAsync(fileToVerify);
                        }

                        sourceStream.Dispose();

                        FFMpegConverter ffMpeg = new FFMpegConverter();

                        ConvertSettings outputSettings = new ConvertSettings
                        {
                            MaxDuration = duracionVideoPermitidaParaEstaPublicacion
                        };

                        ffMpeg.ConvertMedia(fullFileNameToVerify, null, fullFileNameToCopy, Format.mp4, outputSettings);

                        using (FileStream fileToCopy = new FileStream(fullFileNameToCopy, FileMode.Open, FileAccess.Read))
                        {
                            WrapperSimpleTypesDTO wrapperModificarArchivoCandidatoVideos = await _archivoBusiness.ModificarArchivoCandidatoVideos(codigoTipoArchivo, codigoCandidatoVideo, codigoArchivo, fileToCopy);

                            fileToCopy.Dispose();
                            fileToCopy.Close();

                            if (File.Exists(fullFileNameToVerify))
                            {
                                File.Delete(fullFileNameToVerify);
                            }
                            if (File.Exists(fullFileNameToCopy))
                            {
                                File.Delete(fullFileNameToCopy);
                            }

                            return Ok(wrapperModificarArchivoCandidatoVideos);
                        }
                    }
                    else
                    {
                        WrapperSimpleTypesDTO wrapperModificarArchivoCandidatoVideos = await _archivoBusiness.ModificarArchivoCandidatoVideos(codigoTipoArchivo, codigoCandidatoVideo, codigoArchivo, sourceStream);

                        return Ok(wrapperModificarArchivoCandidatoVideos);
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [Route("api/Archivos/AsignarArchivoReciboPago/{codigoHistorialPago}/{codigoArchivo}")]
        public async Task<IHttpActionResult> AsignarArchivoReciboPago(int codigoHistorialPago, int codigoArchivo)
        {
            if (codigoHistorialPago <= 0)
            {
                return BadRequest("codigoHistorialPago nulo o invalido!.");
            }

            try
            {
                using (Stream sourceStream = await Request.Content.ReadAsStreamAsync())
                {
                    if (sourceStream == null)
                    {
                        return BadRequest("Stream del archivo nulo o invalido!.");
                    }

                    WrapperSimpleTypesDTO wrapperAsignarArchivoReciboPago = await _archivoBusiness.AsignarArchivoReciboPago(codigoHistorialPago, codigoArchivo, sourceStream);

                    return Ok(wrapperAsignarArchivoReciboPago);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
