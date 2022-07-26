using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    [Index(nameof(ListItemCategorySystemName), IsUnique=true)]
    public class ListItemCategory : BaseEntity
    {
        [Key]
        public int ListItemCategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string ListItemCategoryName { get; set; }

        [Required]
        [StringLength(50)]
        public string ListItemCategorySystemName { get; set; }
        [Required]
        public bool IsSystemConfig { get; set; }

    }
}