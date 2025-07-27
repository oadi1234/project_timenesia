using _2_Scripts.Global;
using UnityEngine;

public class OnCollisionSpawner : MonoBehaviour
{

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Layers layer;

    private GameObject spawnInstance;
    private Collider2D collider2d;

    private void Start()
    {
        collider2d = GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //when trigger
        if (!other.gameObject.layer.Equals((int)layer)) return;
        InitializePrefabOnCollision(other.gameObject.transform.position);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        //when rigidbody + kinematic collider
        if (!other.gameObject.layer.Equals((int)layer)) return;
        
        InitializePrefabOnCollision(other.contacts[0]);
        collider2d.enabled = false;
    }

    private void InitializePrefabOnCollision(Vector3 position)
    {
        SpawnPrefabAtPosition(position);
    }

    private void InitializePrefabOnCollision(ContactPoint2D contact)
    {
        SpawnPrefabAtPosition(contact.point);
    }

    private void SpawnPrefabAtPosition(Vector3 position)
    {
        //TODO
        // I presume most of the stuff the spawned on collision instance would want to do should be in its own script
        // but certain things might need to be handled here, such as passing correct vectors for shockwaves etc.
        spawnInstance = Instantiate(spawnPrefab, position, Quaternion.identity);
    }
}