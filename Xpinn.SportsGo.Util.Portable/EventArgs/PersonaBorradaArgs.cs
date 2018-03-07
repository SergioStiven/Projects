using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Args
{
    public class PersonaBorradaArgs : EventArgs
    {
        public int CodigoPersonaBorrada { get; set; }

        public PersonaBorradaArgs(int codigoPersonaBorrada)
        {
            CodigoPersonaBorrada = codigoPersonaBorrada;
        }
    }
}
