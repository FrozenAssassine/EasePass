using System;

namespace EasePass.Models;

public class FetchedExtension
{
    public Uri URL { get; set; }
    public string Name { get; set; }
    public string Source { get; set; }

    public FetchedExtension(Uri url, string name, string source)
    {
        this.URL = url;
        this.Name = name;
        Source = source;
    }
}
