using UnityEngine;
using System;

public class PlayerLife : MonoBehaviour
{
    public event Action OnPlayerDeath;
    public event Action OnPlayerRespawn;

    private Rigidbody2D rb2D;
    private PlayerStateMachine playerSM;
    [Header("Death Jump")]
    [SerializeField] private float _deathJumpForce;
    [SerializeField] private Collider2D physicalCollider;
    private bool _isDead;
    public bool IsDead => _isDead;
    [Header("Respawn")]
    [SerializeField] private float respawnCD;
    private float respawnTimer;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerSM = GetComponent<PlayerStateMachine>();
    }

    private void Update()
    {
        if (!_isDead) return;

        if (respawnTimer <= 0)
        {
            PlayerRespawn();
        }
        else
        {
            respawnTimer -= Time.deltaTime;
        }
    }

    public void Die()
    {
        Debug.Log("[PlayerLife] you died");
        _isDead = true;
        respawnTimer = respawnCD;
        playerSM.ChangeState(PlayerState.Dead);
        OnPlayerDeath?.Invoke();

        rb2D.linearVelocity = new Vector2(0f, _deathJumpForce);
        physicalCollider.enabled = false;
    }

    public void PlayerRespawn()
    {
        if (CheckpointSave.ActiveCheckpoint == null)
        {
            Debug.LogWarning("[PlayerLife] Belum ada checkpoint yang tersentuh!");
            _isDead = false;
            return;
        }

        transform.position = CheckpointSave.ActiveCheckpoint.RespawnPos.position;
        CheckpointSave.ActiveCheckpoint.RestoreSegment();

        rb2D.linearVelocity = Vector2.zero;
        physicalCollider.enabled = true;
        _isDead = false;
        playerSM.RespawnState();

        OnPlayerRespawn?.Invoke(); // tetap di-invoke, buat sistem LAIN yang mungkin perlu tau (UI, dsb) - bukan checkpoint lagi
    }
}