using _2_Scripts.Global.Animation.Model;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.Global.Animation
{
    public abstract class AbstractStateHandler : MonoBehaviour, IStateHandler
    {
        public abstract int GetCurrentState();

        public virtual int GetCurrentHurtState()
        {
            return AC.None;
        }

        public virtual bool LockXFlip()
        {
            return false;
        }

        public virtual bool ShouldRestartAnim()
        {
            return false;
        }
    }
}