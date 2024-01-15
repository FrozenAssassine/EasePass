namespace EasePass.Models;

public class FetchedExtension
{
    public string URL { get; set; }
    public string Name { get; set; }

    public FetchedExtension(string url, string name)
    {
        this.URL = url;
        this.Name = name;
    }
}
