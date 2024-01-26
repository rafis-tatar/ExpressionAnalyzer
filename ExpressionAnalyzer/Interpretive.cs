using System.Globalization;
using System.Text;

namespace ExpressionAnalyzer;
public class Interpretive
{
        private readonly StringBuilder _valueItemBuilder;
        private readonly Dictionary<string,Delegate> _functions;
        private readonly Dictionary<string,object> _variables;
        public Interpretive()
        {       
                _valueItemBuilder = new StringBuilder();   
                _functions = new Dictionary<string, Delegate>();    
                _variables= new Dictionary<string, object>();                 
        }
        public bool SetVariable<T>(string name, object variable)                 
        {
                if (!name.StartsWith("@"))
                        throw new SyntaxException("Variable name must start with \'@\' symbol");
                return _variables.TryAdd(name, variable);
        }
        public void RegisterFunction(string name, Delegate func)
        {
                if(_functions.ContainsKey(name)) 
                        throw new ArgumentException($"Function with name {name} already registered");
                _functions.Add(name, func);                
        }       
        public double Calculate(string? expression)=>Calculate(expression, CultureInfo.CurrentCulture);
        public double Calculate(string? expression, CultureInfo cultureInfo)
        {            
                var formattedString = SetVariableToExpression(expression?.Replace(" ",string.Empty),cultureInfo);
                var items = GetInterpretiveElements(formattedString,cultureInfo);
                var result = items.Calculate();        
                return result;
        }        
       
        private string? SetVariableToExpression(string? expression, CultureInfo culture)
        {                
                if (string.IsNullOrWhiteSpace(expression)) 
                        return expression;

                string? _expression = expression;
                foreach (var item in _variables)
                {
                        _expression = _expression.Replace(item.Key, $"{item.Value}", false, culture);
                }
                return _expression;
        }
        private IEnumerable<IInterpretiveElement> GetInterpretiveElements(string? expression, CultureInfo cultureInfo)
        {
                string? currentExpression = expression;
                List<IInterpretiveElement> items = new List<IInterpretiveElement>();
                int index = 0;
                while(currentExpression!=null && currentExpression.Length>0)
                {                     
                     var item =FeedCharacter(ref currentExpression, index, cultureInfo);
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
        private IInterpretiveElement? FeedCharacter(ref string currentExpression, int index,CultureInfo cultureInfo)
        {                
                char? next = currentExpression.Length > index ? currentExpression[index] : null;
                if (next is null)
                {
                        var str = _valueItemBuilder.ToString();
                        var item = CreateOperandItem(str,cultureInfo);                           
                        _valueItemBuilder.Clear();
                        return item;               
                }
                if(IsSeparate(next))
                {        
                        if (_valueItemBuilder.Length > 0)
                        {
                                var str = _valueItemBuilder.ToString();
                                var item = CreateOperandItem(str,cultureInfo);                           
                                _valueItemBuilder.Clear();
                                return item;                        
                        }                
                        return new InterpretiveSeparateItem($"{next}");
                }

                if(IsOperatorCharacter(next))
                {
                        if (_valueItemBuilder.Length > 0)
                        {
                                var str = _valueItemBuilder.ToString();
                                var item = CreateOperandItem(str,cultureInfo);                           
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

                                var arguments = GetInterpretiveElements(functionContentExpression.Substring(1,endIndex- 1),cultureInfo);
                                var function = _functions[str];
                                var classType = typeof(InterpretiveFuncItem<>);
                                Type[] typeParams = new Type[] { function.Method.ReturnType };
                                Type constructedType = classType.MakeGenericType(typeParams);
                                var genericFunType = (IInterpretiveFuncItem?)Activator.CreateInstance(constructedType);
                                if(genericFunType != null)
                                {
                                        genericFunType.Expression = $"{str}{functionContentExpression}";        
                                        genericFunType.Arguments = arguments;
                                        genericFunType.Function = function;
                                }
                                return genericFunType;
                        }
                        else
                        {
                                var endIndex = GetCloseBracketedIndex(currentExpression);                       
                                var bracketContentExpression = currentExpression.Substring(0,endIndex+1);
                                var arguments = GetInterpretiveElements(bracketContentExpression.Substring(1,endIndex-1),cultureInfo);
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
        private IInterpretiveElement? CreateOperandItem(string raw, CultureInfo cultureInfo)
        {
                
                if(!raw.All(o=>Char.IsNumber(o)||cultureInfo.NumberFormat.NumberDecimalSeparator == $"{o}"))
                {
                        throw new SyntaxException("Syntaxes not valid or CultureInfo");
                }
                if(double.TryParse(raw, NumberStyles.Number, cultureInfo, out double result))
                {
                        return new InterpretiveNumberItem(result);
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
