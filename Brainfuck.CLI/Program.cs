using Brainfuck.Logic;
using Brainfuck.Logic.Exceptions;

namespace Brainfuck.CLI;

public static class Program
{
    private const string Help = "Usage: Brainfuck.exe\nParameters:\n\t--file <program file>\n\t--memory <bytes of memory to use>\n\t--display <whether to display the memory>\n\t--delay <delay (ms) between each step>";
    private static void Main(string[] args)
    {
        if (args.Length < 2)
            Exit(false, Help);

        var argData = new ArgParser(args);
        
        
        if (!argData.IsFilled("file"))
            Exit(false, "[Config] No program was specified.");
        
        if(!argData.IsFilled("memory"))
            Exit(false, "[Config] Memory size must be an integer.");
        
        var input = FileService.ReadFile(argData.Get("file"));
        if(input is null)
            Exit(false, "[Config] Specified file could not be read.");
        
        Console.Clear();
        Interpreter interpreter = new(argData.GetAsInt("memory"), argData.Has("display"), argData.GetAsInt("delay") ?? 0);
        Console.CancelKeyPress += delegate
        {
            interpreter.Stop();
            Console.SetCursorPosition(0, argData.Has("display") ? 6 : 1);
            Console.WriteLine("\n[Interpreter] Stopped.");
        };
        
        try
        {
            interpreter.Parse(input!);
            interpreter.Run();
            Console.SetCursorPosition(0, argData.Has("display") ? 6 : 1);
            Exit(true, "\n[Success] Finished execution.");
        }
        catch (BrainfuckException ex)
        {
            Console.SetCursorPosition(0, argData.Has("display") ? 6 : 1);
            Exit(false, ex.Message);
        }
    }

    private static void Exit(bool success, string? message = null)
    {
        if (success)
        {
            Console.Out.WriteLine(message);
            Environment.Exit(0);
        }
        else
        {
            Console.Error.WriteLine(message);
            Environment.Exit(1);
        }
    }
}