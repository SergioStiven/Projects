using System;
using System.Threading.Tasks;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.iOS.Dependencies
{
    [Foundation.Preserve(AllMembers = true)]
    class iOSConverterImage : IHelperImagen
    {
        public Task<string> ConvertImage(string imagenFilePath, int quality, FormatoImagen formatoImagen)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTransparencyFromAnImage(string imagenFilePath, int quality, FormatoImagen formatoImagen)
        {
            throw new NotImplementedException();
        }
    }
}