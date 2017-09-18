using ir.EmIT.EmITBotNet;

namespace ir.EmIT.TeleZanbil
{
    class Program
    {
        static void Main(string[] args)
        {
            new EmITBotNetRunner(new TeleZanbil());
        }
    }
}
