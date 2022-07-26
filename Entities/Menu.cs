using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Entities
{
    [Index(nameof(MenuSlug), IsUnique = true)]
    public class Menu : BaseEntity
    {
        [Key]
        public int MenuId { get; set; }

        [Required]
        public string MenuName { get; set; }

        [Required]
        public string MenuSlug { get; set; }

        public Menu(int menuId, string menuName, string menuSlug)
        {
            MenuId = menuId;
            MenuName = menuName;
            MenuSlug = menuSlug;
            InsertDate = DateTimeOffset.UtcNow;
            UpdateDate = DateTimeOffset.UtcNow;
            InsertPersonId = 1;
            UpdatePersonId = 1;
        }
    }
}
