using System;
using System.Collections;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class BeamSpellHandler : MonoBehaviour
    {

        [SerializeField] private PolygonCollider2D wandBlastCollider;

        [SerializeField] private GameObject beamHitBlast;
        
        [SerializeField] private float destroyAfterSeconds = 0.25f;
        [SerializeField] private float activeTimer = 0.125f;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private LineRenderer lineRenderer;

        private float beamLength;
        private RaycastHit2D hit;
        private float distance;
        private int linePositionCount;
        private Vector3[] startingPoints;
        private Vector3[] points;
        private bool alreadyHitSomething = false;
    
        private Vector2 beamDirection = Vector2.zero;
        private Vector2 absCoordBeamDirection = Vector2.zero;
        private GameObject beamHitBlastInstance;
        
        // Start is called before the first frame update
        private IEnumerator Start()
        {
            linePositionCount = lineRenderer.positionCount;  //Each line has multiple points so it can bend.
            startingPoints = new Vector3[linePositionCount]; // we need to remember each line "max" position
            points = new Vector3[linePositionCount];         // so we can scale each line individually if it collides,
            lineRenderer.GetPositions(startingPoints);       // or it extends due to no longer colliding.
            lineRenderer.GetPositions(points);
            beamLength = startingPoints[linePositionCount - 1].magnitude;
            GetLineLength(); //prevents first frame from extending through collision.
            yield return new WaitForSeconds(destroyAfterSeconds);
            Destroy(gameObject);
        }

        private void Update()
        {
            if (activeTimer <= 0 || alreadyHitSomething) return;
            activeTimer -= Time.deltaTime;
            GetLineLength();
            Debug.DrawRay(transform.position, beamDirection * beamLength, Color.red);
        }

        public void SetDirection(Vector2 direction)
        {
            beamDirection = direction;
            beamDirection.x *= -1; // I may have spaghetti'd a little.
            absCoordBeamDirection.Set(Math.Abs(beamDirection.x), beamDirection.y);
        }

        private void GetLineLength()
        {
            hit = Physics2D.Raycast(transform.position, beamDirection, beamLength, layerMask);
            if (hit)
            {
                alreadyHitSomething = true;
                distance = (hit.point - (Vector2)transform.position).magnitude;
                InstantiateHitBlast();
            }
            else distance = beamLength;
            
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = startingPoints[i] * distance / beamLength; 
            }
            lineRenderer.SetPositions(points);
        }

        private void InstantiateHitBlast()
        {
            beamHitBlastInstance = Instantiate(beamHitBlast, hit.point, Quaternion.identity);
            beamHitBlastInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, hit.normal);
        }
    }
}
