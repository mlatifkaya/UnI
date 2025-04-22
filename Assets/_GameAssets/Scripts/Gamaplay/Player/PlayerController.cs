using Unity.VisualScripting;
using UnityEngine;
using System;
public class PlayerController : MonoBehaviour
{

    public event Action OnPlayerJump;
    public event Action <PlayerState> OnPlayerStateChanged;


    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _jumpCooldown = 0.5f;
    // [SerializeField] private float _airMultiplier = 0.4f;
    [SerializeField] private float _airdrag = 0.05f;

    [Header("Ground Settings")]
    [SerializeField] private float _playerHeight = 2f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;
    // [SerializeField] private float _groundMultiplier = 1f;

    [Header("Sliding Settings")]
    [SerializeField] private KeyCode _slideKey = KeyCode.Q;
    [SerializeField] private float _slideMultiplier = 1.5f;
    [SerializeField] private float _slideDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidbody;
    private Vector3 _movementDirection;
    private float _startingMovementSpeed, _startingJumpForce;
    private float _horizontalInput, _verticalInput;
    private bool _canJump = true;
    private bool _isSlideModeActive = false;

    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;

        _startingMovementSpeed = _movementSpeed;
        _startingJumpForce = _jumpForce;
    }

    
    private void Update()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play
            && GameManager.Instance.GetCurrentGameState() != GameState.Resume)
        {
            return;
        }
        SetInputs();
        SetStates();
        SetPlayerDrag();
        LimitPlayerSpeed();
        CheckSliding(); // gerekli mi? // Evet, çünkü slide modunu kontrol ediyor
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play
            && GameManager.Instance.GetCurrentGameState() != GameState.Resume)
        {
            return;
        }
        SetPlayerMovement();
        SetPlayerDrag();
    }

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _movementDirection = _orientationTransform.forward * _verticalInput + _orientationTransform.right * _horizontalInput;
    /*
        if (_orientationTransform == null)
        {
            Debug.LogError("Orientation Transform is not assigned in the inspector.");
            return;
        }
*/
    }

    private void SetStates()
    {
        var movementDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var currentState = _stateController.GetCurrentPlayerState();

        var newState = currentState switch
        {
            _ when movementDirection == Vector3.zero && isGrounded && !_isSlideModeActive => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !_isSlideModeActive => PlayerState.Move,
            _ when movementDirection == Vector3.zero && isGrounded && _isSlideModeActive => PlayerState.SlideIdle,
            _ when movementDirection != Vector3.zero && isGrounded && _isSlideModeActive => PlayerState.Slide,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState            
        };

        if(newState != currentState)
        {
            _stateController.ChangeState(newState);
            OnPlayerStateChanged?.Invoke(newState); // State değiştiğinde event tetikle
        }
    }

    private void SetPlayerMovement()
    {

       if (_movementDirection.magnitude > 0) // WASD ile hareket varsa
        {
            float currentSpeed = _stateController.GetCurrentPlayerState() switch
            {
                // Slide durumunda ve yerdeyken slideMultiplier uygulanır
                PlayerState.Slide when IsGrounded() => _movementSpeed * _slideMultiplier,
                // Diğer durumlarda normal hız kullanılır
                _ => _movementSpeed
            };

            Vector3 moveVelocity = _movementDirection.normalized * currentSpeed;
            moveVelocity.y = _playerRigidbody.linearVelocity.y; // Y eksenindeki mevcut hızı koru
            _playerRigidbody.linearVelocity = moveVelocity;
        }
        else
        {
            // Hareket inputu yoksa yatay hızı sıfırla
            Vector3 currentVelocity = _playerRigidbody.linearVelocity;
            _playerRigidbody.linearVelocity = new Vector3(0, currentVelocity.y, 0);
        }


       /* if (_movementDirection.magnitude > 0) // WASD ile hareket varsa
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
        */

    }

    private void SetPlayerDrag()
    {

        _playerRigidbody.linearDamping = _stateController.GetCurrentPlayerState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airdrag,
            _ => _playerRigidbody.linearDamping
        };
        /*
        if(_isSlideModeActive)
        {
            _playerRigidbody.linearDamping = _slideDrag; 
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
        */
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
        OnPlayerJump?.Invoke(); // Jump eventini tetikle
        _canJump = false;
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        Invoke(nameof(ResetJump), _jumpCooldown);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    #region Helper Methods
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, (_playerHeight / 2) + 0.2f, _groundLayer);
    }

    private void CheckSliding()
    {
        // _slideKey tuşuna basıldığında slide modunu toggle et
        if (Input.GetKeyDown(_slideKey))
        {
            _isSlideModeActive = !_isSlideModeActive;
        }

        if (Input.GetKeyDown(_jumpKey) && _canJump && IsGrounded())
        {
            SetPlayerJump();
        }
    }

    private Vector3 GetMovementDirection()
    {
        return _movementDirection.normalized;
    }

    public void SetMovementSpeed(float speed, float duration)
    {
        _movementSpeed += speed;
        Invoke(nameof(ResetMovementSpeed), duration);
        
    }

    private void ResetMovementSpeed()
    {
        _movementSpeed = _startingMovementSpeed;
    }

    public void SetJumpForce(float force, float duration)
    {
        _jumpForce += force;
        Invoke(nameof(ResetJumpForce), duration);
    }
    private void ResetJumpForce()
    {
        _jumpForce = _startingJumpForce;
    }

    public Rigidbody GetPlayerRigidbody()
    {
        return _playerRigidbody;
    }

    #endregion


    
}