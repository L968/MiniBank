namespace MiniBank.Api.Exceptions;

#pragma warning disable CA1515
public class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}
