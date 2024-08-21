using Godot;

namespace ProjGMTK.Scripts;

public partial class CreatureNode: Control
{
    public class SetupArgs
    {
        public Creature Creature;
    }

    public Label NameLabel;
    public Label LevelLabel;
    public Creature Creature;

    public ulong LastTick { get; set; }
    
    public void Setup(object o)
    {
        var args = (SetupArgs)o;
        Creature = args.Creature;
        NameLabel = GetNode<Label>("MarginContainer/Control/Name");
        LevelLabel = GetNode<Label>("MarginContainer/Control/Level");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        NameLabel.Text = Creature.DisplayName();
        LevelLabel.Text = $"Level: {Creature.Level}";
    }
}