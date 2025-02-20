using UnityEngine;

//Renamed from AttackController to SpellController to better show what it does.
namespace _2_Scripts.Player.Controllers
{
    public class PlayerSpellController : MonoBehaviour
    {
        //private Animator animator; //anything related to animator needs to be tweaked or redone. I.e. you cannot simply get Animator component from children.
        private Spellbook spellbook; // it should not be here. Spells are going to be a distinct thing from normal attacks. Left for now to avoid it crapping all over the compilation
        private PlayerInputManager playerInputManager;
        private PlayerMovementController playerMovementController;

        private static readonly int AttackBoltTrigger = Animator.StringToHash("AttackBoltTrigger");
        private static readonly int AttackSelfAoETrigger = Animator.StringToHash("AttackSelfAoETrigger");

        private void Awake()
        {
            //animator = GetComponentInChildren<Animator>();

        }

        #region Actions

        public void Attack(int spellIndex)
        {
            switch (spellIndex)
            {
                case 0:
                    Attack_Bolt();
                    break;
                case 1:
                    Attack_SelfAoE();
                    break;
            }
        }

        private void Attack_Bolt()
        {
            //animator.SetTrigger(AttackBoltTrigger);
        }

        private void Attack_SelfAoE()
        {
            //animator.SetTrigger(AttackSelfAoETrigger);
        }

        public void Test(int spellIndex)
        {
            switch (spellIndex)
            {
                case 0:
                    spellbook.CastFireBall(playerMovementController.IsFacingLeft() ? -1 : 1);
                    break;
                case 1:
                    spellbook.CastEyeBall(playerMovementController.IsFacingLeft() ? -1 : 1);
                    break;
            }
            playerInputManager.CastingAnimationFinished();
            playerInputManager.SetInputEnabled(true);
        }

        #endregion
    }
}
