using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private RectTransform _playerStateTransform;
    [SerializeField] private RectTransform _boosterSpeedTransform;
    [SerializeField] private RectTransform _boosterJumpTransform;
    [SerializeField] private RectTransform _boosterSlowTransform;

    [Header("Images")]
    [SerializeField] private Image _goldBoosterWheatImage;
    [SerializeField] private Image _holyBoosterWheatImage;
    [SerializeField] private Image _rottenBoosterWheatImage;


    [Header("Sprites")]
    [SerializeField] private Sprite _playerWalkingSprite;
    [SerializeField] private Sprite _playerSlidingSprite;

    [Header("Settings")]
    [SerializeField] private float _moveDuration;
    [SerializeField] private Ease _moveEase;

    public RectTransform GetBoosterSpeedTransform => _boosterSpeedTransform;
    public RectTransform GetBoosterJumpTransform => _boosterJumpTransform;
    public RectTransform GetBoosterSlowTransform => _boosterSlowTransform;
    public Image GetGoldBoosterWheatImage => _goldBoosterWheatImage;
    public Image GetHolyBoosterWheatImage => _holyBoosterWheatImage;
    public Image GetRottenBoosterWheatImage => _rottenBoosterWheatImage;

    private Image _playerStateImage;

    private void Awake()
    {
        _playerStateImage = _playerStateTransform.GetComponent<Image>();
    }

    private void Start()
    {
        _playerController.OnPlayerStateChanged += PlayerController_OnPlayerStateChanged;
        UpdateStateUI(PlayerState.Idle); // Varsayılan durumu ayarla
    }

    private void PlayerController_OnPlayerStateChanged(PlayerState playerState)
    {
        UpdateStateUI(playerState);
    }

    private void UpdateStateUI(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                _playerStateImage.sprite = _playerWalkingSprite;
                _playerStateTransform.DOAnchorPosX(-25f, _moveDuration).SetEase(_moveEase);
                break;
            case PlayerState.SlideIdle:
            case PlayerState.Slide:
                _playerStateImage.sprite = _playerSlidingSprite;
                _playerStateTransform.DOAnchorPosX(25f, _moveDuration).SetEase(_moveEase);
                break;
        }
    }

    private IEnumerator SetBoosterUserInterface(RectTransform activeTransform, Image boosterImage, Image wheatImage, Sprite activeSprite, Sprite passiveSprite, Sprite activeWheatSprite, Sprite passiveWheatSprite, float duration)
    {
        if (boosterImage == null) Debug.LogError("boosterImage is NULL!");
        if (wheatImage == null) Debug.LogError("wheatImage is NULL!");
        if (activeSprite == null) Debug.LogError("activeSprite is NULL!");
        if (passiveSprite == null) Debug.LogError("passiveSprite is NULL!");
        if (activeWheatSprite == null) Debug.LogError("activeWheatSprite is NULL!");
        if (passiveWheatSprite == null) Debug.LogError("passiveWheatSprite is NULL!");
        if (activeTransform == null) Debug.LogError("activeTransform is NULL!");
        boosterImage.sprite = activeSprite;
        wheatImage.sprite = activeWheatSprite;
        activeTransform.DOAnchorPosX(25f, _moveDuration).SetEase(_moveEase);
        yield return new WaitForSeconds(duration);

        boosterImage.sprite = passiveSprite;
        wheatImage.sprite = passiveWheatSprite;
        activeTransform.DOAnchorPosX(90f, _moveDuration).SetEase(_moveEase);
    }
  
    public void PlayBoosterUIAnimation(RectTransform activeTransform, Image boosterImage, Image wheatImage, Sprite activeSprite, Sprite passiveSprite, Sprite activeWheatSprite, Sprite passiveWheatSprite, float duration)

    {
        StartCoroutine(SetBoosterUserInterface(activeTransform, boosterImage, wheatImage, activeSprite, passiveSprite, activeWheatSprite, passiveWheatSprite, duration));
    }
}
