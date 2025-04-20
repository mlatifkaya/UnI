using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private EggCounterUI _eggCounterUI; // Reference to the EggCounterUI script
    [Header("Settings")]
    [SerializeField] private int _maxEggCount = 5;
    
    private int _currentEggCount;


    private void Awake()
    {
       Instance = this;
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount, _maxEggCount); 
        Debug.Log($"Egg collected! Current count: {_currentEggCount}");

        if (_currentEggCount == _maxEggCount)
        {
            _eggCounterUI.SetEggCompleted(); 
            Debug.Log("You win!");
           
        }
    }
}
