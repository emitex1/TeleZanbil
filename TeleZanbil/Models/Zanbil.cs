using System.Collections.Generic;

namespace ir.EmIT.TeleZanbil.Models
{
    public class Zanbil
    {
        public int ZanbilId { get; set; }
        public string ZanbilName { get; set; }
        public virtual Family Family { get; set; }
        public IList<ZanbilItem> ZanbilItems { get; set; }
    }
}
