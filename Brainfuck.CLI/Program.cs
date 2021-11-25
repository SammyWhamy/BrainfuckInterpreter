using Brainfuck.Logic;
using Brainfuck.Logic.Exceptions;

if (args.Length < 2)
{
    Console.WriteLine("Usage: Brainfuck.exe");
    Console.WriteLine("Parameters:");
    Console.WriteLine("\t--file <program file>");
    Console.WriteLine("\t--memory <bytes of memory to use>");
    Console.WriteLine("\t--display <whether to display the memory>");
    Console.WriteLine("\t--delay <delay (ms) between each step>");
    Environment.Exit(1);
}

var argData = new ArgParser(args);

try
{
    if (!argData.IsFilled("file"))
        throw new ConfigurationException("[Config] No program was specified.");

    if(!argData.IsFilled("memory"))
        throw new ConfigurationException("[Config] Memory size must be an integer.");

    var input = FileService.ReadFile(argData.Get("file"));
    if(input is null)
        throw new ConfigurationException("[Config] Specified file could not be read.");
    
    Console.Clear();
    Interpreter interpreter = new(argData.GetAsInt("memory"), argData.Has("display"), argData.GetAsInt("delay") ?? 0);
    interpreter.Parse(input);
    try
    {
        interpreter.Run();
        Console.Out.Write("\n[Runtime] Finished execution.");
    }
    catch (BrainfuckException ex)
    {
        Console.SetCursorPosition(0, argData.Has("display") ? 6 : 1);
        Console.Error.Write(ex.Message);
    }
}
catch (BrainfuckException ex)
{
    Console.Error.WriteLine(ex.Message);
}


