using System.Globalization;
using ExpressionAnalyzer;

var interpretive = new Interpretive();
interpretive.RegisterFunction("sin",(double a) =>Math.Sin(a));
interpretive.RegisterFunction("date",() => DateTimeOffset.Now.Date);
interpretive.RegisterFunction("day",(DateTime date) => date.Day);
interpretive.SetVariable("@a",22);
interpretive.SetVariable("@b","day(date())");
var expression = "@b+2^3*(4/2+5)^2+@a";
try
{
    var d = interpretive.Calculate(expression, CultureInfo.InvariantCulture);
    Console.WriteLine($"{expression} => {interpretive.FormattedString}");    
    Console.WriteLine($"{expression} = {d.ToString(CultureInfo.InvariantCulture)}");    
}
catch (Exception exception)
{
    Console.WriteLine($"{exception.Message}");    
}

Console.ReadLine();
