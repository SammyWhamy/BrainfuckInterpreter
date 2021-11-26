using Brainfuck.Logic.Exceptions;
using System.Text.RegularExpressions;

namespace Brainfuck.Logic;

public class Interpreter
{
    private static readonly Regex BrainfuckSyntax = new(@"[^><+.,\-\[\]]+");
    private readonly Dictionary<char, int> _tokens = new();
    private readonly int _memSize;
    private readonly int _delay;
    private string? _input;
    private char[]? _program;
    private byte[]? _memory;
    private int _pointer;
    private bool _display;
    public int TopOffset;

    public Interpreter(int? memSize, bool display, int delay = 0)
    {
        if(memSize is null or < 1)
            throw new ConfigurationException("[Config] Memory size must be greater than 0");
        _memSize = memSize.Value;
        _display = display;
        if(delay < 0)
            throw new ConfigurationException("[Config] Delay must be greater than or equal to 0");
        _delay = delay;
    }

    public void Parse(string program)
    {
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
        _memory = new byte[_memSize];
        _pointer = 0;
        var written = 0;
        TopOffset = _display ? 5 : 0;
        var length = _program.Length;

        if (_display)
            InitializeDisplay();
        
        Console.SetCursorPosition(0, TopOffset);
        Console.Write("Input: [  ] | Output: ");
        
        for (var i = 0; i < length; i++)
        {
            if(_display)
                DisplayTape();
            switch (_program[i])
            {
                case '>':
                    _pointer++;
                    if(_pointer >= _memSize)
                        throw new RuntimeException($"[Runtime] Memory pointer is out of bounds (Overflow / [{_pointer}])");
                    break;
                case '<':
                    _pointer--;
                    if(_pointer < 0)
                        throw new RuntimeException($"[Runtime] Memory pointer is out of bounds (Underflow / [{_pointer}])");
                    break;
                case '+':
                    _memory[_pointer]++;
                    break;
                case '-':
                    _memory[_pointer]--;
                    break;
                case '.':
                    Console.SetCursorPosition(22+written, TopOffset);
                    Console.Write((char)_memory[_pointer]);
                    written++;
                    if(written >= Console.WindowWidth-22)
                    {
                        written = 0;
                        TopOffset++;
                    }
                    break;
                case ',':
                    Console.SetCursorPosition(9, TopOffset);
                    _memory[_pointer] = (byte)Console.ReadKey().KeyChar;
                    Console.SetCursorPosition(9, TopOffset);
                    Console.Out.Write(' ');
                    break;
                case '[':
                    if (_memory[_pointer] == 0) {
                        var skip = 0;
                        var ptr = i + 1;
                        while (_program[ptr] != ']' || skip > 0) {
                            switch (_program[ptr])
                            {
                                case '[':
                                    skip += 1;
                                    break;
                                case ']':
                                    skip -= 1;
                                    break;
                            }
                            ptr += 1;
                            i = ptr;
                        }
                    }
                    break;
                case ']':
                    if (_memory[_pointer] != 0) {
                        var skip = 0;
                        var ptr = i - 1;
                        while (_program[ptr] != '[' || skip > 0) {
                            switch (_program[ptr])
                            {
                                case ']':
                                    skip += 1;
                                    break;
                                case '[':
                                    skip -= 1;
                                    break;
                            }
                            ptr -= 1;
                            i = ptr;
                        }
                    }
                    break;
            }}
    }

    private static void InitializeDisplay()
    {
        Console.SetCursorPosition(0, 0);
        Console.Out.Write(new string('-', Console.WindowWidth));
        Console.SetCursorPosition(0, 2);
        Console.Out.WriteLine(new string('-', Console.WindowWidth));
    }

    private void DisplayTape()
    {
        if(_memory is null)
            throw new RuntimeException("[Runtime] Memory is not initialized while displaying tape");
        
        var width = Console.WindowWidth;
        var left = _pointer - width / 2 / 6;
        
        Console.SetCursorPosition(0, 1);
        for (var i = left > 0 ? left : 0; i < _memSize && i < width/6; i++)
        {
            Console.Out.Write("| ");
            if (i == _pointer)
                Console.ForegroundColor = ConsoleColor.Red;
            Console.Out.Write($"{_memory[i].ToString().PadLeft(3, '0')} ");
            Console.ForegroundColor = ConsoleColor.White;
        }
        Console.Out.Write("|");
        
        if(_delay > 0)
            Thread.Sleep(_delay);
    }

    public void Stop()
    {
        _display = false;
    }
}