namespace ExpressionAnalyzer;
public interface IInterpretiveItem:IInterpretiveElement
{        
        object? GetValue();
}
public interface IInterpretiveItem<T>:IInterpretiveElement
{        
        T GetValue();
}