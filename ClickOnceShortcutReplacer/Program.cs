using Shell32;
using System.Reflection;

namespace ClickOnceShortcutReplacer;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        CheckShortcut();
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }

    static void CheckShortcut()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var taskBarPath = Path.Combine(appData, "Microsoft", "Internet Explorer", "Quick Launch", "User Pinned", "TaskBar");
        if (!Directory.Exists(taskBarPath))
            return;
        var myPath = Assembly.GetExecutingAssembly().Location;
        if (Path.GetExtension(myPath) == ".dll")
            myPath = Path.ChangeExtension(myPath, ".exe");
        var files = Directory.GetFiles(taskBarPath, "*.lnk");
        foreach (var file in files)
        {
            var path = GetShortcutTargetFile(file);
            if (Path.GetFileNameWithoutExtension(path)?.ToUpper() == Path.GetFileNameWithoutExtension(myPath).ToUpper())
            {
                SetLinkTarget(file, myPath);
                break;
            }
        }
    }

    public static string? GetShortcutTargetFile(string shortcutFilename)
    {
        var pathOnly = Path.GetDirectoryName(shortcutFilename);
        var filenameOnly = Path.GetFileName(shortcutFilename);

        var shell = new Shell();
        var folder = shell.NameSpace(pathOnly);
        var folderItem = folder.ParseName(filenameOnly);
        if (folderItem != null)
        {
            var link = (ShellLinkObject)folderItem.GetLink;
            return link.Path;
        }

        return null;
    }

    private static void SetLinkTarget(string shortcutPath, string newTarget)
    {
        var shell = new Shell();
        var folder = shell.NameSpace(Path.GetDirectoryName(shortcutPath));
        var folderItem = folder.Items().Item(Path.GetFileName(shortcutPath));
        var currentLink = (ShellLinkObject)folderItem.GetLink;
        currentLink.Path = newTarget;
        currentLink.Save();
    }

}