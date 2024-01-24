// See https://aka.ms/new-console-template for more information
public class OperatorToken : IToken
    {
        public OperatorType OperatorType { get; }

        public OperatorToken(OperatorType operatorType)
        {
            OperatorType = operatorType;
        }
    }
public class FunctionToken:IToken
{
    public Delegate Function{get;set;}
    public FunctionToken(Delegate @delegate)
    {
        Function = @delegate;
    }
}