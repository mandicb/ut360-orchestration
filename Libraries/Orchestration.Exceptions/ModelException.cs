namespace Orchestration.Exceptions;

public class ModelException : Exception
{
    public ModelException(string message, Exception innerException) : base(message, innerException)
    {
        LocalMessage = message;
    }

    public ModelException(string message) : base(message)
    {
        LocalMessage = message;
    }

    public override string Message => LocalMessage;

    public string LocalMessage;
}