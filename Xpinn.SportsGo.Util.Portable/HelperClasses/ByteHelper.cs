using System;

namespace Xpinn.SportsGo.Util.Portable.HelperClasses
{
    public static class ByteHelper
    {
        public static string ConvertByteArrToBase64String(byte[] byteArr)
        {
            if (byteArr == null) throw new ArgumentNullException("Array de bytes a convertir no puede ser nulo!.");

            string base64Image = string.Empty;

            if (byteArr != null)
            {
                base64Image = @"data:image/jpeg;base64," + Convert.ToBase64String(byteArr);
            }

            return base64Image;
        }

        // Sirve para mostrar el pdf en un iframe, el iframe no carga controles para imprimir, etc.
        public static string ConvertByteArrToBase64StringForPDFView(byte[] byteArr)
        {
            if (byteArr == null) throw new ArgumentNullException("Array de bytes a convertir no puede ser nulo!.");

            string base64Image = string.Empty;

            if (byteArr != null)
            {
                base64Image = @"data:application/pdf;base64," + Convert.ToBase64String(byteArr);
            }

            return base64Image;
        }
        
    }
}
