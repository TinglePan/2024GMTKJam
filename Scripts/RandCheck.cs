using System;

namespace ProjGMTK.Scripts;

public class RandCheck
{
    public string Name;
    public int Chance;
    public Func<int> ChanceFunc;
    public Action SuccessAction;
    public Action FailAction;
    
    public RandCheck(string name, int chance, Action successAction=null, Action failAction=null)
    {
        Name = name;
        Chance = chance;
        SuccessAction = successAction;
        FailAction = failAction;
    }
    
    public RandCheck(string name, Func<int> chanceFunc, Action successAction=null, Action failAction=null)
    {
        Name = name;
        ChanceFunc = chanceFunc;
        SuccessAction = successAction;
        FailAction = failAction;
    }
    
    public bool Check(Random rand)
    {
        var chance = ChanceFunc?.Invoke() ?? Chance;
        if (Utils.CheckRand(chance, rand))
        {
            SuccessAction?.Invoke();
            return true;
        } else 
        {
            FailAction?.Invoke();
            return false;
        }
    }
}