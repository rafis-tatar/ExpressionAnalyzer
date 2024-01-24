// See https://aka.ms/new-console-template for more information
public class OperandToken : IToken
    {
        public double Value { get; }

        public OperandToken(double value)
        {
            Value = value;
        }
    }
