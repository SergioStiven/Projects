using PropertyChanged;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Models
{
    [AddINotifyPropertyChangedInterface]
    public class IdiomaModel
    {
        public string NombreIdioma
        {
            get
            {
                string idioma = string.Empty;

                switch (Idioma)
                {
                    case Idioma.Español:
                        idioma = SportsGoResources.Español;
                        break;
                    case Idioma.Ingles:
                        idioma = SportsGoResources.Ingles;
                        break;
                    case Idioma.Portugues:
                        idioma = SportsGoResources.Portugues;
                        break;
                }

                return idioma;
            }
        }

        public Idioma Idioma { get; set; }

        public IdiomaModel(Idioma idioma)
        {
            Idioma = idioma;
        }

        public IdiomaModel(IdiomasDTO idioma)
        {
            Idioma = idioma.Idioma;
        }
    }
}
