using System.IO;

namespace EasePass.Models;

public class DatabaseBackupFile
{
    public string FileName { get; set; }
    public string DisplayText => FileName + " [" + GetBackupDate() + "]";
    public string FilePath { get; }
    public DatabaseBackupFile(string filePath)
    {
        this.FilePath = filePath;
        this.FileName = Path.GetFileName(filePath);
    }

    private string GetBackupDate()
    {
        var splitted = FileName.Split("_");
        if(splitted.Length >= 3)
            return splitted[1] + "/" + splitted[0] + "/" + splitted[2];

        return "";
    }
}
