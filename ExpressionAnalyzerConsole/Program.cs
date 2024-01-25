Interpretive interpretive = new Interpretive();
var expression = "-10+30+SUM(SQRT(4);3)";
var d = interpretive.Calculate(expression);
Console.WriteLine($"{expression}={d}");
Console.ReadLine();
