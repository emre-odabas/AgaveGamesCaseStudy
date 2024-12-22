
using AgaveGames.Managers;
using TMPro;
using UnityEngine;

namespace AgaveGames.UI
{
    public class GameScreen : BaseScreen<GameScreen>
    {
        #region VARIABLES
        
        //Parameters
        //[SerializeField] 
    
        //Components
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI m_TxtRemainingMovement;
        [SerializeField] private TextMeshProUGUI m_TxtTargetScore;
        [SerializeField] private TextMeshProUGUI m_TxtCurrentScore;

        //Properties

        //Privates
        
        #endregion
        
        #region MONOBEHAVIOUR
        
        private void OnEnable()
        {
            GameManager.Instance.OnInit += OnInit;
            GameManager.Instance.OnMove += OnMove;
            GameManager.Instance.OnScore += OnScore;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnInit -= OnInit;
                GameManager.Instance.OnMove -= OnMove;
                GameManager.Instance.OnScore -= OnScore;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        #endregion
        
        #region CALLBACKS
        
        private void OnInit()
        {
            m_TxtRemainingMovement.text = GameManager.Instance.LevelData.m_MaxMovement.ToString();
            m_TxtTargetScore.text = $"Target Score: {GameManager.Instance.LevelData.m_TargetGoal:n0}";
            m_TxtCurrentScore.text = "0";
        }
        
        private void OnMove(int remainingMovement)
        {
            m_TxtRemainingMovement.text = remainingMovement.ToString();
        }

        private void OnScore(int score)
        {
            m_TxtCurrentScore.text = score.ToString("n0");
        }

        #endregion
        
        #region METHODS
        
        public override void Show(bool ignoreState = false)
        {
            base.Show(ignoreState);
        }

        public override void Hide()
        {
            base.Hide();
        }
        
        #endregion
    }
}