using EasePass.Core.Database.Format.Serialization;

namespace EasePass.Models;

public struct DatabaseValidationResult(PasswordValidationResult result, DatabaseFile dbFile)
{
    public PasswordValidationResult result = result;
    public DatabaseFile database = dbFile;
}
