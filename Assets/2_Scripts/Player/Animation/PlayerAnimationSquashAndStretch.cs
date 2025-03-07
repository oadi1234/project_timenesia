using _2_Scripts.Player.Controllers;
using UnityEngine;

public class PlayerAnimationSquashAndStretch : MonoBehaviour
{
    
    // This class should do the following:
    //  when a player character moves quickly, for example dashes, but maybe not runs, or gets knocked back, the animation
    //  should stretch in the direction of movement and squash in the direction perpendicular to the movement.
    //  when the player character attacks a small outward bounce might also look good
    
    // the issue is the perpendicular part - it might not be as simple as scaling X or Y to make it look good, but I will see.
    
    
    
    // the thing below breaks stuff.
    // private void Squash()
    // {
    //     Vector3 x = playerMovementController.GetVelocityVector();
    //     Vector3 y = Vector3.zero;
    //     Vector3 z = Vector3.zero;
    //     
    //     Vector3.OrthoNormalize(ref x,ref y,ref z);
    //     
    //     Matrix4x4 matrix = new Matrix4x4();
    //     matrix.SetRow(0, x);
    //     matrix.SetRow(1, y);
    //     matrix.SetRow(2, z);
    //     matrix.SetRow(3, new Vector4(0,0,0,1));
    //     Matrix4x4 inverseMatrix = matrix.inverse;
    //
    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         var vertex = matrix.MultiplyPoint3x4(originalVertices[i]);
    //         vertex.x *= scale;
    //         
    //         vertices[i] = inverseMatrix.MultiplyPoint3x4(vertex);
    //     }
    //     
    //     sprite.OverrideGeometry(vertices, sprite.triangles);
    // }
}
