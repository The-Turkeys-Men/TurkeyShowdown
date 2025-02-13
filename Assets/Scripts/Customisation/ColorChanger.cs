using System.Threading.Tasks;
using AYellowpaper.SerializedCollections;
using Unity.Netcode;
using UnityEngine;
using SpriteRenderer = UnityEngine.SpriteRenderer;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] private Transform _spriteParentBody;
    [SerializeField] private Transform _spriteParentArmFeather;
    [SerializeField] private SerializedDictionary<string, Sprite> _sprites = new();

    private async void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer && IsOwner)
        {
            await OnStart();
        }
    }

    private async Task OnStart()
    {
        await PlayerDataManager.Datainstance.ReceivingJSON(OwnerClientId);
    }

    public void ChangeSprite(string spriteName)
    {  
        SpriteRenderer bodySpriteRenderer = _spriteParentBody.GetComponent<SpriteRenderer>();
        SpriteRenderer armFeatherSpriteRenderer = _spriteParentArmFeather.GetComponent<SpriteRenderer>();

        bodySpriteRenderer.sprite = _sprites[spriteName];
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void ChangeColorClientRpc(string color)
    {
        ChangeSprite(color);
    }
} 
