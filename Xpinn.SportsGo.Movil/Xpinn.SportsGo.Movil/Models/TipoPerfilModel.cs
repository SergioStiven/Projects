using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    public class TipoPerfilModel
    {
        public string NombreTipoPerfil
        {
            get
            {
                return RecuperarNombreTipoPerfil(TipoPerfil);
            }
        }

        public TipoPerfil TipoPerfil { get; set; }

        public TipoPerfilModel(TipoPerfil tipoPerfil)
        {
            TipoPerfil = tipoPerfil;
        }

        public TipoPerfilModel(PersonasDTO persona)
        {
            TipoPerfil = persona.TipoPerfil;
        }

        public TipoPerfilModel(UsuariosDTO usuario)
        {
            TipoPerfil = usuario.TipoPerfil;
        }

        public static string RecuperarNombreTipoPerfil(TipoPerfil tipoPerfil)
        {
            string tipoPerfilNombre = string.Empty;

            switch (tipoPerfil)
            {
                case TipoPerfil.Candidato:
                    tipoPerfilNombre = SportsGoResources.Deportistas;
                    break;
                case TipoPerfil.Grupo:
                    tipoPerfilNombre = SportsGoResources.Grupos;
                    break;
                case TipoPerfil.Representante:
                    tipoPerfilNombre = SportsGoResources.Representantes;
                    break;
            }

            return tipoPerfilNombre;
        }
    }
}
