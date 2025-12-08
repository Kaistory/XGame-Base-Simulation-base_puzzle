using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _JigblockPuzzle;
using mygame.sdk;
using UnityEngine;
using UnityEngine.Serialization;

public enum BuyType
{
    None,
    Trade,
    Iap,
    Ads
}

[CreateAssetMenu(fileName = "PackageData", menuName = "Data/PackageData")]
public class PackageData : ScriptableObject
{
    public PackageInfo[] ticketPackInfos;
    public PackageInfo[] goldPackInfos;
    public PackageInfo[] packageInfos;
    public PackageFlashSaleInfo[] packageFlashSaleInfos;

    public enum ResetType
    {
        None,
        Daily,
        Weekly,
        Monthly,
    }

    [Serializable]
    public class PackageInfo
    {
        public string tile;
        public string desc;
        public int id;
        public int count;
        public int levelUnlock;

        public bool isX2IfFirst;
        public bool onlyOnce;
        public string skuId;

        public RES_type priceType;
        public Sprite priceIcon;
        public int price;

        public ResetType resetType;

        public BuyType buyType;

        public ItemBuyInfo[] allItems;

        private int buyCountInStep
        {
            get => PlayerPrefsBase.Instance().getInt("buy_count_in_step_" + skuId, 0);
            set => PlayerPrefsBase.Instance().setInt("buy_count_in_step_" + skuId, value);
        }

        public int buyAmount => count - buyCountInStep;

        public bool isBuy => levelUnlock < GameRes.GetLevel() && (count == 0 || buyCountInStep < count);

        public bool isActive => !onlyOnce || buyCountInStep == 0;

        private long lastBuyTime
        {
            get => long.Parse(PlayerPrefsBase.Instance().getString("last_buy_time" + skuId, "0"));
            set => PlayerPrefsBase.Instance().setString("last_buy_time" + skuId, value.ToString());
        }

        public void CheckReset()
        {
            if (lastBuyTime == 0 || count == 0) return;
            var dateTime = new DateTime(lastBuyTime, DateTimeKind.Local);
            switch (resetType)
            {
                case ResetType.Daily:
                    if (DateTime.Now.Day != dateTime.Day)
                    {
                        buyCountInStep = 0;
                        lastBuyTime = 0;
                    }
                    break;
                case ResetType.Weekly:
                    if (DateTime.Now.DayOfYear - dateTime.DayOfYear > 7 || DateTime.Now.DayOfWeek < dateTime.DayOfWeek)
                    {
                        buyCountInStep = 0;
                        lastBuyTime = 0;
                    }
                    break;
                case ResetType.Monthly:
                    if (DateTime.Now.Month != dateTime.Month)
                    {
                        buyCountInStep = 0;
                        lastBuyTime = 0;
                    }
                    break;
            }

        }

        public void BuyPackage()
        {
            if (lastBuyTime == 0) lastBuyTime = DateTime.Now.Ticks;
            buyCountInStep++;
        }

        public TimeSpan ResetDuration()
        {
            var now = DateTime.Now;
            switch (resetType)
            {
                case ResetType.Daily:
                    var endOfDay = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                    return endOfDay - now;
                case ResetType.Weekly:
                    var daysUntilEndOfWeek = DayOfWeek.Saturday - now.DayOfWeek + 1;
                    var endOfWeek = now.AddDays(daysUntilEndOfWeek).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    return endOfWeek - now;
                case ResetType.Monthly:
                    var daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);
                    var endOfMonth = new DateTime(now.Year, now.Month, daysInMonth, 23, 59, 59);
                    return endOfMonth - now;
            }

            return TimeSpan.Zero;
        }
    }

    [Serializable]
    public class ItemBuyInfo : ItemInfo
    {
        public Sprite previewIcon;

        public ItemBuyInfo(Sprite ic, RES_type rwType, int am) : base(ic, rwType, am)
        {
        }
    }

    public PackageInfo FindPackage(int id)
    {
        return packageInfos.SingleOrDefault(x => x.id == id);
    }
    [Serializable]
    public class PackageFlashSaleInfo
    {
        public string skuId;
        public RES_type boosterType;
        public Sprite spriteBooster;
        public ItemInfo[] itemInfos;
    }
}
