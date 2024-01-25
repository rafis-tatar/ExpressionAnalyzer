// See https://aka.ms/new-console-template for more information
using System.Runtime.Serialization;

[Serializable]
internal class SyntaxException : Exception
{
    public SyntaxException():this("Syntaxes not valid")
    {

    }

    public SyntaxException(string? message) : base(message)
    {
    }

    public SyntaxException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected SyntaxException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}