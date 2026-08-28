using System.Collections.Generic;
using UnityEngine;

public class BridgeTrap : MonoBehaviour, IResetable
{
    [SerializeField] private float collapseTime;
    [SerializeField] private Collider2D bridgeCollider;
    [SerializeField]private Rigidbody2D rb2D;
    [SerializeField] private GameObject[] planks;
    [SerializeField]private int _currentIdx;
    private Vector3[] _initialPlankPositions;
    private float _timer;
    private bool _activated;
    private bool _collapsed;
    
    private void Awake()
    {
        bridgeCollider = GetComponent<Collider2D>();
        rb2D = GetComponent<Rigidbody2D>();

        _initialPlankPositions = new Vector3[planks.Length];
        for (int i = 0; i < planks.Length; i++) _initialPlankPositions[i] = planks[i].transform.position;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_activated || _collapsed)
            return;
        if (collision.gameObject.CompareTag("Player")){
            _activated = true;
            _timer = collapseTime;
        }
    }

    private void Update()
    {
        if (!_activated)
            return;
        
        if (_timer <= 0f)
        {
            Collapse();
        } else
        {
            _timer -= Time.deltaTime;
        }
    }

    private void Collapse()
    {
        if (_currentIdx>=planks.Length) return;
        Rigidbody2D plankRb = planks[_currentIdx].GetComponent<Rigidbody2D>();
        Collider2D plankCol = planks[_currentIdx].GetComponent<Collider2D>();
        plankRb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        plankRb.gravityScale = 1f;
        plankCol.enabled = false;
        _timer = collapseTime;
        _currentIdx++;
    }
    public void ResetTrap()
    {
        for (int i = 0; i < planks.Length; i++)
        {
            Rigidbody2D plankRb = planks[i].GetComponent<Rigidbody2D>();
            Collider2D plankCol = planks[i].GetComponent<Collider2D>();

            plankRb.constraints |= RigidbodyConstraints2D.FreezePositionY; // kunci lagi
            plankRb.gravityScale = 0f; // matikan gravity lagi
            plankRb.linearVelocity = Vector2.zero; // penting! plank yg sudah jatuh masih punya momentum
            plankCol.enabled = true;

            planks[i].transform.position = _initialPlankPositions[i]; // balik ke posisi awal
        }

        _currentIdx = 0;
        _activated = false;
        _collapsed = false;
        _timer = 0f;
    }
}
