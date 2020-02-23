using Rightek.HttpClient.Enums;

namespace Rightek.HttpClient.Dtos
{
    public class Auth
    {
        public AuthType AuthType { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string BearerToken { get; set; }
    }
}