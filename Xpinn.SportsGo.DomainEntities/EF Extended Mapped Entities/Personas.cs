using System;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.DomainEntities
{
    public partial class Personas : BaseEntity
    {
        public int ConsecutivoViendoPersona { get; set; }
        public bool YaEstaAgregadaContactos { get; set; }

        public TipoPerfil TipoPerfil
        {
            get
            {
                TipoPerfil result;
                return Enum.TryParse(CodigoTipoPerfil.ToString(), true, out result) ? result : default(TipoPerfil);
            }
            set
            {
                CodigoTipoPerfil = (int)value;
            }
        }

        public Idioma IdiomaDeLaPersona
        {
            get
            {
                Idioma result;
                return Enum.TryParse(CodigoIdioma.ToString(), true, out result) ? result : default(Idioma);
            }
            set
            {
                CodigoIdioma = (int)value;
            }
        }
    }
}
