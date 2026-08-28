using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine playerSM;
    private void Awake()
    {
        playerSM = GetComponent<PlayerStateMachine>();
        AudioManager.Instance.PlayMusic("BGM");
    }
    private void Update()
    {
        if (playerSM.CurrentState == PlayerState.Running)
        {
            AudioManager.Instance.PlaySFX("Walk");
        } else {
            AudioManager.Instance.StopSFX();
        }
    }
}
