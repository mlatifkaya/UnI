using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerVisualTransform;
    private PlayerController _playercontroller;
    private Rigidbody _playerRigidbody;
    private void Awake() {
        _playercontroller = GetComponent<PlayerController>();
        _playerRigidbody = GetComponent<Rigidbody>();
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

    private void OnParticleCollision(GameObject other) 
    {
        if(other.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.GiveDamage(_playerRigidbody, _playerVisualTransform);
        }
        
    }
}
