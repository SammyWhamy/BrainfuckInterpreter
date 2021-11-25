namespace Brainfuck.Logic.Exceptions;

public class SyntaxErrorException : BrainfuckException
{
    public SyntaxErrorException(string message) : base(message) { }
}