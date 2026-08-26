using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("General")]
    [SerializeField] private PlayerInput _inputSystem;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private PlayerLife playerLife;
    private Vector2 _modifiedVelocity;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _runSpeed;
    private Vector2 moveDirection;
    private bool _runRequested;
    [Header("Jumping")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _checkRadius;
    [SerializeField] private Transform _checkerPos;
    private bool _isGrounded;
    private bool _jumpRequest;
    private bool _jumpPressed;
    [SerializeField] private float _coyoteTime;
    private float _coyoteTimer;
    [Header("Falling")]
    [SerializeField] private float _cutGravity;
    [SerializeField] private float _normalGravity;
    [SerializeField] private float _fallGravity;
    [Header("Platform Interaction")]
    [SerializeField] private Collider2D _playerCollider;
    [SerializeField]private float _checkerLength;
    [SerializeField] private float _ignorePlatDur;
    private float _ignorePlatTime;
    private Collider2D _ignoredPlatform;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        this._rigidbody2D = GetComponent<Rigidbody2D>();
        _inputSystem = new PlayerInput();
        playerLife = GetComponent<PlayerLife>();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
    }
    private void Update()
    {
        ReadMoveInput();
        ReadJumpInput();
        ReadDownInput();
    }
    private void FixedUpdate()
    {
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
            _modifiedVelocity.x = _runSpeed * moveDirection.x;
        } else {
            _modifiedVelocity.x = _moveSpeed * moveDirection.x;
        }
    }
    private void ApplyJump()
    {
        if (!_jumpRequest) return;
        
        if (_coyoteTimer <= 0) return; 

        _modifiedVelocity.y = _jumpForce;
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
    }
    private Collider2D FindPlatformUnder()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _checkerLength, _groundLayer);
        if (hit.collider == null) return null; 

        PlaceableItem platform = hit.collider.GetComponent<PlaceableItem>();
        if (platform == null) return null;

        return platform.GetCollider();
    } 
    private void TryDropThroughPlatform()
    {
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
    #endregion

    #region  Debug
     void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkerPos.position, _checkRadius);
    }
    #endregion
}
