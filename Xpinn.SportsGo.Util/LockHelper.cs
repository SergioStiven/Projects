using Nito.AsyncEx;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util
{
    public class LockHelper : ILockeable
    {
        AsyncLock _mutex;

        public LockHelper()
        {
            _mutex = new AsyncLock();
        }

        public async Task<IDisposable> LockAsync()
        {
            return await _mutex.LockAsync();
        }

        public async Task<IDisposable> LockAsync(CancellationToken cancellationToken)
        {
            return await _mutex.LockAsync(cancellationToken);
        }

        public IDisposable Lock()
        {
            return _mutex.Lock();
        }

        public IDisposable Lock(CancellationToken cancellationToken)
        {
            return _mutex.Lock(cancellationToken);
        }
    }
}
