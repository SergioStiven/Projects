using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Abstract
{
    public interface IDateTimeHelper
    {
        int DifferenceBetweenGMTAndLocalTimeZone { get; }
    }
}
