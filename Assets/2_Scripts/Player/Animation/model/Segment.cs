using UnityEngine;

public class Segment : MonoBehaviour
{
    public Vector2 currentPosition { get; set; }
    public Vector2 oldPosition { get; set; }
    public float distanceToParent { get; set; }
    public float rotation { get; set; }
    private Vector2 startingLocalCoordinates { get; set; }

    private void Awake()
    {
        startingLocalCoordinates = transform.localPosition;
    }

    public void SetLocalPositionToStartingLocalCoordinates()
    {
        transform.localPosition = startingLocalCoordinates;
    }
}