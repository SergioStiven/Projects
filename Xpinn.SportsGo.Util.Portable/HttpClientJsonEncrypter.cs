using Newtonsoft.Json;
using Polly;
using Polly.Timeout;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.Portable
{
    public class HttpClientJsonEncrypter : IHttpClient
    {
        // Todo esto es static porque es reusable sin problema
        static HttpClient _clientWithHandler;
        static HttpClient _cleanClient;

        readonly HttpMessageHandler _handler;
        readonly ISecureableMessage _secureMessageHelper;
        readonly TimeSpan _maximunTimeUntilFail = TimeSpan.FromMinutes(20);
        readonly TimeSpan _minimunTimeUntilFail = TimeSpan.FromSeconds(100);
        readonly JsonSerializerSettings _settings = JsonSerializerOptionsConfiguration.ReturnJsonSerializerSettings();
        readonly JsonSerializer _serializer = new JsonSerializer();
        readonly Policy _policy = Policy
                                            .Handle<WebException>()
                                            .Or<HttpRequestException>()
                                            .Or<IOException>()
                                            .WaitAndRetryAsync(new[] {
                                                            TimeSpan.FromSeconds(1),
                                                            TimeSpan.FromSeconds(2),
                                                            TimeSpan.FromSeconds(2)
                                            });

        public HttpClientJsonEncrypter(ISecureableMessage secureMessageHelper, HttpMessageHandler handler)
        {
            if (secureMessageHelper == null) throw new ArgumentNullException("No puedes usar un secureMessageHelper nulo!.");
            if (handler == null) throw new ArgumentNullException("No puedes usar un handler nulo!.");

            _secureMessageHelper = secureMessageHelper;
            _handler = handler;
        }

        public async Task<TBack> PostStreamAsync<TBack>(string requestUri, Stream streamToSend) where TBack : class
        {
            //PushStreamContent content = new PushStreamContent(async (stream, httpContent, transportContext) =>
            //{
            //    try
            //    {
            //        await streamToSend.CopyToAsync(stream);
            //    }
            //    catch (Exception ex)
            //    {

            //    }
            //    finally
            //    {
            //        streamToSend.Dispose();
            //    }
            //});

            if (streamToSend.Length >= 157286400)
            {
                throw new InvalidOperationException("El contenido a enviar no puede superar los 150MB");
            }

            StreamContent content = new StreamContent(streamToSend);

            return await ExecutePostAsync<TBack>(requestUri, content, false);
        }

        public async Task<TBack> PostAsync<TBack>(string requestUri) where TBack : class
        {
            return await ExecutePostAsync<TBack>(requestUri, null);
        }

        public async Task<TBack> PostAsync<T, TBack>(string requestUri, T entityToSend) where T : class where TBack : class
        {
            StringContent stringContent = SerializeRequestEntity(entityToSend);

            return await ExecutePostAsync<TBack>(requestUri, stringContent);
        }

        public async Task<T> PostAsync<T>(string requestUri, T entityToSend) where T : class
        {
            return await PostAsync<T, T>(requestUri, entityToSend);
        }

        StringContent SerializeRequestEntity<T>(T entityToSend) where T : class
        {
            string serializedObject = JsonConvert.SerializeObject(entityToSend, _settings);
            StringContent stringContent = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            return stringContent;
        }

        async Task<T> ExecutePostAsync<T>(string requestUri, HttpContent content, bool handlerEncrypted = true) where T : class
        {
            HttpClient client = ConfigureHttpClient(handlerEncrypted);

            TimeSpan timeSpan = _minimunTimeUntilFail;
            if (content is StreamContent)
            {
                timeSpan = _maximunTimeUntilFail;
            }

            CancellationToken cts = new CancellationTokenSource(timeSpan).Token;
            TimeoutPolicy timeOutPolicy = Policy.TimeoutAsync(timeSpan);

            using (content)
            using (HttpResponseMessage response = await timeOutPolicy.WrapAsync(_policy).ExecuteAsync(async () => await client.PostAsync(requestUri, content, cts)))
            {
                return await ProcessResponse<T>(response);
            }
        }

        HttpClient ConfigureHttpClient(bool handlerEncrypted = true)
        {
            HttpClient client;

            if (handlerEncrypted)
            {
                if (_clientWithHandler == null)
                {
                    _clientWithHandler = new HttpClient(new EncryptedClientHandler(_secureMessageHelper, _handler));
                    _clientWithHandler.BaseAddress = new Uri(URL.UrlBase);
                    _clientWithHandler.DefaultRequestHeaders.Accept.Clear();
                    _clientWithHandler.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _clientWithHandler.Timeout = _maximunTimeUntilFail;
                }

                client = _clientWithHandler;
            }
            else
            {
                if (_cleanClient == null)
                {
                    _cleanClient = new HttpClient();
                    _cleanClient.BaseAddress = new Uri(URL.UrlBase);
                    _cleanClient.DefaultRequestHeaders.Accept.Clear();
                    _cleanClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    _clientWithHandler.Timeout = _maximunTimeUntilFail;
                }

                client = _cleanClient;
            }

            return client;
        }

        async Task<TBack> ProcessResponse<TBack>(HttpResponseMessage response) where TBack : class
        {
            if (response != null && response.IsSuccessStatusCode)
            {
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(stream))
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    return _serializer.Deserialize<TBack>(jsonReader);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<Stream> GetStreamAsync(string requestUri)
        {
            HttpClient client = ConfigureHttpClient(handlerEncrypted: false);

            using (HttpResponseMessage response = await _policy.ExecuteAsync(async () => await client.GetAsync(requestUri)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    return null;
                }
            }
        }

    }
}