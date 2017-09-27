﻿using System;
using System.Collections.Generic;
using ir.EmIT.EmITBotNet;
using Telegram.Bot.Types;
using ir.EmIT.EmITBotNet.NFAUtility;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using ir.EmIT.TeleZanbil.Models;
using System.IO;

namespace ir.EmIT.TeleZanbil
{
    class TeleZanbil : EmITBotNetBase
    {
        #region کلاس های مورداستفاده
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
            public static BotState AddNewZanbilItemForFather = new BotState(10, "اضافه کردن آیتم جدید به زنبیل برای پدر");

            public static BotState ShowInvalidCommand = new BotState(11, "نمایش ورودی نامعتبر");
            public static BotState Login = new BotState(12, "ورود اعضای خانواده");

            public static BotState GetInputCode = new BotState(13, "دریافت کد ورود");
            public static BotState CheckInputCode = new BotState(14, "بررسی کد ورود");
            public static BotState FalseInputCode = new BotState(15, "غلط بودن کد ورود");
            public static BotState ShowFalseInputCode = new BotState(16, "نمایش غلط بودن کد ورود");
            public static BotState TrueInputCode = new BotState(17, "درست بودن کد ورود");

            public static BotState ShowZanbilContentForNormalUser = new BotState(18, "نمایش محتوی زنبیل برای کاربر عادی");

            public static BotState AddNewZanbilItemForChild = new BotState(19, "اضافه کردن آیتم جدید به زنبیل برای فرزندان");

            public static BotState ShowSuggestion = new BotState(20, "نمایش پیشنهادات کالا");
            public static BotState GetZanbilItemName = new BotState(21, "پرسیدن اسم کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemAmount = new BotState(22, "پرسیدن مقدار کالا برای افزودن به زنبیل");
            public static BotState GetZanbilItemUnit = new BotState(23, "پرسیدن واحد کالا برای افزودن به زنبیل");
            public static BotState SaveZanbilItem = new BotState(24, "افزودن کالا به زنبیل");

            public static BotState ShowAdminMenu = new BotState(30, "نمایش منوی مدیر سیستم");
        }

        //todo: تبدیل این ساختار سشن فعلی به متغیرهای موجود در محیط
        internal class TeleZanbilSessionData : SessionData
        {
            public TeleZanbilSessionData(long userID) : base(userID)
            {
            }

            public Family family;
            public int lastMsgId;

            public string zanbilItemName;
            public int zanbilItemAmount;
            public string zanbilItemUnit;
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

        public override void defineNFARules()
        {
            nfa.addRule(TeleZanbilStates.Start, "/start", TeleZanbilStates.CheckUserType);
            nfa.addElseRule(TeleZanbilStates.Start, TeleZanbilStates.Start);

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

            nfa.addRule(TeleZanbilStates.GetMainCommand, 3, TeleZanbilStates.Login);

            //int itemCount = tzdb.ZanbilItems
            nfa.addRule(TeleZanbilStates.ShowZanbilContentForFather, 0, TeleZanbilStates.AddNewZanbilItemForFather);
            nfa.addRegexRule(TeleZanbilStates.ShowZanbilContentForFather, "[0-9]+", TeleZanbilStates.AcceptZanbilItem);

            nfa.addRule(TeleZanbilStates.AcceptZanbilItem, TeleZanbilStates.ShowZanbilContentForFather);

            nfa.addRule(TeleZanbilStates.AddNewZanbilItemForFather, TeleZanbilStates.GetZanbilItemName);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemName, ".*", TeleZanbilStates.GetZanbilItemAmount);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemAmount, "[0-9]+", TeleZanbilStates.GetZanbilItemUnit);
            nfa.addRegexRule(TeleZanbilStates.GetZanbilItemUnit, ".+", TeleZanbilStates.SaveZanbilItem);

            nfa.addRule(TeleZanbilStates.SaveZanbilItem, TeleZanbilStates.ShowZanbilContentForFather);

            /*            
            ShowInvalidCommand
            Login
            ShowZanbilContentForNormalUser
            ShowAdminMenu
            */
        }

        public override void defineNFARulePostFunctions()
        {
            nfa.addRulePostFunction(TeleZanbilStates.Start, TeleZanbilStates.Start, async (PostFunctionData pfd) =>
            {
                await bot.SendTextMessageAsync(pfd.target, "لطفاً برای شروع 🏃 از دستور زیر استفاده کنید :\n/start");
            });

            nfa.addRulePostFunction(TeleZanbilStates.CheckUserType, TeleZanbilStates.Start, (PostFunctionData pfd) =>
            {
                string roleName = "";

                // بررسی اینکه آیا کاربری متناظر کاربر جاری بات در دیتابیس وجود دارد یا نه؟
                var userList = tzdb.Users.Where(u => u.TelegramUserID == pfd.m.Chat.Id);
                if (userList.Count() > 0)
                {
                    var user = userList.First();

                    // گرفتن نقش کاربر جاری (ذخیره شده در دیتابیس)
                    roleName = user.UserRole.RoleName;

                    // ذخیره کردن اطلاعات خانواده کاربر جاری در داده های جلسه، در صورتی که شخص ورودی پدر باشد
                    if (roleName == "Father")
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
                    "درباره 💡 تله زنبیل"
                }, 2, false);
                Message m2 = await bot.SendTextMessageAsync(pfd.target, "لطفاً انتخاب کنید", replyMarkup: mainKeyboard);
                currentTZSessionData.lastMsgId = m2.MessageId;
            });

            // نمایش درباره ما
            nfa.addRulePostFunction(TeleZanbilStates.ShowAboutUs, async (PostFunctionData pfd) =>
            {
                await bot.DeleteMessageAsync(pfd.target, currentTZSessionData.lastMsgId);

                //todo: تکمیل متن و عکس درباره ما
                await bot.SendTextMessageAsync(pfd.target, "تله زنبیل\nمدیریت زنبیل خانواده");
                await bot.SendPhotoAsync(pfd.target, new FileToSend("AboutPoster", new FileStream("Images\\AboutZanbil.png", FileMode.Open)));
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

            nfa.addRulePostFunction(TeleZanbilStates.ShowZanbilContentForFather, async (PostFunctionData pfd) =>
            {
                if (currentTZSessionData.lastMsgId != 0)
                {
                    // حذف کیبورد قبلی
                    await bot.DeleteMessageAsync(pfd.target, currentTZSessionData.lastMsgId);
                }

                // بدست آوردن محتوی زنبیل در قالب یک کیبورد
                InlineKeyboardMarkup zanbilContentKeyboard = makeZanbilContentKeyboard();

                // نمایش پیام و کیبورد لیست آیتم های زنبیل
                Message keyboardMsg = await bot.SendTextMessageAsync(pfd.target, "زنبیل 🛍 خانواده " + currentTZSessionData.family.FamilyName, replyMarkup: zanbilContentKeyboard);
                currentTZSessionData.lastMsgId = keyboardMsg.MessageId;
            });

            nfa.addRulePostFunction(TeleZanbilStates.AcceptZanbilItem, TeleZanbilStates.ShowZanbilContentForFather, async (PostFunctionData pfd) =>
            {
                var mainZanbil = getMainZanbil();

                int zanbilItemNo = Convert.ToInt32(pfd.m.Text);
                var zanbilItems = tzdb.ZanbilItems.Where(zi => zi.Zanbil.ZanbilId == mainZanbil.ZanbilId && zi.IsBought == false);
                if (zanbilItemNo > 0 && zanbilItemNo <= zanbilItems.Count())
                {
                    var zanbilItem = zanbilItems.ToArray()[zanbilItemNo - 1];
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
                InlineKeyboardMarkup numberKeyboard = KeyboardGenerator.makeNumberMatrixKeyboard(1, 9, 3);
                await bot.SendTextMessageAsync(pfd.target, "لطفاً مقدار کالای درخواستی 🛒 را انتخاب کنید یا در صورت نیاز مقدار دقیق آن را وارد نمائید", replyMarkup: numberKeyboard);
            });

            nfa.addRulePostFunction(TeleZanbilStates.GetZanbilItemUnit, async (PostFunctionData pfd) =>
            {
                // گرفتن مقدار کالای درخواستی از مرحله قبل
                currentTZSessionData.zanbilItemAmount = Convert.ToInt32(pfd.action);

                var unitNames = tzdb.Units.Select(u => u.Title);
                string[] unitNamesStr = new string[unitNames.Count()];
                for (int i = 0; i < unitNames.Count(); i++)
                {
                    unitNamesStr[i] = unitNames.ToArray()[i];
                }
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

            // ساخت لیست رشته شامل معرفی آیتم های زنبیل
            string[][] zanbilItemsTitle = new string[zanbilItems.Count() + 1][];
            for (int i = 0; i < zanbilItems.Count(); i++)
            {
                zanbilItemsTitle[i] = new string[2];
                ZanbilItem zi = zanbilItems.ToArray<ZanbilItem>()[i];
                zanbilItemsTitle[i][0] = (i + 1).ToString();
                zanbilItemsTitle[i][1] = zi.ItemTitle + " (" + zi.ItemAmount + " " + zi.ItemUnit.Title + ")";
            }
            zanbilItemsTitle[zanbilItems.Count()] = new string[2];
            zanbilItemsTitle[zanbilItems.Count()][0] = "0";
            zanbilItemsTitle[zanbilItems.Count()][1] = "🛒 افزودن مورد جدید";

            // ساخت کیبورد عمودی با استفاده از لیست آیتم های زنبیل
            InlineKeyboardMarkup zanbilContentKeyboard = KeyboardGenerator.makeVerticalKeyboard(zanbilItemsTitle);

            return zanbilContentKeyboard;
        }

        private Zanbil getMainZanbil()
        {
            // گرفتن زنبیل اصلی خانواده
            var mainZanbil = tzdb.Zanbils.Where(z => z.Family.FamilyId == currentTZSessionData.family.FamilyId).First();
            return mainZanbil;
        }
        #endregion
    }
}
