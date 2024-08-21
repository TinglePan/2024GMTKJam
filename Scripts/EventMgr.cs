using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ProjGMTK.Scripts;

public partial class EventMgr: Node
{
    public static EventMgr Instance;
    public Dictionary<string, List<(object, Action<object[]>)>> Events;

    public override void _Ready()
    {
        Instance = this;
        Events = new Dictionary<string, List<(object, Action<object[]>)>>();
    }
    
    public void RegisterEvent(string name, Action<object[]> action, object receiver)
    {
        if (!Events.ContainsKey(name))
        {
            Events[name] = new List<(object, Action<object[]>)>();
        }
        Events[name].Add((receiver, action));
    }
    
    public void UnregisterEvent(string name, Action<object[]> action, object receiver)
    {
        if (!Events.ContainsKey(name))
        {
            return;
        }
        Events[name].Remove((receiver, action));
    }

    public void FireEvent(string name, params object[] args)
    {
        if (!Events.TryGetValue(name, out var handlers))
        {
            return;
        }

        foreach (var (receiver, handler) in handlers.ToList())
        {
            if (receiver is Node node && !IsInstanceValid(node)) continue;
            handler.Invoke(args);
        }
    }
}
