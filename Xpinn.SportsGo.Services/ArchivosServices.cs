using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using System.IO;
using Xpinn.SportsGo.Util.Portable;

namespace Xpinn.SportsGo.Services
{
    public class ArchivosServices : BaseService
    {
        public async Task<Stream> BuscarArchivo(int codigoArchivo)
        {
            if (codigoArchivo <= 0) throw new ArgumentException("No puedes buscar un archivo si codigoArchivo es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            Stream streamToFile = await client.GetStreamAsync("Archivos/BuscarArchivo/" + codigoArchivo);

            return streamToFile;
        }

        public async Task<WrapperSimpleTypesDTO> CrearArchivoStream(int codigoTipoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes crear un archivo si streamSource es nulo!.");
            if (codigoTipoArchivo <= 0) throw new ArgumentException("No puedes crear un archivo si codigoTipoArchivo es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearArchivoStream = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/CrearArchivoStream/" + codigoTipoArchivo, streamSource);

            return wrapperCrearArchivoStream;
        }

        public async Task<WrapperSimpleTypesDTO> CrearArchivoStreamYControlarDuracionVideo(int codigoTipoArchivo, int duracionVideoPermitida, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes crear un archivo si streamSource es nulo!.");
            if (codigoTipoArchivo <= 0 || duracionVideoPermitida < AppConstants.MinimoSegundos || duracionVideoPermitida > AppConstants.MaximoSegundos)
            {
                throw new ArgumentException("No puedes crear un archivo si codigoTipoArchivo o la duracion permitida del video es invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearArchivoStreamYControlarDuracion = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/CrearArchivoStreamYControlarDuracionVideo/" + codigoTipoArchivo + "/" + duracionVideoPermitida, streamSource);

            return wrapperCrearArchivoStreamYControlarDuracion;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoStream(int consecutivoArchivo, int codigoTipoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes modificar un archivo si streamSource es nulo!.");
            if (codigoTipoArchivo <= 0 || consecutivoArchivo <= 0) throw new ArgumentException("No puedes modificar un archivo si consecutivoArchivo o codigoTipoArchivo es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarArchivoStream = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/ModificarArchivoStream/" + consecutivoArchivo + "/" + codigoTipoArchivo, streamSource);

            return wrapperModificarArchivoStream;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfilPersona(int codigoPersona, int codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes asignar un archivo si streamSource es nulo!.");
            if (codigoPersona <= 0) throw new ArgumentException("No puedes crear un archivo de perfil si codigoPersona es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarImagenPerfilPersona = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/AsignarImagenPerfilPersona/" + codigoPersona + "/" + codigoArchivo, streamSource);

            return wrapperAsignarImagenPerfilPersona;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarImagenBannerPersona(int codigoPersona, int codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes asignar un archivo si streamSource es nulo!.");
            if (codigoPersona <= 0) throw new ArgumentException("No puedes crear un archivo de banner si codigoPersona es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarImagenBannerPersona = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/AsignarImagenBannerPersona/" + codigoPersona + "/" + codigoArchivo, streamSource);

            return wrapperAsignarImagenBannerPersona;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoAnuncio(int codigoTipoArchivo, int codigoAnuncio, int? codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes modificar un archivo si streamSource es nulo!.");
            if (codigoAnuncio <= 0 || codigoTipoArchivo <= 0) throw new ArgumentException("No puedes modificar el archivo de un anuncio si codigoAnuncio es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            string codigoArchivoFormated = codigoArchivo.HasValue ? codigoArchivo.Value.ToString() : "null";

            WrapperSimpleTypesDTO wrapperModificarArchivoAnuncio = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/ModificarArchivoAnuncio/" + codigoTipoArchivo + "/" + codigoAnuncio + "/" + codigoArchivoFormated, streamSource);

            return wrapperModificarArchivoAnuncio;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoNoticia(int codigoTipoArchivo, int codigoNoticia, int? codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes modificar un archivo si streamSource es nulo!.");
            if (codigoNoticia <= 0 || codigoTipoArchivo <= 0) throw new ArgumentException("No puedes modificar el archivo de una noticia si codigoNoticia es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            string codigoArchivoFormated = codigoArchivo.HasValue ? codigoArchivo.Value.ToString() : "null";

            WrapperSimpleTypesDTO wrapperModificarArchivoNoticia = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/ModificarArchivoNoticia/" + codigoTipoArchivo + "/" + codigoNoticia + "/" + codigoArchivoFormated, streamSource);

            return wrapperModificarArchivoNoticia;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoEventos(int codigoTipoArchivo, int codigoEvento, int? codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes modificar un archivo si streamSource es nulo!.");
            if (codigoEvento <= 0 || codigoTipoArchivo <= 0) throw new ArgumentException("No puedes modificar el archivo de un evento si codigoEvento es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            string codigoArchivoFormated = codigoArchivo.HasValue ? codigoArchivo.Value.ToString() : "null";

            WrapperSimpleTypesDTO wrapperModificarArchivoEventos = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/ModificarArchivoEventos/" + codigoTipoArchivo + "/" + codigoEvento + "/" + codigoArchivoFormated, streamSource);

            return wrapperModificarArchivoEventos;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarArchivoCandidatoVideos(int codigoTipoArchivo, int codigoCandidatoVideo, int codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes modificar un archivo si streamSource es nulo!.");
            if (codigoCandidatoVideo <= 0 || codigoTipoArchivo <= 0) throw new ArgumentException("No puedes modificar el archivo de un candidatoVideo si codigoCandidatoVideo es invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarArchivoCandidatoVideos = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/ModificarArchivoCandidatoVideos/" + codigoTipoArchivo + "/" + codigoCandidatoVideo + "/" + codigoArchivo, streamSource);

            return wrapperModificarArchivoCandidatoVideos;
        }

        public async Task<WrapperSimpleTypesDTO> AsignarArchivoReciboPago(int codigoHistorialPago, int? codigoArchivo, Stream streamSource)
        {
            if (streamSource == null) throw new ArgumentNullException("No puedes asignar un archivo si streamSource es nulo!.");
            if (codigoHistorialPago <= 0)
            {
                throw new ArgumentException("No puedes asignar el archivo de un pago si codigoHistorialPago y/o codigoArchivo es invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarArchivoReciboPago = await client.PostStreamAsync<WrapperSimpleTypesDTO>("Archivos/AsignarArchivoReciboPago/" + codigoHistorialPago + "/" + (codigoArchivo.HasValue ? codigoArchivo.Value : 0), streamSource);

            return wrapperAsignarArchivoReciboPago;
        }
    }
}