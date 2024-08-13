namespace Shared.Exceptions;

public class InvalidChecksumException : BaseException
{
    public InvalidChecksumException(string message) : base(message)
    {
    }

    public InvalidChecksumException(string message, Exception innerException) : base(message, innerException)
    {
    }
}