using System;

namespace ProjGMTK.Scripts.DataBinding;

public interface INotifyValueChanged
{
    event EventHandler<ValueChangedEventArgs> ValueChanged;
}

public interface INotifyValueChanged<T>
{
    event EventHandler<ValueChangedEventDetailedArgs<T>> DetailedValueChanged;
}

public interface INotifyLazyValueChanged<T>
{
    event EventHandler<ValueChangedEventDetailedArgs<Lazy<T>>> DetailedValueChanged;
}