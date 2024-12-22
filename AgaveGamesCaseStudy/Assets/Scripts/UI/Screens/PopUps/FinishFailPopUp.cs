
using AgaveGames.Managers;
using TMPro;
using UnityEngine;

namespace AgaveGames.UI
{
    public class FinishFailPopUp : BaseScreen<FinishFailPopUp>
    {
        #region VARIABLES
        
        //Parameters
        //[SerializeField] 
    
        //Components
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI m_TxtCurrentScore;

        //Properties

        //Privates
        
        #endregion
        
        #region MONOBEHAVIOUR
        
        private void OnEnable()
        {
            GameManager.Instance.OnFinishFail += OnFinish;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFinishFail -= OnFinish;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        #endregion
        
        #region CALLBACKS
        
        private void OnFinish(int score)
        {
            Show();
            m_TxtCurrentScore.text = "Your Score: " + score.ToString("n0");
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

        #region BUTTONS

        public void OnClickPlayAgain()
        {
            Hide();
            GameManager.Instance.Replay();
        }

        #endregion
    }
}