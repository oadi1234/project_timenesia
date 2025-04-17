using _2_Scripts.Player.Animation.model;
using UnityEngine;

public class AttackAnimationStateHandler : MonoBehaviour, IStateHandler
{
    public int GetCurrentState()
    {
        return AC.StaffBasicSwoosh;
    }

    public int GetCurrentHurtState()
    {
        return AC.None;
    }

    public bool LockXFlip()
    {
        return false;
    }

    public bool ShouldRestartAnim()
    {
        return false;
    }
}
