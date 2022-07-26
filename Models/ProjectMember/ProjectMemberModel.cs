using System.Collections.Generic;

namespace Models.ProjectMember
{
    public class ProjectMemberModel
    {   
        public int ProjectMemberId { get; set; }
        public string ProjectSlug { get; set; }
        public List<int> PersonId { get; set; }    
        public int ProjectRoleListItemId { get; set; }

    }
   


}
