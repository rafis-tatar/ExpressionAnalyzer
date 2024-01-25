// See https://aka.ms/new-console-template for more information
public static class InterpretiveCalculateExtension
{
        private static bool IsMultiplicationOperation(IInterpretiveElement argument)
                => argument is InterpretiveOperationItem operationItem && operationItem.OperatorType== OperatorType.Multiplication;
        
        private static bool IsDivisionOperation(IInterpretiveElement argument)
                => argument is InterpretiveOperationItem operationItem && operationItem.OperatorType== OperatorType.Division;
        
        public static double Calculate(this IEnumerable<IInterpretiveElement> Arguments)
        {
               double result =  double.NaN;                
               InterpretiveOperationItem? currentOperation = null;
               var calculatePosition = SetCalculatePosition(Arguments);
                foreach (var argument in calculatePosition)
                {            
                        if(argument is IInterpretiveItem arg)
                        {
                                result = SetOperation(result,arg, currentOperation);  
                                currentOperation = null;                                 
                        }   
                        else if(argument is InterpretiveOperationItem operation)
                        {
                                if(currentOperation!=null)
                                {
                                        throw new SyntaxException();
                                }
                                if(double.IsNaN(result))
                                {
                                        result = 0.0;
                                }                                
                                currentOperation = operation;                                
                        }               
                }
                return result;
        }

        static IEnumerable<IInterpretiveElement> SetCalculatePosition(IEnumerable<IInterpretiveElement> Arguments)
        {
                if (Arguments.Count(operation=> operation is InterpretiveOperationItem) >= Arguments.Count(operation=> operation is not InterpretiveOperationItem))
                        throw new SyntaxException();
                        
                if (!Arguments.Any(operation=>IsDivisionOperation(operation) || IsMultiplicationOperation(operation)))
                        return Arguments;

                if(Arguments.Count() ==3 && Arguments.Count(operation=> operation is InterpretiveOperationItem) == 1)
                        return Arguments;
                
                var _calcPositions = new List<IInterpretiveElement>();
                var ls = Arguments.ToList();
                var index = ls.IndexOf(ls.First(operation=>IsDivisionOperation(operation) || IsMultiplicationOperation(operation)));
                var operation = (InterpretiveOperationItem)ls[index];
                _calcPositions.AddRange(ls.Take(index-1));
                var arg = ls.Skip(index-1).Take(3);
                _calcPositions.Add(new InterpretiveBracketItem(){
                        Expression = $"{string.Join("",arg)}",
                        Arguments = arg
                });
                _calcPositions.AddRange(ls.Skip(index+2));
                _calcPositions = SetCalculatePosition(_calcPositions)
                        .ToList() ;                                  

                return _calcPositions;   
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
