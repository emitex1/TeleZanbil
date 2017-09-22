using System;
using System.Collections.Generic;
using ir.EmIT.EmITBotNet;
using Telegram.Bot.Types;
using ir.EmIT.EmITBotNet.NFAUtility;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using System.Linq;
using ir.EmIT.TeleZanbil.Models;

namespace ir.EmIT.TeleZanbil
{
    class TeleZanbil : EmITBotNetBase
    {
        class TeleZanbilStates : BotStates
        {
            public static BotState CheckUserType = new BotState(2, "بررسی نوع کاربر");
            public static BotState GetMainCommand = new BotState(3, "گرفتن دستور اصلی");
            public static BotState ShowAboutUs = new BotState(4, "نمایش درباره ما");
            public static BotState StartRegFamily = new BotState(5, "شروع روال ثبت خانواده");
            public static BotState GetFamilyName = new BotState(6, "دریافت اسم خانواده");
            public static BotState RegisterFamily = new BotState(7, "ثبت خانواده");
            public static BotState ShowZanbilContentForFather = new BotState(8, "نمایش محتوی زنبیل برای پدر");
            public static BotState AcceptZanbilItem = new BotState(9, "تایید خرید آیتم زنبیل");
            public static BotState AddNewZanbilItem = new BotState(10, "اضافه کردن آیتم جدید به زنبیل");

            public static BotState ShowInvalidCommand = new BotState(11, "نمایش ورودی نامعتبر");
            public static BotState Login = new BotState(12, "ورود اعضای خانواده");

            public static BotState ShowZanbilContentForNormalUser = new BotState(18, "نمایش محتوی زنبیل برای کاربر عادی");

            public static BotState ShowAdminMenu = new BotState(30, "نمایش منوی مدیر سیستم");
        }

        //todo: تبدیل این ساختار سشن فعلی به متغیرهای موجود در محیط
        internal class TeleZanbilSessionData : SessionData
        {
            public TeleZanbilSessionData(long userID) : base(userID)
            {
            }

            public Family family;
        }

        internal TeleZanbilSessionData currentTZSessionData;
        internal TeleZanbilContext tzdb;

        public TeleZanbil()
        {
            tzdb = (TeleZanbilContext)db;

            if (tzdb.Roles.Count() == 0)
            {
                var adminRole = tzdb.Roles.Add(new Role() { RoleName = "Admin" });
                tzdb.Roles.Add(new Role() { RoleName = "Father" });
                tzdb.Roles.Add(new Role() { RoleName = "Normal" });

                //tzdb.Users.Add(new Models.User() { TelegramUserID = 88008464, UserRole = adminRole });
            }

            if (tzdb.Units.Count() == 0)
            {
                tzdb.Units.Add(new Unit() { Title = "عدد" });
                tzdb.Units.Add(new Unit() { Title = "بسته" });
                tzdb.Units.Add(new Unit() { Title = "کیلوگرم" });
                tzdb.Units.Add(new Unit() { Title = "گرم" });
                tzdb.Units.Add(new Unit() { Title = "میلی گرم" });
                tzdb.Units.Add(new Unit() { Title = "متر" });
                tzdb.Units.Add(new Unit() { Title = "سانتی متر" });
                tzdb.Units.Add(new Unit() { Title = "میلی متر" });
                tzdb.Units.Add(new Unit() { Title = "لیتر" });
            }
        }

        public override void addNewUserSession(long currentUserID)
        {
            sessionDataList.Add(new TeleZanbilSessionData(currentUserID));
        }

        public override Message convertData(Message m)
        {
            return m;
        }

        public override void defineNFARulePostFunctions()
        {
            nfa.addRulePostFunction(TeleZanbilStates.CheckUserType, TeleZanbilStates.Start, (PostFunctionData pfd) =>
            {
                string roleName = "";
                var userList = tzdb.Users.Where(u => u.TelegramUserID == pfd.m.Chat.Id);
                if (userList.Count() > 0)
                {
                    var user = userList.First();

                    roleName = user.UserRole.RoleName;

                    if (roleName == "Father")
                    {
                        int familyID = user.UserFamily.FamilyId;
                        currentTZSessionData.family = tzdb.Families.Where(f => f.FamilyId == familyID).First();
                    }
                }

                actUsingCustomAction(pfd.m, roleName);
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetMainCommand, async (PostFunctionData pfd) =>
            {
                InlineKeyboardMarkup mainKeyboard = KeyboardGenerator.makeKeyboard(new string[] { "ثبت خانواده جدید", "درباره تله زنبیل" }, 2, false);
                await bot.SendTextMessageAsync(pfd.target, "لطفاً انتخاب کنید", replyMarkup: mainKeyboard);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowAboutUs, async (PostFunctionData pfd) =>
            {
                //todo نوشتن تابع حذف آخرین کیبورد
                //todo: تکمیل متن و عکس درباره ما
                await bot.SendTextMessageAsync(pfd.target, "تله زنبیل\nمدیریت زنبیل خانواده");
            });

            nfa.addRulePostFunction(TeleZanbilStates.StartRegFamily, (PostFunctionData pfd) =>
            {
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetFamilyName, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً نام خانواده خود را وارد نمائید");
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContentForFather, async (PostFunctionData pfd) =>
            {
                var mainZanbil = tzdb.Zanbils.Where(z => z.Family.FamilyId == currentTZSessionData.family.FamilyId).First();
                var zanbilItems = tzdb.ZanbilItems.Where(zi => zi.Zanbil.ZanbilId == mainZanbil.ZanbilId);
                string[] zanbilItemsTitle = new string[zanbilItems.Count()];
                for (int i = 0; i < zanbilItems.Count(); i++)
                {
                    ZanbilItem zi = zanbilItems.ToArray<ZanbilItem>()[i];
                    zanbilItemsTitle[i] = zi.ItemTitle + " (" + zi.ItemAmount + " " + zi.ItemUnit.Title + ")";
                }
                InlineKeyboardMarkup zanbilContentKeyboard = KeyboardGenerator.makeVerticalKeyboard(zanbilItemsTitle);

                await bot.SendTextMessageAsync(pfd.target, "زنبیل خانواده " + currentTZSessionData.family.FamilyName, replyMarkup: zanbilContentKeyboard);
            });

            // ثبت خانواده
            nfa.addRulePostFunction(TeleZanbilStates.RegisterFamily, (PostFunctionData pfd) =>
            {
                // گرفتن اسم خانواده از ورودی کاربر
                //currentTZSessionData.familyName = pfd.action;
                string familyName = pfd.action;

                //todo: بررسی تکراری نبودن خانواده
                //ثبت خانواده
                var family = tzdb.Families.Add(new Family() { FamilyName = familyName });
                currentTZSessionData.family = family;
                tzdb.SaveChanges();

                // ثبت کاربر و زنبیل اصلی مربوط به این خانواده
                var fatherRole = tzdb.Roles.Where(r => r.RoleName == "Father").First();
                tzdb.Users.Add(new Models.User() { UserRole = fatherRole, TelegramUserID = pfd.m.Chat.Id, UserFamily = family });
                var mainZanbil = tzdb.Zanbils.Add(new Zanbil() { ZanbilName = "زنبیل اصلی خانواده " + familyName, Family = family });
                tzdb.SaveChanges();

                /*tzdb.ZanbilItems.Add(new ZanbilItem() { ItemTitle = "سیب", ItemAmount = 5, Zanbil = mainZanbil, IsBought = false, ItemUnit = tzdb.Units.Where(u => u.Title == "عدد").First(), BuyDate = DateTime.Now });
                tzdb.ZanbilItems.Add(new ZanbilItem() { ItemTitle = "شیر", ItemAmount = 2, Zanbil = mainZanbil, IsBought = false, ItemUnit = tzdb.Units.Where(u => u.Title == "لیتر").First(), BuyDate = DateTime.Now });
                tzdb.ZanbilItems.Add(new ZanbilItem() { ItemTitle = "پنیر", ItemAmount = 1, Zanbil = mainZanbil, IsBought = false, ItemUnit = tzdb.Units.Where(u => u.Title == "بسته").First(), BuyDate = DateTime.Now });
                tzdb.SaveChanges();*/
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowAdminMenu, TeleZanbilStates.CheckUserType, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "منوی مدیر سیستم");
            });

            //nfa.addRulePostFunction(TeleZanbilStates.GetMainCommand, (PostFunctionData pfd) =>
            //{
            //});
        }

        public override void defineNFARules()
        {
            nfa.addRule(TeleZanbilStates.Start, "/start", TeleZanbilStates.CheckUserType);

            nfa.addRule(TeleZanbilStates.CheckUserType, "", TeleZanbilStates.GetMainCommand);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Admin", TeleZanbilStates.ShowAdminMenu);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Father", TeleZanbilStates.ShowZanbilContentForFather);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Normal", TeleZanbilStates.ShowZanbilContentForNormalUser);

            nfa.addRule(TeleZanbilStates.GetMainCommand, 1, TeleZanbilStates.StartRegFamily);
            nfa.addRule(TeleZanbilStates.GetMainCommand, 2, TeleZanbilStates.ShowAboutUs);
            nfa.addRule(TeleZanbilStates.GetMainCommand, 3, TeleZanbilStates.Login);
            nfa.addElseRule(TeleZanbilStates.GetMainCommand, TeleZanbilStates.ShowInvalidCommand);

            nfa.addRule(TeleZanbilStates.ShowAboutUs, TeleZanbilStates.GetMainCommand);

            nfa.addRule(TeleZanbilStates.ShowInvalidCommand, TeleZanbilStates.GetMainCommand);

            nfa.addRule(TeleZanbilStates.StartRegFamily, TeleZanbilStates.GetFamilyName);

            nfa.addRegexRule(TeleZanbilStates.GetFamilyName, ".*", TeleZanbilStates.RegisterFamily);

            nfa.addRule(TeleZanbilStates.RegisterFamily, TeleZanbilStates.ShowZanbilContentForFather);
        }

        public override List<long> getAuthenticatedUsers()
        {
            return new List<long>();
        }

        public override void getConvertedSessionData(Message m)
        {
            //todo انتقال این خط به کتابخانه
            currentSessionData = checkSessionAndGetCurrentUserData(m);
            currentTZSessionData = (TeleZanbilSessionData)currentSessionData;
        }

        public override void initDatabase()
        {
            this.db = new TeleZanbilContext();
        }
    }
}
