using System;
using Unity.VisualScripting;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<GameState> OnGameStateChanged; // Event to notify when the game state changes

    [Header("References")]
    [SerializeField] private EggCounterUI _eggCounterUI; // Reference to the EggCounterUI script
    [Header("Settings")]
    [SerializeField] private int _maxEggCount = 5;
    [SerializeField] private WinLoseUI _winLoseUI;
    
    private GameState _currentGameState;
    private int _currentEggCount;

    private void OnEnable()
    {
        ChangeGameState(GameState.Play);
    }
    private void Awake()
    {
       Instance = this;
    }
    public void ChangeGameState(GameState gameState)
    {
        OnGameStateChanged?.Invoke(gameState);
        _currentGameState = gameState;
        Debug.Log($"Game state: {gameState}");         
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount, _maxEggCount); 
        if (_currentEggCount == _maxEggCount)
        {
            _eggCounterUI.SetEggCompleted(); 
            ChangeGameState(GameState.GameOver);
            _winLoseUI.OnGameWin(); 

        }
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }
}
