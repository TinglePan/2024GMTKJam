using System.Collections.Generic;
using System.Reflection.Metadata;
using Godot;

namespace ProjGMTK.Scripts;

public partial class LandTab: Control
{
    public PackedScene BuildingScene;
    public GridContainer Grid;
    public Dictionary<Building, BuildingNode> BuildingNodes;
    
    public override void _Ready()
    {
        Grid = GetNode<GridContainer>("MarginContainer/ScrollContainer/MarginContainer/GridContainer");
        BuildingNodes = new Dictionary<Building, BuildingNode>();
        EventMgr.Instance.RegisterEvent(Constants.NewBuildEventName, OnAddBuilding, this);
        BuildingScene = GD.Load<PackedScene>("res://Scenes/LandBuilding.tscn");
    }
    
    private void OnAddBuilding(object[] args)
    {
        var building = (Building)args[0];
        var buildingNode = BuildingScene.Instantiate<BuildingNode>();
        Grid.AddChild(buildingNode);
        buildingNode.Setup(new BuildingNode.SetupArgs
        {
            Building = building
        });
        BuildingNodes[building] = buildingNode;
    }
}