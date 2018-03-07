using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.Abstract
{
    public interface IHelperImagen
    {
        Task DeleteTransparencyFromAnImage(string imagenFilePath, int quality, FormatoImagen formatoImagen);
        Task<string> ConvertImage(string imagenFilePath, int quality, FormatoImagen formatoImagen);
    }
}
