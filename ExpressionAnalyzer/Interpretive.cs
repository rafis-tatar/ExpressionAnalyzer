// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Text;

public class Interpretive
{
        private readonly StringBuilder _valueItemBuilder;
        Dictionary<string,Delegate> _functions;
        public Interpretive()
        {       
                _valueItemBuilder = new StringBuilder();   
                _functions = new Dictionary<string, Delegate>();     
                LoadFunctions();
        }
        public void RegisterFunction(string name, Delegate func)
        {
                if(_functions.ContainsKey(name)) 
                        throw new ArgumentException($"Function with name {name} already registered");
                _functions.Add(name, func);
        }
        public double Calculate(string? expression)
        {                
                try                
                {
                        var items = GetInterpretiveElements(expression?.Replace(" ",string.Empty));
                        var result = items.Calculate();        
                        return result;
                }
                catch
                {                        
                        throw new SyntaxException("Syntaxes not valid");
                }                
        }        
       
        private void LoadFunctions()
        {                
                RegisterFunction("SUM", (double a, double b)=> a+b);
                RegisterFunction("SQRT", (double a )=> Math.Sqrt(a));
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
                if(IsSeparate(next))
                {                        
                        return new InterpretiveSeparateItem($"{next}");
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
                        var operatorToken = CreateOperatorItem(next);
                        return operatorToken;                       
                         
                }   
                if(IsBracket(next))
                {     
                        var str = _valueItemBuilder.ToString();
                        if(IsFunction(str))
                        {
                                _valueItemBuilder.Clear();

                                var endIndex = GetCloseBracketedIndex(currentExpression.Substring(str.Length));
                                var functionContentExpression = currentExpression.Substring(str.Length,endIndex+1);                                                                

                                var arguments = GetInterpretiveElements(functionContentExpression.Substring(1,endIndex- 1));
                                var functionItem = new InterpretiveFuncItem()
                                {
                                        Expression = $"{str}{functionContentExpression}",
                                        Arguments = arguments, 
                                        Function = _functions[str]
                                };
                                return functionItem;
                        }
                        else
                        {
                                var endIndex = GetCloseBracketedIndex(currentExpression);                       
                                var bracketContentExpression = currentExpression.Substring(0,endIndex+1);
                                var arguments = GetInterpretiveElements(bracketContentExpression.Substring(1,endIndex-1));
                                var bracketItem = new InterpretiveBracketItem()
                                {
                                        Expression = bracketContentExpression,
                                        Arguments=arguments
                                };
                                return bracketItem;
                        }                       
                }
                _valueItemBuilder.Append(next);
                return null;
              
        }
        private IInterpretiveElement CreateOperatorItem(char? c)
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
        private bool IsFunction(string c) 
                => this._functions.ContainsKey(c);
        private static int GetCloseBracketedIndex(string raw)
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
        private static bool IsSeparate(char? c) 
                => c==';' ;
        private static bool IsOperatorCharacter(char? c) 
                => c!= null && c switch
                {           
                var x when new char?[]{'+', '-', '*', '/'}.Contains(x) => true,
                _ => false
                };       
}
