namespace Brainfuck.Logic.Exceptions;

public class RuntimeException : BrainfuckException
{
    public RuntimeException(string message) : base(message) { }
}