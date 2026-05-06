using EasePass.Core.Database;
using System.Collections.Generic;
using System.Linq;

namespace EasePass.Helper;

public class TagSearchManager
{
    public List<string> UniqueTagList { get; private set; }

    private IEnumerable<string> IndexItems(DatabaseItem database)
    {
        var seen = new HashSet<string>();
        foreach (var item in database.Items)
        {
            if (item.Tags != null)
            {
                foreach (var tag in item.Tags)
                {
                    if (seen.Add(tag))
                        yield return tag;
                }
            }
        }
    }

    public void UpdateTagList(DatabaseItem database)
    {
        var tags = IndexItems(database).ToList();
        tags.Sort();
        UniqueTagList = tags;
    }

    public List<string> Filter(string searchKW, string prefix)
    {
        if (string.IsNullOrEmpty(searchKW))
            return UniqueTagList.Select(tag => prefix + tag).ToList();

        return UniqueTagList
            .Where(tag => tag != null && tag.Contains(searchKW, System.StringComparison.OrdinalIgnoreCase)).Select(tag => prefix + tag)
            .ToList();
    }
}
