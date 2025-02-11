using System;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerJSON
{
    public int id;
    public string color;
    public string[] skins;
    public string pseudo;
    public int highScore;
    public int[] scoreTable;
    public int nbrVictory;
    public int nbrDefeat;
}