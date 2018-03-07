using System;
using System.Collections.Generic;
using Xpinn.SportsGo.Util.Portable.BaseClasses;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Entities
{
    public class BuscadorDTO : BaseEntity
    {
        public int CodigoPlanUsuario { get; set; }
        public int ConsecutivoPerfil { get; set; }
        public int ConsecutivoPersona { get; set; }
        public int EstaturaInicial { get; set; }
        public int EstaturaFinal { get; set; }
        public int PesoInicial { get; set; }
        public int PesoFinal { get; set; }
        public List<int> CategoriasParaBuscar { get; set; }
        public List<int> PaisesParaBuscar { get; set; }
        public List<int> EstadosParaBuscar { get; set; }
        public List<int> PlanesParaBuscar { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFinal { get; set; }
        public bool EsConsultaEnLaApp { get; set; }

        public int ConsecutivoTipoPerfil { get; set; }

        public TipoPerfil TipoDePerfil
        {
            get
            {
                TipoPerfil tipoPerfil;

                Enum.TryParse(ConsecutivoTipoPerfil.ToString(), true, out tipoPerfil);

                return tipoPerfil;
            }
            set
            {
                ConsecutivoTipoPerfil = (int)value;
            }
        }

        public int EdadInicio { get; set; }
        public int EdadFinal { get; set; }
    }
}
