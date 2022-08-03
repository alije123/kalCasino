using DSharpPlus.Entities;

namespace kalCasino.Commands;

public class KostylFile
{
    private string _path;

    public KostylFile(string path)
    {
        _path = path;
    }

    public async Task UploadToDiscord()
    {
        await using var fs = new FileStream(_path, FileMode.Open, FileAccess.Read);
        var msg = new DiscordMessageBuilder().WithFile(fs);
    }
}