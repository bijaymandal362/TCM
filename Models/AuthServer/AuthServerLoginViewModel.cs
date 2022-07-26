using System.Runtime.Serialization;

namespace Models.AuthServer
{
    [DataContract]
    public class AuthServerLoginData
    {
        [DataMember(Name ="data")]
        public AuthServerLoginViewModel Data { get; set; }
    }

    [DataContract]
    public class AuthServerLoginViewModel
    {
        [DataMember(Name ="username")]
        public string UserName { get; set; }

        [DataMember(Name ="password")]
        public string Password { get; set; }

        [DataMember(Name ="grant-type")]
        public string GrantType { get; set; }

        [DataMember(Name ="client-id")]
        public string ClientId { get; set; }

        [DataMember(Name ="client-secret")]
        public string ClientSecret { get; set; }

        [DataMember(Name ="scope")]
        public string Scope { get; set; }
    }
}
