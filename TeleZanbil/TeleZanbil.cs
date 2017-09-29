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
        //todo: امکان دعوت از دیگران با ارسال کد
        //todo: نمایش سابقه خرید
        //todo: تحلیل پنل مدیریتی
        //todo: اگر کاربر معمولی روی کالایی کلیک کند رفرش می شود
        //todo: نمایش پیام خوش آمدگویی پس از لاگین
        //todo: امکان خروج از سیستم
        //todo: نمایش پیام مبنی بر خالی بودن زنبیل
        //todo: دکمه بازگشت از بخش ورود به سیستم
        //todo: همیشه پس از کلیک روی دکمه ها، آن صفحه کلید حذف شده و لاگ آن بماند
        //todo: تست همزمان دو کاربر
        //todo: کاربر معمولی دکمه های کد ورود را نبیند
        //todo: گذاشتن دکمه کانفیگ
        //todo: دکمه راهنما
        //todo: دکمه ها آیکونی
        //todo: در اولین ورود به اپ راهنما نمایش داده شود
        //todo: انتقال دکمه ها به منوی اصلی جدا از لیست زنبیل
        //todo: کانفیگ تغییر زبان به کرمونی
        //todo: دیدن لیست خانواده
        //todo: امکان حذف اعضای خانواده
        //todo: خروج اعضا به راحتی با حذف کاربر
        //todo: خروج پدر هم با حذف منطقی همه چیز باشد
        //todo: حذف منطقی
        //todo: کاربر ثبت کننده
        //todo: کانفیگ دکمه ها این.لاین یا در باکس اصلی
        //todo: درباره ما در زمان پس از لاگین

        #region کلاس های مورداستفاده
        class TeleZanbilStates : BotStates
        {
            public static BotState CheckUserType = new BotState(2, "بررسی نوع کاربر");
            public static BotState GetMainCommand = new BotState(3, "گرفتن دستور اصلی");
            public static BotState ShowAboutUs = new BotState(4, "نمایش درباره ما");
            public static BotState StartRegFamily = new BotState(5, "شروع روال ثبت خانواده");
            public static BotState Login = new BotState(6, "ورود اعضای خانواده");
            public static BotState ShowInvalidCommand = new BotState(7, "نمایش ورودی نامعتبر");

            public static BotState GetFamilyName = new BotState(8, "دریافت اسم خانواده");
            public static BotState RegisterFamily = new BotState(9, "ثبت خانواده");

            public static BotState GetInputCode = new BotState(10, "دریافت کد ورود");
            public static BotState CheckInputCode = new BotState(11, "بررسی کد ورود");
            public static BotState FalseInputCode = new BotState(12, "غلط بودن کد ورود");
            public static BotState ShowFalseInputCode = new BotState(13, "نمایش غلط بودن کد ورود");
            public static BotState TrueInputCode = new BotState(14, "درست بودن کد ورود");

            public static BotState ShowZanbilContent = new BotState(15, "نمایش محتوی زنبیل");

            public static BotState AddNewZanbilItem = new BotState(16, "اضافه کردن آیتم جدید به زنبیل");

            public static BotState ShowSuggestion = new BotState(17, "نمایش پیشنهادات کالا");
            public static BotState GetZanbilItemName = new BotState(18, "پرسیدن اسم کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemAmount = new BotState(19, "پرسیدن مقدار کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemUnit = new BotState(20, "پرسیدن واحد کالا برای افزودن به زنبیل");
            public static BotState SaveZanbilItem = new BotState(21, "افزودن کالا به زنبیل");

            public static BotState CheckAcceptZanbilItemPermission = new BotState(22, "بررسی مجوز تایید خرید آیتم زنبیل براساس نقش کاربر جاری");
            public static BotState AcceptZanbilItem = new BotState(23, "تایید خرید آیتم زنبیل");
            public static BotState NotHaveAcceptPermission = new BotState(24, "عدم داشتن مجوز برای تایید خرید آیتم زنبیل");

            public static BotState RefreshZanbil = new BotState(25, "تازه سازی زنبیل");

            public static BotState ShowInviteCode = new BotState(26, "نمایش کد دعوت");
            public static BotState RegenerateInviteCode = new BotState(27, "بازسازی کد دعوت");
            public static BotState ShowHistory = new BotState(28, "نمایش سابقه");
            public static BotState Logout = new BotState(29, "خروج");

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
                tzdb.Units.Add(new Unit() { Title = "عدد" });
                tzdb.Units.Add(new Unit() { Title = "بسته" });
                tzdb.Units.Add(new Unit() { Title = "قالب" });
                tzdb.Units.Add(new Unit() { Title = "تا" });
                tzdb.Units.Add(new Unit() { Title = "کیلو" });
                tzdb.Units.Add(new Unit() { Title = "گرم" });
                tzdb.Units.Add(new Unit() { Title = "میلی گرم" });
                tzdb.Units.Add(new Unit() { Title = "مثقال" });
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
            nfa.addRule(TeleZanbilStates.RegisterFamily, TeleZanbilStates.ShowZanbilContent);


            nfa.addRule(TeleZanbilStates.Login, "CANCEL_LOGIN_CMD_EMIT", TeleZanbilStates.GetMainCommand);
            nfa.addRegexRule(TeleZanbilStates.Login, ".+", TeleZanbilStates.GetInputCode);
            nfa.addRule(TeleZanbilStates.GetInputCode, TeleZanbilStates.CheckInputCode);
            nfa.addRule(TeleZanbilStates.CheckInputCode, "0", TeleZanbilStates.FalseInputCode);
            nfa.addRule(TeleZanbilStates.CheckInputCode, "1", TeleZanbilStates.TrueInputCode);
            nfa.addRule(TeleZanbilStates.FalseInputCode, TeleZanbilStates.ShowFalseInputCode);
            nfa.addRule(TeleZanbilStates.ShowFalseInputCode, TeleZanbilStates.Login);
            nfa.addRule(TeleZanbilStates.TrueInputCode, TeleZanbilStates.ShowZanbilContent);


            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "add", TeleZanbilStates.AddNewZanbilItem);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "refresh", TeleZanbilStates.RefreshZanbil);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "inviteCode", TeleZanbilStates.ShowInviteCode);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "regenerateInviteCode", TeleZanbilStates.RegenerateInviteCode);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "history", TeleZanbilStates.ShowHistory);
            nfa.addRule(TeleZanbilStates.ShowZanbilContent, "logout", TeleZanbilStates.Logout);            
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
                var userList = tzdb.Users.Where(u => u.TelegramUserID == pfd.m.Chat.Id);
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

                //todo: تکمیل عکس درباره ما
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

                //todo: بررسی تکراری نبودن خانواده
                //ثبت خانواده
                var family = tzdb.Families.Add(new Family() { FamilyName = familyName , InviteCode = getNewInviteCode() });
                currentTZSessionData.family = family;
                tzdb.SaveChanges();

                // ثبت کاربر و زنبیل اصلی مربوط به این خانواده
                var fatherRole = tzdb.Roles.Where(r => r.RoleName == "Father").First();
                tzdb.Users.Add(new Models.User() { UserRole = fatherRole, TelegramUserID = pfd.m.Chat.Id, UserFamily = family });
                var mainZanbil = tzdb.Zanbils.Add(new Zanbil() { ZanbilName = "زنبیل اصلی خانواده " + familyName, Family = family });
                tzdb.SaveChanges();
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

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.RegisterFamily, async (PostFunctionData pfd) =>
            {
                await showZanbilContentAsync(pfd);
            });

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContent, TeleZanbilStates.TrueInputCode, async (PostFunctionData pfd) =>
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

            /*nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContentForFather, TeleZanbilStates.AcceptZanbilItem, async (PostFunctionData pfd) =>
            {
                // حذف کیبورد قبلی
                await bot.DeleteMessageAsync(pfd.target, currentTZSessionData.lastMsgId);

                // بدست آوردن محتوی زنبیل در قالب یک کیبورد
                InlineKeyboardMarkup zanbilContentKeyboard = makeZanbilContentKeyboard();

                // آپدیت کیبورد مربوط به پیام لیست آیتم های زنبیل
                //await bot.EditMessageReplyMarkupAsync(pfd.target, currentTZSessionData.lastMsgId, zanbilContentKeyboard);

                // ساخت کیبورد جدید آیتم ها
                Message keyboardMsg = await bot.SendTextMessageAsync(pfd.target, "زنبیل 🛍 خانواده " + currentTZSessionData.family.FamilyName, replyMarkup: zanbilContentKeyboard);
                currentTZSessionData.lastMsgId = keyboardMsg.MessageId;
            });*/

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemName, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً نام کالای درخواستی 🛒 را وارد نمائید");
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemAmount, async (PostFunctionData pfd) =>
            {
                // گرفتن اسم کالای درخواستی از مرحله قبل
                currentTZSessionData.zanbilItemName = pfd.action;

                // نمایش لیست و درخواست ورود مقدار کالای درخواستی
                //todo: کیبورد شامل ربع و نیم و ضرایب 10 هم باشد
                InlineKeyboardMarkup numberKeyboard = KeyboardGenerator.makeNumberMatrixKeyboard(1, 9, 3);
                await bot.SendTextMessageAsync(pfd.target, "لطفاً مقدار کالای درخواستی 🛒 را انتخاب کنید یا در صورت نیاز مقدار دقیق آن را وارد نمائید", replyMarkup: numberKeyboard);
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemUnit, async (PostFunctionData pfd) =>
            {
                //todo: امکان ثبت اعداد اعشار
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
                // ثبت آیتم در زنبیل
                tzdb.ZanbilItems.Add(new ZanbilItem() { ItemTitle = currentTZSessionData.zanbilItemName, ItemAmount = currentTZSessionData.zanbilItemAmount, Zanbil = mainZanbil, IsBought = false, ItemUnit = unit, BuyDate = DateTime.Now });
                tzdb.SaveChanges();

                //todo: حذف همه پیام های در حین افزودن کالا به زنبیل
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
                var families = tzdb.Families.Where(f => f.InviteCode.Equals(currentTZSessionData.inputCode));
                if(families.Count() == 0)
                {
                    actUsingCustomAction(pfd.m, "0");
                }
                else
                {
                    var normalRole = tzdb.Roles.Where(r => r.RoleName == "Normal").First();
                    tzdb.Users.Add(new Models.User() { UserFamily = families.First(), TelegramUserID = pfd.m.From.Id, UserRole = normalRole });
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

            //nfa.addRulePostFunction(TeleZanbilStates.GetMainCommand, (PostFunctionData pfd) =>
            //{
            //});
        }

        private async Task showInviteCode(PostFunctionData pfd)
        {
            await bot.SendTextMessageAsync(pfd.target,
                    "🛍 تله زنبیل 🛍" + "\n" +
                    "کد دعوت 👨‍👩‍👧‍👧 خانواده " + currentTZSessionData.family.FamilyName + ":" + "\n" +
                    currentTZSessionData.family.InviteCode + "\n" +
                    "برای دعوت از سایر اعضای خانواده خود، این کد دعوت را برای آن ها بفرستید" + "\n" +
                    "🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟⚡️🌟" + "\n" +
                    "🛍 تله زنبیل، زنبیل تلگرامی خانواده 🛍" + "\n" +
                    "@TeleZanbilBot"
                    );
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

            string[][][] zanbilItemsTitle;
            if (currentTZSessionData.userRole == "Father")
                zanbilItemsTitle = new string[ziCount + 3][][];
            else //else if (currentTZSessionData.userRole == "Normal")
                zanbilItemsTitle = new string[ziCount + 2][][];

            // ساخت لیست رشته شامل معرفی آیتم های زنبیل
            int i;
            for (i = 0; i < ziCount; i++)
            {
                zanbilItemsTitle[i] = new string[1][];
                zanbilItemsTitle[i][0] = new string[2];
                ZanbilItem zi = zanbilItems.ToArray<ZanbilItem>()[i];
                zanbilItemsTitle[i][0][1] = (i + 1).ToString();
                zanbilItemsTitle[i][0][0] = zi.ItemTitle + " (" + zi.ItemAmount + " " + zi.ItemUnit.Title + ")";
            }

            i = ziCount;
            zanbilItemsTitle[i] = new string[2][];
            zanbilItemsTitle[i][0] = new string[2];
            zanbilItemsTitle[i][1] = new string[2];

            // دکمه افزودن کالای جدید
            zanbilItemsTitle[i][0][1] = "add";
            zanbilItemsTitle[i][0][0] = "✏️ افزودن مورد جدید";

            // دکمه رفرش
            zanbilItemsTitle[i][1][1] = "refresh";
            zanbilItemsTitle[i][1][0] = "💥 رفرش زنبیل";

            if (currentTZSessionData.userRole == "Father")
            {
                i++;
                zanbilItemsTitle[i] = new string[2][];
                zanbilItemsTitle[i][0] = new string[2];
                zanbilItemsTitle[i][1] = new string[2];

                // دکمه نمایش کد دعوت
                zanbilItemsTitle[i][0][1] = "inviteCode";
                zanbilItemsTitle[i][0][0] = "💥 نمایش کد دعوت";

                // دکمه بازسازی کد دعوت
                zanbilItemsTitle[i][1][1] = "regenerateInviteCode";
                zanbilItemsTitle[i][1][0] = "💥 بازسازی کد دعوت";
            }

            i++;
            zanbilItemsTitle[i] = new string[2][];
            zanbilItemsTitle[i][0] = new string[2];
            zanbilItemsTitle[i][1] = new string[2];

            // دکمه نمایش سابقه خرید
            zanbilItemsTitle[i][0][1] = "history";
            zanbilItemsTitle[i][0][0] = "💥 سابقه خرید";

            // دکمه خروج
            zanbilItemsTitle[i][1][1] = "logout";
            zanbilItemsTitle[i][1][0] = "💥 خروج";

            // ساخت کیبورد عمودی با استفاده از لیست آیتم های زنبیل
            //InlineKeyboardMarkup zanbilContentKeyboard = KeyboardGenerator.makeVerticalKeyboard(zanbilItemsTitle);
            InlineKeyboardMarkup zanbilContentKeyboard = KeyboardGenerator.makeKeyboard(zanbilItemsTitle);

            return zanbilContentKeyboard;
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
            Message keyboardMsg = await bot.SendTextMessageAsync(pfd.target, "🛍 زنبیل خانواده «" + currentTZSessionData.family.FamilyName + "»", replyMarkup: zanbilContentKeyboard);
            currentTZSessionData.lastMsgId = keyboardMsg.MessageId;
        }
        #endregion
    }
}
