using System;
using _2_Scripts.ExtensionMethods;
using _2_Scripts.Global;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _2_Scripts.Enemies.Agents
{
    public class AllRounderAgent : DynamicEnemyBase
    {
        private bool _done = false;
        private bool _reverseAngle = false;
        private float _angle;
        private Vector2 _force;
        private void FixedUpdate()
        {
            if (!_done)
                RigidBody.velocity = new Vector2(0,-1);
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            _done = true;
            
            if (other.gameObject.layer is (int) Layers.Wall or (int) Layers.Default)
            {
                // Print how many points are colliding with this transform
                Debug.Log("Points colliding: " + other.contacts.Length);

                // Print the normal of the first point in the collision.
                Debug.Log("Normal of the first point: " + other.contacts[0].normal);

                // // Draw a different colored ray for every normal in the collision
                // foreach (var item in other.contacts)
                // {
                //     Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
                // }
                Debug.DrawRay(other.contacts[0].point, other.contacts[0].normal * 100,
                    Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
                var gravityForce = new Vector2(other.contacts[0].normal.x, other.contacts[0].normal.y) * -0.5f;
                // var moveForce = other.contacts[0].normal.PerpendicularClockwise();
                // var moveForce = Vector2.Perpendicular(gravityForce);
                // var moveForce = new Vector2(1, -1);
                // var resultForce = gravityForce * moveForce;
                _force = new Vector2(gravityForce.y * -1, gravityForce.x);
                // RigidBody.AddForce(gravityForce, ForceMode2D.Impulse);

                var angle = Vector2.Angle(Vector2.up,other.contacts[0].normal);
                
                if (angle > _angle)
                    _reverseAngle = false;
                else if (angle < _angle)
                    _reverseAngle = true;

                _angle = angle;
                
                RigidBody.MoveRotation(-(_reverseAngle ? 180 + (180 - _angle) : _angle));
                // RigidBody.MoveRotation(toRotation);
                // RigidBody.MoveRotation(Quaternion.FromToRotation(Vector3.up, other.contacts[0].normal));
                RigidBody.velocity = _force;
            }
        }

        // private void OnCollisionExit2D(Collision2D other)
        // {
        //     if (other.gameObject.layer is (int) Layers.Wall or (int) Layers.Default)
        //         RigidBody.velocity = new Vector2(-RigidBody.velocity.y, -RigidBody.velocity.x);
        // }
    }
}
