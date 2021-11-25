namespace Brainfuck.Logic;

public static class FileService
{
    public static string? ReadFile(string? path)
    {
        if(path is null)
            throw new ArgumentNullException(nameof(path), "Path may not be null");
        
        if (File.Exists(path))
            return File.ReadAllText(path);
        
        var combinedPath = Path.Combine(Environment.CurrentDirectory, path);
        return File.Exists(combinedPath) ? File.ReadAllText(combinedPath) : null;
    }
}