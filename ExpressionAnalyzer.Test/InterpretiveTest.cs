namespace ExpressionAnalyzer.Test;

public class InterpretiveTest
{
     [Fact]
    public void TestRegisterDoubleVariable()
    {
        Interpretive interpretive = new Interpretive();   
        bool a = false ,b = false;     
        try {

        a = interpretive.SetVariable("@a", 2);
        b = interpretive.SetVariable("@a", 2);       

        }
        catch { }
        Assert.False(a && b);
    }
    [Fact]
    public void TestVariable()
    {
        Interpretive interpretive = new Interpretive();
        interpretive.SetVariable("@a", 2);
        interpretive.SetVariable("@b", 2);
        var result = interpretive.Calculate("@a+@b");
        Assert.True(result == 4, interpretive.FormattedString);
    }
    
    [Fact]
    public void TestRegisterFunction()
    {
        Interpretive interpretive = new Interpretive();
        interpretive.RegisterFunction("sum", (double a, double b)=>a+b);
        interpretive.SetVariable("@b", 2);
        var result = interpretive.Calculate("sum(@b+2)");
        Assert.True(result == 4, interpretive.FormattedString);
    }

    [Fact]
    public void TestFunctionInFunction()
    {
        Interpretive interpretive = new Interpretive();
        interpretive.RegisterFunction("date", ()=>DateTimeOffset.Now.Date);
        interpretive.RegisterFunction("day", (DateTime dateTime)=>dateTime.Day);        
        var result = interpretive.Calculate("day(date())");
        Assert.True(result == DateTimeOffset.Now.Date.Day, interpretive.FormattedString);
    }


    [Fact]
    public void TestOperation()
    {
        Interpretive interpretive = new Interpretive();        
        var result = interpretive.Calculate("2+2*(6+(2*2))^2/10");
        Assert.True(result == 22, interpretive.FormattedString);
    }
}