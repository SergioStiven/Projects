using Plugin.Connectivity.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Movil.Infraestructure
{
    class ConnectionChangedEventArgs
    {
        public ConnectionChangedEventArgs(bool isConnect)
        {
            IsConnect = isConnect;
        }

        public bool IsConnect { get; set; }
    }
}
