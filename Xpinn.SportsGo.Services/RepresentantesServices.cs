using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Util.Portable.Abstract;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Services
{
    public class RepresentantesServices : BaseService
    {


        #region Metodos Representantes


        public async Task<WrapperSimpleTypesDTO> CrearRepresentante(RepresentantesDTO representanteParaCrear)
        {
            if (representanteParaCrear == null || representanteParaCrear.Personas == null || representanteParaCrear.Personas.Usuarios == null || representanteParaCrear.CategoriasRepresentantes == null)
            {
                throw new ArgumentNullException("No puedes crear un representante si representanteParaCrear, la persona, las categorias o el usuario del representante es nulo!.");
            }
            else if (string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Nombres) || representanteParaCrear.Personas.CodigoPais <= 0 || representanteParaCrear.Personas.TipoPerfil == TipoPerfil.SinTipoPerfil
                || representanteParaCrear.Personas.CodigoIdioma <= 0 || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Telefono) || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.CiudadResidencia))
            {
                throw new ArgumentException("Persona de representanteParaCrear vacio y/o invalido!.");
            }
            else if (string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Usuario) || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Clave)
                     || string.IsNullOrWhiteSpace(representanteParaCrear.Personas.Usuarios.Email))
            {
                throw new ArgumentException("Usuario de representanteParaCrear vacio y/o invalido!.");
            }
            else if (representanteParaCrear.CategoriasRepresentantes.Count <= 0 || !representanteParaCrear.CategoriasRepresentantes.All(x => x.CodigoCategoria > 0))
            {
                throw new ArgumentException("Categorias de representanteParaCrear vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperCrearRepresentante = await client.PostAsync<RepresentantesDTO, WrapperSimpleTypesDTO>("Representantes/CrearRepresentante", representanteParaCrear);

            return wrapperCrearRepresentante;
        }

        public async Task<RepresentantesDTO> BuscarRepresentantePorCodigoPersona(RepresentantesDTO representanteParaBuscar)
        {
            if (representanteParaBuscar == null || representanteParaBuscar.Personas == null) throw new ArgumentNullException("No puedes buscar un representante si representanteParaBuscar es nulo!.");
            if (representanteParaBuscar.Personas.Consecutivo <= 0) throw new ArgumentException("representanteParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            RepresentantesDTO representanteBuscado = await client.PostAsync("Representantes/BuscarRepresentantePorCodigoPersona", representanteParaBuscar);

            return representanteBuscado;
        }

        public async Task<RepresentantesDTO> BuscarGrupoPorCodigoGrupo(RepresentantesDTO representanteParaBuscar)
        {
            if (representanteParaBuscar == null) throw new ArgumentNullException("No puedes buscar un representante si representanteParaBuscar es nulo!.");
            if (representanteParaBuscar.Consecutivo <= 0) throw new ArgumentException("representanteParaBuscar vacio y/o invalido!.");

            IHttpClient client = ConfigurarHttpClient();

            RepresentantesDTO representanteBuscado = await client.PostAsync("Representantes/BuscarRepresentantePorCodigoRepresentante", representanteParaBuscar);

            return representanteBuscado;
        }

        public async Task<List<RepresentantesDTO>> ListarRepresentantes(RepresentantesDTO representanteParaListar)
        {
            if (representanteParaListar == null) throw new ArgumentNullException("No puedes buscar un representante si representanteParaListar es nulo!.");
            if (representanteParaListar.SkipIndexBase < 0 || representanteParaListar.TakeIndexBase <= 0)
            {
                throw new ArgumentException("representanteParaListar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            List<RepresentantesDTO> listaInformacionRepresentante = await client.PostAsync<RepresentantesDTO, List<RepresentantesDTO>>("Representantes/ListarRepresentantes", representanteParaListar);

            return listaInformacionRepresentante;
        }

        public async Task<WrapperSimpleTypesDTO> ModificarInformacionRepresentante(RepresentantesDTO representanteParaModificar)
        {
            if (representanteParaModificar == null) throw new ArgumentNullException("No puedes modificar un representante si representanteParaModificar es nulo!.");
            if (representanteParaModificar.Consecutivo <= 0)
            {
                throw new ArgumentException("representanteParaModificar vacio y/o invalido!.");
            }

            IHttpClient client = ConfigurarHttpClient();

            WrapperSimpleTypesDTO wrapperModificarRepresentante = await client.PostAsync<RepresentantesDTO, WrapperSimpleTypesDTO>("Representantes/ModificarInformacionRepresentante", representanteParaModificar);

            return wrapperModificarRepresentante;
        }


        #endregion


    }
}
