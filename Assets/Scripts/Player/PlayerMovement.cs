using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("General")]
    [SerializeField] private PlayerInput _inputSystem;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private PlayerLife playerLife;
    [SerializeField] private Vector2 _modifiedVelocity;
    private PlayerStateMachine playerSM;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _maxSpeedDebuff = 0.9f;
    [SerializeField] private float _maxJumpDebuff = 0.9f;
    private float _actualMovementSpeed;
    private float _actualRunSpeed;
    private Vector2 moveDirection;
    private bool _runRequested;
    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    private float _actualJumpForce;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _checkRadius;
    [SerializeField] private Transform _checkerPos;
    [SerializeField]private bool _isGrounded;
    private bool _jumpRequest;
    private bool _jumpPressed;
    [SerializeField] private float _coyoteTime;
    private float _coyoteTimer;
    [Header("Falling")]
    [SerializeField] private float _cutGravity;
    [SerializeField] private float _normalGravity;
    [SerializeField] private float _fallGravity;
    [Header("Weight System")]
    [SerializeField] private float _weightSpeedMultiplier = 0f; 
    [SerializeField] private float _weightJumpMultiplier = 0f; 
    private PlayerInventory playerInventory;
    [Header("Platform Interaction")]
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField]private float _checkerLength;
    [SerializeField] private float _ignorePlatDur;
    private float _ignorePlatTime;
    private Collider2D _ignoredPlatform;
    [Header("Sinking")]
    [SerializeField] private float _sinkSpeedMultiplier = 0f;
    [SerializeField] private float _sinkJumpMultiplier = 0f;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        this._rigidbody2D = GetComponent<Rigidbody2D>();
        _inputSystem = new PlayerInput();
        playerLife = GetComponent<PlayerLife>();
        playerInventory = GetComponent<PlayerInventory>();
        playerSM = GetComponent<PlayerStateMachine>();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
        playerInventory.OnWeightChanged += ApplyWeightDebuff;
    }
    private void Update()
    {
        ApplyFlip();
        if (playerSM.CurrentState == PlayerState.Aiming || playerSM.CurrentState == PlayerState.Throwing)
        {
          return;  
        } 
        ReadMoveInput();
        ReadJumpInput();
        ReadDownInput();
        CalculateRealSpeed();
    }
    private void FixedUpdate()
    {
        if (playerSM.CurrentState == PlayerState.Aiming || playerSM.CurrentState == PlayerState.Throwing)
        {
            _rigidbody2D.linearVelocityX = 0;
          return;  
        } 
        _modifiedVelocity = _rigidbody2D.linearVelocity;
        ApplyFall();
        if (playerLife.IsDead) return;
        ApplyMovement();
        ApplyGrounded();
        ApplyJump();

        ApplyVelocity();    
    }
    private void OnDisable()
    {
        _inputSystem.Disable();
        playerInventory.OnWeightChanged -= ApplyWeightDebuff;
    }
    #endregion

    #region Private Methods
    private void ReadMoveInput()
    {
        moveDirection = _inputSystem.Player.Movement.ReadValue<Vector2>();
        _runRequested = _inputSystem.Player.Run.IsPressed();
    }
    private void ReadJumpInput()
    {
        _jumpPressed = _inputSystem.Player.Jump.IsPressed();
        if (_inputSystem.Player.Jump.WasPressedThisFrame())
        {
            _jumpRequest = true;
        }        
    }
    private void ReadDownInput()
    {
        if (_ignorePlatTime > 0)
        {
            _ignorePlatTime-=Time.deltaTime;
            if (_ignorePlatTime <= 0)
            {
                if (_ignoredPlatform == null) return;
                Physics2D.IgnoreCollision(
                    _playerCollider,
                    _ignoredPlatform,
                    false
                );
                _ignoredPlatform = null;
            }
        }
        if (_inputSystem.Player.Down.WasPressedThisFrame())
        {
            TryDropThroughPlatform();
        }
    }
    private void ApplyMovement()
    {
        if (_runRequested)
        {
            _modifiedVelocity.x = _actualRunSpeed * moveDirection.x;
        } else {
            _modifiedVelocity.x = _actualMovementSpeed * moveDirection.x;
        }
    }
    private void ApplyJump()
    {
        if (!_jumpRequest) return;
        
        if (_coyoteTimer <= 0) return; 
        _modifiedVelocity.y = _actualJumpForce;
        _jumpRequest = false;
        _coyoteTimer = 0;
    }
    private void ApplyGrounded()
    {
        _isGrounded = Physics2D.OverlapCircle(
            _checkerPos.position, 
            _checkRadius, 
            _groundLayer);
        if (_isGrounded)
        {
            _coyoteTimer = _coyoteTime;    
        } 
        else
        {
            _coyoteTimer -= Time.fixedDeltaTime;
        }
    }
     private void ApplyFall()
    {
        if (_rigidbody2D.linearVelocityY > 0)
        {
            _rigidbody2D.gravityScale = _jumpPressed ? _normalGravity : _cutGravity;
        }  else
        {
            _rigidbody2D.gravityScale = _fallGravity;
        }
    }
    private void ApplyVelocity()
    {
        _rigidbody2D.linearVelocity = _modifiedVelocity;
        if (!_isGrounded)
        {
            playerSM.ChangeState(PlayerState.Jumping);
            return;
        } else if (Mathf.Abs(_modifiedVelocity.x)>0.1f)
        {
            playerSM.ChangeState(PlayerState.Running);
        } else
        {
            playerSM.ChangeState(PlayerState.Idle);
        }
    }
    private void CalculateRealSpeed()
    {
        _actualMovementSpeed = _moveSpeed - _moveSpeed  * Mathf.Clamp(_sinkSpeedMultiplier+_weightSpeedMultiplier, 0, _maxSpeedDebuff );
        _actualRunSpeed = _runSpeed - _runSpeed *  Mathf.Clamp (_sinkSpeedMultiplier+_weightSpeedMultiplier, 0, _maxSpeedDebuff);
        _actualJumpForce = _jumpForce - _jumpForce *  Mathf.Clamp (_sinkJumpMultiplier+_weightJumpMultiplier, 0, _maxJumpDebuff);;
    }
    private void ApplyFlip()
    {
        float flipDirection = 0f;
        if (playerSM.CurrentState == PlayerState.Aiming)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 screenPos = new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane);
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            worldPos.z = 0f; 
            flipDirection = worldPos.x - transform.position.x;
        }
        else
        {
            flipDirection = moveDirection.x;
        }

        if (Mathf.Abs(flipDirection) > 0.01f)
        {
            float direction = Mathf.Sign(flipDirection);
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x) * direction,
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
    private Collider2D FindPlatformUnder()
    {
        Debug.DrawRay(transform.position, Vector2.down * _checkerLength, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _checkerLength, _groundLayer);
        if (hit.collider == null) {
            Debug.Log("[PlayerMovement] Di bawah ga ada apa-apa");
            return null;
        } 
        PlaceableItem platform = hit.collider.GetComponent<PlaceableItem>();
        if (platform == null) return null;

        Debug.Log("[PlayerMovement] Ada platform nih di bawah");
        return platform.GetCollider();
    } 
    private void TryDropThroughPlatform()
    {
            Debug.Log("[PlayerMovement] mencoba turun ke bawah plaform");
        Collider2D platformIgnore = FindPlatformUnder();
        if(platformIgnore == null) return;
        DoFallThroughPlatform(platformIgnore);
    }
    private void DoFallThroughPlatform(Collider2D platform)
    {
        Physics2D.IgnoreCollision(_playerCollider, platform, true);
        _ignoredPlatform = platform;
        _ignorePlatTime = _ignorePlatDur;
    }
    #endregion

    #region Public Methods
    public void ApplyQuicksandDebuff(float speedMult, float jumpMult)
    {
        _sinkSpeedMultiplier = speedMult;
        _sinkJumpMultiplier = jumpMult;
    }
    public void RemoveQuicksandDebuff()
    {
        _sinkSpeedMultiplier = 0;
        _sinkJumpMultiplier = 0;
    }
    public void ApplyWeightDebuff(float weightPercent)
    {
        _weightSpeedMultiplier = weightPercent;
        _weightJumpMultiplier = weightPercent;
    }
    #endregion

    #region  Debug
     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkerPos.position, _checkRadius);
    }
    #endregion
}
