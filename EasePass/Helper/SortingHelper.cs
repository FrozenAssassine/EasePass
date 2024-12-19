/*
MIT License

Copyright (c) 2023 Julius Kirsch

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
*/

using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace EasePass.Helper
{
    internal class SortingHelper
    {
        private static int SortBy(string item1, string item2)
        {
            return (item1 ?? "").CompareTo(item2 ?? "");
        }
        private static int CountPopularityLast30Days(PasswordManagerItem pmi, DateTime now, int daysDeadline = 30)
        {
            int pmi1Count = 0;
            for (int i = 0; i < pmi.Clicks.Count; i++)
            {
                string[] splitted = pmi.Clicks[i].Split('.');
                DateTime date = new DateTime(
                    Convert.ToInt32(splitted[2]),
                    Convert.ToInt32(splitted[1]),
                    Convert.ToInt32(splitted[0])
                    );

                if (now - date < TimeSpan.FromDays(daysDeadline))
                    pmi1Count++;
            }

            return pmi1Count;
        }

        public static int ByDisplayName(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            return SortBy(pmi1.DisplayName, pmi2.DisplayName);
        }
        public static int ByUsername(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            return SortBy(pmi1.Username, pmi2.Username);

        }
        public static int ByNotes(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            return SortBy(pmi1.Notes, pmi2.Notes);
        }
        public static int ByWebsite(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            return SortBy(pmi1.Website, pmi2.Website);
        }       
        public static int ByPopularAllTime(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            int comp = -pmi1.Clicks.Count.CompareTo(pmi2.Clicks.Count);
            if (comp == 0)
                return (pmi1.DisplayName ?? "").CompareTo(pmi2.DisplayName ?? "");
            return comp;
        }
        public static int ByPopularLast30Days(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            DateTime now = DateTime.Now;

            int pmi1Count = CountPopularityLast30Days(pmi1, now);
            int pmi2Count = CountPopularityLast30Days(pmi2, now);
            Debug.WriteLine(pmi1.DisplayName + $"({pmi1Count}) -> " + pmi2.DisplayName + $"({pmi2Count})");

            int comp = -pmi1Count.CompareTo(pmi2Count);
            if (comp == 0)
                return (pmi1.DisplayName ?? "").CompareTo(pmi2.DisplayName ?? "");
            return comp;
        }

        public static int ByPasswordStrength(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            bool[] strength1 = PasswordHelper.EvaluatePassword(pmi1.Password);
            bool[] strength2 = PasswordHelper.EvaluatePassword(pmi2.Password);
            if (strength1.Length != strength2.Length)
            {
                return 0;
            }

            int pmi1_res = 0, pmi2_res = 0;
            for (int i = 0; i < strength1.Length; i++)
            {
                pmi1_res += strength2[i] ? 1 : -1;
                pmi2_res += strength2[i] ? 1 : -1;
            }
            return -pmi1_res.CompareTo(pmi2_res);
        }
    }
}
