using System;
using _2_Scripts.Player.Animation.model;
using UnityEngine;

namespace _2_Scripts.UI.Animation
{
    //TODO it might be worthwhile to implement separate AnimatorHandler for these states, and then remove
    // unneeded garbage like everything below GetCurrentState.
    public abstract class AbstractGUIPointStateHandler<T> : MonoBehaviour, IGUIPointStateHandler<T> where T : Enum
    {
        protected int CurrentState;

        public abstract void SetCurrentState(T effortType);

        public int GetCurrentState()
        {
            return CurrentState;
        }

        public int GetCurrentHurtState() => AC.None;
        public bool LockXFlip() => false;
        public bool ShouldRestartAnim() => false;
        public bool IsOverrideDirection() => false;
        public bool IsLeftOverride() => true;
    }
}