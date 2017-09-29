using System.ComponentModel.DataAnnotations.Schema;

namespace ir.EmIT.TeleZanbil.Models
{
    public class User
    {
        public int UserId { get; set; }
        public long TelegramUserID { get; set; }
        public virtual Role UserRole { get; set; }
        public virtual Family UserFamily { get; set; }
        public bool IsDeleted { get; set; }
    }
}
