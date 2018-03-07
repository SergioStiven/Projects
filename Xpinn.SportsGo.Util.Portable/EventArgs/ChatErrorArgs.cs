using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Args
{
    public class ExceptionArgs : EventArgs
    {
        public Exception Exception { get; set; }

        public ExceptionArgs(Exception ex)
        {
            Exception = ex;
        }
    }
}
