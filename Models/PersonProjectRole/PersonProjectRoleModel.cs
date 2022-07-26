namespace Models.PersonProjectRole
{
    public class PersonProjectRoleModel
    {
        public int PersonId { get; set; }
        public int ProjectId { get; set; }
        public string PersonName { get; set; }
        public string PersonRole { get; set; }
        public string ProjectRole { get; set; }

        public string ProjectSlug { get; set; }
    }
}
