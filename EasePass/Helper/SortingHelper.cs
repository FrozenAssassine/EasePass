using EasePass.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasePass.Helper
{
    internal class SortingHelper
    {
        public static int ByDisplayName(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.DisplayName == null) pmi1.DisplayName = "";
            if (pmi2.DisplayName == null) pmi2.DisplayName = "";
            return pmi1.DisplayName.CompareTo(pmi2.DisplayName);
        }

        public static int ByUsername(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Username == null) pmi1.Username = "";
            if (pmi2.Username == null) pmi2.Username = "";
            return pmi1.Username.CompareTo(pmi2.Username);
        }

        public static int ByNotes(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Notes == null) pmi1.Notes = "";
            if (pmi2.Notes == null) pmi2.Notes = "";
            return pmi1.Notes.CompareTo(pmi2.Notes);
        }

        public static int ByWebsite(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            if (pmi1.Website == null) pmi1.Website = "";
            if (pmi2.Website == null) pmi2.Website = "";
            return pmi1.Website.CompareTo(pmi2.Website);
        }

        public static int ByPasswordStrength(PasswordManagerItem pmi1, PasswordManagerItem pmi2)
        {
            bool?[] strength1 = PasswordHelper.EvaluatePassword(pmi1.Password);
            bool?[] strength2 = PasswordHelper.EvaluatePassword(pmi2.Password);
            if (strength1.Length != strength2.Length) return 0;
            int pmi1_res = 0;
            int pmi2_res = 0;
            for(int i = 0; i < strength1.Length; i++)
            {
                if (strength1[i] == true) pmi1_res++;
                if (strength1[i] == false) pmi1_res--;
                if (strength2[i] == true) pmi2_res++;
                if (strength2[i] == false) pmi2_res--;
            }
            return -pmi1_res.CompareTo(pmi2_res);
        }
    }
}
