using System.Globalization;
using ExpressionAnalyzer;

var interpretive = new Interpretive();
interpretive.RegisterFunction("SIN",(double a) =>Math.Sin(a));
interpretive.RegisterFunction("date",() => DateTime.Now.Day);
var expression = "SIN(date())";

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
