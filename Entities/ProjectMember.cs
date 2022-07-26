using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{

    [Index(nameof(ProjectId), nameof(PersonId), IsUnique = true)]
    public class ProjectMember : BaseEntity
    {
        [Key]
        public int ProjectMemberId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }

        [Required]
        public int PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; }

       

        [Required]
        public int ProjectRoleListItemId { get; set; }

        [ForeignKey(nameof(ProjectRoleListItemId))]
        public ListItem ProjectRoleListItem { get; set; }
    }
}