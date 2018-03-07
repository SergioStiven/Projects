using FreshMvvm;
using System;
using System.Net.Http;
using Xpinn.SportsGo.Util.Portable;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.Portable.BaseClasses
{
    public abstract class BaseService
    {
        HttpClientJsonEncrypter _httpClient;

        protected IHttpClient ConfigurarHttpClient()
        {
            if (_httpClient == null)
            {
                // Se debe registrar en el container del FreshMvvm del proyecto llamando esta capa la implementación 
                // De "ISecureableMessage" para asi poder resolver la dependencia y comunicarse encriptando y desencriptando
                ISecureableMessage secureMessageHelper = FreshIOC.Container.Resolve<ISecureableMessage>();
                if (secureMessageHelper == null) throw new NullReferenceException("No se pudo resolver la dependencia para el HttpClientEncrypted (ISecureableMessage)!.");

                HttpMessageHandler handler = FreshIOC.Container.Resolve<HttpMessageHandler>();
                if (handler == null) throw new NullReferenceException("No se pudo resolver la dependencia para el HttpClientEncrypted (HttpMessageHandler)!.");

                _httpClient = new HttpClientJsonEncrypter(secureMessageHelper, handler);
            }

            return _httpClient;
        }

        static ILockeable _lockeable;
        protected static ILockeable Lockeable
        {
            get
            {
                if (_lockeable == null)
                {
                    _lockeable = FreshIOC.Container.Resolve<ILockeable>();
                }

                return _lockeable;
            }
        }
    }
}