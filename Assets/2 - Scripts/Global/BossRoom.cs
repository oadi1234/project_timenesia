using _2___Scripts.Global.Events;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    public GameObject[] gameObjects;
    private Collider2D _collider2D;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        Collector.OnCollected += OpenDoor;
    }

    private void OpenDoor(IBaseEvent obj)
    {
        foreach (var gameObject in gameObjects)
        {
            Destroy(_collider2D);
            var animator = gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("OpenTheDoor");
            }
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(_collider2D);
            foreach (var gameObject in gameObjects)
            {
                var animator = gameObject.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("PlayerEnteredTheRoom");
                }
            }
        }
    }
}

