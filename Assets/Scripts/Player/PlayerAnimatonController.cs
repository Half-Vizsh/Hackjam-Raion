using System.Diagnostics.Tracing;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerAnimatonController : MonoBehaviour
{
    #region Variables
    [SerializeField] private PlayerStateMachine playerSM;
    [SerializeField] private Animator animator;
    #endregion Variables

    #region Unity Methods
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerSM = GetComponent<PlayerStateMachine>();
    }
    private void OnEnable()
    {
        playerSM.OnStateChanged += ChangeAnimation;
    }
    private void OnDisable()
    {
        playerSM.OnStateChanged -= ChangeAnimation;
    }
    private void Update()
    {
        ChangeAnimation();
    }
    private void ChangeAnimation()
    {
        switch (playerSM.CurrentState)
        {
            case PlayerState.Idle:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAiming", false);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsLaunching", false);
                animator.SetBool("IsDead", false);
            break;
            case PlayerState.Running:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsAiming", false);
            break;
            case PlayerState.Jumping:
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAiming", false);
            break;
            case PlayerState.Aiming:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAiming", true);
            break;
            case PlayerState.Throwing:
                animator.SetBool("IsAiming", false);
                animator.SetBool("IsLaunching", true);
            break;
            case PlayerState.Dead:
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsAiming", false);
                animator.SetBool("IsDead", true);
            break;
        }
    }
    #endregion
}
