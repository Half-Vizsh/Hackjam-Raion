using UnityEngine;
public enum PlayerState{
    Idle,  
    Running,
    Jumping,
    Aiming
}
public class PlayerStateMachine : MonoBehaviour
{
   #region Variables
   public PlayerState CurrentState  {get; private set;}
   #endregion

   #region Public Methods
   public void ChangeState(PlayerState newState)
    {
        CurrentState = newState;
    }
   #endregion
}
