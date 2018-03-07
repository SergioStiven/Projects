using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xpinn.SportsGo.Util.Portable.Abstract
{
    public interface ILockeable
    {
        Task<IDisposable> LockAsync();
        Task<IDisposable> LockAsync(CancellationToken cancellationToken);
        IDisposable Lock();
        IDisposable Lock(CancellationToken cancellationToken);
    }
}
