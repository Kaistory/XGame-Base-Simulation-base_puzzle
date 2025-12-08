using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;
using System.Linq;

namespace mygame.sdk
{
    public class PlayerPrefsUtil
    {
        #region Audio

        public static bool IsSoundEnable
        {
            get => PlayerPrefs.GetInt("is_sound_enable", 1) == 1;
            set => PlayerPrefs.SetInt("is_sound_enable", value ? 1 : 0);
        }

        public static bool IsMusicEnable
        {
            get => PlayerPrefs.GetInt("is_music_enable", 1) == 1;
            set => PlayerPrefs.SetInt("is_music_enable", value ? 1 : 0);
        }

        #endregion

        #region PlayerPrefs

        public static string CompletedDays
        {
            get => PlayerPrefs.GetString("completed_days", "");
            set
            {
                PlayerPrefs.SetString("completed_days", value);
                PlayerPrefs.Save();
            }
        }

        public static void AddCompletedDay(int day)
        {
            var current = CompletedDays;
            var list = new List<int>();

            if (!string.IsNullOrEmpty(current))
            {
                list = current.Split(',')
                              .Select(s => int.TryParse(s, out int val) ? val : -1)
                              .Where(v => v > 0)
                              .ToList();
            }

            if (!list.Contains(day))
                list.Add(day);

            CompletedDays = string.Join(",", list);
        }

        public static int currentSelectedIndex // level dailly hiện tại đang chơi
        {
            get => PlayerPrefs.GetInt("current_Selected_Index", 1);
            set =>PlayerPrefs.SetInt("current_Selected_Index", value);
        }

        #endregion
    }
}