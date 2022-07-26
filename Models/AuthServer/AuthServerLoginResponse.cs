using System.Runtime.Serialization;

namespace Models.AuthServer
{
    [DataContract]
    public class AuthServerLoginResponse
    {
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }

        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        [DataMember(Name = "refresh_token")]
        public string RefershToken { get; set; }
    }
}
