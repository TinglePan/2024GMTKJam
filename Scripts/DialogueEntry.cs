using System.Threading.Tasks;
using Godot;
using ProjGMTK.Scripts.DataBinding;

namespace ProjGMTK.Scripts;

public partial class DialogueEntry: Label
{
    public override void _Ready()
    {
        AutowrapMode = TextServer.AutowrapMode.Word;
        CustomMinimumSize = new Vector2(832, 0);
    }

    public async Task SetTextAndTween(string text)
    {
        Text = text;
        Modulate = new Color(Modulate.R, Modulate.G, Modulate.B, 0);
        var newTween = CreateTween();
        newTween.TweenProperty(this, "modulate:a", 1, 0.5f);
        await ToSignal(newTween, Tween.SignalName.Finished);
    }
}