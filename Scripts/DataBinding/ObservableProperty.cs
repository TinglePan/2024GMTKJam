using System;
using System.Collections.Generic;

namespace ProjGMTK.Scripts.DataBinding;

public class ObservableProperty<T>: IObservableProperty<T>
{
    private T _value;
    private object _owner;
    private List<ObservableProperty<T>> _syncedProperties;
    
    public ObservableProperty(string name, object owner, T initialValue)
    {
        Name = name;
        _owner = owner;
        _value = initialValue;
        _syncedProperties = new List<ObservableProperty<T>>();
    }
    
    ~ObservableProperty()
    {
        ValueChanged = null;
        DetailedValueChanged = null;
        foreach (var syncedProperty in _syncedProperties)
        {
            syncedProperty.DetailedValueChanged -= SyncValueChangeHandler;
        }
    }
    
    public event EventHandler<ValueChangedEventArgs> ValueChanged = delegate { };
    public event EventHandler<ValueChangedEventDetailedArgs<T>> DetailedValueChanged = delegate { };

    public string Name { get; }

    public T Value
    {
        get => _value;
        set
        {
            if (Equals(value, _value)) return;
            var oldValue = _value;
            _value = value;
            ValueChangeHandler(oldValue, _value);
        }
    }

    public void FireValueChangeEventsOnInit()
    {
        ValueChangeHandler(default, _value);
    }
    
    public void SyncWith(ObservableProperty<T> otherProperty)
    {
        if (_syncedProperties.Contains(otherProperty))
        {
            return;
        }
        Value = otherProperty.Value;
        _syncedProperties.Add(otherProperty);
        otherProperty.DetailedValueChanged += SyncValueChangeHandler;
        otherProperty.SyncWith(this);
    }
    
    protected void SyncValueChangeHandler(object sender, ValueChangedEventDetailedArgs<T> args)
    {
        _value = args.NewValue;
    }
    
    protected void ValueChangeHandler(T from, T to)
    {
        ValueChanged?.Invoke(this, new ValueChangedEventArgs {PropertyName = Name});
        DetailedValueChanged?.Invoke(this, new ValueChangedEventDetailedArgs<T>
        {
            PropertyOwner = _owner,
            PropertyName = Name,
            OldValue = from,
            NewValue = to
        });
    }

}