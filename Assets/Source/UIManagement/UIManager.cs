using Assets.Source.GameManagement;
using UnityEngine;

namespace Assets.Source.UIManagement
{
    /// <summary>
    /// Controlls UI elements
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public void StartGameButtonClick()
        {
            GameManager.Instance.QueueGame();
        }
    }
}
