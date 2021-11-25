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

if (!argData.IsFilled("file"))
{
    Console.Error.WriteLine("[Config] No program was specified.");
    Environment.Exit(1);
}

if(!argData.IsFilled("memory"))
{
    Console.Error.WriteLine("[Config] Memory size must be an integer.");
    Environment.Exit(1);
}

var input = FileService.ReadFile(argData.Get("file"));
if(input is null)
{
    Console.Error.WriteLine("[Config] Specified file could not be read.");
    Environment.Exit(1);
}
    
Console.Clear();
Interpreter interpreter = new(argData.GetAsInt("memory"), argData.Has("display"), argData.GetAsInt("delay") ?? 0);

try
{
    interpreter.Parse(input);
    interpreter.Run();
    Console.Out.Write("\n[Runtime] Finished execution.");
}
catch (BrainfuckException ex)
{
    Console.SetCursorPosition(0, argData.Has("display") ? 6 : 1);
    Console.Error.Write(ex.Message);
}


