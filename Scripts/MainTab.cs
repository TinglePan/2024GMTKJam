using Godot;

namespace ProjGMTK.Scripts;

public partial class MainTab: Control
{
    public Label PlayTimeLabel;
    public Label ScaleCountLabel;
    public Label InvertedScaleCountLabel;
    public Label TimeScaleCountLabel;
    public Label BuildingStatisticsLabel;
    public Label CreatureStatisticsLabel;

    public OptionsMenu OptionsMenu;
    
    public DialogueBox Log; 
    
    public override void _Ready()
    {
        PlayTimeLabel = GetNode<Label>("Info/Content/VBoxContainer/PlayTime");
        ScaleCountLabel = GetNode<Label>("Info/Content/VBoxContainer/Scales");
        TimeScaleCountLabel = GetNode<Label>("Info/Content/VBoxContainer/TimeScales");
        InvertedScaleCountLabel = GetNode<Label>("Info/Content/VBoxContainer/InvertedScales");
        BuildingStatisticsLabel = GetNode<Label>("Info/Content/VBoxContainer/Buildings");
        CreatureStatisticsLabel = GetNode<Label>("Info/Content/VBoxContainer/Creatures");
        Log = GetNode<DialogueBox>("Log");
        OptionsMenu = GetNode<OptionsMenu>("Options");
        OptionsMenu.AddOption("Scale self", GameMgr.Instance.ScaleSelf, () => 0, 1000);
    }

    public override void _Process(double delta)
    {
        var gameState = GameMgr.Instance.GameState;
        PlayTimeLabel.Text = $"Play Time: {Utils.PrettyPrintMSec(gameState.PlayTime)}";
        ScaleCountLabel.Text = $"Scales: {gameState.ScaleCount}";
        TimeScaleCountLabel.Text = $"Time Scales: {gameState.TimeScaleCount}";
        TimeScaleCountLabel.Visible = gameState.KnowTimeScale;
        InvertedScaleCountLabel.Text = $"Inverted Scales: {gameState.InvertedScaleCount}";
        InvertedScaleCountLabel.Visible = gameState.InvertedScaleCount > 0;
        BuildingStatisticsLabel.Text = $"Buildings: {GetBuildingStatistics()}";
        BuildingStatisticsLabel.Visible = gameState.BuiltFirstBuilding;
        CreatureStatisticsLabel.Text = $"Creatures: {GetCreatureStatistics()}";
        CreatureStatisticsLabel.Visible = gameState.GotFirstCreature;
    }

    private string GetBuildingStatistics()
    {
        var gameState = GameMgr.Instance.GameState;
        var pondCount = gameState.BuildingCount(Building.TypeEnum.Pond);
        var caveCount = gameState.BuildingCount(Building.TypeEnum.Cave);
        var res = $"Buildings: {pondCount + caveCount} total";
        if (gameState.KnowPond)
        {
            res += $", {pondCount} pond";
        }
        if (gameState.KnowCave)
        {
            res += $", {caveCount} cave";
        }
        return res;
    }

    private string GetCreatureStatistics()
    {
        var gameState = GameMgr.Instance.GameState;
        var snakeCount = 0;
        var carpCount = 0;
        var loongCount = 0;
        foreach (var buildings in gameState.Buildings.Values)
        {
            foreach (var building in buildings)
            {
                snakeCount += building.CreatureCount(Creature.TypeEnum.Snake);
                carpCount += building.CreatureCount(Creature.TypeEnum.Carp);
                loongCount += building.CreatureCount(Creature.TypeEnum.Loong);
            }
        }
        var res = $"Creatures: {snakeCount + carpCount} total";
        
        if (gameState.KnowCarp)
        {
            res += $", {carpCount} carp";
        }
        
        if (gameState.KnowSnake)
        {
            res += $", {snakeCount} snake";
        }

        
        if (gameState.KnowLoong)
        {
            res += $", {loongCount} loong";
        }
        return res;
    }
}