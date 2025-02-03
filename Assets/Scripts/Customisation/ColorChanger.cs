using Unity.Netcode;
using UnityEngine;

public class ColorChanger : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;
    
    private void Start()
    {
        foreach (var spriteRenderer in spriteRenderers)
        {
            
        }
    }
}
