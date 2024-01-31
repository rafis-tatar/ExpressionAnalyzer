namespace ExpressionAnalyzer;
public class InterpretiveNumberItem : IInterpretiveItem
{        
        double _value;       
        public InterpretiveNumberItem(double value)
        {
                _value = value;
                Expression = $"{_value}";
        }

        public string? Expression { get ; set; }    
        object? IInterpretiveItem.GetValue()=> this.GetValue();
        public double GetValue() 
                => _value;
        public override string ToString()
        {
                return $"{this.Expression}";
        }
}
