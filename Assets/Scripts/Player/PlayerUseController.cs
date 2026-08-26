using System;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerUseController : MonoBehaviour
{
    #region Variables; 
    [SerializeField] private PlayerStateMachine _playerSM;
    [SerializeField] private PlayerInventory _playerInventory;
    public event Action OnItemUsed;
    private PlayerInput _inputSystem;
    private Vector2 _targetPos;
    private Vector2 _playerPos;
    [Header("Throwing")]
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _throwingPower;
    #endregion
    #region Unity Methods;
    private void Awake()
    {
        _inputSystem = new PlayerInput();
        _playerSM = this.GetComponent<PlayerStateMachine>();
    } 
    private void Update()
    {
        _playerPos = transform.position;
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        _targetPos = ((Vector2)mouseWorldPos - (Vector2)_shootingPoint.position).normalized;
        
        AdjustInputReading(_playerInventory.GetCurrentSelected());
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
    }
    private void OnDisable()
    {
        _inputSystem.Disable();
    }
    #endregion
    #region Private Methods
    public void AdjustInputReading (ItemStack usedItem)
    {
        if (usedItem == null) return;
        switch (usedItem.ItemData.itemType)
        {
            case ItemType.Throwable:
                ReadAimInput();
                break;
            case ItemType.Placeable:
                ReadPlacingInput();
                break;
            case ItemType.Valuable:
                break;
            default:
                break;
        }
    }
    private void ReadAimInput()
    {
        Debug.DrawLine(_shootingPoint.position, (Vector2)_shootingPoint.position + _targetPos * 5f, Color.red);
        // Mulai aiming
        if (_inputSystem.Player.Use.WasPressedThisFrame())
        {
            _playerSM.ChangeState(PlayerState.Aiming);
            return;
        }
        // Hanya membaca input berikutnya kalau sedang aiming
        if (_playerSM.CurrentState != PlayerState.Aiming)
            return;

        // RMB = cancel
        if (_inputSystem.Player.CancelAim.WasPressedThisFrame())
        {
            _playerSM.ChangeState(PlayerState.Idle);
            return;
        }

        // LMB dilepas = throw
        if (_inputSystem.Player.Use.WasReleasedThisFrame())
        {
            HandleShoot(_playerInventory.GetCurrentSelected());
            _playerSM.ChangeState(PlayerState.Idle);
        }
    }
    private void HandleShoot(ItemStack UsedItem)
    {
        if (_playerSM.CurrentState != PlayerState.Aiming) return;
        Debug.Log("TEMBAK");
        GameObject launchedProjectile = Instantiate(UsedItem.ItemData.physicalPrefab, _shootingPoint.position, quaternion.identity);
        Rigidbody2D projectileRB = launchedProjectile.GetComponent<Rigidbody2D>();
        projectileRB.AddForce(_targetPos*_throwingPower, ForceMode2D.Impulse);
        OnItemUsed?.Invoke();
    }
    private void ReadPlacingInput()
    {
        if (_inputSystem.Player.Use.WasPressedThisFrame())
        {
            HandlePlacement(_playerInventory.GetCurrentSelected());
        }
    }
    private void HandlePlacement(ItemStack placedItem)
    {
        GameObject placedObject = Instantiate(placedItem.ItemData.physicalPrefab, _shootingPoint.position, quaternion.identity);
        OnItemUsed?.Invoke();
    }
    #endregion
    #region Public Methods
    #endregion
}
