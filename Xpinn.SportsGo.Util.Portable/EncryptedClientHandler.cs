using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.Portable
{
    public class EncryptedClientHandler : DelegatingHandler
    {
        ISecureableMessage _secureMessageHelper;

        public EncryptedClientHandler(ISecureableMessage secureMessageHelper) : this(secureMessageHelper, new HttpClientHandler())
        {

        }

        public EncryptedClientHandler(ISecureableMessage secureMessageHelper, HttpMessageHandler nextHandler) : base(nextHandler)
        {
            if (secureMessageHelper == null) throw new ArgumentNullException("No puedes usar un secureMessageHelper nulo!.");

            _secureMessageHelper = secureMessageHelper;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await ProcessRequest(request);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            await ProcessResponse(response);

            return response;
        }

        async Task ProcessRequest(HttpRequestMessage request)
        {
            if (request.Content == null) return;

            string jsonContentRequest = await request.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonContentRequest)) return;

            string encryptedMessage = _secureMessageHelper.EncryptJson(jsonContentRequest);

            IEncrypteableEntity encryptedEntity = new EntidadEncrypted
            {
                messageEncrypted = encryptedMessage
            };

            string serializedObject = JsonConvert.SerializeObject(encryptedEntity, JsonSerializerOptionsConfiguration.ReturnJsonSerializerSettings());
            request.Content = new StringContent(serializedObject, Encoding.UTF8, "application/json");
        }

        async Task ProcessResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode || response.Content == null) return;

            string jsonContentResponse = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(jsonContentResponse)) return;

            IEncrypteableEntity entityEncrypted = await Task.Run(() => JsonConvert.DeserializeObject<EntidadEncrypted>(jsonContentResponse, JsonSerializerOptionsConfiguration.ReturnJsonSerializerSettings()));
            string decryptMessage = _secureMessageHelper.DecryptJson(entityEncrypted.messageEncrypted);
            response.Content = new StringContent(decryptMessage, Encoding.UTF8, "application/json");
        }
    }
}
