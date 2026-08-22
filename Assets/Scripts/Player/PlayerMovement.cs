using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("General")]
    [SerializeField] private PlayerInput _inputSystem;
    [SerializeField] private Rigidbody2D _rigidbody2D;
    private Vector2 modifiedVelocity;
    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    private Vector2 moveDirection;
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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        this._rigidbody2D = GetComponent<Rigidbody2D>();
        _inputSystem = new PlayerInput();
    }
    private void OnEnable()
    {
        _inputSystem.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        ReadMoveInput();
        ReadJumpInput();
    }
    private void FixedUpdate()
    {
        modifiedVelocity = _rigidbody2D.linearVelocity;
        ApplyMovement();
        ApplyGrounded();
        ApplyJump();
        ApplyFall();

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
    }
    private void ReadJumpInput()
    {
        _jumpPressed = _inputSystem.Player.Jump.IsPressed();
        if (_inputSystem.Player.Jump.WasPressedThisFrame())
        {
            _jumpRequest = true;
        }        
    }
    private void ApplyMovement()
    {
        modifiedVelocity.x = _moveSpeed * moveDirection.x;
    }
    private void ApplyJump()
    {
        if (!_jumpRequest) return;
        
        if (_coyoteTimer <= 0) return; 

        modifiedVelocity.y = _jumpForce;
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
        _rigidbody2D.linearVelocity = modifiedVelocity;
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
