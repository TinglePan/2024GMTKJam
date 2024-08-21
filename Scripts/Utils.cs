using System;
using System.Threading.Tasks;
using Godot;

namespace ProjGMTK.Scripts;

public static class Utils
{
    public static string PrettyPrintMSec(ulong mSec)
    {
        var hour = mSec / 3600000;
        var min = (mSec % 3600000) / 60000;
        var sec = (mSec % 60000) / 1000;
        return $"{hour}:{min}:{sec}";
    }
    
    public static async Task WaitSignal(SignalAwaiter signalAwaiter)
    {
        await signalAwaiter;
    }

    public static bool CheckRand(int p, Random rand)
    {
        return rand.Next(10000) < p;
    }
}