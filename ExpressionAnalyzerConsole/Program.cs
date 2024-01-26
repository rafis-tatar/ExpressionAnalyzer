using System.Globalization;
using ExpressionAnalyzer;

var interpretive = new Interpretive();

interpretive.RegisterFunction("Sin",(double a) =>Math.Sin(a));
interpretive.RegisterFunction("Date", ()=> DateTime.Now);
interpretive.RegisterFunction("Day", (DateTime date)=> date.Day);
interpretive.RegisterFunction("Sum", (double a, double b)=> a+b);

interpretive.SetVariable<double>("@a", 12);
interpretive.SetVariable<double>("@b", 2);

var expression = "Sum(Sin(Day(Date()));Sum(@a;@b)) * 2 + 27/3";

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
