using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.Portable
{
    public class TokenHelper<TToken> where TToken : IAuthenticableToken
    {
        public async Task<TToken> ObtenerToken(string usuario, string clave, HttpClient client)
        {
            if (string.IsNullOrWhiteSpace(usuario)) throw new ArgumentNullException("Usuario no puede estar vació!.");
            if (string.IsNullOrWhiteSpace(clave)) throw new ArgumentNullException("Clave no puede estar vació!.");
            if (client == null) throw new ArgumentNullException("HttpClient no puede ser nulo!.");

            HttpResponseMessage response = null;

            try
            {
                response = await client.PostAsync("token", new FormUrlEncodedContent(new[]
                                                {
                                                new KeyValuePair<string,string>("grant_type","password"),
                                                new KeyValuePair<string,string>("username", usuario),
                                                new KeyValuePair<string,string>("password", clave)                                            }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el Token, " + ex.Message);
            }

            if (response.IsSuccessStatusCode)
            {
                string responseMessage = await response.Content.ReadAsStringAsync();
                return await Task.Run(() => JsonConvert.DeserializeObject<TToken>(responseMessage));
            }
            else
            {
                return default(TToken);
            }
        }

        public HttpClient AsignarTokenAuthenticationHeader(TToken token, HttpClient client)
        {
            if (token == null) throw new ArgumentNullException("Token no puede ser nulo!.");
            if (client == null) throw new ArgumentNullException("HttpClient no puede ser nulo!.");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.access_token);
            return client;
        }
    }
}
