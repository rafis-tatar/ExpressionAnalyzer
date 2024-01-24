// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Security.Cryptography;
using System.Text;


public interface IInterpretiveElement
{
        string? Expression {get;}     
}
public interface IInterpretiveItem:IInterpretiveElement
{        
        double GetValue();
}
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
        return $"{_value}";
    }
}
public class InterpretiveBracketItem : IInterpretiveItem
{
    public string? Expression { get ; set; }
    public IEnumerable<IInterpretiveElement>? Arguments {get;set;}     
    public double GetValue()
    {
        var result = Arguments?.Calculate()??double.NaN;
        return result;
    }
    public override string ToString()
    {
        return $"({string.Join("",Arguments!)})";
    }
}
public class InterpretiveFuncItem : IInterpretiveItem
{
        public string? Expression {get;  set;}
        public Delegate? Function {get;set;}    
        public List<IInterpretiveElement> Arguments {get;set;} = new List<IInterpretiveElement>();    
        public double GetValue()
        {
                double result = .0;
                return result;
                // foreach (var argument in Arguments)
                // {
                //         if(argument is InterpretiveOperationItem operation)
                //         {
                //                 result = operation.Expression switch 
                //                 {
                //                         "+" => result + argument
                                        
                //                 }
                //         }                        
                // }
                // var arguments = Arguments
                // .Select(o=>o.GetValue())
                // .ToArray();
                // double? value =(double?)Function?.DynamicInvoke(arguments);
                // return value;
        }
}

public static class InterpretiveCalculateExtension
{
        public static double Calculate(this IEnumerable<IInterpretiveElement> Arguments)
        {
                double result =  double.NaN;                
                InterpretiveOperationItem? currentOperation = null;


                foreach (var argument in Arguments)
                {            
                        if(argument is IInterpretiveItem arg)
                        {
                                result = SetOperation(result,arg, currentOperation);                               
                        }   
                        else if(argument is InterpretiveOperationItem operation)
                        {
                                if(double.IsNaN(result))
                                {
                                        result = SetOperation(result,new InterpretiveValueItem(0.0), operation);                                        
                                }
                                else
                                {
                                        currentOperation = operation;
                                }                                       
                        }               
                }
                return result;
        }
        static double SetOperation(double value, IInterpretiveItem argument, InterpretiveOperationItem? operation)
        {
                if(operation is null)
                        return argument.GetValue();
                var newValue = operation.Expression switch
                                        {
                                                "+" => value + argument.GetValue(),
                                                "-"=> value - argument.GetValue(),
                                                "*"=> value * argument.GetValue(),
                                                "/"=> value / argument.GetValue(),
                                                _ => throw new SyntaxException($"There's no a suitable operator for the char {operation.Expression}")
                                        };
                return newValue;
        }
}
public class Interpretive
{
        private readonly StringBuilder _valueItemBuilder;
        public Interpretive()
        {       
                _valueItemBuilder = new StringBuilder();              
        }
        public double Calculate(string? expression)
        {
                var items = GetInterpretiveElements(expression?.Replace(" ",string.Empty));
                var result = items.Calculate();
                return result;
        }        
        private IEnumerable<IInterpretiveElement> GetInterpretiveElements(string? expression)
        {
                string? currentExpression = expression;
                List<IInterpretiveElement> items = new List<IInterpretiveElement>();
                int index = 0;
                while(currentExpression!=null && currentExpression.Length>0)
                {                     
                     var item =FeedCharacter(ref currentExpression, index);
                     if (item != null) {
                        items.Add(item);
                        currentExpression = currentExpression.Substring(item.Expression?.Length ?? 0);
                        index=0;
                        continue;
                     }
                     index++;
                }
                return items;
        }

        private IInterpretiveElement? FeedCharacter(ref string currentExpression, int index)
        {
                char? next = currentExpression.Length > index ? currentExpression[index] : null;
                if (next is null)
                {
                        var str = _valueItemBuilder.ToString();
                        var item = CreateOperandItem(str);                           
                        _valueItemBuilder.Clear();
                        return item;               
                }
                if(IsOperatorCharacter(next))
                {
                        if (_valueItemBuilder.Length > 0)
                        {
                                var str = _valueItemBuilder.ToString();
                                var item = CreateOperandItem(str);                           
                                _valueItemBuilder.Clear();
                                return item;                        
                        }
                        var operatorToken = CreateOperatorToken(next);
                        return operatorToken;                       
                         
                }   
                if(IsBracket(next))
                {                        
                        var endIndex = GetCloseBracketedIndex(currentExpression);
                        var bracketContentExpression = currentExpression.Substring(0,endIndex+1);
                        
                        var item = new InterpretiveBracketItem(){
                                Expression = bracketContentExpression,
                                Arguments=GetInterpretiveElements(bracketContentExpression.Substring(1,endIndex-1))
                        };
                        return item;
                }
                _valueItemBuilder.Append(next);
                return null;
              
        }
        private IInterpretiveElement CreateOperatorToken(char? c)
        {
                 var item = c switch
                {              
                        '+' => new InterpretiveOperationItem(OperatorType.Addition),
                        '-' => new InterpretiveOperationItem(OperatorType.Subtraction),
                        '*' => new InterpretiveOperationItem(OperatorType.Multiplication),
                        '/' => new InterpretiveOperationItem(OperatorType.Division),
                        _ => throw new SyntaxException($"There's no a suitable operator for the char {c}"),
                };                
                return item;
        }
        private IInterpretiveElement? CreateOperandItem(string raw)
        {
                if(double.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
                {
                        return new InterpretiveValueItem(result);
                }
                return default;
        }
        


        private static bool IsFunction(string c) 
                => c switch
                {
                var x when new string[]{"SUM","SQRT"}.Contains(x) => true,
                _ => false
                };

        public int GetCloseBracketedIndex(string raw)
        {                
                int bracket =0;   
                int i ; 
                for (i=0; i < raw.Length; i++)
                {
                        if(raw[i]=='(') 
                        {
                                bracket++;
                                continue;
                        }
                        if(raw[i]==')')
                        {
                                bracket--;
                                if(bracket==0)
                                {
                                        break;
                                }
                        }                         
                }  
                return i;
        }
        private static bool IsBracket(char? c) 
                => c=='(' || c==')';

        private static bool IsOperatorCharacter(char? c) 
                => c!= null && c switch
                {           
                var x when new char?[]{'+', '-', '*', '/'}.Contains(x) => true,
                _ => false
                };       
}
