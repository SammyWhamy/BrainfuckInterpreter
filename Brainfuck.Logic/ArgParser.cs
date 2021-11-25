namespace Brainfuck.Logic;

public class ArgParser
{
    private readonly Dictionary<string, string> _dict = new();

    public ArgParser(IReadOnlyList<string> args)
    {
        for(var i = 0; i < args.Count; i++)
            if (args[i].StartsWith("--"))
                _dict[args[i][2..]] = i + 1 < args.Count && !args[i+1].StartsWith("--") ? args[i + 1] : "";
    }

    public string Get(string key)
        => _dict.ContainsKey(key) ? _dict[key] : "";

    public bool Has(string key)
        => _dict.ContainsKey(key);
    
    public int? GetAsInt(string key) 
    {
        if (int.TryParse(Get(key), out var result))
            return result;
        return null;
    }

    public bool IsFilled(string key)
        => !string.IsNullOrEmpty(Get(key));
}