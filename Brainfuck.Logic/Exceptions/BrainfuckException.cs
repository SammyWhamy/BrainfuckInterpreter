namespace Brainfuck.Logic.Exceptions;

public abstract class BrainfuckException : Exception
{
    protected BrainfuckException(string message) : base(message) { }
}