using System;
using UnityEngine;
public enum PlayerState{
    Respawn,
    Idle,  
    Running,
    Jumping,
    Aiming,
    Throwing,
    Dead
}
public class PlayerStateMachine : MonoBehaviour
{
   #region Variables
   public PlayerState CurrentState  {get; private set;}
   public event Action OnStateChanged;
    #endregion
    private void Start()
    {
        ChangeState(PlayerState.Idle);
    }
    #region Public Methods
    public void ChangeState(PlayerState newState)
    {
        if (CurrentState == newState || CurrentState == PlayerState.Aiming || CurrentState == PlayerState.Throwing) return;
        if (CurrentState == PlayerState.Dead && newState != PlayerState.Respawn) return;
        OnStateChanged?.Invoke();
        CurrentState = newState;
        Debug.Log("[PlayerStateMachine] Current State: "+CurrentState);
    }
    public void RespawnState()
    {
        CurrentState = PlayerState.Idle;
    }
    //For handling the aiming and throwing state
    public void ReturnIdle()
    {
        CurrentState = PlayerState.Idle;
        OnStateChanged?.Invoke();        
    }
    public void EnterAim()
    {
        CurrentState = PlayerState.Aiming;
        OnStateChanged?.Invoke();
        Debug.Log("[PlayerStateMachine] masuk ke mode "+CurrentState);
    }
    public void CancelAim()
    {
        CurrentState = PlayerState.Idle;
        OnStateChanged?.Invoke();
    }
    public void ReleaseAim()
    {
        CurrentState = PlayerState.Throwing;
        OnStateChanged?.Invoke();
        Debug.Log("[PlayerStateMachine] Berhasil melempar berpindah ke mode "+CurrentState);
    }
   #endregion
}
