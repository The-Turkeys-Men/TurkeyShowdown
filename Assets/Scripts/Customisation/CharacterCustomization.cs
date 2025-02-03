using TMPro;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _previewRenderers;
    [SerializeField] private receivingJSON _receivingJSON;
    [SerializeField] private JSONSender _jsonSender;
    [SerializeField] private TextMeshProUGUI DebugText;
    public string NewColor = "default";
    
    public void OnColorButtonPressed(string colorName)
    {
        Color newColor = Color.clear;
        
        
        switch (colorName.ToLower())
        {
            case "red":
                newColor = new Color32(255, 0, 0, 255 / 4);
                NewColor = "red";
                break;
            case "orange":
                newColor = new Color32(255, 133, 27, 255 / 4);
                NewColor = "orange";
                break;
            case "yellow":
                newColor = new Color32(255, 220, 0, 255 / 4);
                NewColor = "yellow";
                break;
            case "green":
                newColor = new Color32(46, 204, 64, 255 / 4);
                NewColor = "green";
                break;
            case "blue":
                newColor = new Color32(0, 116, 217, 255 / 4);
                NewColor = "blue";
                break;
            case "purple":
                newColor = new Color32(177, 13, 201, 255 / 4);
                NewColor = "purple";
                break;
            default:
                newColor = Color.clear;
                NewColor = "default";
                break;
        }
        Previsualisation(newColor);
    }

    public async void SaveColor()
    {
        Debug.Log(NewColor);
        //Task<PlayerJSON> playerJson = _receivingJSON.FetchJSONValue();
        //await playerJson;
        PlayerJSON playerJson = new PlayerJSON()
        {
            Id = 2,
            Pseudo = "Wighthood",   // Replace with dynamical data
            HighScore = 0,        // Replace with dynamical data
            ScoreTable = new int[] {  }, // Replace with dynamical data
            NbrVictory = 0,    // Replace with dynamical data
            NbrDefeat = 0,     // Replace with dynamical data
            Skins = new Skin[] // Replace with dynamical data
            { },
            color = "default",
        };
        
        PlayerDataManager.Instance.playerData = playerJson;
        PlayerDataManager.Instance.playerData.color =NewColor;
        StartCoroutine(_jsonSender.SendJsonToServer(JsonUtility.ToJson(PlayerDataManager.Instance.playerData)));
    }
    
    private void Previsualisation(Color newColor)
    {
        foreach (var previewRenderer in _previewRenderers)
        {
            previewRenderer.color = newColor;
        }
    }
}