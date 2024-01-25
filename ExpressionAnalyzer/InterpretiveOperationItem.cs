// See https://aka.ms/new-console-template for more information
public class InterpretiveOperationItem : IInterpretiveElement
{
        public InterpretiveOperationItem(OperatorType operatorType)
        {
                OperatorType = operatorType;                
                Expression = OperatorType switch {
                        OperatorType.Addition => "+",
                        OperatorType.Subtraction => "-",
                        OperatorType.Multiplication => "*",
                        OperatorType.Division => "/",
                        _ => throw new SyntaxException($"There's no a suitable operator"),
                };                
        }

        public string? Expression { get; }
        public OperatorType OperatorType {get;}
        public override string ToString()
        {
                return $"{this.Expression}";
        }
}
