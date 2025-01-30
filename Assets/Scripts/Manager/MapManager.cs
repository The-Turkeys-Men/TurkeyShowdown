using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    [SerializeField] private Tilemap _map;

    [SerializeField] List<TileData> _tileDatas;

    private Dictionary<TileBase, TileData> _dataFromTiles;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _dataFromTiles = new Dictionary<TileBase, TileData>();

        foreach (var tileData in _tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                _dataFromTiles.Add(tile, tileData);
            }
        }
    }

    public TileData GetTileData(Vector2 worldPosition)
    {
        Vector3Int gridPos = _map.WorldToCell(worldPosition);
        TileBase clickedTile = _map.GetTile(gridPos);

        if (!_dataFromTiles.ContainsKey(clickedTile))
        {
            return null;
        }

        TileData tileData = _dataFromTiles[clickedTile];
        float walkingSpeed = tileData.walkSpeed;

        print("Walking speed on " + clickedTile + " is " + walkingSpeed);
        return tileData;
    }

    //public float GetTileWalkingSpeed(Vector2 worldPos)
    //{
    //    Vector3Int gridPos = _map.WorldToCell(worldPos);

    //    TileBase tile = _map.GetTile(gridPos);

    //    if (tile == null)
    //        return 1f;

    //    float walkingSpeed = _dataFromTiles[tile].walkSpeed;
    //    return walkingSpeed;
    //}
}
