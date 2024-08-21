using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ProjGMTK.Scripts;

public partial class BuildingTab: Control
{
    public class SetupArgs
    {
        public Building Building;
    }

    public bool HasSetup;
    public ulong LastTick { get; set; }

    public PackedScene BuildingCreatureScene;
    
    public Building Building;
    
    public Label BuildingNameLabel;
    public Label LevelLabel;
    public VBoxContainer CreatureContainer;
    public Dictionary<Creature, CreatureNode> CreatureNodes;
    public bool ActiveTick;

    public override void _Ready()
    {
        BuildingCreatureScene = GD.Load<PackedScene>("res://Scenes/BuildingCreature.tscn");
        BuildingNameLabel = GetNode<Label>("MarginContainer/Info/HBoxContainer/Name");
        LevelLabel = GetNode<Label>("MarginContainer/Info/HBoxContainer/Level");
        CreatureContainer = GetNode<VBoxContainer>("MarginContainer/Creatures/ScrollContainer/VBoxContainer");
        CreatureNodes = new Dictionary<Creature, CreatureNode>();
        foreach (var node in CreatureContainer.GetChildren().ToList())
        {
            if (node is CreatureNode creatureNode)
            {
                CreatureNodes[creatureNode.Creature] = creatureNode;
            }
            else
            {
                node.QueueFree();
            }
        }

        HasSetup = false;
        // Visible = GameMgr.Instance.MainTab.OptionsMenu.CanBuildCave || GameMgr.Instance.MainTab.OptionsMenu.CanBuildPond;
    }
    
    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        Building = args.Building;
        Building.OnAddCreature += OnAddCreature;
        Building.OnRemoveCreature += OnRemoveCreature;
        HasSetup = true;
    }

    public override void _Process(double delta)
    {
        ActiveTick = true;
        if (HasSetup)
        {
            BuildingNameLabel.Text = $"{Building.DisplayName()}";
            LevelLabel.Text = $"Level: {Building.Level}";
        }
    }
    
    private void OnAddCreature(Creature creature)
    {
        var creatureNode = BuildingCreatureScene.Instantiate<CreatureNode>();
        CreatureContainer.AddChild(creatureNode);
        creatureNode.Setup(new CreatureNode.SetupArgs
        {
            Creature = creature
        });
        CreatureNodes[creature] = creatureNode;
    }
    
    private void OnRemoveCreature(Creature creature)
    {
        if (CreatureNodes.ContainsKey(creature))
        {
            CreatureNodes[creature].QueueFree();
            CreatureNodes.Remove(creature);
        }
    }
}