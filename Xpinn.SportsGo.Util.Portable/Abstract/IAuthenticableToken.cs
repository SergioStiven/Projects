namespace Xpinn.SportsGo.Util.Portable.Abstract
{
    public interface IAuthenticableToken
    {
        string access_token { get; set; }
        string token_type { get; set; }
        int expires_in { get; set; }
    }
}
