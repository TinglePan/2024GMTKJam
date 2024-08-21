using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjGMTK.Scripts.DataBinding;

public class DerivedObservableProperty<T> : IObservableProperty<T>, INotifyValueChanged<Lazy<T>>
{
    private Lazy<T> _value;
    private readonly Func<T> _valueGetter;
    private List<INotifyValueChanged> _valueChangesToObserve;
    
    public DerivedObservableProperty(
        string derivedPropertyName, Func<T> valueGetter,
        params INotifyValueChanged[] valueChangesToObserve)
    {
        Name = derivedPropertyName;
        _valueGetter = valueGetter;
        _value = new Lazy<T>(valueGetter);
        _valueChangesToObserve = new List<INotifyValueChanged>();
        if (valueChangesToObserve != null)
        {
            foreach (var valueChangeToObserve in valueChangesToObserve)
            {
                Watch(valueChangeToObserve);
            }
        }
    }
    
    ~DerivedObservableProperty()
    {
        foreach (var valueChangeToObserve in _valueChangesToObserve.ToList())
        {
            Unwatch(valueChangeToObserve);
        }
    }

    public event EventHandler<ValueChangedEventArgs> ValueChanged;
    public event EventHandler<ValueChangedEventDetailedArgs<Lazy<T>>> DetailedValueChanged;

    public T Value => _value.Value;
    
    public string Name { get; }

    public void RefreshProperty()
    {
        // Ensure we retrieve the value anew the next time it is requested
        var oldValue = _value;
        _value = new Lazy<T>(_valueGetter);
        ValueChanged?.Invoke(this, new ValueChangedEventArgs { PropertyName = Name });
        DetailedValueChanged?.Invoke(this, new ValueChangedEventDetailedArgs<Lazy<T>>
        {
            PropertyName = Name,
            OldValue = oldValue,
            NewValue = _value,
        });
    }

    public void Watch(INotifyValueChanged valueToWatch)
    {
        valueToWatch.ValueChanged += OnObservedValueChanged;
        _valueChangesToObserve.Add(valueToWatch);
    }
    
    public void Unwatch(INotifyValueChanged valueToUnwatch)
    {
        valueToUnwatch.ValueChanged -= OnObservedValueChanged;
        _valueChangesToObserve.Remove(valueToUnwatch);
    }
    
    protected void OnObservedValueChanged(object sender, ValueChangedEventArgs args)
    {
        RefreshProperty();
    }
}
