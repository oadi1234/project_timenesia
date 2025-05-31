using _2_Scripts.Player;
using _2_Scripts.Player.Statistics;
using UnityEngine;

namespace _2_Scripts.Global.Spells
{
    public class ShieldSpellHandler : MonoBehaviour
    {
        [SerializeField] private GameObject shieldEnd;
        [SerializeField] private GameObject shieldDust;
        [SerializeField] private GameObject shieldInterrupt;
        private GameObject stageInstance;
        private PlayerHealth playerHealth;
        private PlayerInputManager playerInputManager;

        private int shieldIntensity = 1;

        private void Awake()
        {
            playerHealth = transform.parent.GetComponent<PlayerHealth>();
            playerInputManager = transform.parent.GetComponent<PlayerInputManager>();
            playerHealth.Damaged += InterruptAnimation;
            playerInputManager.InputReceived += InterruptAnimation;
        }

        public void SetShieldIntensity(int intensity)
        {
            shieldIntensity = intensity;
        }

        public void StartEndAnimation()
        {
            stageInstance = Instantiate(shieldEnd, transform.parent);
            stageInstance.transform.localScale = transform.localScale;
            playerHealth.Damaged -= InterruptAnimation;
            playerInputManager.InputReceived -= InterruptAnimation;
            playerHealth.AddShield(shieldIntensity);
            Destroy(gameObject);
        }

        public void StartDustAnimation()
        {
            stageInstance = Instantiate(shieldDust, transform.parent);
            stageInstance.transform.localScale = transform.localScale;
        }

        private void InterruptAnimation()
        {
            stageInstance = Instantiate(shieldInterrupt, transform.parent);
            stageInstance.transform.localScale = transform.localScale;
            playerHealth.Damaged -= InterruptAnimation;
            playerInputManager.InputReceived -= InterruptAnimation;
            Destroy(gameObject);
        }
    }
}