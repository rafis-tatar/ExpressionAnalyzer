// See https://aka.ms/new-console-template for more information
using System.Globalization;
using System.Text;
public class Tokenizer
    {
        public Tokenizer()
        {
            _valueTokenBuilder = new StringBuilder();
            _infixNotationTokens = new List<IToken>();
        }

        public IEnumerable<IToken> Parse(string expression)
        {
            Reset();
            foreach (char next in expression)
            {
                FeedCharacter(next);
            }
            return GetResult();
        }

        private void Reset()
        {
            _valueTokenBuilder.Clear();
            _infixNotationTokens.Clear();
        }

        private void FeedCharacter(char next)
        {
            if (IsSpacingCharacter(next))
            {
                if (_valueTokenBuilder.Length > 0)
                {
                    var token = CreateOperandToken(_valueTokenBuilder.ToString());
                    _valueTokenBuilder.Clear();
                    _infixNotationTokens.Add(token);
                }
            }
            else if (IsOperatorCharacter(next))            
            {                
                if (_valueTokenBuilder.Length > 0)
                {

                    var token = CreateOperandToken(_valueTokenBuilder.ToString());
                    _valueTokenBuilder.Clear();
                    _infixNotationTokens.Add(token);
                }

                var operatorToken = CreateOperatorToken(next);
                _infixNotationTokens.Add(operatorToken);
            }            
            else
            {
                _valueTokenBuilder.Append(next);
            }
        }

        private static bool IsOperatorCharacter(char c) => c switch
        {
            var x when new char[]{'(', ')', '+', '-', '*', '/'}.Contains(x) => true,
            _ => false
        };

        private static bool IsFunction(string c) => c switch
        {
            var x when new string[]{"SUM","SQRT"}.Contains(x) => true,
            _ => false
        };

        private static bool IsSpacingCharacter(char c)
        {
            return c switch
            {
                ' ' => true,
                _ => false,
            };
        }

        private static IToken CreateOperandToken(string raw)
        {
            if (double.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out double result))
            {
                return new OperandToken(result);
            }
            if (IsFunction(raw))
            {
                var token = CreateFunctionToken(raw);
                return token;
            }

            throw new SyntaxException($"The operand {raw} has an invalid format.");
        }

        private static FunctionToken CreateFunctionToken(string raw)
        {
            return raw switch
            {
                "SUM" => new FunctionToken((double a, double b) => a+b),
                "SQRT" => new FunctionToken((double a)=>Math.Sqrt(a)),
                _ => throw new SyntaxException($"There's no a suitable function for the string {raw}"),
            };
        }

        private static OperatorToken CreateOperatorToken(char c)
        {
            return c switch
            {
                // '(' => new OperatorToken(OperatorType.OpeningBracket),
                // ')' => new OperatorToken(OperatorType.ClosingBracket),
                '+' => new OperatorToken(OperatorType.Addition),
                '-' => new OperatorToken(OperatorType.Subtraction),
                '*' => new OperatorToken(OperatorType.Multiplication),
                '/' => new OperatorToken(OperatorType.Division),
                _ => throw new SyntaxException($"There's no a suitable operator for the char {c}"),
            };
        }

        private IEnumerable<IToken> GetResult()
        {
            if (_valueTokenBuilder.Length > 0)
            {
                var token = CreateOperandToken(_valueTokenBuilder.ToString());
                _valueTokenBuilder.Clear();
                _infixNotationTokens.Add(token);
            }

            return _infixNotationTokens.ToList();
        }

        private readonly StringBuilder _valueTokenBuilder;
        private readonly List<IToken> _infixNotationTokens;
    }
