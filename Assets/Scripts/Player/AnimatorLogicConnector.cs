using UnityEngine;

public class AnimatorLogicConnector : MonoBehaviour
{
    [SerializeField] private PlayerUseController useController;
    [SerializeField] private PlayerStateMachine playerSM;
    public void ShowItemSprite()
    {
        useController.DrawThrowingSprite();
    }
    public void HideItemSprite()
    {
        useController.RemoveThrowingSprite();        
    }
    public void FinishThrowingAnimation()
    {
        playerSM.ReturnIdle();
    }
}
