using _2_Scripts.Player.Animation.model;
using UnityEngine;

public class AttackAnimationStateHandler : MonoBehaviour, IStateHandler
{
    public int GetCurrentState()
    {
        return AC.StaffBasicSwoosh;
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
