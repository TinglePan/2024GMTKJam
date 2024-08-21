using System;
using System.Collections.Generic;
using Godot;

namespace ProjGMTK.Scripts;

public partial class OptionsMenu: Control
{
    public Dictionary<string, Option> Options;
    public VBoxContainer Container;

    private PackedScene OptionScene;
    
    public override void _Ready()
    {
        Options = new Dictionary<string, Option>();
        Container = GetNode<VBoxContainer>("Content/VBoxContainer");
        OptionScene = GD.Load<PackedScene>("res://Scenes/Option.tscn");
        EventMgr.Instance.RegisterEvent(Constants.ScaleCountChangedEventName, OnScaleCountChanged, this);
    }

    public void AddOption(string text, Action action, Func<int> getCostFunc, int coolDown=0)
    {
        var option = OptionScene.Instantiate<Option>();
        Container.AddChild(option);
        option.Setup(new Option.SetupArgs()
        {
            ButtonText = text,
            ButtonPressed = action,
            GetCostFunc = getCostFunc,
            CoolDown = coolDown,
        });
        Options.Add(text, option);
    }
    
    private void OnScaleCountChanged(object[] args)
    {
        var scaleCount = (int)args[0];
        var gameState = GameMgr.Instance.GameState;
        var pondBuildPrice = GameMgr.Instance.GameState.BuildPrices[Building.TypeEnum.Pond];
        if (scaleCount >= pondBuildPrice && !gameState.KnowPond)
        {
            AddOption("Build pond", () =>
            {
                GameMgr.Instance.Build(Building.TypeEnum.Pond);
            }, () => GameMgr.Instance.GameState.BuildPrices[Building.TypeEnum.Pond]);
            gameState.SetKnowPond();
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue("You can now build a pond. Pond raises carps. Carps randomly drop scales. Carps regularly level up. Carps randomly grow into loongs. If they fail, they die after they max out their levels. This will grant a large amount of scales");
        }
        
        var caveBuildPrice = GameMgr.Instance.GameState.BuildPrices[Building.TypeEnum.Cave];
        if (scaleCount >= caveBuildPrice && !gameState.KnowCave)
        {
            AddOption("Build cave", () =>
            {
                GameMgr.Instance.Build(Building.TypeEnum.Cave);
            }, () => GameMgr.Instance.GameState.BuildPrices[Building.TypeEnum.Cave]);
            gameState.SetKnowCave();
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue("You can now build a cave. Cave raises snakes. Snakes randomly drop scales. Snakes regularly level up and when they level up, they grant you some additional scales. Snake grows into loong as long as both their level and their living cave's level are maxed out. Snakes do not die.");
        }
        
        var buyTimeScalePrice = GameMgr.Instance.GameState.BuyTimeScalePrice;
        if (scaleCount >= buyTimeScalePrice && !gameState.KnowTimeScale)
        {
            AddOption("Buy time scale", () =>
            {
                GameMgr.Instance.BuyTimeScale();
            }, () => GameMgr.Instance.GameState.BuyTimeScalePrice);
            gameState.KnowTimeScale = true;
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue("You can now buy a time scale. Time scale increases the speed of the game.");
        }
    }
}