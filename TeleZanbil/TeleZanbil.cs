using System;
using System.Collections.Generic;
using ir.EmIT.EmITBotNet;
using Telegram.Bot.Types;
using ir.EmIT.EmITBotNet.NFAUtility;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using ir.EmIT.TeleZanbil.Models;
using System.IO;
using System.Threading.Tasks;

namespace ir.EmIT.TeleZanbil
{
    class TeleZanbil : EmITBotNetBase
    {
        //todo: imp: امکان خروج از سیستم
        //todo: imp: خروج اعضا به راحتی با حذف کاربر
        //todo: imp: خروج پدر هم با حذف منطقی همه چیز باشد
        //todo: imp: دکمه بازگشت از بخش ورود به سیستم
        //todo: imp: دکمه بازگشت از بخش تنظیمات
        //todo: imp: تست همزمان دو کاربر
        //todo: imp: کانفیگ تغییر زبان به کرمونی
        //todo: imp: دیدن لیست خانواده
        //todo: imp: درباره ما در زمان پس از لاگین
        //todo: imp: همیشه پس از کلیک روی دکمه ها، آن صفحه کلید حذف شده و لاگ آن بماند

        //todo: امکان دعوت از دیگران با ارسال کد
        //todo: نمایش سابقه خرید
        //todo: تحلیل پنل مدیریتی
        //todo: انتقال دکمه ها به منوی اصلی جدا از لیست زنبیل
        //todo: کانفیگ دکمه ها این.لاین یا در باکس اصلی
        //todo: امکان حذف اعضای خانواده

        #region کلاس های مورداستفاده
        class TeleZanbilStates : BotStates
        {
            // بررسی نوع کاربر
            public static BotState CheckUserType = new BotState(2, "بررسی نوع کاربر");

            // منوی اصلی قبل از ورود
            public static BotState GetMainCommand = new BotState(3, "گرفتن دستور اصلی");
            public static BotState ShowAboutUs = new BotState(4, "نمایش درباره ما");
            public static BotState StartRegFamily = new BotState(5, "شروع روال ثبت خانواده");
            public static BotState Login = new BotState(6, "ورود اعضای خانواده");
            public static BotState ShowWelcomeForNormalUsers = new BotState(27, "نمایش پیام خوش آمدگویی اعضا و پیام راهنما");
            public static BotState ShowInvalidCommand = new BotState(7, "نمایش ورودی نامعتبر");

            // روال ثبت خانواده
            public static BotState GetFamilyName = new BotState(8, "دریافت اسم خانواده");
            public static BotState RegisterFamily = new BotState(9, "ثبت خانواده");
            public static BotState ShowWelcomeForFather = new BotState(26, "نمایش پیام خوش آمدگویی پدر و پیام راهنما");

            // روال ورود
            public static BotState GetInputCode = new BotState(10, "دریافت کد ورود");
            public static BotState CheckInputCode = new BotState(11, "بررسی کد ورود");
            public static BotState FalseInputCode = new BotState(12, "غلط بودن کد ورود");
            public static BotState ShowFalseInputCode = new BotState(13, "نمایش غلط بودن کد ورود");
            public static BotState TrueInputCode = new BotState(14, "درست بودن کد ورود");

            // نمایش محتوی زنبیل
            public static BotState ShowZanbilContent = new BotState(15, "نمایش محتوی زنبیل");

            // دکمه های دیگر صفحه نمایش زنبیل
            public static BotState AddNewZanbilItem = new BotState(16, "اضافه کردن آیتم جدید به زنبیل");
            public static BotState RefreshZanbil = new BotState(25, "تازه سازی زنبیل");
            public static BotState ShowInviteCode = new BotState(28, "نمایش کد دعوت");
            public static BotState RegenerateInviteCode = new BotState(29, "بازسازی کد دعوت");
            public static BotState ShowAboutA = new BotState(31, "نمایش درباره تله زنبیل a");
            public static BotState ShowFamilyList = new BotState(32, "نمایش لیست خانواده");
            public static BotState ShowHelp = new BotState(33, "نمایش راهنما");
            public static BotState Logout = new BotState(34, "خروج از سیستم");
            public static BotState AskHistoryType = new BotState(35, "پرسیدن نوع  نمایش سابقه زنبیل");
            public static BotState Config = new BotState(38, "نمایش صفحه کانفیگ");

            // روال افزودن کالا
            public static BotState ShowSuggestion = new BotState(17, "نمایش پیشنهادات کالا");
            public static BotState GetZanbilItemName = new BotState(18, "پرسیدن اسم کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemAmount = new BotState(19, "پرسیدن مقدار کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemUnit = new BotState(20, "پرسیدن واحد کالا برای افزودن به زنبیل");
            public static BotState SaveZanbilItem = new BotState(21, "افزودن کالا به زنبیل");

            // روال تایید خرید کالا
            public static BotState CheckAcceptZanbilItemPermission = new BotState(22, "بررسی مجوز تایید خرید آیتم زنبیل براساس نقش کاربر جاری");
            public static BotState AcceptZanbilItem = new BotState(23, "تایید خرید آیتم زنبیل");
            public static BotState NotHaveAcceptPermission = new BotState(24, "عدم داشتن مجوز برای تایید خرید آیتم زنبیل");

            // نمایش سابقه زنبیل
            public static BotState ShowSummaryHistory = new BotState(36, "نمایش خلاصه سابقه زنبیل");
            public static BotState ShowFullHistory = new BotState(37, "نمایش مفصل سابقه زنبیل");

            // صفحه کانفیگ
            public static BotState AskLanguage = new BotState(39, "نمایش صفحه تغییر زبان");
            public static BotState ChangeLanguage = new BotState(40, "تغییر زبان");
            public static BotState AskKeyboardPlace = new BotState(41, "نمایش صفحه تغییر محل دکمه ها");
            public static BotState ChangeKeyboardPlace = new BotState(42, "تغییر محل دکمه ها");

            // مدیریت سیستم
            public static BotState ShowAdminMenu = new BotState(30, "نمایش منوی مدیر سیستم");

        }

        //todo: تبدیل این ساختار سشن فعلی به متغیرهای موجود در محیط
        internal class TeleZanbilSessionData : SessionData
        {
            public TeleZanbilSessionData(long userID) : base(userID)
            {
            }

            public Family family;
            public string userRole;

            public int lastMsgId;

            public int zanbilItemNo;

            public string zanbilItemName;
            public int zanbilItemAmount;
            public string zanbilItemUnit;

            public string inputCode;
        }
        #endregion

        internal TeleZanbilSessionData currentTZSessionData;
        internal TeleZanbilContext tzdb;

        #region توابع سیستمی
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
                tzdb.Units.Add(new Unit() { Title = "تا" });
                tzdb.Units.Add(new Unit() { Title = "قالب" });
                tzdb.Units.Add(new Unit() { Title = "بسته" });
                tzdb.Units.Add(new Unit() { Title = "عدد" });
                tzdb.Units.Add(new Unit() { Title = "مثقال" });
                tzdb.Units.Add(new Unit() { Title = "میلی گرم" });
                tzdb.Units.Add(new Unit() { Title = "گرم" });
                tzdb.Units.Add(new Unit() { Title = "کیلو" });
                tzdb.Units.Add(new Unit() { Title = "لیتر" });
                tzdb.Units.Add(new Unit() { Title = "میلی متر" });
                tzdb.Units.Add(new Unit() { Title = "سانتی متر" });
                tzdb.Units.Add(new Unit() { Title = "متر" });
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

        public override void defineNFARules()
        {
            nfa.addRule(TeleZanbilStates.Start, "/start", TeleZanbilStates.CheckUserType);
            //nfa.addRule(TeleZanbilStates.Start, "Normal", TeleZanbilStates.ShowZanbilContent);
            nfa.addElseRule(TeleZanbilStates.Start, TeleZanbilStates.Start);

            nfa.addRule(TeleZanbilStates.CheckUserType, "Unauthorized", TeleZanbilStates.GetMainCommand);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Admin", TeleZanbilStates.ShowAdminMenu);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Father", TeleZanbilStates.ShowZanbilContent);
            nfa.addRule(TeleZanbilStates.CheckUserType, "Normal", TeleZanbilStates.ShowZanbilContent);

            nfa.addRule(TeleZanbilStates.GetMainCommand, 1, TeleZanbilStates.StartRegFamily);
            nfa.addRule(TeleZanbilStates.GetMainCommand, 2, TeleZanbilStates.Login);
            nfa.addRule(TeleZanbilStates.GetMainCommand, 3, TeleZanbilStates.ShowAboutUs);
            nfa.addElseRule(TeleZanbilStates.GetMainCommand, TeleZanbilStates.ShowInvalidCommand);

            nfa.addRule(TeleZanbilStates.ShowAboutUs, TeleZanbilStates.GetMainCommand);

            nfa.addRule(TeleZanbilStates.ShowInvalidCommand, TeleZanbilStates.GetMainCommand);


            nfa.addRule(TeleZanbilStates.StartRegFamily, TeleZanbilStates.GetFamilyName);
            nfa.addRegexRule(TeleZanbilStates.GetFamilyName, ".*", TeleZanbilStates.RegisterFamily);
            nfa.addRule(TeleZanbilStates.RegisterFamily, TeleZanbilStates.ShowWelcomeForFather);
            nfa.addRule(TeleZanbilStates.ShowWelcomeForFather, TeleZanbilStates.ShowZanbilContent);


            nfa.addRule(TeleZanbilStates.Login, "CANCEL_LOGIN_CMD_EMIT", TeleZanbilStates.GetMainCommand);
            nfa.addRegexRule(TeleZanbilStates.Login, ".+", TeleZanbilStates.GetInputCode);
            nfa.addRule(TeleZanbilStates.GetInputCode, TeleZanbilStates.CheckInputCode);
            nfa.addRule(TeleZanbilStates.CheckInputCode, "0", TeleZanbilStates.FalseInputCode);
            nfa.addRule(TeleZanbilStates.CheckInputCode, "1", TeleZanbilStates.TrueInputCode);
            nfa.addRule(TeleZanbilStates.FalseInputCode, TeleZanbilStates.ShowFalseInputCode);
            nfa.addRule(TeleZanbilStates.ShowFalseInputCode, TeleZanbilStates.Login);
            nfa.addRule(TeleZanbilStates.TrueInputCode, TeleZanbilStates.ShowWelcomeForNormalUsers);
            nfa.addRule(TeleZanbilStates.ShowWelcomeForNormalUsers, TeleZanbilStates.ShowZanbilContent);


            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "add", TeleZanbilStates.AddNewZanbilItem);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "refresh", TeleZanbilStates.RefreshZanbil);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "config", TeleZanbilStates.Config);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "-1", TeleZanbilStates.ShowZanbilContent);
            nfa.addRegexRule(TeleZanbilStates.ShowZanbilContent, "[0-9]+", TeleZanbilStates.CheckAcceptZanbilItemPermission);

            nfa.addRule(TeleZanbilStates.RefreshZanbil, TeleZanbilStates.ShowZanbilContent);

            //todo: سه مرحله دریافت اطلاعات هرکدام تقسیم شوند به دو مرحله نمایش پیام و دریافت مقدار
            nfa.addRule(TeleZanbilStates.AddNewZanbilItem, TeleZanbilStates.GetZanbilItemName);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemName, ".*", TeleZanbilStates.GetZanbilItemAmount);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemAmount, "[0-9]+", TeleZanbilStates.GetZanbilItemUnit);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemUnit, ".+", TeleZanbilStates.SaveZanbilItem);
            nfa.addRule(TeleZanbilStates.SaveZanbilItem, TeleZanbilStates.ShowZanbilContent);


            nfa.addRule(TeleZanbilStates.CheckAcceptZanbilItemPermission, "Father", TeleZanbilStates.AcceptZanbilItem);
            nfa.addElseRule(TeleZanbilStates.CheckAcceptZanbilItemPermission, TeleZanbilStates.NotHaveAcceptPermission);
            nfa.addRule(TeleZanbilStates.AcceptZanbilItem, TeleZanbilStates.ShowZanbilContent);
            nfa.addRule(TeleZanbilStates.NotHaveAcceptPermission, TeleZanbilStates.ShowZanbilContent);

            nfa.addRule(TeleZanbilStates.ShowInviteCode, TeleZanbilStates.ShowZanbilContent);
            nfa.addRule(TeleZanbilStates.RegenerateInviteCode, TeleZanbilStates.ShowZanbilContent);

            nfa.addRule(TeleZanbilStates.Config, "inviteCode", TeleZanbilStates.ShowInviteCode);
            nfa.addRule(TeleZanbilStates.Config, "regenerateInviteCode", TeleZanbilStates.RegenerateInviteCode);
            nfa.addRule(TeleZanbilStates.Config, "history", TeleZanbilStates.AskHistoryType);
            nfa.addRule(TeleZanbilStates.Config, "family", TeleZanbilStates.ShowFamilyList);
            nfa.addRule(TeleZanbilStates.Config, "language", TeleZanbilStates.AskLanguage);
            nfa.addRule(TeleZanbilStates.Config, "keyboardPlace", TeleZanbilStates.AskKeyboardPlace);
            nfa.addRule(TeleZanbilStates.Config, "help", TeleZanbilStates.ShowHelp);
            nfa.addRule(TeleZanbilStates.Config, "logout", TeleZanbilStates.Logout);
            nfa.addRule(TeleZanbilStates.Config, "about", TeleZanbilStates.ShowAboutA);

            nfa.addRule(TeleZanbilStates.ShowHelp, TeleZanbilStates.ShowZanbilContent);

            /*
            ShowAdminMenu
            */
        }

        public override void defineNFARulePostFunctions()
        {
            nfa.addRulePostFunction(TeleZanbilStates.Start, TeleZanbilStates.Start, async (PostFunctionData pfd) =>
            {
                //todo: بررسی وجود کد ورود در کامند start
                await bot.SendTextMessageAsync(pfd.target, "لطفاً برای شروع 🏃 از دستور زیر استفاده کنید :\n/start");
            });

            nfa.addRulePostFunction(TeleZanbilStates.CheckUserType, TeleZanbilStates.Start, (PostFunctionData pfd) =>
            {
                string roleName = "Unauthorized";

                // بررسی اینکه آیا کاربری متناظر کاربر جاری بات در دیتابیس وجود دارد یا نه؟
                var userList = tzdb.Users.Where(u => u.TelegramUserID == pfd.m.Chat.Id && u.IsDeleted == false);
                if (userList.Count() > 0)
                {
                    var user = userList.First();

                    // گرفتن نقش کاربر جاری (ذخیره شده در دیتابیس)
                    roleName = user.UserRole.RoleName;

                    currentTZSessionData.userRole = roleName;
                    currentTZSessionData.telegramUserID = user.TelegramUserID;

                    // ذخیره کردن اطلاعات خانواده کاربر جاری در داده های جلسه، در صورتی که شخص ورودی پدر باشد
                    if (roleName == "Father")
                    {
                        int familyID = user.UserFamily.FamilyId;
                        currentTZSessionData.family = tzdb.Families.Where(f => f.FamilyId == familyID).First();
                    }
                    else if (roleName == "Normal")
                    {
                        int familyID = user.UserFamily.FamilyId;
                        currentTZSessionData.family = tzdb.Families.Where(f => f.FamilyId == familyID).First();
                    }
                }

                // ایجاد یک عمل (اکشن) جدید با استفاده از نقش کاربر جاری
                actUsingCustomAction(pfd.m, roleName);
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetMainCommand, async (PostFunctionData pfd) =>
            {
                InlineKeyboardMarkup mainKeyboard = KeyboardGenerator.makeKeyboard(new string[] {
                    "ثبت خانواده 👨‍👩‍👧‍👧 جدید",
                    "پیوستن به خانواده 🚶",
                    "درباره 💡 تله زنبیل"
                }, 2, false);
                Message m2 = await bot.SendTextMessageAsync(pfd.target, "لطفاً انتخاب کنید", replyMarkup: mainKeyboard);
                currentTZSessionData.lastMsgId = m2.MessageId;
            });

            // نمایش درباره ما
            nfa.addRulePostFunction(TeleZanbilStates.ShowAboutUs, async (PostFunctionData pfd) =>
            {
                await bot.DeleteMessageAsync(pfd.target, currentTZSessionData.lastMsgId);

                //todo: imp: تکمیل عکس درباره ما
                await bot.SendPhotoAsync(pfd.target,
                    new FileToSend("AboutPoster", new FileStream("Images\\AboutZanbil.png", FileMode.Open)),
                    "🛍 تله زنبیل 🛍" + "\r\n" +
                    "💥 زنبیل تلگرامی خانواده 💥" + "\n" +
                    "🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟" + "\n" +
                    "با استفاده از تله زنبیل می توانید لیست خرید خود و خانواده تان را مدیریت کنید" + "\n" +
                    "🔸🔹🔸🔹🔸🔹🔸🔹🔸" + "\n" +
                    "@TeleZanbilBot"
                    );
            });

            // پرسیدن نام خانواده
            nfa.addRulePostFunction(TeleZanbilStates.GetFamilyName, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً نام خانواده 👨‍👩‍👧‍👧 خود را وارد نمائید");
            });

            // ثبت خانواده
            nfa.addRulePostFunction(TeleZanbilStates.RegisterFamily, (PostFunctionData pfd) =>
            {
                // گرفتن اسم خانواده از ورودی کاربر
                string familyName = pfd.action;

                //ثبت خانواده
                var family = tzdb.Families.Add(new Family() { FamilyName = familyName , InviteCode = getNewInviteCode(), IsDeleted = false });
                currentTZSessionData.family = family;
                currentTZSessionData.userRole = "Father";
                currentTZSessionData.telegramUserID = pfd.m.From.Id;
                tzdb.SaveChanges();

                // ثبت کاربر و زنبیل اصلی مربوط به این خانواده
                var fatherRole = tzdb.Roles.Where(r => r.RoleName == "Father").First();
                tzdb.Users.Add(new Models.User() { UserRole = fatherRole, TelegramUserID = pfd.m.Chat.Id, UserFamily = family, IsDeleted = false });
                var mainZanbil = tzdb.Zanbils.Add(new Zanbil() { ZanbilName = "زنبیل اصلی خانواده " + familyName, Family = family });
                tzdb.SaveChanges();
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowWelcomeForFather, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, 
                    "آقا/خانم " + pfd.m.From.FirstName + " " + pfd.m.From.LastName + (pfd.m.From.Username != "" ? " (" + pfd.m.From.Username + ")" : "") +
                    " خوش آمدید 🌼" + "\n" +
                    "خانواده شما با نام «" + currentTZSessionData.family.FamilyName + "» ثبت شد 👌🏻"
                );
                await showHelpForFatherAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.AcceptZanbilItem, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.SaveZanbilItem, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.CheckUserType, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.ShowWelcomeForFather, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.ShowWelcomeForNormalUsers, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.RefreshZanbil, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.ShowHelp, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.CheckAcceptZanbilItemPermission, (PostFunctionData pfd) =>
            {
                currentTZSessionData.zanbilItemNo = Convert.ToInt32(pfd.m.Text);
                actUsingCustomAction(pfd.m, currentTZSessionData.userRole);
            });

            nfa.addRulePostFunction(TeleZanbilStates.AcceptZanbilItem, TeleZanbilStates.CheckAcceptZanbilItemPermission, async (PostFunctionData pfd) =>
            {
                var mainZanbil = getMainZanbil();

                var zanbilItems = tzdb.ZanbilItems.Where(zi => zi.Zanbil.ZanbilId == mainZanbil.ZanbilId && zi.IsBought == false);
                if (currentTZSessionData.zanbilItemNo > 0 && currentTZSessionData.zanbilItemNo <= zanbilItems.Count())
                {
                    var zanbilItem = zanbilItems.ToArray()[currentTZSessionData.zanbilItemNo - 1];
                    zanbilItem.IsBought = true;
                    zanbilItem.BuyDate = DateTime.Now;
                    tzdb.SaveChanges();

                    await bot.SendTextMessageAsync(pfd.target, "«" + zanbilItem.ItemAmount + " " + zanbilItem.ItemUnit.Title + " " + zanbilItem.ItemTitle + "» خریداری و از لیست حذف شد 💵");
                }
                else
                    await bot.SendTextMessageAsync(pfd.target, "شماره آیتم ورودی نامعتبر ⛔️ می باشد");
            });


            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemName, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً نام کالای درخواستی 🛒 را وارد نمائید");
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemAmount, async (PostFunctionData pfd) =>
            {
                // گرفتن اسم کالای درخواستی از مرحله قبل
                currentTZSessionData.zanbilItemName = pfd.action;

                // نمایش لیست و درخواست ورود مقدار کالای درخواستی
                //todo: imp: کیبورد شامل ربع و نیم و ضرایب 10 هم باشد
                InlineKeyboardMarkup numberKeyboard = KeyboardGenerator.makeNumberMatrixKeyboard(1, 9, 3);
                await bot.SendTextMessageAsync(pfd.target, "لطفاً مقدار کالای درخواستی 🛒 را انتخاب کنید یا در صورت نیاز مقدار دقیق آن را وارد نمائید", replyMarkup: numberKeyboard);
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemUnit, async (PostFunctionData pfd) =>
            {
                //todo: imp: امکان ثبت اعداد اعشار
                // گرفتن مقدار کالای درخواستی از مرحله قبل
                currentTZSessionData.zanbilItemAmount = Convert.ToInt32(pfd.action);

                var unitNames = tzdb.Units.Select(u => u.Title);
                string[] unitNamesStr = new string[unitNames.Count()];
                for (int i = 0; i < unitNames.Count(); i++)
                {
                    unitNamesStr[i] = unitNames.ToArray()[i];
                }
                //todo: کیبورد راست به چپ باشد
                InlineKeyboardMarkup unitsKeyboard = KeyboardGenerator.makeKeyboard(unitNamesStr, 4, false, unitNamesStr);
                    
                await bot.SendTextMessageAsync(pfd.target, "لطفاً واحد کالای درخواستی 🛒 را انتخاب یا یا در صورت عدم وجود در لیست نام دقیق آن را وارد نمائید", replyMarkup: unitsKeyboard);
            });

            nfa.addRulePostFunction(TeleZanbilStates.SaveZanbilItem, async (PostFunctionData pfd) =>
            {
                // گرفتن واحد کالای درخواستی از مرحله قبل
                currentTZSessionData.zanbilItemUnit = pfd.action;

                // گرفتن زنبیل اصلی خانواده
                var mainZanbil = getMainZanbil();
                // پیدا کردن واحد متناظر واحد ورودی
                var foundUnits = tzdb.Units.Where(u => u.Title == currentTZSessionData.zanbilItemUnit);
                Unit unit;
                if (foundUnits.Count() == 0)
                    unit = tzdb.Units.Add(new Unit() { Title = currentTZSessionData.zanbilItemUnit });
                else
                    unit = foundUnits.First();
                int userID = tzdb.Users.Where(u => u.TelegramUserID == currentTZSessionData.telegramUserID).First().UserId;
                // ثبت آیتم در زنبیل
                tzdb.ZanbilItems.Add(new ZanbilItem() { ItemTitle = currentTZSessionData.zanbilItemName, ItemAmount = currentTZSessionData.zanbilItemAmount, Zanbil = mainZanbil, IsBought = false, ItemUnit = unit, BuyDate = DateTime.Now, CreatorUserID = userID });
                tzdb.SaveChanges();

                //todo: imp: حذف همه پیام های در حین افزودن کالا به زنبیل
                await bot.SendTextMessageAsync(pfd.target, "«" + currentTZSessionData.zanbilItemAmount + " " + currentTZSessionData.zanbilItemUnit + " " + currentTZSessionData.zanbilItemName + "» 🛒 به زنبیل خانواده شما اضافه شد 👌");
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowAdminMenu, TeleZanbilStates.CheckUserType, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "منوی مدیر سیستم");
            });

            nfa.addRulePostFunction(TeleZanbilStates.Login, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً کد ورودی خود را وارد فرمائید");
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetInputCode, (PostFunctionData pfd) =>
            {
                currentTZSessionData.inputCode = pfd.action;
            });

            nfa.addRulePostFunction(TeleZanbilStates.CheckInputCode, (PostFunctionData pfd) =>
            {
                var families = tzdb.Families.Where(f => f.InviteCode.Equals(currentTZSessionData.inputCode) && f.IsDeleted == false);
                if(families.Count() == 0)
                {
                    actUsingCustomAction(pfd.m, "0");
                }
                else
                {
                    var normalRole = tzdb.Roles.Where(r => r.RoleName == "Normal").First();
                    tzdb.Users.Add(new Models.User() { UserFamily = families.First(), TelegramUserID = pfd.m.From.Id, UserRole = normalRole, IsDeleted = false });
                    tzdb.SaveChanges();

                    currentTZSessionData.family = families.First();
                    currentTZSessionData.telegramUserID = pfd.m.From.Id;
                    currentTZSessionData.userRole = "Normal";

                    actUsingCustomAction(pfd.m, "1");
                }
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowFalseInputCode, async (PostFunctionData pfd) =>
            {
                //todo: ایجاد ساز و کار وقفه در صورت ورود سه باره کد ورود
                await bot.SendTextMessageAsync(pfd.target, "کد ورودی شما نامعتبر می باشد");
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowWelcomeForNormalUsers, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target,
                    "آقا/خانم " + pfd.m.From.FirstName + " " + pfd.m.From.LastName + (pfd.m.From.Username != "" ? " (" + pfd.m.From.Username + ")" : "") +
                    " خوش آمدید 🌼" + "\n" +
                    "شما به زنبیل خانواده  «" + currentTZSessionData.family.FamilyName + "» پیوستید 👌🏻"
                );
                await showHelpForNormalAsync(pfd);
            });


            nfa.addRulePostFunction(TeleZanbilStates.ShowInviteCode, async (PostFunctionData pfd) =>
            {
                await showInviteCode(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.RegenerateInviteCode, async (PostFunctionData pfd) =>
            {
                // ساخت کد دعوت جدید
                string newInviteCode = getNewInviteCode();

                // ذخیره کردن کد دعوت جدید در دیتابیس
                tzdb.Families.Where(f => f.FamilyId == currentTZSessionData.family.FamilyId).First().InviteCode = newInviteCode;
                tzdb.SaveChanges();

                // ذخیره کد دعوت جدید در داده های سشن فعلی
                currentTZSessionData.family.InviteCode = newInviteCode;

                // نمایش پیام کد دعوت
                await showInviteCode(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.Config, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "................⚙️ تنظیمات ⚙️................", replyMarkup: makeConfigKeyboard());
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowHelp, async (PostFunctionData pfd) =>
            {
                if(currentTZSessionData.userRole.Equals("Father"))
                    await showHelpForFatherAsync(pfd);
                else
                    await showHelpForNormalAsync(pfd);
            });

            //nfa.addRulePostFunction(TeleZanbilStates.GetMainCommand, (PostFunctionData pfd) =>
            //{
            //});
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

        #endregion

        #region توابع بیزینسی
        private InlineKeyboardMarkup makeZanbilContentKeyboard()
        {
            // گرفتن زنبیل اصلی خانواده
            var mainZanbil = getMainZanbil();

            // گرفتن لیست آیتم های زنبیل اصلی
            var zanbilItems = tzdb.ZanbilItems.Where(zi => zi.Zanbil.ZanbilId == mainZanbil.ZanbilId && zi.IsBought == false);
            int ziCount = zanbilItems.Count();

            string[][][] zanbilItemsData;
            if(ziCount == 0)
                zanbilItemsData = new string[2][][];
            else
                zanbilItemsData = new string[ziCount + 1][][];

            // ساخت لیست رشته شامل معرفی آیتم های زنبیل
            int i;
            for (i = 0; i < ziCount; i++)
            {
                zanbilItemsData[i] = new string[1][];
                zanbilItemsData[i][0] = new string[2];
                ZanbilItem zi = zanbilItems.ToArray<ZanbilItem>()[i];
                zanbilItemsData[i][0][0] = zi.ItemTitle + " (" + zi.ItemAmount + " " + zi.ItemUnit.Title + ")";
                zanbilItemsData[i][0][1] = (i + 1).ToString();
            }

            if (ziCount == 0)
            {
                zanbilItemsData[0] = new string[1][];
                zanbilItemsData[0][0] = new string[2];
                zanbilItemsData[0][0][0] = "«زنبیل شما خالی است»";
                zanbilItemsData[0][0][1] = "-1";

                i++;
            }

            //i = ziCount;
            zanbilItemsData[i] = new string[3][];
            zanbilItemsData[i][0] = new string[2];
            zanbilItemsData[i][1] = new string[2];
            zanbilItemsData[i][2] = new string[2];

            // دکمه افزودن کالای جدید
            zanbilItemsData[i][0][0] = "✏️ افزودن";
            zanbilItemsData[i][0][1] = "add";

            // دکمه رفرش
            zanbilItemsData[i][1][0] = "💥 رفرش";
            zanbilItemsData[i][1][1] = "refresh";

            // دکمه کانفیگ
            zanbilItemsData[i][2][0] = "⚙️ تنظیمات";
            zanbilItemsData[i][2][1] = "config";


            // ساخت کیبورد عمودی با استفاده از لیست آیتم های زنبیل
            InlineKeyboardMarkup zanbilContentKeyboard = KeyboardGenerator.makeKeyboard(zanbilItemsData);

            return zanbilContentKeyboard;
        }

        private IReplyMarkup makeConfigKeyboard()
        {
            int rowsCount , i;
            string[][][] configItems;
            if (currentTZSessionData.userRole == "Father")
                rowsCount = 5;
            else //else if (currentTZSessionData.userRole == "Normal")
                rowsCount = 4;

            configItems = new string[rowsCount][][];
            for (i = 0; i < rowsCount; i++)
            {
                configItems[i] = new string[2][];
                configItems[i][0] = new string[2];
                configItems[i][1] = new string[2];
            }

            i = 0;

            if (currentTZSessionData.userRole == "Father")
            {
                // دکمه نمایش کد دعوت
                configItems[i][0][0] = "📣 نمایش کد دعوت";
                configItems[i][0][1] = "inviteCode";

                // دکمه بازسازی کد دعوت
                configItems[i][1][0] = "📜 بازسازی کد دعوت";
                configItems[i][1][1] = "regenerateInviteCode";

                i++;
            }

            // دکمه نمایش سابقه خرید
            configItems[i][0][0] = "🗓 سابقه خرید";
            configItems[i][0][1] = "history";

            // دکمه نمایش لیست اعضای خانواده
            configItems[i][1][0] = "👨‍👩‍👧‍👧 مشخصات خانواده";
            configItems[i][1][1] = "family";

            i++;

            // دکمه نمایش سابقه خرید
            configItems[i][0][0] = "🌏👅 تغییر زبان";
            configItems[i][0][1] = "language";

            // دکمه نمایش لیست اعضای خانواده
            configItems[i][1][0] = "⌨️ تغییر محل کیبورد";
            configItems[i][1][1] = "keyboardPlace";

            i++;

            // دکمه راهنما
            configItems[i][0][0] = "⁉️ راهنما";
            configItems[i][0][1] = "help";

            // دکمه خروج
            configItems[i][1][0] = "🖐❌ خروج از خانواده";
            configItems[i][1][1] = "logout";

            i++;

            configItems[i] = new string[1][];
            configItems[i][0] = new string[2];

            /*// دکمه نمایش سابقه خرید
            configItems[i][0][0] = "💥 سابقه خرید";
            configItems[i][0][1] = "history";*/

            // دکمه درباره ما
            configItems[i][0][0] = "👁‍ درباره ما";
            configItems[i][0][1] = "about";

            InlineKeyboardMarkup configKeyboard = KeyboardGenerator.makeKeyboard(configItems);

            return configKeyboard;
        }

        private Zanbil getMainZanbil()
        {
            // گرفتن زنبیل اصلی خانواده
            var mainZanbil = tzdb.Zanbils.Where(z => z.Family.FamilyId == currentTZSessionData.family.FamilyId).First();
            return mainZanbil;
        }

        private string getNewInviteCode()
        {
            //todo: بررسی تکراری نبودن کد دعوت
            string str = "";
            Random rnd = new Random();
            for (int i = 0; i < 6; i++)
            {
                str += rnd.Next(1, 9).ToString();
            }
            return str;
        }

        private async Task showZanbilContentAsync(PostFunctionData pfd)
        {
            if (currentTZSessionData.lastMsgId != 0)
            {
                // حذف کیبورد قبلی
                await bot.DeleteMessageAsync(pfd.target, currentTZSessionData.lastMsgId);
            }

            // بدست آوردن محتوی زنبیل در قالب یک کیبورد
            InlineKeyboardMarkup zanbilContentKeyboard = makeZanbilContentKeyboard();

            // نمایش پیام و کیبورد لیست آیتم های زنبیل
            Message keyboardMsg = await bot.SendTextMessageAsync(pfd.target, "....🛍 زنبیل خانواده «" + currentTZSessionData.family.FamilyName + "» 🛍....", replyMarkup: zanbilContentKeyboard);
            currentTZSessionData.lastMsgId = keyboardMsg.MessageId;
        }

        private async Task showHelpForFatherAsync(PostFunctionData pfd)
        {
            await bot.SendTextMessageAsync(pfd.target,
                "شما می توانید با کلیک بر روی دکمه «افزودن ✏️» اقلام مختلف کالایی را وارد زنبیل خانواده خود نمائید" + "\n" +
                "همچنین پس از خرید هر یک از اقلام، با کلیک روی آن، آن کالا از زنبیل شما حذف و سابقه خرید آن در بخش سوابق خرید ثبت خواهد شد" + "\n" +
                "همچنین برای افزودن هریک از اعضای خانواده خود، کافی به صفحه «⚙️ تنظیمات» رفته و پس از دریافت (یا بازسازی) کد دعوت، آن را برای اعضای خانواده خود ارسال نمائید" + "\n" +
                "هریک از اعضای خانواده پس از پیوستن به زنبیل خانواده شما می توانند نسبت به افزودن اقلام خریدنی به زنبیل اقدام نمایند. شما با هربار کلیک روی دکمه «💥 رفرش» می توانید از آخرین تغییرات زنبیل خانواده تان مطلع شوید" + "\n" +
                "در صورتی نیاز به پاسخگویی در مورد سوالات بیشتر به کانال «تله زنبیل» مراجعه نموده و یا با مدیر تماس حاصل فرمائید" + "\n" +
                "مدیر : @Em_IT" + "\n" +
                "کانال : @TeleZanbil"
            );
        }

        private async Task showHelpForNormalAsync(PostFunctionData pfd)
        {
            await bot.SendTextMessageAsync(pfd.target,
                "شما می توانید با کلیک بر روی دکمه «افزودن ✏️» اقلام مختلف کالایی را وارد زنبیل خانواده خود نمائید" + "\n" +
                "پدر خانواده پس از خرید هریک از این اقلام، آن ها را از لیست حذف خواهد کرد" + "\n" +
                "هریک از اعضای خانواده پس از پیوستن به زنبیل خانواده شما می توانند نسبت به افزودن اقلام خریدنی به زنبیل اقدام نمایند. شما با هربار کلیک روی دکمه «💥 رفرش» می توانید از آخرین تغییرات زنبیل خانواده تان مطلع شوید" + "\n" +
                "در صورتی نیاز به پاسخگویی در مورد سوالات بیشتر به کانال «تله زنبیل» مراجعه نموده و یا با مدیر تماس حاصل فرمائید" + "\n" +
                "مدیر : @Em_IT" + "\n" +
                "کانال : @TeleZanbil"
            );
        }

        private async Task showInviteCode(PostFunctionData pfd)
        {
            await bot.SendTextMessageAsync(pfd.target,
                    "..................🛍 تله زنبیل 🛍.................." + "\n" +
                    "🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟" + "\n\n" +
                    "کد دعوت 👨‍👩‍👧‍👧 خانواده «" + currentTZSessionData.family.FamilyName + "»:" + "\n" +
                    currentTZSessionData.family.InviteCode + "\n" +
                    "〰️〰️〰️〰️〰️〰️〰️〰️〰️〰️〰〰️〰️" + "\n" +
                    "برای دعوت از سایر اعضای خانواده خود، این کد دعوت را برای آن ها بفرستید" + "\n" +
                    "🛍 تله زنبیل، زنبیل تلگرامی خانواده 🛍" + "\n" +
                    "@TeleZanbilBot"
                    );
        }

        #endregion
    }
}
