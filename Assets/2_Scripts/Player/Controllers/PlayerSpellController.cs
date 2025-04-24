using System;
using UnityEngine;

namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        private Spellbook spellbook;
        private PlayerInputManager playerInputManager;
        private PlayerMovementController playerMovementController;
        private SpellType spellType;
        
        public event Action Spellcasted;

        #region Actions

        public void CastSpell(int spellIndex, bool isCasting)
        {
            // TODO change switch-case to dictionary<index, spelltype> lookup, it might be more easily expandable.
            if (isCasting)
            {
                switch (spellIndex)
                {
                    case 0:
                        spellType = SpellType.Bolt;
                        Spellcasted?.Invoke();
                        break;
                    case 1:
                        spellType = SpellType.Aoe;
                        Spellcasted?.Invoke();
                        break;
                    case 2:
                        spellType = SpellType.Heavy;
                        Spellcasted?.Invoke();
                        break;
                    default:
                        spellType = SpellType.None;
                        break;
                }
            }
        }

        // public void Test(int spellIndex)
        // {
        //     switch (spellIndex)
        //     {
        //         // TODO I know this is testing stuff, but now PMC is a bit unreliable for facing direction check.
        //         //  I think there are cases where movement controller thinks it looks left, while sprite does not.
        //         //  alternatively use a similar logic like in AnimationHandler.cs:43
        //         case 0:
        //             spellbook.CastFireBall(playerMovementController.IsFacingLeft() ? -1 : 1);
        //             break;
        //         case 1:
        //             spellbook.CastEyeBall(playerMovementController.IsFacingLeft() ? -1 : 1);
        //             break;
        //     }
        //     playerInputManager.SetInputEnabled(true);
        // }

        #endregion
        
        public SpellType GetSpellType()
        {
            return spellType;
        }
    }
}
