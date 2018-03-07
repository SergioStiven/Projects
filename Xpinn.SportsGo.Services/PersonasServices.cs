using System;
using System.IO;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class PersonasServices : BaseService
    {
        public async Task<PersonasDTO> BuscarPersona(PersonasDTO personaParaBuscar)
        {
            if (personaParaBuscar == null) throw new ArgumentNullException("No puedes buscar una persona si personaParaBuscar es nulo!.");
            if (personaParaBuscar.Consecutivo <= 0 || personaParaBuscar.IdiomaDeLaPersona == Idioma.SinIdioma)
            {
                throw new ArgumentException("personaParaBuscar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            PersonasDTO personaBuscada = await client.PostAsync("Personas/BuscarPersona", personaParaBuscar);

            return personaBuscada;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarPersona(PersonasDTO personaParaModificar)
        {
            if (personaParaModificar == null) throw new ArgumentNullException("No puedes modificar una persona si personaParaModificar es nula!.");
            if (personaParaModificar.Consecutivo <= 0 || string.IsNullOrWhiteSpace(personaParaModificar.Nombres) || string.IsNullOrWhiteSpace(personaParaModificar.Telefono)
                || personaParaModificar.CodigoIdioma <= 0 || personaParaModificar.CodigoPais <= 0 || string.IsNullOrWhiteSpace(personaParaModificar.CiudadResidencia))
            {
                throw new ArgumentException("personaParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarPersona = await client.PostAsync<PersonasDTO, WrapperSimpleTypesDTO>("Personas/ModificarPersona", personaParaModificar);

            return wrapperModificarPersona;
        }

        [Obsolete("Usa el asignar imagen perfil de archivosServices", true)]
        public async Task<WrapperSimpleTypesDTO> AsignarImagenPerfil(PersonasDTO personaParaAsignarImagenPerfil)
        {
            if (personaParaAsignarImagenPerfil == null) throw new ArgumentNullException("No puedes asignar una imagen perfil para la persona si personaParaAsignarImagenPerfil es nulo!.");
            if (personaParaAsignarImagenPerfil.Consecutivo <= 0 || personaParaAsignarImagenPerfil.ArchivosPerfil == null || personaParaAsignarImagenPerfil.ArchivosPerfil.ArchivoContenido == null)
            {
                throw new ArgumentException("personaParaAsignarImagenPerfil vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarImagenPerfil = await client.PostAsync<PersonasDTO, WrapperSimpleTypesDTO>("Personas/AsignarImagenPerfil", personaParaAsignarImagenPerfil);

            return wrapperAsignarImagenPerfil;
        }

        [Obsolete("Usa el asignar imagen banner de archivosServices", true)]
        public async Task<WrapperSimpleTypesDTO> AsignarImagenBanner(PersonasDTO personaParaAsignarImagenBanner)
        {
            if (personaParaAsignarImagenBanner == null) throw new ArgumentNullException("No puedes asignar una imagen de banner para la persona si personaParaAsignarImagenBanner es nulo!.");
            if (personaParaAsignarImagenBanner.Consecutivo <= 0 || personaParaAsignarImagenBanner.ArchivosBanner == null || personaParaAsignarImagenBanner.ArchivosBanner.ArchivoContenido == null)
            {
                throw new ArgumentException("personaParaAsignarImagenBanner vacia y/o invalida!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperAsignarImagenBanner = await client.PostAsync<PersonasDTO, WrapperSimpleTypesDTO>("Personas/AsignarImagenBanner", personaParaAsignarImagenBanner);

            return wrapperAsignarImagenBanner;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarImagenPerfil(PersonasDTO personaParaEliminarImagenPerfil)
        {
            if (personaParaEliminarImagenPerfil == null) throw new ArgumentNullException("No puedes eliminar la imagen perfil de una persona si personaParaEliminarImagenPerfil es nulo!.");
            if (!personaParaEliminarImagenPerfil.CodigoArchivoImagenPerfil.HasValue || personaParaEliminarImagenPerfil.CodigoArchivoImagenPerfil <= 0 || personaParaEliminarImagenPerfil.Consecutivo <= 0)
            {
                throw new ArgumentException("personaParaEliminarImagenPerfil vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarImagenPerfil = await client.PostAsync<PersonasDTO, WrapperSimpleTypesDTO>("Personas/EliminarImagenPerfil", personaParaEliminarImagenPerfil);

            return wrapperEliminarImagenPerfil;
        }

        public async Task<WrapperSimpleTypesDTO> EliminarImagenBanner(PersonasDTO personaParaEliminarImagenBanner)
        {
            if (personaParaEliminarImagenBanner == null) throw new ArgumentNullException("No puedes eliminar el banner de una persona si personaParaEliminarImagenBanner es nulo!.");
            if (!personaParaEliminarImagenBanner.CodigoArchivoImagenBanner.HasValue || personaParaEliminarImagenBanner.CodigoArchivoImagenBanner <= 0 || personaParaEliminarImagenBanner.Consecutivo <= 0)
            {
                throw new ArgumentException("personaParaEliminarImagenBanner vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperEliminarImagenBanner = await client.PostAsync<PersonasDTO, WrapperSimpleTypesDTO>("Personas/EliminarImagenBanner", personaParaEliminarImagenBanner);

            return wrapperEliminarImagenBanner;
        }
    }
}
