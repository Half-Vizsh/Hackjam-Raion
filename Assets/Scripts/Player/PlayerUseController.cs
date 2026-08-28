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
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private float _throwingPower;
    [Header("Placement")]
    [SerializeField]private Transform placingPoint;
    [Header("Trajectory Line")]
    [SerializeField] private LineRenderer _trajectoryLine;
    [SerializeField] private int _trajectoryPoints = 30;
    [SerializeField] private float _trajectoryTimeStep = 0.05f;
    [SerializeField] private LayerMask _trajectoryCollisionLayer;    
    #endregion
    #region Unity Methods;
    private void Awake()
    {
        _inputSystem = new PlayerInput();
        _playerSM = this.GetComponent<PlayerStateMachine>();
    } 
    private void Update()
    {
        if (PauseController.instance.IsPause) return;            
        if (_playerSM.CurrentState == PlayerState.Dead) return;

        _playerPos = transform.position;
        Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        _targetPos = ((Vector2)mouseWorldPos - (Vector2)shootingPoint.position).normalized;
        
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
        Debug.DrawLine(shootingPoint.position, (Vector2)shootingPoint.position + _targetPos * 5f, Color.red);
        if (_inputSystem.Player.Use.WasPressedThisFrame())
        {
            _playerSM.ChangeState(PlayerState.Aiming);
            return;
        }
        if (_playerSM.CurrentState != PlayerState.Aiming) return;

        _trajectoryLine.enabled = true;
        DrawTrajectory();
        if (_inputSystem.Player.CancelAim.WasPressedThisFrame())
        {
            _trajectoryLine.enabled = false;
            _playerSM.ChangeState(PlayerState.Idle);
            return;
        }
        if (_inputSystem.Player.Use.WasReleasedThisFrame())
        {
            _trajectoryLine.enabled = false;
            HandleShoot(_playerInventory.GetCurrentSelected());
            _playerSM.ChangeState(PlayerState.Idle);
        }
    }
    private void HandleShoot(ItemStack UsedItem)
    {
        if (_playerSM.CurrentState != PlayerState.Aiming) return;
        Debug.Log("TEMBAK");
        GameObject launchedProjectile = Instantiate(UsedItem.ItemData.physicalPrefab, shootingPoint.position, quaternion.identity);
        launchedProjectile.GetComponent<ItemPickup>().MakeFragile();
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
        GameObject placedObject = Instantiate(placedItem.ItemData.physicalPrefab, placingPoint.position, quaternion.identity);
        OnItemUsed?.Invoke();
    }
private void DrawTrajectory()
{
    Rigidbody2D rb = _playerInventory
        .GetCurrentSelected()
        .ItemData.physicalPrefab
        .GetComponent<Rigidbody2D>();

    Vector2 startPosition = shootingPoint.position;

    Vector2 velocity =
        _targetPos * _throwingPower / rb.mass;

    Vector2 gravity =
        Physics2D.gravity * rb.gravityScale;

    _trajectoryLine.positionCount = _trajectoryPoints;

    Vector2 previousPosition = startPosition;

    int pointCount = 0;

    for (int i = 1; i <= _trajectoryPoints; i++)
    {
        float time = i * _trajectoryTimeStep;

        Vector2 currentPosition =
            startPosition +
            velocity * time +
            0.5f * gravity * time * time;

        Vector2 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        RaycastHit2D hit = Physics2D.Raycast(
            previousPosition,
            direction.normalized,
            distance,
            _trajectoryCollisionLayer
        );

        if (hit.collider != null)
        {
            _trajectoryLine.positionCount = pointCount + 1;
            _trajectoryLine.SetPosition(pointCount, hit.point);
            return;
        }

        _trajectoryLine.SetPosition(pointCount, currentPosition);

        previousPosition = currentPosition;
        pointCount++;
    }

    _trajectoryLine.positionCount = pointCount;
}
    #endregion
    #region Public Methods
    #endregion
}
