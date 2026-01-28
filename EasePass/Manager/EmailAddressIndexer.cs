
using EasePass.Core.Database;
using System;
using System.Collections.Generic;

namespace EasePass.Manager;

//index email addrresses for the edit/add item page, to provide suggestions when typing email adresses
public class EmailAddressIndexer
{
    public HashSet<string> UniqueEmailAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);


    public void IndexUniqueEmail(DatabaseItem loadedDb)
    {
        foreach (var item in loadedDb.Items)
        {
            if (!string.IsNullOrEmpty(item.Email))
            {
                var email = item.Email.Trim().ToLower();
                UniqueEmailAddresses.Add(item.Email.Trim());
            }
        }
    }

}
