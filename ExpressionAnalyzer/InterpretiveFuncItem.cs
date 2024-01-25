// See https://aka.ms/new-console-template for more information
public class InterpretiveFuncItem : IInterpretiveItem
{
        public string? Expression {get;  set;}
        public Delegate? Function {get;set;}    
        public IEnumerable<IInterpretiveElement>? Arguments {get;set;}
        public double GetValue()
        {
                if(Function!= null)
                {
                        List<double?> ls = new List<double?>();
                        foreach(var item in Arguments ?? Array.Empty<IInterpretiveElement>())
                        {
                                if(item is InterpretiveSeparateItem) continue;
                                var arg = item as IInterpretiveItem;
                                if(arg!= null && arg.GetValue() is double value)
                                {                                        
                                        ls.Add(value);
                                }                                        
                        } 
                        object?[]? arguments = ls.Select(o=>(object?)o).ToArray();                        
                        var result = Function.DynamicInvoke(arguments);
                        if (result is double) return (double) result;
                }                
                return double.NaN;
        }
        public override string ToString()
                => Expression ?? string.Empty;
}
