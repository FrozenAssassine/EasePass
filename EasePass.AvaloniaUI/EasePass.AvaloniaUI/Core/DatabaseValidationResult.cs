
namespace EasePass.AvaloniaUI.Core;

public struct DatabaseValidationResult(PasswordValidationResult result, DatabaseFile dbFile)
{
    public PasswordValidationResult result = result;
    public DatabaseFile database = dbFile;
}
