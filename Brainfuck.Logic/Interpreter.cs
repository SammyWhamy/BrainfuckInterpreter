using Brainfuck.Logic.Exceptions;
using System.Text.RegularExpressions;

namespace Brainfuck.Logic;

public class Interpreter
{
    private static readonly Regex BrainfuckSyntax = new(@"[^><+.,\-\[\]]+");
    private readonly Dictionary<char, int> _tokens = new();
    private int _memSize;
    private string? _input;
    private char[]? _program;

    public void Parse(string program, int memSize)
    {
        if(memSize < 1)
            throw new ConfigurationException("[Config] Memory size must be greater than 0");
        _memSize = memSize;
        _input = BrainfuckSyntax.Replace(program, "");
        var len = _input.Length;
        _program = _input.ToCharArray();
        for (var i = 0; i < len; i++)
            if (_tokens.ContainsKey(_program[i]))
                _tokens[_program[i]]++;
            else _tokens[_program[i]] = 1;
        if(GetTokenCount('[') != GetTokenCount(']'))
            throw new SyntaxErrorException("[Analysis] Brackets are not balanced.");
    }
    
    private int GetTokenCount(char token)
        => _tokens.ContainsKey(token) ? _tokens[token] : 0;

    public void Run()
    {
        if (_input is null || _program is null)
            throw new NullReferenceException("Program has not been loaded");
        var memory = new byte[_memSize];
        var pointer = 0;
        var written = 0;
        var length = _program.Length;

        for (var i = 0; i < length; i++)
        {
            // DisplayTape(memory, pointer);
            switch (_program[i])
            {
                case '>':
                    pointer++;
                    if(pointer >= _memSize)
                        throw new RuntimeException("[Runtime] Memory pointer is out of bounds");
                    break;
                case '<':
                    pointer--;
                    if(pointer < 0)
                        throw new RuntimeException("[Runtime] Memory pointer is out of bounds");
                    break;
                case '+':
                    memory[pointer]++;
                    break;
                case '-':
                    memory[pointer]--;
                    break;
                case '.':
                    Console.SetCursorPosition(written, 5);
                    Console.Write((char)memory[pointer]);
                    written++;
                    break;
                case ',':
                    Console.SetCursorPosition(0, 5);
                    memory[pointer] = (byte)Console.ReadKey().KeyChar;
                    Console.SetCursorPosition(0, 5);
                    Console.Out.Write(' ');
                    break;
                case '[':
                    if (memory[pointer] == 0)
                    {
                        var j = i;
                        while (_program[j] != ']') j++;
                        i = j;
                    }
                    break;
                case ']':
                    if (memory[pointer] != 0)
                    {
                        var j = i;
                        while (_program[j] != '[') j--;
                        i = j;
                    }
                    break;
            }}
    }

    public void DisplayTape(byte[] memory, int pointer)
    {
        Console.SetCursorPosition(0, 0);
        var width = Console.WindowWidth;
        Console.Out.Write(new string('-', width)); // Draw top border
        
        var left = pointer - width / 2 / 6;
        
        Console.SetCursorPosition(0, 1);
        for (var i = left > 0 ? left : 0; i < width/4; i++)
        {
            Console.Out.Write("| ");
            if (i == pointer)
                Console.ForegroundColor = ConsoleColor.Red;
            Console.Out.Write($"{memory[i].ToString().PadLeft(3, '0')} ");
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.Out.Write("|");
        
        Console.SetCursorPosition(0, 2);
        Console.Out.WriteLine(new string('-', width)); // Draw bottom border
        
        Thread.Sleep(10);
    }
}