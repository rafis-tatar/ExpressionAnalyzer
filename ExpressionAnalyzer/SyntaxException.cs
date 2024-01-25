using System.Runtime.Serialization;

namespace ExpressionAnalyzer;

[Serializable]
public class SyntaxException : Exception
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