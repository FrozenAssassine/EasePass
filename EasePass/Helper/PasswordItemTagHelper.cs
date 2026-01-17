using EasePass.Models;
using Microsoft.UI.Xaml.Controls;
using System;

namespace EasePass.Helper;

internal class PasswordItemTagHelper
{
    public static void ParseToTags(string raw, PasswordManagerItem item)
    {
        if (raw == null || raw.Length == 0)
        {
            item.Tags = null;
            return;
        }

        item.Tags = raw.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    }

    public static void FillTags(TextBox tb, PasswordManagerItem item)
    {
        if (tb == null || item.Tags == null || item.Tags.Length == 0)
        {
            tb.Text = "";
            return;
        }

        tb.Text = string.Join(" ", item.Tags);
    }
}
