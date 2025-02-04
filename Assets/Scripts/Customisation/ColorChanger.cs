using Unity.Netcode;
using UnityEngine;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        PlayerDataManager.Datainstance.receivingJSON();
        PlayerDataManager.Datainstance.AddColorData(OwnerClientId,PlayerDataManager.Datainstance.playerData.color);
    }

    public void ChangeColor(string color)
    {
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
                _ => Color.clear
            };
        }
    }
} 
