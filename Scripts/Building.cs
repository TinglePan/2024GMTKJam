using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace ProjGMTK.Scripts;

public partial class Building: ITick
{
    public enum TypeEnum
    {
        Pond,
        Cave
    }

    public TypeEnum Type;
    public int Index;
    public int Level;
    public int UpgradePrice;
    public Dictionary<Creature.TypeEnum, List<Creature>> Creatures;
    
    public Action OnUpgrade;
    public Action<Creature> OnAddCreature;
    public Action<Creature> OnRemoveCreature;
    
    public List<RandEvent> RandEvents;
    
    public int LastTick { get; set; }
    
    public Building(TypeEnum type, int index, int level)
    {
        Type = type;
        Index = index;
        Level = level;
        UpgradePrice = GetUpgradePrice();
        Creatures = new Dictionary<Creature.TypeEnum, List<Creature>>();
        RandEvents = new List<RandEvent>()
        {
            new RandEvent("Spawn", 10, 300, SpawnCheck),
            new RandEvent("LuckScales", 60, 600, LuckScales),
        };
        if (Type == TypeEnum.Pond)
        {
            RandEvents.Add(new RandEvent("Pond", 30, 600, CarpAscend));
        }
    }
    
    public void AddCreature(Creature creature)
    {
        if (!Creatures.ContainsKey(creature.Type))
        {
            Creatures[creature.Type] = new List<Creature>();
        }
        Creatures[creature.Type].Add(creature);
        OnAddCreature?.Invoke(creature);
    }
    
    public void RemoveCreature(Creature creature)
    {
        if (!Creatures.ContainsKey(creature.Type))
        {
            return;
        }
        Creatures[creature.Type].Remove(creature);
        for (int i = creature.Index; i < Creatures[creature.Type].Count; i++)
        {
            Creatures[creature.Type][i].Index = i;
        }
        OnRemoveCreature?.Invoke(creature);
    }
    
    public void Tick(int tick)
    {
        foreach (var creatures in Creatures.Values)
        {
            foreach (var creature in creatures.ToList())
            {
                creature.Tick(tick);
            }
        }
        foreach (var randEvent in RandEvents)
        {
            randEvent.Tick(tick);
        }
        LastTick = tick;
    }

    public int CreatureCount(Creature.TypeEnum type)
    {
        if (!Creatures.ContainsKey(type) || Creatures[type] == null)
        {
            return 0;
        }
        return Creatures[type].Count;
    }

    public void Upgrade()
    {
        var state = GameMgr.Instance.GameState;
        state.SetScaleCount(state.ScaleCount - UpgradePrice);
        Level++;
        UpdateUpgradePrice();
        OnUpgrade?.Invoke();
    }
    
    public void UpdateUpgradePrice()
    {
        UpgradePrice = GetUpgradePrice();
    }

    public string DisplayName()
    {
        return $"{Type} #{Index + 1}";
    }

    private int GetUpgradePrice()
    {
        switch (Type)
        {
            case TypeEnum.Pond:
                return 10 + (int)(5 * Mathf.Pow(1.5f, Level));
            case TypeEnum.Cave:
                return 50 + (int)(20 * Mathf.Pow(1.8f, Level));
        }
        return 0;
    }

    private void SpawnCheck(RandEvent ev)
    {
        var creatureCount = Creatures.SelectMany(x => x.Value).Count();
        if (creatureCount < Level)
        {
            var creature = Type switch
            {
                TypeEnum.Pond => new Creature(Creature.TypeEnum.Carp, creatureCount, 1, this),
                TypeEnum.Cave => new Creature(Creature.TypeEnum.Snake, creatureCount, 1, this),
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!GameMgr.Instance.GameState.KnowCarp && creature.Type == Creature.TypeEnum.Carp)
            {
                GameMgr.Instance.GameState.SetKnowCarp();
            }
            if (!GameMgr.Instance.GameState.KnowSnake && creature.Type == Creature.TypeEnum.Snake)
            {
                GameMgr.Instance.GameState.SetKnowSnake();
            }
            AddCreature(creature);
            GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"{creature.DisplayName()} just spawned in {DisplayName()}.");
        }
    }

    private void LuckScales(RandEvent ev)
    {
        int getScales = 0;
        var gameState = GameMgr.Instance.GameState;
        switch (Type)
        {
            case TypeEnum.Pond:
                getScales = (int)(10 * Mathf.Pow(2, Level));
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"You found {getScales} scales at the bottom of {DisplayName()}.");
                break;
            case TypeEnum.Cave:
                getScales = (int)(40 * Mathf.Pow(2.5, Level));
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"You found {getScales} scales in the depth of {DisplayName()}.");
                break;
        }
        gameState.SetScaleCount(gameState.ScaleCount + getScales);
    }
    
    private void CarpAscend(RandEvent ev)
    {
        if (!Creatures.ContainsKey(Creature.TypeEnum.Carp) || Creatures[Creature.TypeEnum.Carp].Count <= 0)
        {
            return;
        }
        GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"You see rainbows high above {DisplayName()}, shaped like an arch. Carps are attempting to reach that place.");
        foreach (var carp in Creatures[Creature.TypeEnum.Carp])
        {
            if (carp.CheckAscend())
            {
                GameMgr.Instance.GameTabs.MainTab.Log.AddDialogue($"Upon the carp reached the arch, it fades away. Only the most competitive carp may ascend.");
                break;
            };
        }
    }
}