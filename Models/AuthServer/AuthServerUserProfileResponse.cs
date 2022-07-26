using System.Runtime.Serialization;

namespace Models.AuthServer
{
    [DataContract]
    public class AuthServerUserProfileResponse
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "phone")]
        public string Phone { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }
    }
}
