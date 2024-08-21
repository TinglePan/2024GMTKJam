using System.Reflection.Metadata;
using Godot;

namespace ProjGMTK.Scripts;

public partial class BuildingNode: Control
{
    public class SetupArgs
    {
        public Building Building;
    }
    
    public Label LevelLabel;
    public Button OpenButton;
    public Option Upgrade;

    public Building Building;

    public override void _Ready()
    {
        LevelLabel = GetNode<Label>("Level");
        OpenButton = GetNode<Button>("Button");
        Upgrade = GetNode<Option>("Upgrade");
        OpenButton.Pressed += () =>
        {
            GameMgr.Instance.OpenBuilding(Building);
        };
    }

    public override void _Process(double delta)
    {
        OpenButton.Text = Building.DisplayName();
        LevelLabel.Text = $"Level: {Building.Level}";
    }

    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        Building = args.Building;
        Upgrade.Setup(new Option.SetupArgs()
        {
            ButtonPressed = Building.Upgrade,
            ButtonText = "Upgrade",
            GetCostFunc = () => Building.UpgradePrice
        });
    }
}