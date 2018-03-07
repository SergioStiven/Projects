using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Models
{
    public class YahooQuery
    {
        public string Count { get; set; }
        public string Created { get; set; }
        public string Lang { get; set; }
        public YahooResults Results { get; set; }
    }
}
