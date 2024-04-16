using System;
using _2_Scripts.Global;
using UnityEngine;

namespace _2_Scripts.Enemies.Agents
{
    public class RoundWalkerAgent : DynamicEnemyBase
    {
        private Direction _currentDirection = Direction.RIGHT;
        private bool _rotate = false;
        private bool _wallHit = false;

        protected override void Awake()
        {
            base.Awake();
        }

        private void FixedUpdate()
        {
            if (ShouldRotateAroundCorner()) {
                transform.Rotate(0, 0, _wallHit ? 90 : -90);
                // transform.Rotate(Vector2.up, RotationSpeed * Time.deltaTime);
                Flip();
                _rotate = false;
                _wallHit = false;
            }
            else
            {
                var velo = _currentDirection switch
                {
                    Direction.LEFT => new Vector2(-1, 0),
                    Direction.RIGHT => new Vector2(1, 0),
                    Direction.UP => new Vector2(0, 1),
                    Direction.DOWN => new Vector2(0, -1),
                    _ => new Vector2()
                };

                RigidBody.velocity = velo;
            }
        }

        private bool ShouldRotateAroundCorner()
        {
            return _rotate || _wallHit;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.layer is (int) Layers.Wall or (int) Layers.Default)
            {
                _rotate = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer is (int) Layers.Wall or (int) Layers.Default)
                _wallHit = true;
        }

        // private void OnCollisionEnter2D(Collision2D other)
        // {
        //     _rotate = true;
        // }
        // private void OnCollisionExit2D(Collision2D other)
        // {
        //     switch (_currentDirection)
        //     {
        //         case Direction.LEFT:
        //             _currentDirection = Direction.UP;
        //             Flip();
        //             break;
        //         case Direction.RIGHT:
        //             _currentDirection = Direction.DOWN;
        //             Flip();
        //             break;
        //         case Direction.UP:
        //             _currentDirection = Direction.RIGHT;
        //             Flip();
        //             break;
        //         case Direction.DOWN:
        //             _currentDirection = Direction.LEFT;
        //             Flip();
        //             break;
        //     }
        // }

        private void Flip()
        {
            // transform.Rotate(0, 0, -45);
            if (_wallHit)
                switch (_currentDirection)
                {
                    case Direction.LEFT:
                        _currentDirection = Direction.DOWN;
                        break;
                    case Direction.RIGHT:
                        _currentDirection = Direction.UP;
                        break;
                    case Direction.UP:
                        _currentDirection = Direction.LEFT;
                        break;
                    case Direction.DOWN:
                        _currentDirection = Direction.RIGHT;
                        break;
                }
            else
                switch (_currentDirection)
                {
                    case Direction.LEFT:
                        _currentDirection = Direction.UP;
                        break;
                    case Direction.RIGHT:
                        _currentDirection = Direction.DOWN;
                        break;
                    case Direction.UP:
                        _currentDirection = Direction.RIGHT;
                        break;
                    case Direction.DOWN:
                        _currentDirection = Direction.LEFT;
                        break;
                }
        }
    }
}