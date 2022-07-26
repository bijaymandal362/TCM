using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
	[Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Person : BaseEntity
    {
        [Key]
        public int PersonId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }


     
        public int UserRoleListItemId { get; set; }

        [ForeignKey(nameof(UserRoleListItemId))]
        public ListItem UserRoleListItem { get; set; }


     
        public int? UserMarketListItemId { get; set; }

        [ForeignKey(nameof(UserMarketListItemId))]
        public  ListItem UserMarketListItem { get; set; }
    }

}