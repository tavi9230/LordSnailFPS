using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class MapManager
{
    public SquareGrid SquareGrid;
    public List<List<MapItem>> Map;
    public List<MapItem> MapItems;
    public Vector3 DeadSpot = new Vector3(10, 6);

    public void Setup(Grid map)
    {
        MapItems = new List<MapItem>();
        Map = new List<List<MapItem>>();
        
        var tilemaps = map.GetComponentsInChildren<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            if (tilemap.name == "Ground")
            {
                SquareGrid = new SquareGrid(0, 0, Mathf.Abs(tilemap.cellBounds.yMin) + Mathf.Abs(tilemap.cellBounds.yMax), Mathf.Abs(tilemap.cellBounds.xMin) + Mathf.Abs(tilemap.cellBounds.xMax));
                SetupGroundGrid(tilemap);
            }
            else if (tilemap.name == "SolidObjects")
            {
                SetupObjectGrid(tilemap);
            }
        }

        // Adding map objects
        var gameObjects = GameObject.Find("Objects");
        foreach (Transform child in gameObjects.transform)
        {
            ToggleObjectOnMap(child.transform.GetChild(0).transform.position, TileType.HideableObject);
        }
    }

    public void ToggleObjectOnMap(Vector3 worldPosition, TileType tileType)
    {
        var positionInGrid = GetGridPosition(worldPosition);
        var x = (int)positionInGrid.y;
        var y = (int)positionInGrid.x;
        Map[y][x].TileType = tileType;
        SquareGrid.tiles[y, x] = Map[y][x].TileType;
        var mi = MapItems.Find(m => m.GridLocation == Map[y][x].GridLocation);
        MapItems[MapItems.IndexOf(mi)].TileType = Map[y][x].TileType;
    }

    public List<Location> FindPath(Vector3 startPos, Vector3 endPos)
    {
        var pathfinder = new AStarSearch();
        pathfinder.Initialize(SquareGrid, new Location(startPos), new Location(endPos));
        var paths = pathfinder.FindPath();
        return paths;
    }

    public Vector3 GetGridPosition(Vector3 position)
    {
        var x = Mathf.RoundToInt(position.x - .5f);
        var y = Mathf.RoundToInt(position.y + .5f);
        var xPos = x + 0.5f;
        var yPos = y - 0.5f;
        var mapItem = MapItems.Find(mi => mi.Location == new Vector3(xPos, yPos, position.z));
        return new Vector3(mapItem.GridLocation.y, mapItem.GridLocation.x);
    }

    public Vector3 GetWorldPosition(Vector3 position)
    {
        var x = Mathf.RoundToInt(position.x);
        var y = Mathf.RoundToInt(position.y);
        var mapItem = MapItems.Find(mi => mi.GridLocation == new Vector3(y, x, position.z));
        return new Vector3(mapItem.Location.x, mapItem.Location.y, 0f);
    }

    private void SetupGroundGrid(Tilemap tileMap)
    {
        for (int p = tileMap.cellBounds.yMax - 1, j = 0; p >= tileMap.cellBounds.yMin; p--, j++)
        {
            Map.Add(new List<MapItem>());
            for (int n = tileMap.cellBounds.xMin, i = 0; n < tileMap.cellBounds.xMax; n++, i++)
            {
                Vector3Int worldPlace = new Vector3Int(n, p, (int)tileMap.transform.position.y);
                Vector3 worldPlaceOffset = new Vector3(worldPlace.x + 0.5f, worldPlace.y + 0.5f, worldPlace.z);
                //Vector3 worldPlaceOffset = new Vector3(worldPlace.x, worldPlace.y, worldPlace.z);
                var mapItem = new MapItem(worldPlaceOffset, new Vector3(i, j, 0));

                if (tileMap.HasTile(worldPlace))
                {
                    mapItem.TileType = TileType.Floor;
                    Map[j].Add(mapItem);
                    SquareGrid.tiles[j, i] = mapItem.TileType;
                    MapItems.Add(mapItem);
                }
                else
                {
                    mapItem.TileType = TileType.Empty;
                    Map[j].Add(mapItem);
                    SquareGrid.tiles[j, i] = mapItem.TileType;
                    MapItems.Add(mapItem);
                }
            }
        }
    }

    private void SetupObjectGrid(Tilemap tileMap)
    {
        for (int p = tileMap.cellBounds.yMax - 1, j = 0; p >= tileMap.cellBounds.yMin; p--, j++)
        {
            for (int n = tileMap.cellBounds.xMin, i = 0; n < tileMap.cellBounds.xMax; n++, i++)
            {
                Vector3Int worldPlace = new Vector3Int(n, p, (int)tileMap.transform.position.y);
                Vector3 worldPlaceOffset = new Vector3(worldPlace.x + 0.5f, worldPlace.y + 0.5f, worldPlace.z);
                //Vector3 worldPlaceOffset = new Vector3(worldPlace.x, worldPlace.y, worldPlace.z);
                var mapItem = new MapItem(worldPlaceOffset, new Vector3(i, j, 0));

                if (tileMap.HasTile(worldPlace))
                {
                    Map[j][i].TileType = TileType.Wall;
                    SquareGrid.tiles[j, i] = Map[j][i].TileType;
                    MapItems[i + j].TileType = Map[j][i].TileType;
                }
            }
        }
    }
}

public class MapItem
{
    public Vector3 Location { get; set; }
    public Vector3 GridLocation { get; set; }
    public TileType TileType { get; set; }

    public MapItem(Vector3 location, Vector3 gridLocation)
    {
        Location = location;
        GridLocation = gridLocation;
    }
}