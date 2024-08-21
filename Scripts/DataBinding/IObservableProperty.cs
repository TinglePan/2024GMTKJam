namespace ProjGMTK.Scripts.DataBinding;

public interface IObservableProperty<out T>: INotifyValueChanged
{
    public string Name { get; }
    public T Value { get; }
}

public struct ValueChangedEventArgs
{
    public object PropertyOwner;
    public string PropertyName;
}

public struct ValueChangedEventDetailedArgs<T>
{
    public object PropertyOwner;
    public string PropertyName;
    public T OldValue;
    public T NewValue;
}