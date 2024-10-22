using AstekUtility.ServiceLocatorTool;
using UnityEngine;

namespace Gameplay
{
    public enum GameMode
    {
        JackpotMode,
        RubicsMode
    }

    /// <summary>
    /// When we have time and going for optimization use this for knowing what is going on
    /// </summary>
    public class GameStates : MonoBehaviour, IGameService
    {
        public GameMode Mode { get; private set; }
        public delegate void ChangeGameMode(GameMode mode);
        public ChangeGameMode ChangeOngoingGameMode { get; private set; }

        private void Awake()
        {
            ServiceLocator.Instance.Register<GameStates>(this);
            ChangeOngoingGameMode += ChangeState;
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<GameStates>();
        }

        private void ChangeState(GameMode mode)
        {
            Mode = mode;
        }
    }
}
