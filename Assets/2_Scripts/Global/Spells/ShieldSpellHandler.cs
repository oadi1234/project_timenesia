using _2_Scripts.Player;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class ShieldSpellHandler : AbstractInterruptibleSpell
    {
        [SerializeField] private GameObject shieldEnd;
        [SerializeField] private GameObject shieldDust;
        private GameObject stageInstance;
        private PlayerInputManager playerInputManager;

        private int shieldIntensity = 1;

        protected override void Awake()
        {
            playerInputManager = transform.GetComponentInParent<PlayerInputManager>();
            base.Awake();
        }

        public void SetShieldIntensity(int intensity)
        {
            shieldIntensity = intensity;
        }

        public void StartEndAnimation()
        {
            stageInstance = Instantiate(shieldEnd, transform.parent);
            stageInstance.transform.localScale = transform.localScale;
            DeregisterEvents();
            PlayerHealth.AddShield(shieldIntensity);
            Destroy(gameObject);
        }

        public void StartDustAnimation()
        {
            stageInstance = Instantiate(shieldDust, transform.parent);
            stageInstance.transform.localScale = transform.localScale;
        }

        protected override void DeregisterEvents()
        {
            base.DeregisterEvents();
            if (playerInputManager)
                playerInputManager.InputReceived -= InterruptAnimation;
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();
            if (playerInputManager)
                playerInputManager.InputReceived += InterruptAnimation;
        }
    }
}