using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpCooldown = 0.5f;

    [Header("Ground Settings")]
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey = KeyCode.Q;
    [SerializeField] private float _slideMultiplier = 1.5f;
    [SerializeField] private float _slideDrag;

    private Rigidbody _playerRigidbody;
    private Vector3 _movementDirection;
    private float _horizontalInput, _verticalInput;
    private bool _canJump = true;
    private bool _isSlideModeActive = false;

    private void Awake()
    {
        _playerRigidbody = GetComponent<Rigidbody>();
        if (_playerRigidbody == null)
        {
            Debug.LogError("Rigidbody component is missing from the Player GameObject.");
        }
        _playerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        SetInputs();
        LimitPlayerSpeed();

        CheckSliding();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
        SetPlayerDrag();
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (_orientationTransform == null)
        {
            Debug.LogError("Orientation Transform is not assigned in the Inspector.");
            return;
        }

        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;
    }

    private void SetPlayerMovement()
    {
        if (_movementDirection.magnitude > 0) // WASD ile hareket varsa
        {
            // Slide sadece yerdeyken ve slide modu aktifken çalışır
            float currentSpeed = (_isSlideModeActive && IsGrounded()) ? _movementSpeed * _slideMultiplier : _movementSpeed;
            
            Vector3 moveVelocity = _movementDirection.normalized * currentSpeed;
            moveVelocity.y = _playerRigidbody.linearVelocity.y; // Y ekseninde mevcut hızı koru
            _playerRigidbody.linearVelocity = moveVelocity;
        }
        else
        {
            // Hareket inputu yoksa yatay hızı sıfırla
            _playerRigidbody.linearVelocity = new Vector3(0, _playerRigidbody.linearVelocity.y, 0);
        }
    }

    private void SetPlayerDrag()
    {
        if(_isSlideModeActive)
        {
            _playerRigidbody.linearDamping = _slideDrag; 
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }

    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
        
    }

    private void SetPlayerJump()
    {
        _canJump = false;
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), _jumpCooldown);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, (_playerHeight / 2) + 0.2f, _groundLayer);
    }

    private void CheckSliding()
    {
        // Q tuşuna basıldığında slide modunu toggle et
        if (Input.GetKeyDown(_slideKey))
        {
            _isSlideModeActive = !_isSlideModeActive;
        }

        if (Input.GetKeyDown(_jumpKey) && _canJump && IsGrounded())
        {
            SetPlayerJump();
        }
    }
}