using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Business;
using Xpinn.SportsGo.DomainEntities;
using Xpinn.SportsGo.Util.Portable.Enums;
using Xpinn.SportsGo.Util.HelperClasses;

namespace Xpinn.SportsGo.WebAPI.Infraestructure
{
    public class FileStreamAPI : IHttpActionResult
    {
        readonly ArchivosBusiness _archivoBusiness;
        readonly Archivos _archivoParaStremear;
        readonly RangeHeaderValue _rangeHeader;

        public FileStreamAPI(ArchivosBusiness archivoBusiness, Archivos archivoParaStremear, RangeHeaderValue rangeHeader)
        {
            _archivoBusiness = archivoBusiness;
            _archivoParaStremear = archivoParaStremear;
            _rangeHeader = rangeHeader;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                // Hallar el mime type segun la extension del nombre del archivo
                //var contentType = MimeMapping.GetMimeMapping(Path.GetExtension(_tipoDeArchivo));

                string contentType = _archivoParaStremear.TipoArchivo == TipoArchivo.Imagen ? "image/jpeg" : "video/mp4";
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

                if (_rangeHeader == null || !_rangeHeader.Ranges.Any())
                {
                    response.Content = new PushStreamContent(WriteToStreamFull, contentType);
                    return response;
                }
                else
                {
                    long start = 0, end = 0, totalLenght;

                    totalLenght = await _archivoBusiness.StreamArchivoGetLenght(_archivoParaStremear);

                    // 1. If the unit is not 'bytes'.
                    // 2. If there are multiple ranges in header value.
                    // 3. If start or end position is greater than file length.
                    if (_rangeHeader.Unit != "bytes" || _rangeHeader.Ranges.Count > 1 ||
                            !MediaStreamHelper.TryReadRangeItem(_rangeHeader.Ranges.First(), totalLenght, out start, out end))
                    {
                        response.StatusCode = HttpStatusCode.RequestedRangeNotSatisfiable;
                        response.Content = new StreamContent(Stream.Null);  // No content for this status.
                        response.Content.Headers.ContentRange = new ContentRangeHeaderValue(totalLenght);
                        response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                        return response;
                    }

                    var contentRange = new ContentRangeHeaderValue(start, end, totalLenght);

                    // We are now ready to produce partial content.
                    response.StatusCode = HttpStatusCode.PartialContent;
                    response.Content = new PushStreamContent(async (outputStream, httpContent, transpContext)
                    =>
                    {
                        await WriteToStreamRange(outputStream, httpContent, transpContext, start, end);
                    }, contentType);

                    response.Content.Headers.ContentLength = end - start + 1;
                    response.Content.Headers.ContentRange = contentRange;

                    return response;
                }
            }, cancellationToken);
        }

        public async Task WriteToStreamFull(Stream outputStream, HttpContent content, TransportContext context)
        {
            try
            {
                await _archivoBusiness.StreamArchivoFull(_archivoParaStremear, outputStream);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }

        public async Task WriteToStreamRange(Stream outputStream, HttpContent content, TransportContext context, long start, long end)
        {
            try
            {
                await _archivoBusiness.StreamArchivoRange(_archivoParaStremear, outputStream, start, end);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                outputStream.Close();
            }
        }
    }
}