using Xpinn.SportsGo.Util.Portable.Abstract;

namespace Xpinn.SportsGo.Util.Portable
{
    public class AuthenticationToken : IAuthenticableToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
}
