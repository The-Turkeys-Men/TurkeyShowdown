using System;
using UnityEngine.Serialization;

[System.Serializable]
public class PlayerJSON
{
    public int Id;
    public string Pseudo;
    public int HighScore;
    public int[] ScoreTable;
    public int NbrVictory;
    public int NbrDefeat;
    public string[] Skins;
    public string Color;
}