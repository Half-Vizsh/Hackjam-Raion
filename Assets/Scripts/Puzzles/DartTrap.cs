using Unity.Mathematics;
using UnityEngine;

public class DartTrap : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float _cdDur;
    [SerializeField] private float _shootPower;
    private float _cdTimer;
    [SerializeField]private bool _canShoot = true;
    [SerializeField] private bool _isActive = true;
    #endregion
    #region Unity Methods
    private void Update()
    {
        _cdTimer-=Time.deltaTime;
        if (_cdTimer<=0) _canShoot = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShootTrap();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Treasure"))
        {
            // Debug.Log("[Dart] Nabrak treasure");
            _canShoot = false;
            _isActive = false;
        }
    }
    #endregion
    #region Private Methods
    private void ShootTrap()
    {
        if (!_canShoot || !_isActive) return;        
        GameObject dartBullet = Instantiate(bulletPrefab, shootingPoint.position, quaternion.identity);
        Rigidbody2D projectileRB = dartBullet.GetComponent<Rigidbody2D>();
        projectileRB.AddForce(Vector2.right * _shootPower, ForceMode2D.Impulse);
        
        _cdTimer = _cdDur;
    }
    #endregion
    #region Public Methods
    #endregion
}
