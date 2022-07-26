using System;

namespace Models.Profile
{
    public class UserProfileModel
    {
        public int UserProfileId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
    }
    public class PersonPersonalizationModel
    {
        public int PersonPersonalizationId { get; set; }
        public int PersonId { get; set; }
        public int ThemeListItemId { get; set; }
        public string TimeZone { get; set; }
    }
    public class PersonPersonalizationListModel
    {
        public int PersonPersonalizationId { get; set; }
        public int PersonId { get; set; }
        public int ThemeListItemId { get; set; }
        public string ThemeType { get; set; }
        public string TimeZone { get; set; }
    }
}
