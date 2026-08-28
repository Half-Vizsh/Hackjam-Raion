using UnityEngine;
public enum PlayerState{
    Respawn,
    Idle,  
    Running,
    Jumping,
    Aiming,
    Dead
}
public class PlayerStateMachine : MonoBehaviour
{
   #region Variables
   public PlayerState CurrentState  {get; private set;}
   #endregion

   #region Public Methods
   public void ChangeState(PlayerState newState)
    {
        if (CurrentState == PlayerState.Dead && newState != PlayerState.Respawn) return;
        CurrentState = newState;
    }
   #endregion
}
