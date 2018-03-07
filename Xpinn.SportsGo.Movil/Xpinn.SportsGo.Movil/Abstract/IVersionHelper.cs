using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Movil.Abstract
{
    public interface IVersionHelper
    {
        string  VersionName { get; }
        string VersionCode { get; }
    }
}
