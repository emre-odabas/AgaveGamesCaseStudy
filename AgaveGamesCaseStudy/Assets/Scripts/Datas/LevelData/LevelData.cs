using System.Collections.Generic;
using UnityEngine;
using AgaveGames.Tiles;

namespace AgaveGames.Data
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Datas/Level Data", order = 1)]
    public class LevelData : ScriptableObject
    { 
        #region VARIABLES
        
        //Parameters
        public Vector2Int m_GridSize = new Vector2Int(5, 5);
        public int m_MaxMovement = 10;
        public int m_TargetGoal = 1000;
        [Space]
        public List<TileProperty> m_TilePropertyList;
        public List<TileObstacleProperty> m_TileObstaclePropertyList;
        
        #endregion
        
        #region RETURN METHODS
        
        public int GetIndex(int row, int col)
        {
            return  ((row * (m_GridSize.y)) + col);
        }
    
        #endregion
    }
    
    [System.Serializable]
    public class TileProperty
    {
        public Tile m_TilePrefab;
        [Range(0f, 1f)] public float m_SpawnChance;
    }
    
    [System.Serializable]
    public class TileObstacleProperty
    {
        public Tile m_TileObstaclePrefab;
        public Vector2Int m_Coordinate;
    }
}


