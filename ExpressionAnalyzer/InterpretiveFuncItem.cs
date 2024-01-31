using System.Globalization;

namespace ExpressionAnalyzer;
public class InterpretiveFuncItem : IInterpretiveItem
{
        public string? Expression {get;  set;}
        public Delegate? Function {get;set;}    
        public IEnumerable<IInterpretiveElement>? Arguments {get;set;}        
        public object? GetValue()
        {
                if(Function!= null)
                {
                        List<object?> ls = new List<object?>();
                        foreach(var item in Arguments ?? Array.Empty<IInterpretiveElement>())
                        {
                                if(item is InterpretiveSeparateItem) continue;
                                var arg = item as IInterpretiveItem;
                                if(arg!= null)
                                {                                        
                                        ls.Add(arg.GetValue());
                                }                                        
                        } 
                        object?[]? arguments = ls.Any() ? ls.ToArray() : null;                        
                        var result = Function.DynamicInvoke(arguments);
                        return result;
                        // if (result is double) 
                        //         return (double) result;
                        // else 
                        //         return Convert.ToDouble(result);
                }                
                return default;
        }
        public override string ToString()
                => Expression ?? string.Empty;
}
