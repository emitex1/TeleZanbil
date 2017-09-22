using System.Collections.Generic;

namespace ir.EmIT.TeleZanbil.Models
{
    public class Unit
    {
        public int UnitId { get; set; }
        public string Title { get; set; }
        public IList<ZanbilItem> ZanbilItems { get; set; }
    }
}
