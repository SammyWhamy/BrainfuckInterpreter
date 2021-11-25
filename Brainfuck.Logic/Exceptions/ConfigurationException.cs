namespace Brainfuck.Logic.Exceptions;

public class ConfigurationException : BrainfuckException
{
    public ConfigurationException(string message) : base(message) { }
}