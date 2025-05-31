using System;

namespace _2_Scripts.Player.Animation.model
{
    public interface IGUIPointStateHandler<T> : IStateHandler where T : Enum
    {
        public void SetCurrentState(T state);

    }
}