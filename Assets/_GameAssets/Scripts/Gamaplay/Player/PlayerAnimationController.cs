using System;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;
    private PlayerController _playerController;

    private StateController _stateController;

    private void Awake()
    {
        _stateController = GetComponent<StateController>();
    }
    // private void Start() {
    //     _playerController.OnPlayerJump += PlayerController_OnPlayerJump;
    // }

    

    private void Update()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play
            && GameManager.Instance.GetCurrentGameState() != GameState.Resume)
        {
            return;
        }
        SetPlayerAnimations();
    }
    private void PlayerController_OnPlayerJump()
    {
        _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_JUMPING, true);
        Invoke(nameof(ResetJumping), 0.5f); // Reset the jump animation after a short delay

    }
    private void ResetJumping()
    {
        _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_JUMPING, false);
    }

    

    private void SetPlayerAnimations()
    {
        var currentState = _stateController.GetCurrentPlayerState();


        _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_MOVING, false);
        _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, false);
        _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_JUMPING, false);

        switch (currentState)
        {
            case PlayerState.Idle:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, false);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_MOVING, false);
             break;
            case PlayerState.Move:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_MOVING, true);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, false);
                break;
            case PlayerState.SlideIdle:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, true);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING_ACTIVE, false);
                break;
            case PlayerState.Slide:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, true);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING_ACTIVE, true);
                break;
            case PlayerState.Jump:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_JUMPING, true);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_MOVING, true);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, false);
                break;
            default:
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_MOVING, false);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_SLIDING, false);
                _playerAnimator.SetBool(Consts.SetPlayerAnimations.IS_JUMPING, false);
                break;
        }
    }
}