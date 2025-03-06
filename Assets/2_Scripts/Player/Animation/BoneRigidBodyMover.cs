using _2_Scripts.Player.Animation;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

//While this works, in a meantime I will try to make another implementation using Verlet Integration and skipping the
// unity physics system all together. (https://www.youtube.com/watch?v=FcnvwtyxLds)
// The main gain is that I can easily simulate the physics at lower fps, which is actually helpful for visual consistency
// (as most animations are 12fps), but also allows to add both stretching like with SpringJoint2d and limiting the angles
// like with HingeJoint2d.
// also, it will then become virtually impossible to keep the cloak/hair firmly upwards by balancing it like a pendulum
public class BoneRigidBodyMover : MonoBehaviour
{
    [SerializeField] private AnimationPivotHandler pivotHandler;
    [SerializeField] private AnimatorHandler animatorHandler;
    [SerializeField] private Transform pivotTransform;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _rigidbody.MoveRotation(pivotHandler.GetRotationAngle() - 90.619f);
        _rigidbody.MovePosition(pivotHandler.GetPivot());
        SetFacingDirection();
    }

    private void SetFacingDirection()
    {
        pivotTransform.localScale = new Vector3(pivotTransform.localScale.x, animatorHandler.IsFacingLeft() ? 1 : -1,
            pivotTransform.localScale.z);
    }
}