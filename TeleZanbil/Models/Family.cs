using System.Collections.Generic;

namespace ir.EmIT.TeleZanbil.Models
{
    public class Family
    {
        public int FamilyId { get; set; }
        public string FamilyName { get; set; }
        public string InviteCode { get; set; }
        public bool IsDeleted { get; set; }
        public IList<User> Users { get; set; }
        public IList<Zanbil> Zanbils { get; set; }
    }
}
