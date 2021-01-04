using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : SingletonBase<MapManager>
{
    //get the grid first
    private Grid m_GameGrid;
    private Dictionary<Vector2Int, bool> m_Map = new Dictionary<Vector2Int, bool>(); //store the map

    [Header("Tilemaps")]
    public Tilemap m_WallTileMap;
    public Tilemap m_TrapTileMap;
    
    void Awake()
    {
        m_GameGrid = GetComponent<Grid>();

        m_WallTileMap.CompressBounds();
        BoundsInt bounds = m_WallTileMap.cellBounds;

        Vector2Int startingGridOffset = (Vector2Int)bounds.position;

        //store the walls in a dictionary 
        TileBase[] allWallTiles = m_WallTileMap.GetTilesBlock(bounds);
        for (int y = 0; y < bounds.size.y; ++y)
        {
            for (int x = 0; x < bounds.size.x; ++x)
            {
                //if tile exists store
                if (allWallTiles[x + y * bounds.size.x] != null)
                    m_Map.Add(new Vector2Int(x + startingGridOffset.x, y + startingGridOffset.y), true);
            }
        }
    }

    public Vector2Int GetWorldToTilePos(Vector2 worldPos)
    {
        if (m_GameGrid == null)
            return Vector2Int.zero;

        return (Vector2Int)(m_GameGrid.WorldToCell((Vector3)worldPos));
    }

    public Vector2 GetTileToWorldPos(Vector2Int tilePos)
    {
        if (m_GameGrid == null)
            return Vector2.zero;

        return (Vector2)(m_GameGrid.CellToWorld((Vector3Int)tilePos));
    }

    public bool IsThereTileOnMap(Vector2Int pos)
    {
        return m_Map.ContainsKey(pos);
    }
}
