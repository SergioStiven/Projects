using System;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.HelperClasses
{
    public class DateTimeHelperNoPortable : IDateTimeHelper
    {
        public int DifferenceBetweenGMTAndLocalTimeZone
        {
            get
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;

                return localZone.GetUtcOffset(DateTime.Now).Hours;
            }
        }

        public DateTime ConvertDateTimeFromAnotherTimeZone(int destinyTimeZone, int sourceTimeZone, DateTime sourceDateTime)
        {
            if (sourceDateTime != DateTime.MinValue && sourceDateTime != DateTime.MaxValue)
            {
                int value = destinyTimeZone - sourceTimeZone;
                //int diferenciaHoraria = Math.Abs(value);

                return sourceDateTime.AddHours(value);
            }
            else
            {
                return sourceDateTime;
            }
        }
    }
}