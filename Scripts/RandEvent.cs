using System;
using Godot;

namespace ProjGMTK.Scripts;

public class RandEvent: ITick
{
    public string Name;
    public int MinCoolDown;
    public int MaxCoolDown;
    
    public int LastTriggerTick;
    public int LastTick { get; set; }
    public Action<RandEvent> Action;
    
    public RandEvent(string name, int minCoolDown, int maxCoolDown, Action<RandEvent> action)
    {
        Name = name;
        MinCoolDown = minCoolDown;
        MaxCoolDown = maxCoolDown;
        LastTriggerTick = 0;
        LastTick = 0;
        Action = action;
    }
    
    public void Tick(int tick)
    {
        if (CheckEvent())
        {
            Action.Invoke(this);
            LastTriggerTick = tick;
        }
        LastTick = tick;
    }
    
    private bool CheckEvent()
    {
        return Utils.CheckRand(Chance(), GameMgr.Instance.Rand);
    }
    
    private int Chance()
    {
        var ticksSinceLastTrigger = LastTick - LastTriggerTick;
        if (ticksSinceLastTrigger < MinCoolDown)
        {
            return 0;
        }
        if (ticksSinceLastTrigger > MaxCoolDown)
        {
            return 10000;
        }
        return 10000 * (ticksSinceLastTrigger - MinCoolDown) / (MaxCoolDown - MinCoolDown);
    }

}