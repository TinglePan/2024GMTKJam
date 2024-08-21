using Godot;
using GodotPlugins.Game;

namespace ProjGMTK.Scripts;

public partial class GameTabs: TabContainer
{
    public MainTab MainTab;
    public LandTab LandTab;
    public BuildingTab BuildingTab;

    public int LastTick { get; set; }
    
    public override void _Ready()
    {
        GameMgr.Instance.GameTabs = this;
        MainTab = GetNode<MainTab>("Main");
        LandTab = GetNode<LandTab>("Land");
        SetTabHidden(GetTabIdxFromControl(LandTab), true);
        EventMgr.Instance.RegisterEvent("FirstBuild", (args) =>
        {
            SetTabHidden(GetTabIdxFromControl(LandTab), false);
        }, this);
        BuildingTab = GetNode<BuildingTab>("Building");
        SetTabHidden(GetTabIdxFromControl(BuildingTab), true);
    }
}