namespace MiniBank.Api.Exceptions;

#pragma warning disable CA1515
public class MissingConfigurationException : Exception
{
    public MissingConfigurationException(string message) : base(message)
    {
    }
}
