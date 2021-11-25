using System.Text.RegularExpressions;
using Brainfuck.Logic;
using Brainfuck.Logic.Exceptions;

var bfSyntaxRegex = new Regex(@"^[<>.,+\-\[\]]*$");
string? input = null;
string? memInput;

if (args.Length > 0)
{
    if(args.Length < 2)
    {
        Console.Error.WriteLine("Usage: brainfuck.exe [memory] [program]");
        return;
    }
    memInput = args[0];
    input = bfSyntaxRegex.IsMatch(args[1]) ? args[1] : ReadFile(args[1]);
}
else
{
    Console.Out.WriteLine("Do you want to run a [f]ile, or input some [t]ext?");
    char? choice = Console.ReadKey().KeyChar;
    Console.SetCursorPosition(0, Console.CursorTop);
    switch (choice)
    {
        case 'f':
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Out.WriteLine("Please enter the file name:");
            input = ReadFile(Console.ReadLine());
            break;
        case 't':
            Console.Out.WriteLine("Please enter your code:");
            input = Console.ReadLine();
            break;
        default:
            Console.Error.WriteLine("Invalid input.");
            break;
    }
    Console.Out.WriteLine("Please enter the amount of memory (Cells) to allocate:");
    memInput = Console.ReadLine();
}

if (string.IsNullOrWhiteSpace(input))
    throw new ConfigurationException("[Config] No program was specified.");
if(!int.TryParse(memInput, out var memSize))
    throw new ConfigurationException("[Config] Memory size must be an integer.");

try
{
    Console.Clear();
    Interpreter interpreter = new();
    interpreter.Parse(input, memSize);
    interpreter.Run();
    Console.Out.Write("\n\n[Runtime] Finished execution.");
}
catch (BrainfuckException ex)
{
    Console.Error.WriteLine(ex.Message);
}

string? ReadFile(string? path)
{
    if (File.Exists(path))
        return File.ReadAllText(path);
    if (File.Exists(Directory.GetCurrentDirectory() + "\\" + input))
        return File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + input);
    return null;
}
