using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.WebAPI.Infraestructure
{
    public class FileResultAPI : IHttpActionResult
    {
        readonly Stream _stream;
        readonly TipoArchivo _tipoDeArchivo;

        public FileResultAPI(Stream memoryStream, TipoArchivo tipoDeArchivo)
        {
            _stream = memoryStream;
            _tipoDeArchivo = tipoDeArchivo;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(_stream)
                };

                // Hallar el mime type segun la extension del nombre del archivo
                //var contentType = MimeMapping.GetMimeMapping(Path.GetExtension(_tipoDeArchivo));

                var contentType = _tipoDeArchivo == TipoArchivo.Imagen ? "image/jpeg" : "video/mp4";
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                return response;
            }, cancellationToken);
        }
    }
}
