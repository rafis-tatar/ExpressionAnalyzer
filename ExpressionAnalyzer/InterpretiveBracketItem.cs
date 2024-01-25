// See https://aka.ms/new-console-template for more information
public class InterpretiveBracketItem : IInterpretiveItem
{
    public string? Expression { get ; set; }
    public IEnumerable<IInterpretiveElement>? Arguments {get;set;}     
    public double GetValue()
    {
        var result = Arguments?.Calculate()??double.NaN;
        return result;
    }
    public override string ToString()
    {
        return $"({string.Join("",Arguments!)})";
    }
}
