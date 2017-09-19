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
            public static BotState CheckUserType = new BotState(2, "");
            public static BotState GetMainCommand = new BotState(3, "گرفتن دستور اصلی");
            public static BotState ShowAboutUs = new BotState(4, "");
            public static BotState StartRegFamily = new BotState(5, "");
            public static BotState GetFamilyName = new BotState(6, "");
            public static BotState RegisterFamily = new BotState(7, "");
            public static BotState ShowZanbilContentForFather = new BotState(8, "");
            public static BotState AcceptZanbilItem = new BotState(9, "تایید آیتم زنبیل");
            public static BotState AddNewZanbilItem = new BotState(10, "اضافه کردن آیتم جدید به زنبیل");

            public static BotState ShowInvalidCommand = new BotState(11, "نمایش ورودی نامعتبر");

            public static BotState ShowZanbilContentForNormalUser = new BotState(18, "نمایش محتوی زنبیل برای کاربر عادی");

            public static BotState ShowAdminMenu = new BotState(30, "نمایش منوی مدیر سیستم");
        }

        internal class TeleZanbilSessionData : SessionData
        {
            public TeleZanbilSessionData(long userID) : base(userID)
            {
            }
        }

        internal TeleZanbilSessionData currentTZSessionData;
        internal TeleZanbilContext tzdb;

        public TeleZanbil()
        {
            tzdb = (TeleZanbilContext)db;

            if (tzdb.roles.Count() == 0)
            {
                var adminRole = tzdb.roles.Add(new Role() { RoleName = "Admin" });
                tzdb.roles.Add(new Role() { RoleName = "Father" });
                tzdb.roles.Add(new Role() { RoleName = "Normal" });

                //tzdb.users.Add(new Models.User() { TelegramUserID = 88008464, UserRole = new Role() { RoleName = "Admin" } });
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
                var user = tzdb.users.Where(u => u.TelegramUserID == pfd.m.Chat.Id);
                if (user.Count() > 0)
                    roleName = user.First().UserRole.RoleName;
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
                await bot.SendTextMessageAsync(pfd.target, "تله زنبیل\nمدیریت زنبیل خانواده");
                actUsingLambdaAction(pfd.m);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowAdminMenu, TeleZanbilStates.CheckUserType, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "منوی مدیر سیستم");
                //actUsingLambdaAction(pfd.m);
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
            nfa.addElseRule(TeleZanbilStates.GetMainCommand, TeleZanbilStates.ShowInvalidCommand);

            nfa.addRule(TeleZanbilStates.ShowAboutUs, TeleZanbilStates.GetMainCommand);

            nfa.addRule(TeleZanbilStates.ShowInvalidCommand, TeleZanbilStates.GetMainCommand);

            nfa.addRule(TeleZanbilStates.StartRegFamily, TeleZanbilStates.GetFamilyName);

            nfa.addRule(TeleZanbilStates.GetFamilyName, TeleZanbilStates.RegisterFamily);
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
