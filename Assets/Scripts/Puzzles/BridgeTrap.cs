using System.Collections.Generic;
using UnityEngine;

public class BridgeTrap : MonoBehaviour
{
    [SerializeField] private float collapseTime;
    [SerializeField] private Collider2D bridgeCollider;
    [SerializeField]private Rigidbody2D rb2D;
    [SerializeField] private GameObject[] planks;
    [SerializeField]private int _currentIdx;
    private float _timer;
    private bool _activated;
    private bool _collapsed;
    
    private void Awake()
    {
        bridgeCollider = GetComponent<Collider2D>();
        rb2D = GetComponent<Rigidbody2D>();
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
}
