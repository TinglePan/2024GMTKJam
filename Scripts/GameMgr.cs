using System;
using System.Collections.Generic;
using Godot;
using ProjGMTK.Scripts.DataBinding;

namespace ProjGMTK.Scripts;

public partial class GameMgr: Node
{
    public static GameMgr Instance;
    
    public GameState GameState;

    public GameTabs GameTabs;
    public Random Rand;
    public Building CurrentOpenedBuilding;

    private List<ITick> _tickEntities;
    private Timer _tickTimer;

    public override void _Ready()
    {
        Instance = this;
        GameState = new GameState();
        _tickEntities = new List<ITick>();
        _tickTimer = new Timer();
        AddChild(_tickTimer);
        _tickTimer.WaitTime = Constants.TickInterval / ((1 + GameState.TimeScaleCount) * 1000f);
        _tickTimer.Timeout += Tick;
        _tickTimer.Start();
        EventMgr.Instance.RegisterEvent(Constants.TimeScaleCountChangedEventName, (args) =>
        {
            _tickTimer.WaitTime = Constants.TickInterval / ((1 + GameState.TimeScaleCount) * 1000f);
        }, this);
        Rand = new Random();
    }

    public override void _Process(double delta)
    {
        
    }

    public void Tick()
    {
        GameState.Tick();
        foreach (var tick in _tickEntities)
        {
            tick.Tick(GameState.CurrentTick);
        }
    }

    public void RegisterTick(ITick tick)
    {
        _tickEntities.Add(tick);
    }

    public void GameWin()
    {
        GD.Print("Game win");
    }

    public void OpenBuilding(Building building)
    {
        CurrentOpenedBuilding = building;
        GameTabs.SetTabHidden(GameTabs.GetTabIdxFromControl(GameTabs.BuildingTab), false);
        GameTabs.BuildingTab.Setup(new BuildingTab.SetupArgs()
        {
            Building = building
        });
    }

    public void ScaleSelf()
    {
        GameState.SetScaleCount(GameState.ScaleCount + 1);
        GameTabs.MainTab.Log.AddDialogue($"You scaled yourself for 1 scale");
    }

    public void Build(Building.TypeEnum type)
    {
        var price = GameState.BuildPrices[type];
        GameState.SetScaleCount(GameState.ScaleCount - price);
        var building = new Building(type, GameState.BuildingCount(type), 1);
        GameState.AddBuilding(building);
        GameTabs.MainTab.Log.AddDialogue($"{building.DisplayName()} built, costing {price} scales. Creatures may spawn there over time.");
        // if (type == Building.TypeEnum.Pond)
        // {
        //     GameTabs.MainTab.OptionsMenu.Options["Build pond"].Cost.Value = price;
        //     if (buildPrice > ScaleCount)
        //     {
        //         GameTabs.MainTab.OptionsMenu.Options["Build pond"].Button.Disabled = true;
        //     }
        // }
        // else if (type == Building.TypeEnum.Cave)
        // {
        //     var buildPrice = BuildPrice(type);
        //     GameTabs.MainTab.OptionsMenu.Options["Build cave"].Cost.Value = BuildPrice(type);
        //     if (buildPrice > ScaleCount)
        //     {
        //         GameTabs.MainTab.OptionsMenu.Options["Build cave"].Button.Disabled = true;
        //     }
        // }
        GameState.UpdateBuildPrice(type);
    }
    
    
    public void BuyTimeScale()
    {
        var price = GameState.BuyTimeScalePrice;
        GameState.SetScaleCount(GameState.ScaleCount - price);
        GameState.SetTimeScaleCount(GameState.TimeScaleCount + 1);
        GameTabs.MainTab.Log.AddDialogue($"You bought 1 time scale, costing {price} scales");
        // GameTabs.MainTab.OptionsMenu.Options["Buy time scale"].Cost.Value = BuyTimeScalePrice();
        // if (BuyTimeScalePrice() > ScaleCount)
        // {
        //     GameTabs.MainTab.OptionsMenu.Options["Buy time scale"].Button.Disabled = true;
        // }
    }
}