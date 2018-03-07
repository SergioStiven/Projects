using System;

namespace Xpinn.SportsGo.Entities
{
    public class WrapperSimpleTypesDTO
    {
        public bool PagoEnTramite { get; set; }
        public bool Exitoso { get; set; }
        public bool Existe { get; set; }
        public int NumeroRegistrosAfectados { get; set; }
        public long ConsecutivoCreado { get; set; }
        public long ConsecutivoArchivoCreado { get; set; }
        public int NumeroRegistrosExistentes { get; set; }
        public bool EsPosible { get; set; }
        public DateTime? Vencimiento { get; set; }
        public int ConsecutivoPersonaCreado { get; set; }
        public int ConsecutivoUsuarioCreado { get; set; }
        public int ConsecutivoChatRecibe { get; set; }
        public string ClaveCreada { get; set; }
    }
}