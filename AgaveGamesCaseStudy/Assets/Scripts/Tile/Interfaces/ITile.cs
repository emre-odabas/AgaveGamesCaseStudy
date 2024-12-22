using AgaveGames.Data;
using UnityEngine;

public interface ITile
{
    public TileType TileType { get; }
    public Vector2Int Coordinate { get; set; }
    public int Score { get; }
}
