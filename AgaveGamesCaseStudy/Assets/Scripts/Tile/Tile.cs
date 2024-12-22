using System.Collections;
using AgaveGames.Data;
using UnityEngine;
using AgaveGames.Managers;
using AgaveGames.Utilities;

namespace AgaveGames.Tiles
{
    public class Tile : MonoBehaviour, ITile
    {
        #region ACTIONS

        #endregion
        
        #region VARIABLES

        //Parameters
        [Header("Parameters")]
        [SerializeField] private TileType m_TileType;
        [SerializeField] private int m_Score = 100;
        
        //Components
        [Header("Components")]
        [SerializeField] private GameObject m_VisualContainer;
        [SerializeField] private ParticleSystem m_FxExplosion;
        //[SerializeField] private TextMesh m_TxtScoreText;
        [SerializeField] private LineRenderer m_LineRenderer;
        
        //Privates
        private int _extraScore = 50;
        private bool _isFalling = false;
        
        //Properties
        public TileType TileType => m_TileType;
        public Vector2Int Coordinate { get; set; }
        public int Score => m_Score;
        
        #endregion
        
        #region VIRTUAL METHODS
        
        public virtual void Init(int r, int c, Vector2 startPos, Vector2 goalPos)
        {
            Coordinate = new Vector2Int(r, c);
            gameObject.SetActive(true);
            m_FxExplosion.Stop();
            //m_TxtScoreText.text = "";
            m_VisualContainer.gameObject.SetActive(true);
            
            SlideDown(startPos, goalPos);
        }
        
        public virtual void OnMerge(int stackIndex)
        {
            if (stackIndex < GameManager.Instance.MinToConnect)
            {
                //m_TxtScoreText.text = m_Score.ToString("n0");
                GameManager.Instance.AddScore(m_Score);
            }
            else
            {
                int tomultiply = (stackIndex / GameManager.Instance.MinToConnect);
                int extra = _extraScore * tomultiply;
                //m_TxtScoreText.text = (m_Score + extra).ToString("n0");
                GameManager.Instance.AddScore(m_Score + extra);
            }
            StartCoroutine(Break());
        }
        
        protected virtual void OnMouseOver()
        {
            if ((Input.GetMouseButton(0) || (Input.touchCount > 0)) && !GameManager.Instance.IsFinish)
                GameManager.Instance.AddToStack(this);
        }
        
        public virtual void SetLine(int x, int y)
        {
            m_LineRenderer.SetPosition(1, new Vector3(x, y, 0));
        }
        
        public virtual void ResetLine()
        {
            m_LineRenderer.SetPosition(1, Vector3.zero);
        }

        #endregion

        #region METHODS

        public void SetRowCol(int r, int c)
        {
            Coordinate = new Vector2Int(r, c);
        }

        public IEnumerator Break()
        {
            m_VisualContainer.SetActive(false);
            ResetLine();
            m_FxExplosion.Play();
            
            yield return new WaitForSeconds(m_FxExplosion.main.duration + 0.5f);
            
            //gameObject.SetActive(false);
            ObjectPooler.Recycle(this);
        }

        public void SlideDown(Vector2 startPos, Vector2 goalPos)
        {
            transform.localPosition = startPos;
            if (!_isFalling)
                StartCoroutine(SlideDown(goalPos, 15f));
        }

        private IEnumerator SlideDown(Vector2 goalPos, float speed)
        {
            _isFalling = true;
            float t = 0;
            do
            {
                // float yPos = curve.Evaluate(t);
                transform.position = Vector3.Lerp(transform.localPosition, goalPos, t);
                //yield return new WaitForSeconds(deltaTime);
                t += Time.deltaTime * speed; //add the time

                yield return new WaitForFixedUpdate();

            } while (Vector2.Distance(transform.localPosition, goalPos) > 0.2f); //run until the time of the last frame
            transform.localPosition = goalPos; //make sure its a the position we want it to 
            _isFalling = false;
        }

        #endregion
    }
}