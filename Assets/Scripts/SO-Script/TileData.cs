using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject
{
    public float walkSpeed;

    public TileBase[] tiles;
}
