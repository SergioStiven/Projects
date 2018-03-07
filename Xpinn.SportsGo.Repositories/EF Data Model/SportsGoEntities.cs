using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Repositories
{
    public partial class SportsGoEntities : IDbContext
    {
        public SportsGoEntities(bool lazyLoadingEnabled) : this()
        {
            Configuration.LazyLoadingEnabled = lazyLoadingEnabled;
        }
    }
}
