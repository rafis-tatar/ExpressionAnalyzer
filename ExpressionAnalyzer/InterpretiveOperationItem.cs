namespace ExpressionAnalyzer;
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
                        OperatorType.Exponentiation => "^",
                        _ => throw new SyntaxException($"There's no a suitable operator"),
                };                
        }

        public string? Expression { get; set; }
        public OperatorType OperatorType {get;}
        public override string ToString()
        {
                return $"{this.Expression}";
        }
}
