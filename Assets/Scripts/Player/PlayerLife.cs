using UnityEngine;
using System;

public class PlayerLife : MonoBehaviour
{
    public event Action OnPlayerDeath;
    private Rigidbody2D rb2D;
    [SerializeField] private float _deathJumpForce;
    [SerializeField] private Collider2D physicalCollider;
    private bool _isDead;
    public bool IsDead => _isDead;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }
    public void Die()
    {
        Debug.Log("[PlayerLife] you died");
        _isDead = true;
        rb2D.linearVelocity = new Vector2 (0f, _deathJumpForce);
        physicalCollider.enabled = false;
        OnPlayerDeath?.Invoke();
    }
}
 