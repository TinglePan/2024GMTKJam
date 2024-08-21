using System;
using System.Collections.Generic;
using Godot;

namespace ProjGMTK.Scripts;

public class Creature: ITick
{
    public enum TypeEnum
    {
        Carp,
        Snake,
        Loong,
    }

    public TypeEnum Type;
    public int Index;
    public int Level;
    public Building Building;
    public int BaseScaleProduction;
    public int ScaleProduction;

    public int CarpLifeTicks;
    public int FeverTicks;
    public int LevelUpTicks;
    public int AscendCount;
    public int LastTick { get; set; }
    
    public Action OnDeath;
    
    public List<RandEvent> RandEvents;
    
    public Creature(TypeEnum type, int index, int level, Building building)
    {
        Type = type;
        Index = index;
        Level = level;
        Building = building;
        BaseScaleProduction = GetBaseScaleProduction();
        ScaleProduction = GetScaleProduction();
        FeverTicks = 0;
        CarpLifeTicks = GetCarpLifeTicks();
        RandEvents = new List<RandEvent>()
        {
            new RandEvent("Fever", 60, 3600, Fever),
            new RandEvent("Ascend", 60, 3600, AscendEvent)
        };
    }
    
    public void Tick(int tick)
    {
        var gameState = GameMgr.Instance.GameState;
        gameState.SetScaleCount(gameState.ScaleCount + ScaleProduction);
        LevelUpTicks--;
        if (LevelUpTicks <= 0)
        {
            LevelUp();
        }
        if (FeverTicks > 0)
        {
            FeverTicks--;
        }
        if (CarpLifeTicks > 0)
        {
            CarpLifeTicks--;
        }
        if (CarpLifeTicks <= 0)
        {
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{DisplayName()} has reached the end of its lifespan.");
            Die();
            var getScales = GameMgr.Instance.Rand.Next(10 * ScaleProduction, 100 * ScaleProduction);
            gameState.SetScaleCount(gameState.ScaleCount + getScales);
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"You scaled its corpse for {getScales} scales.");
        }
        LastTick = tick;
        
    }

    public string DisplayName()
    {
        return $"{Type} #{Index + 1} @[{Building.DisplayName()}]";
    }
    
    public void UpdateScaleProduction()
    {
        BaseScaleProduction = GetBaseScaleProduction();
        ScaleProduction = GetScaleProduction();
    }
    public bool CheckAscend()
    {
        void CarpAscend()
        {
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
                $"{DisplayName()} has reached the arch. You saw lightnings molting, burning that carp from its tail. Then a loong showed up from withing that burning glowing fish.");
        }
        
        void SnakeAscend()
        {
            AscendCount++;
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
                $"{DisplayName()} absorbed the lightnings and ascended to a new phase({AscendCount}).");
            if (AscendCount > 7)
            {
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
                    $"{DisplayName()} has accumulated enough power to evolve into a loong.");
                Evolve();
            }
        }

        void SnakeAscendFail()
        {
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
                $"{DisplayName()} failed to endure the thunderbolts during its ascension. There's nothing left of it.");
            Die();
        }
        
        RandCheck check;
        bool res = false;
        switch (Type)
        {
            case TypeEnum.Carp:
                check = new RandCheck("Ascend", GetAscendChance, CarpAscend);
                res = check.Check(GameMgr.Instance.Rand);
                break;
            case TypeEnum.Snake:
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
                    $"Dark clouds enshrouded. Snakes drain power from being struck by lightnings. This is a trial.");
                check = new RandCheck("Ascend", GetAscendChance, SnakeAscend, SnakeAscendFail);
                res = check.Check(GameMgr.Instance.Rand);
                break;
        }
        return res;
    }

    public void Evolve()
    {
        if (!GameMgr.Instance.GameState.KnowLoong)
        {
            GameMgr.Instance.GameState.SetKnowLoong();
        }
        GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{DisplayName()} has evolved into a loong.");
        Type = TypeEnum.Loong;
        Level = 1;
        UpdateScaleProduction();
    }

    public void Die()
    {
        GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue(
            $"{DisplayName()} died.");
        OnDeath?.Invoke();
    }
    
    private void LevelUp()
    {
        Level++;
        
        var gameState = GameMgr.Instance.GameState;
        switch (Type)
        {
            case TypeEnum.Carp:
                var getLifeTicks = 60 * (Level + Building.Level);
                
                CarpLifeTicks += getLifeTicks;
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{DisplayName()} extended its lifespan by {getLifeTicks} to {CarpLifeTicks} ticks when it levels up.");
                break;
            case TypeEnum.Snake:
                CheckAscend();
                break;
            case TypeEnum.Loong:
                var getInvertedScales = GameMgr.Instance.Rand.Next(1, Level);
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{DisplayName()} dropped {getInvertedScales} inverted scales when it levels up. These scales are precious ones growing under a loong's neck and contain mysterious power.");
                gameState.SetInvertedScaleCount(gameState.InvertedScaleCount + getInvertedScales);
                break;
        }
        
        
        LevelUpTicks = GetLevelUpTicks();
        GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{DisplayName()} leveled up to {Level}");
        UpdateScaleProduction();
    }
    
    private void Fever(RandEvent ev)
    {
        FeverTicks = 60 * (Level + Building.Level);
    }
    
    private void AscendEvent(RandEvent ev)
    {
        CheckAscend();
    }
    
    private int GetAscendChance()
    {
        switch (Type)
        {
            case TypeEnum.Carp:
                return (int)(5000 * Mathf.Exp(-(1f / Building.Level) * (Level - 5) * (Level - 5)));
            case TypeEnum.Snake:
                return (int)(10000 / (1 + Mathf.Exp(Level - Building.Level)));
        }
        return 0;
    }
    
    private int GetCarpLifeTicks()
    {
        return 180 * (Level + Building.Level);
    }

    private int GetBaseScaleProduction()
    {
        switch (Type)
        {
            case TypeEnum.Carp:
                return 3;
            case TypeEnum.Snake:
                return 15;
            case TypeEnum.Loong:
                return 50;
        }
        return 0;
    }
    
    private int GetScaleProduction()
    {
        var scaleProduction = BaseScaleProduction * Level * Building.Level;
        if (FeverTicks > 0)
        {
            scaleProduction *= Level + Building.Level;
        }
        return scaleProduction;
    }
    
    private int GetLevelUpTicks()
    {
        switch (Type)
        {
            case TypeEnum.Carp:
                return 20 * Level;
            case TypeEnum.Snake:
                return 50 * Level;
            case TypeEnum.Loong:
                return 100 * Level;
        }
        return 0;
    }
}