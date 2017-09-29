using System;

namespace ir.EmIT.TeleZanbil.Models
{
    public class ZanbilItem
    {
        public int ZanbilItemId { get; set; }
        public string ItemTitle { get; set; }
        public float ItemAmount { get; set; }
        public virtual Unit ItemUnit { get; set; }
        public virtual Zanbil Zanbil { get; set; }
        public bool IsBought { get; set; }
        public DateTime BuyDate { get; set; }
        public int CreatorUserID { get; set; }
    }
}
