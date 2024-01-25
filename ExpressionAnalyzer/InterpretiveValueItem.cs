namespace ExpressionAnalyzer;
public class InterpretiveValueItem : IInterpretiveItem
{        
        double _value;
        public InterpretiveValueItem(double value)
        {
                _value = value;
                Expression = $"{_value}";
        }
        public string? Expression { get ; set; }        
        public double GetValue() 
                => _value;
        public override string ToString()
        {
                return $"{this.Expression}";
        }
}
