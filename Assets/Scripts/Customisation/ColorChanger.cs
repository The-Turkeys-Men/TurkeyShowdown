using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;


    private async void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            await OnStart();
        }
    }

    private async Task OnStart()
    {
        await PlayerDataManager.Datainstance.ReceivingJSON(OwnerClientId);
    }

    public void ChangeColor(string color)
    {
        Debug.Log("Changing color to: " + color);
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color.ToLower() switch
            {
                "red" => new Color32(255, 0, 0, 255 / 4),
                "orange" => new Color32(255, 133, 27, 255 / 4),
                "yellow" => new Color32(255, 220, 0, 255 / 4),
                "green" => new Color32(46, 204, 64, 255 / 4),
                "blue" => new Color32(0, 116, 217, 255 / 4),
                "purple" => new Color32(177, 13, 201, 255 / 4),
                "black" => new Color32(0, 0, 0, 255 / 4),
                "white" => new Color32(255, 255, 255, 255 / 4),
                "pink" => new Color32(255, 192, 203, 255 / 4),
                _ => Color.clear
            };
        }
    }
} 
