using System.Globalization;

namespace ExpressionAnalyzer;
public interface IInterpretiveFuncItem:IInterpretiveItem
{    
        Delegate? Function {get;set;}    
        IEnumerable<IInterpretiveElement>? Arguments {get;set;}
}
public class InterpretiveFuncItem<T> : IInterpretiveFuncItem
{
        public string? Expression {get;  set;}
        public Delegate? Function {get;set;}    
        public IEnumerable<IInterpretiveElement>? Arguments {get;set;}
        object? IInterpretiveItem.GetValue()=> this.GetValue();
        public T? GetValue()
        {
                 if(Function!= null)
                {
                        
                        object?[]? arguments =null;
                        if (Arguments != null && Arguments.Any(o=> o is InterpretiveSeparateItem))
                        {
                                var ls = new List<List<IInterpretiveElement>>();
                                ls.Add(new List<IInterpretiveElement>());
                                foreach (var item in Arguments)
                                {
                                        var currentGroup = ls[ls.Count-1];
                                        if(item is InterpretiveSeparateItem)
                                        {                                                
                                                ls.Add(new List<IInterpretiveElement>());
                                                continue;
                                        }
                                        currentGroup.Add(item);
                                }
                                arguments = ls.Select(o=>(object)o.Calculate()).ToArray();
                        }
                        else
                        {
                               arguments = Arguments?
                                        .Where(o=>o is IInterpretiveItem)
                                        .Select(o=> ((IInterpretiveItem)o).GetValue())
                                        .ToArray();
                        }

                        var result = Function.DynamicInvoke(arguments);
                        return (T?) result;
                }                
                return default;
        }
}
        
public class InterpretiveFuncItem : IInterpretiveFuncItem
{
        public string? Expression {get;  set;}
        public Delegate? Function {get;set;}    
        public IEnumerable<IInterpretiveElement>? Arguments {get;set;}
        object? IInterpretiveItem.GetValue()=> this.GetValue();
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
                        object?[]? arguments = ls.Any() ? ls.Select(o=>(object?)o).ToArray() : null;                        
                        var result = Function.DynamicInvoke(arguments);
                        if (result is double) 
                                return (double) result;
                        else 
                                return Convert.ToDouble(result);
                }                
                return double.NaN;
        }
        public override string ToString()
                => Expression ?? string.Empty;
}
