using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Godot;

namespace ProjGMTK.Scripts;

public partial class DialogueBox: Control
{
    public ScrollContainer ScrollContainer;
    public VBoxContainer ContentContainer;

    public List<DialogueEntry> Entries;
    
    public override void _Ready()
    {
        base._Ready();
        ScrollContainer = GetNode<ScrollContainer>("Content/ScrollContainer");
        ContentContainer = ScrollContainer.GetNode<VBoxContainer>("VBoxContainer");
        Entries = new List<DialogueEntry>();
    }
    
    public async void AddDialogue(string text, bool addTime=true)
    {
        if (addTime)
        {
            text = $"{text} [{Utils.PrettyPrintMSec(GameMgr.Instance.GameState.PlayTime)}]";
        }
        var tasks = new List<Task>();
        if (Entries.Count >= 30)
        {
            var removedEntry = Entries[0];
            Entries.Remove(removedEntry);
            removedEntry.QueueFree();
        }
        var entry = new DialogueEntry();
        ContentContainer.AddChild(entry);
        Entries.Add(entry);
        tasks.Add(entry.SetTextAndTween(text));
        var newTween = CreateTween();
        newTween.TweenProperty(ScrollContainer, "scroll_vertical", (int)ScrollContainer.GetVScrollBar().MaxValue,
            0.5f);
        tasks.Add(Utils.WaitSignal(ToSignal(newTween, Tween.SignalName.Finished)));
        await Task.WhenAll(tasks);
    }
}