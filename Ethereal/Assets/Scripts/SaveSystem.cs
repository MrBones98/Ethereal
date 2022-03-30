using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem
{
    public delegate void ProgressCleared();
    public static event ProgressCleared onProgressCleared;
    public bool[] CompletedLevels;
    public int LastSceneIndex;
    public int JumpCount;
    public bool[] AvailableJumps;
    //add stuff for abilities here, autosave will probably happen after
    //getting a new ability or smth
}
