using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AgaveGames.Data;
using AgaveGames.Tiles;
using AgaveGames.Utilities;
using UnityEngine.Events;

namespace AgaveGames.Managers
{
    public class GameManager : SingletonComponent<GameManager>
    {
        #region ACTIONS
        
        public UnityAction OnInit;
        public UnityAction<int> OnMove;
        public UnityAction<int> OnScore;
        public UnityAction<int> OnFinishSuccess;
        public UnityAction<int> OnFinishFail;

        #endregion

        #region VARIABLES

        //Parameters
        [Header("Parameters")]
        [SerializeField] private int m_MinToConnect = 3;
        [SerializeField] private LevelData m_LevelData;
        [Space]
        [SerializeField] private float m_BoardSpace = 0.25f;
        [SerializeField] private float m_StartTilePosition;

        //Components
        [Header("Components")]
        [SerializeField] private Transform m_TileBGParent;
        [SerializeField] private Transform m_TilesParent;
        [SerializeField] private GameObject m_TileBGPrefab;

        //Privates
        private int _score;
        private BoardItemProperty[,] _board;
        private bool _canAdd = false;
        private LinkedList<Tile> _order = new LinkedList<Tile>();
        private int _remainingMove;

        //Properties
        public int MinToConnect => m_MinToConnect;
        public LevelData LevelData => m_LevelData;
        public bool IsFinish { get; private set; }
        
        #endregion

        #region MONOBEHAVIOUR

        private void Awake()
        {
            _board = new BoardItemProperty[m_LevelData.m_GridSize.x, m_LevelData.m_GridSize.y];
            _remainingMove = m_LevelData.m_MaxMovement;
            CreateBoard();
            OnInit?.Invoke();
        }
        
        private void Update()
        {
            CheckSwipeMovement();
        }
        
        #endregion
        
        #region RETURN METHODS
        
        public int GetScore()
        {
            return _score;
        }
        
        /// <summary>
        /// Determines if a given cell in the grid is an obstacle.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>True if the cell is an obstacle; otherwise, false.</returns>
        private bool IsObstacle(int row, int col)
        {
            return m_LevelData.m_TileObstaclePropertyList.Any(c=>c.m_Coordinate.x == row && c.m_Coordinate.y == col);
        }
        
        /// <summary>
        /// Returns the playable tile prefab for a given cell in the grid.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>The playable tile prefab for the given cell.</returns>
        private Tile GetPlayableTilePrefab(int row, int col)
        {
            //Random playable tile
            float chance = 0;
            int index = Random.Range(0, m_LevelData.m_TilePropertyList.Count);
            for (int i = 0; i < m_LevelData.m_TilePropertyList.Count; i++)
            {
                int odds = Random.Range(0, 100);
                if (odds < ((m_LevelData.m_TilePropertyList[i].m_SpawnChance + chance) * 10))
                {
                    index = i;
                    break;
                }
                else
                {
                    chance += m_LevelData.m_TilePropertyList[i].m_SpawnChance;
                }
            }

            return m_LevelData.m_TilePropertyList[index].m_TilePrefab;
        }
        
        /// <summary>
        /// Returns the obstacle tile prefab for a given cell in the grid.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>The obstacle tile prefab for the given cell.</returns>
        private Tile GetObstacleTilePrefab(int row, int col)
        {
            TileObstacleProperty obstacleProperty = m_LevelData.m_TileObstaclePropertyList.FirstOrDefault(c=>c.m_Coordinate.x == row && c.m_Coordinate.y == col);
            if (obstacleProperty == null) return null;
            
            return obstacleProperty.m_TileObstaclePrefab;
        }

        #endregion

        #region METHODS
        
        /// <summary>
        /// Checks if the player has made a swipe movement.
        /// </summary>
        private void CheckSwipeMovement()
        {
            if (_remainingMove <= 0) return;
            
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                if (_order.Count >= m_MinToConnect)
                {
                    _remainingMove--;
                    OnMove?.Invoke(_remainingMove);
                    
                    //Check Finish
                    if (_remainingMove <= 0)
                    {
                        if (_score < m_LevelData.m_TargetGoal)
                            Finish(false);
                        else
                            Finish(true);
                    }
                    
                    StartCoroutine(MergeTiles());
                }
                else
                {
                    ClearOrder();
                }
            }
        }
        
        /// <summary>
        /// Restarts the game.
        /// </summary>
        public void Replay()
        {
            //clear board
            m_TileBGPrefab.RecycleAll();
            for (int row = 0; row < m_LevelData.m_GridSize.x; row++)
            {
                for (int col = 0; col < m_LevelData.m_GridSize.y; col++)
                {
                    StartCoroutine(_board[row, col].m_Tile.Break());
                }
            }

            _score = 0;
            _remainingMove = m_LevelData.m_MaxMovement;
            _board = new BoardItemProperty[m_LevelData.m_GridSize.x, m_LevelData.m_GridSize.y];
            CreateBoard();
            OnInit?.Invoke();
        }
        
        /// <summary>
        /// Creates the game board based on the level data.
        /// </summary>
        private void CreateBoard()
        {
            _canAdd = false;
            for (int row = 0; row < m_LevelData.m_GridSize.x; row++)
            {
                for (int col = 0; col < m_LevelData.m_GridSize.y; col++)
                {
                    //BG Grid
                    Vector2 pos = new Vector2(col + (m_BoardSpace * col), row + (m_BoardSpace * row));
                    GameObject gridBG = ObjectPooler.Spawn(m_TileBGPrefab, m_TileBGParent, pos);
                    
                    //Tile
                    if (IsObstacle(row, col))
                        SpawnObstacleTile(row, col);
                    else
                        SpawnPlayableTile(row, col);
                }
            }
            _canAdd = true;
            IsFinish = false;
        }
        
        /// <summary>
        /// Spawns a playable tile at the given position in the grid.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        private void SpawnPlayableTile(int row, int col)
        {
            Vector2 goalPos = new Vector2(col + (m_BoardSpace * col), row + (m_BoardSpace * row));
            Vector2 startPos = new Vector2(col, m_StartTilePosition);

            Tile tilePrefab = GetPlayableTilePrefab(row, col);
            if (tilePrefab == null)
            {
                Debug.LogError("[GameManager] Tile prefab not found");
                return;
            }
            
            Tile tile = ObjectPooler.Spawn(tilePrefab, m_TilesParent, startPos);
            _board[row, col] = new BoardItemProperty(GridInfo.Playable, tile, goalPos);
            tile.Init(row, col, startPos, goalPos);
        }
        
        /// <summary>
        /// Spawns an obstacle tile at the given position in the grid.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        private void SpawnObstacleTile(int row, int col)
        {
            Vector2 goalPos = new Vector2(col + (m_BoardSpace * col), row + (m_BoardSpace * row));
            Vector2 startPos = new Vector2(col, m_StartTilePosition);

            Tile tilePrefab = GetObstacleTilePrefab(row, col);
            if (tilePrefab == null)
            {
                Debug.LogError("[GameManager] Obstacle tile prefab not found");
                return;
            }
            
            Tile tile = ObjectPooler.Spawn(tilePrefab, m_TilesParent, startPos);
            _board[row, col] = new BoardItemProperty(GridInfo.Obstacle, tile, goalPos);
            tile.Init(row, col, startPos, goalPos);
        }
        
        /// <summary>
        /// Adds the given score to the current score.
        /// </summary>
        /// <param name="valueToAdd">The score to add.</param>
        public void AddScore(int valueToAdd)
        {
            _score += valueToAdd;
            OnScore?.Invoke(_score);
            
            //Check Finish
            if (_score >= m_LevelData.m_TargetGoal)
                Finish(true);
        }
        
        /// <summary>
        /// Adds the given tile to the stack.
        /// </summary>
        /// <param name="tile">The tile to add.</param>
        public void AddToStack(Tile tile)
        {
            if (!_canAdd)
                return;
            
            if (_order.Count == 0)
            {
                _order.AddLast(tile);
            }
            else
            {
                bool isAdjacent = (tile.Coordinate.x <= (_order.Last.Value.Coordinate.x + 1) && 
                                   tile.Coordinate.x >= (_order.Last.Value.Coordinate.x - 1)) &&
                                    (tile.Coordinate.y <= (_order.Last.Value.Coordinate.y + 1) && 
                                     tile.Coordinate.y >= (_order.Last.Value.Coordinate.y - 1));
                
                if (_order.Last.Value.TileType == tile.TileType && !_order.Contains(tile) && isAdjacent)
                {
                    Tile prev = _order.Last.Value;
                    int y = tile.Coordinate.x - prev.Coordinate.x;
                    int x = tile.Coordinate.y - prev.Coordinate.y;

                    prev.SetLine(x, y);

                    //Insert
                    _order.AddLast(tile);
                }
                else if (_order.Last.Previous != null && tile == _order.Last.Previous.Value)
                {
                    //reset lines
                    _order.Last.Value.ResetLine();
                    _order.Last.Previous.Value.ResetLine();
                    _order.RemoveLast();
                }
            }
        }

        /// Finishes the game.
        /// </summary>
        private void Finish(bool isSuccess)
        {
            IsFinish = true;

            if (isSuccess)
                OnFinishSuccess?.Invoke(_score);
            else
                OnFinishFail?.Invoke(_score);
        }
        
        /// <summary>
        /// Merges the tiles in the stack.
        /// </summary>
        private IEnumerator MergeTiles()
        {
            _canAdd = false;
            int count = _order.Count;
            int index = count;
            
            for (int i = 0; i < count; i++, index--)
            {
                Tile tile = _order.First.Value;
                _order.RemoveFirst();
                tile.OnMerge(i);
                _board[tile.Coordinate.x, tile.Coordinate.y].m_Tile = null;
                yield return new WaitForSeconds(0.1f);
            }
            _canAdd = true;

            StartCoroutine(Collapse());
        }

        /// <summary>
        /// Collapses the tiles in the board.
        /// </summary>
        private IEnumerator Collapse()
        {
            for (int row = 0; row < m_LevelData.m_GridSize.x; row++)
            {
                for (int col = 0; col < m_LevelData.m_GridSize.y; col++)
                {
                    if (_board[row, col] == null)// || board[row, col].info != GridInfo.Playable)
                        continue;
                    
                    if (_board[row, col].m_Tile == null)
                    {
                        int current = row;
                        do
                        {
                            int r = current + 1;
                            if (r < m_LevelData.m_GridSize.x)
                            {
                                if (_board[r, col].m_Tile != null)
                                {
                                    _board[row, col].m_Tile = _board[r, col].m_Tile;
                                    _board[row, col].m_Tile.SetRowCol(row, col);
                                    _board[row, col].m_Tile.SlideDown(_board[row, col].m_Tile.transform.localPosition, _board[row, col].m_Coordinate);
                                    _board[r, col].m_Tile = null;
                                    break;
                                }
                                else
                                {
                                    current++;
                                }
                            }
                            else
                                break;
                        } while (_board[row, col].m_Tile == null);
                    }

                }
            }

            yield return new WaitForSeconds(0.125f);
            
            for (int row = 0; row < m_LevelData.m_GridSize.x; row++)
            {
                for (int col = 0; col < m_LevelData.m_GridSize.y; col++)
                {
                    if (_board[row, col].m_Tile == null)
                    {
                        SpawnPlayableTile(row, col);
                    }
                }
            }

            yield return null;
        }

        /// <summary>
        /// Clears the order.
        /// </summary>
        private void ClearOrder()
        {
            _canAdd = false;
            int count = _order.Count;
            // int index = count;

            for (int i = 0; i < count; i++)
            {
                Tile tile = _order.Last.Value;
                _order.RemoveLast();
                tile.ResetLine();
            }
            _canAdd = true;
        }
        
        #endregion
    }
    
    public class BoardItemProperty
    {
        public GridInfo m_GridInfo;
        public Tile m_Tile;
        public Vector2 m_Coordinate;

        public BoardItemProperty(GridInfo gridInfo, Tile tile, Vector2 coordinate)
        {
            this.m_GridInfo = gridInfo;
            this.m_Tile = tile;
            this.m_Coordinate = coordinate;
        }
    }
}
