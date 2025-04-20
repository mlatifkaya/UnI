using UnityEngine;

public class EggCollectible : MonoBehaviour, ICollectible
{
    private bool isCollected = false; // Flag to check if the egg has been collected

    public void Collect() // Implement the Collect method from ICollectible interface
    {
        if (isCollected) return; // Prevent double collection   
        isCollected = true; // Mark as collected

        GameManager.Instance.OnEggCollected(); // Notify the GameManager
        Destroy(gameObject);  // Destroy the egg object
    }
}
