using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private PlayerController _playercontroller;
    private void Awake() {
        _playercontroller = GetComponent<PlayerController>();
    }
    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.TryGetComponent<ICollectible>(out var collectible))
        {
            collectible.Collect();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.TryGetComponent<IBoostable>(out var boostable))
        {
            boostable.Boost(_playercontroller);
        }
    }
}
