using System.Collections.Generic;

namespace ir.EmIT.TeleZanbil.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public IList<User> Users { get; set; }
    }
}
