using System.Text.Json;

namespace WebWindowNetCore;

public record Settings(int x, int y, int width, int height, bool isMaximized);

public abstract class WebWindowBase
{
    protected record Settings(int x, int y, int width, int height, bool isMaximized);

    public WebWindowBase(Configuration configuration) 
        => this.configuration = configuration;

    public void Execute()
    {

        var settings = new Settings(configuration.InitialPosition?.X ?? -1, configuration.InitialPosition?.Y ?? -1, 
            configuration.InitialSize?.Width ?? 800, configuration.InitialSize?.Height ?? 600, configuration.IsMaximized == true);

        if (configuration.SaveWindowSettings == true)
        {
            var appData = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                configuration.Organization!, configuration.Application!);
            if (!Directory.Exists(appData))
                Directory.CreateDirectory(appData);

            settingsFile = Path.Combine(appData, "settings.json");
            if (File.Exists(settingsFile))
            {
                using var reader = new StreamReader(File.OpenRead(settingsFile));
                var settingsString = reader.ReadToEnd();
                if (settingsString?.Length > 0)
                {
                    var s = JsonSerializer.Deserialize<Settings>(settingsString);
                    if (s != null)
                        settings = s;
                }
            }
        }  
        Run(settings);
    }

    protected abstract void Run(Settings settings);

    protected void SaveSettings(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        using var writer = new StreamWriter(File.Create(settingsFile));
        writer.Write(json);
    }

    protected Configuration configuration;
    string settingsFile = "";      
}