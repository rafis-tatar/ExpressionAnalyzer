// See https://aka.ms/new-console-template for more information
public class InterpretiveSeparateItem : IInterpretiveElement
{
        public InterpretiveSeparateItem(string separate)
        {
                Expression = separate;
        }
        public string? Expression { get; }
        public override string ToString()
                => Expression ?? string.Empty;
}
