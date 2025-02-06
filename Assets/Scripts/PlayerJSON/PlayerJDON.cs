[System.Serializable]
public class Skin
{
    public string Id;
    public bool Enabled;
}

[System.Serializable]
public class PlayerJSON
{
    public int Id;
    public string Pseudo;
    public int HighScore;
    public int[] ScoreTable;
    public int NbrVictory;
    public int NbrDefeat;
    public Skin[] Skins;
    public string color;
}
