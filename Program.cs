using System;
using System.IO;
using Avalonia;

namespace GsproMissionControl;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            try
            {
                var exeDir = AppContext.BaseDirectory;
                File.WriteAllText(Path.Combine(exeDir, "crash.txt"), ex.ToString());
            }
            catch
            {
                // ignore
            }

            // Om den råkar ha konsol: skriv ändå
            Console.Error.WriteLine(ex);
            Environment.Exit(1);
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}