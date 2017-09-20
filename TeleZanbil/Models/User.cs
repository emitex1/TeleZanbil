namespace ir.EmIT.TeleZanbil.Models
{
    public class User
    {
        public int UserId { get; set; }
        public long TelegramUserID { get; set; }
        public virtual Role UserRole { get; set; }
    }
}
