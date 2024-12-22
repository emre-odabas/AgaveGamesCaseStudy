using AgaveGames.Data;
using AgaveGames.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace AgaveGames.UI
{
    public class BaseScreen<T> : SingletonComponent<T> where T : Component
    {
        #region VARIABLES

        //Parameters
        [Header("Parameters")]
        [SerializeField] protected ScreenState m_State = ScreenState.Hidden;
        
        //Components
        [Header("Components")]
        [SerializeField] protected GameObject m_Container;
        [SerializeField] protected Canvas m_Canvas;
        [SerializeField] protected CanvasScaler m_CanvasScaler;
        
        //Properties

        #endregion

        #region MONOBEHAVIOUR

        protected new virtual void Awake()
        {
            base.Awake(); 
        }

        protected virtual void Start()
        {
            if (m_State == ScreenState.Hidden) 
                Hide();
            else if (m_State == ScreenState.Shown) 
                Show(true);
        }

        #endregion

        #region METHODS

        public virtual void Show(bool ignoreState = false)
        {
            if (!ignoreState)
                if (m_State == ScreenState.Shown) return;
            
            m_Container.SetActive(true);
            
            m_State = ScreenState.Shown;
        }

        public virtual void Hide()
        {
            m_Container.SetActive(false);
            
            m_State = ScreenState.Hidden;
        }

        #endregion
    }
}