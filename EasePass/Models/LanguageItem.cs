namespace EasePass.Models;

public class LanguageItem
{
    public string Tag { get; set; }
    public string DisplayName { get; set; }

    public LanguageItem(string Tag, string DisplayName)
    {
        this.Tag = Tag;
        this.DisplayName = DisplayName;
    }
}
