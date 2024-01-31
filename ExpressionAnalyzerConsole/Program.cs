using System.Globalization;
using ExpressionAnalyzer;

var interpretive = new Interpretive();
interpretive.RegisterFunction("sin",(double a) =>Math.Sin(a));
interpretive.RegisterFunction("date",() => DateTime.Now.Day);
var expression = "4+3^2";

try
{
    var d = interpretive.Calculate(expression, CultureInfo.InvariantCulture);
    Console.WriteLine($"{expression}={d.ToString(CultureInfo.InvariantCulture)}");    
}
catch (Exception exception)
{
    Console.WriteLine($"{exception.Message}");    
}

Console.ReadLine();
