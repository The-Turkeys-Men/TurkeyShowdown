using Unity.Netcode;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    public ColorChanger ColorChanger;

    public void OnColorButtonPressed(string colorName)
    {
        Color newColor = Color.white;

        switch (colorName.ToLower())
        {
            case "red":
                newColor = new Color32(255, 0, 0, 255 / 2);
                break;
            case "orange":
                newColor = new Color32(255, 133, 27, 255 / 2);
                break;
            case "yellow":
                newColor = new Color32(255, 220, 0, 255 / 2);
                break;
            case "green":
                newColor = new Color32(46, 204, 64, 255 / 2);
                break;
            case "blue":
                newColor = new Color32(0, 116, 217, 255 / 2);
                break;
            case "purple":
                newColor = new Color32(177, 13, 201, 255 / 2);
                break;
            default:
                newColor = Color.clear;
                break;
        }
    }
}