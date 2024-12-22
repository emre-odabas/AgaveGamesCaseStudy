using AgaveGames.Data;
using UnityEngine;

public interface ITile
{
    public TileType TileType { get; } //Green, Blue, Red, Yellow
    public Vector2Int Coordinate { get; set; } //Row, Column
    public int Score { get; } //Score of the tile
}
