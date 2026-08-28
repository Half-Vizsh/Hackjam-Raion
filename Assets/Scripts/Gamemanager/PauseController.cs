using UnityEngine;
using UnityEngine.InputSystem;

public class PauseController : MonoBehaviour
{
    public static PauseController instance;
    private bool _isPause = false;
    public bool IsPause => _isPause;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    public void Update()
    {
        if (Finish.instance.isWin)
        {
            _isPause = false;
            return;            
        } 
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPause)
            {
                _isPause = false;
            } else {
                _isPause = true;
            }
        }
    }
}
