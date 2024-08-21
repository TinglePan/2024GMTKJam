namespace ProjGMTK.Scripts;

public interface ITick
{
    public int LastTick { get; set; }
    public void Tick(int tick);
}