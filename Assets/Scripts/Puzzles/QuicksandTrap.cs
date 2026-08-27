    using UnityEngine;

public class QuicksandTrap : MonoBehaviour
{
    [SerializeField] private float speedMultiplier = 0.4f;
    [SerializeField] private float jumpMultiplier = 0.3f;
    [SerializeField] private float sinkRate = 0.5f;
    [SerializeField] private float timeToDeath = 3f;

    private float _timeInSand = 0f;
    private PlayerMovement _currentPlayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement movement)) return;

        _currentPlayer = movement;
        _timeInSand = 0f;
        movement.ApplyQuicksandDebuff(speedMultiplier, jumpMultiplier, sinkRate);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (_currentPlayer == null) return;

        _timeInSand += Time.deltaTime;
        if (_timeInSand >= timeToDeath)
        {
            other.GetComponent<PlayerLife>()?.Die();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent(out PlayerMovement movement)) return;
        if (movement != _currentPlayer) return;

        movement.RemoveQuicksandDebuff();
        _currentPlayer = null;
        _timeInSand = 0f;
    }
}
