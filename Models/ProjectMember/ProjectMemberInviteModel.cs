using System;

namespace Models.ProjectMember
{
    public class ProjectMemberInviteModel
    {
        public int ProjectMemberId { get; set; }
        public string ProjectSlug { get; set; }
        public int ProjectId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int ProjectRoleListItemId { get; set; }
        public string ProjectRole { get; set; }
        public DateTimeOffset InviteDate { get; set; }   
        public DateTimeOffset LastModifiedDate { get; set; }   
        public string InsertedPersonName { get; set; }
        public string LatestUpdatedPersonName { get; set; }

      

    }

    public class ProjectMemberModelList
    {
        public int ProjectMemberId { get; set; }
        public string ProjectSlug { get; set; }
        public int ProjectId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int ProjectRoleListItemId { get; set; }
        public string ProjectRole { get; set; }
        public int UserRoleListItemId { get; set; }
        public string UserRole { get; set; }




    }
}
