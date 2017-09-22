using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ir.EmIT.TeleZanbil.Models
{
    public class Family
    {
        public int FamilyId { get; set; }
        public string FamilyName { get; set; }
        public IList<User> Users { get; set; }
    }
}
