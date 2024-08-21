using System;
using System.Collections.Generic;
using Godot;

namespace ProjGMTK.Scripts;

public class GameState
{
    public ulong PlayTime;
    public int CurrentTick;
    public int ScaleCount;
    public int TimeScaleCount;
    public int InvertedScaleCount;
    public Dictionary<Building.TypeEnum, List<Building>> Buildings;
    
    public Dictionary<Building.TypeEnum, int> BuildPrices;
    public int BuyTimeScalePrice;

    public bool KnowCarp;
    public bool KnowSnake;
    public bool KnowLoong;
    public bool KnowPond;
    public bool KnowCave;
    public bool KnowTimeScale;
    public bool GotFirstCreature;
    public bool BuiltFirstBuilding;

    private ulong _firstTickTime;
    
    public GameState()
    {
        PlayTime = 0;
        CurrentTick = 0;
        ScaleCount = 0;
        TimeScaleCount = 0;
        InvertedScaleCount = 0;
        Buildings = new Dictionary<Building.TypeEnum, List<Building>>();
        BuildPrices = new Dictionary<Building.TypeEnum, int>()
        {
            { Building.TypeEnum.Pond, GetBuildPrice(Building.TypeEnum.Pond) },
            { Building.TypeEnum.Cave, GetBuildPrice(Building.TypeEnum.Cave) }
        };
        BuyTimeScalePrice = GetBuyTimeScalePrice();
        
        KnowCarp = false;
        KnowSnake = false;
        KnowLoong = false;
        KnowPond = false;
        KnowCave = false;
        GotFirstCreature = false;
        BuiltFirstBuilding = false;
    }
    
    public void Tick()
    {
        var now = Time.GetTicksMsec();
        if (_firstTickTime == 0)
        {
            _firstTickTime = now;
        }
        PlayTime = now - _firstTickTime;
        CurrentTick++;
        foreach (var buildings in Buildings.Values)
        {
            foreach (var building in buildings)
            {
                building.Tick(CurrentTick);
            }
        }
    }
    
    public float Tick2Sec(int tick)
    {
        var tickIntervalSec = Constants.TickInterval / ((1 + TimeScaleCount) * 1000f);
        return tick * tickIntervalSec;
    }
    
    public int Sec2Tick(float sec)
    {
        var tickIntervalSec = Constants.TickInterval / ((1 + TimeScaleCount) * 1000f);
        return Mathf.RoundToInt(sec / tickIntervalSec);
    }

    public void SetScaleCount(int value)
    {
        var delta = value - ScaleCount;
        ScaleCount = value;
        EventMgr.Instance.FireEvent(Constants.ScaleCountChangedEventName, ScaleCount, delta);
    }
    
    public void SetTimeScaleCount(int value)
    {
        var delta = value - TimeScaleCount;
        TimeScaleCount = value;
        BuyTimeScalePrice = GetBuyTimeScalePrice();
        EventMgr.Instance.FireEvent(Constants.TimeScaleCountChangedEventName, TimeScaleCount, delta);
    }
    
    public void SetInvertedScaleCount(int value)
    {
        var delta = value - InvertedScaleCount;
        InvertedScaleCount = value;
        EventMgr.Instance.FireEvent(Constants.InvertedScaleCountChangedEventName, InvertedScaleCount, delta);
        if (InvertedScaleCount >= 81)
        {
            GameMgr.Instance.GameWin();
        }
    }
    
    public void SetKnowCarp()
    {
        KnowCarp = true;
        EventMgr.Instance.FireEvent(Constants.KnowCarpEventName);
    }
    
    public void SetKnowSnake()
    {
        KnowSnake = true;
        EventMgr.Instance.FireEvent(Constants.KnowSnakeEventName);
    }
    
    public void SetKnowLoong()
    {
        KnowLoong = true;
        EventMgr.Instance.FireEvent(Constants.KnowLoongEventName);
    }
    
    public void SetKnowPond()
    {
        KnowPond = true;
        EventMgr.Instance.FireEvent(Constants.KnowPondEventName);
    }
    
    public void SetKnowCave()
    {
        KnowCave = true;
        EventMgr.Instance.FireEvent(Constants.KnowCaveEventName);
    }

    public int BuildingCount()
    {
        var res = 0;
        foreach (var buildingType in Enum.GetValues(typeof(Building.TypeEnum))) 
        {
            var type = (Building.TypeEnum)buildingType;
            res += BuildingCount(type);
        }
        return res;
    }
    
    public int BuildingCount(Building.TypeEnum type)
    {
        if (!Buildings.ContainsKey(type) || Buildings[type] == null)
        {
            return 0;
        }
        return Buildings[type].Count;
    }
    
    public void UpdateBuildPrice(Building.TypeEnum type)
    {
        BuildPrices[type] = GetBuildPrice(type);
        EventMgr.Instance.FireEvent("BuildPriceChanged", type, BuildPrices[type]);
    }
    
    public void AddBuilding(Building building)
    {
        if (!Buildings.ContainsKey(building.Type))
        {
            Buildings[building.Type] = new List<Building>();
        }
        Buildings[building.Type].Add(building);
        if (!BuiltFirstBuilding) 
        {
            BuiltFirstBuilding = true;
            EventMgr.Instance.FireEvent(Constants.FirstBuildEventName);
        }
        EventMgr.Instance.FireEvent(Constants.NewBuildEventName, building);
    }
    
    private int GetBuildPrice(Building.TypeEnum type)
    {
        if (type == Building.TypeEnum.Pond)
        {
            return 5 + (int)(5 * Mathf.Pow(1.5f, BuildingCount(type)));
        }
        if (type == Building.TypeEnum.Cave)
        {
            return 50 + (int)(50 * Mathf.Pow(1.5f, BuildingCount(type)));
        }
        return 0;
    }
    
    private int GetBuyTimeScalePrice()
    {
        return 200 + (int)(Mathf.Pow(2f, TimeScaleCount) * 800);
    }
}