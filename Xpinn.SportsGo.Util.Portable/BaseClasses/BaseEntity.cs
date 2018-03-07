using System;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Util.Portable.BaseClasses
{
    public abstract class BaseEntity
    {
        public int SkipIndexBase { get; set; }
        public int TakeIndexBase { get; set; }
        public Idioma IdiomaBase
        {
            get
            {
                Idioma result;
                return Enum.TryParse(CodigoIdiomaUsuarioBase.ToString(), true, out result) ? result : default(Idioma);
            }
            set
            {
                CodigoIdiomaUsuarioBase = (int)value;
            }
        }
        public int CodigoIdiomaUsuarioBase { get; set; }
        public TipoOperacion TipoOperacionBase { get; set; }
        public int NumeroRegistrosExistentes { get; set; }
        public string IdentificadorParaBuscar { get; set; }
        public DateTime FechaFiltroBase { get; set; }
        public int ZonaHorariaGMTBase { get; set; }
    }
}
