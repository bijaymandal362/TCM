using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities
{
    public class MenuPermission : BaseEntity
    {
        [Key]
        public int MenuPermissionId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public ListItem RoleListItem { get; set; }

        [ForeignKey(nameof(MenuId))]
        public Menu Menu { get; set; }

        public MenuPermission(int roleId, int menuId)
        {
            RoleId = roleId;
            MenuId = menuId;
            InsertDate = DateTimeOffset.UtcNow;
            UpdateDate = DateTimeOffset.UtcNow;
            InsertPersonId = 1;
            UpdatePersonId = 1;
        }
    }
}
