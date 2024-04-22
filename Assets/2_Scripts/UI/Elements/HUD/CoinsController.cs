using _2_Scripts.Global;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace _2_Scripts.UI.Elements.HUD
{
    public class CoinsController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI counterText;
        public int Coins { get; private set; }

        public static CoinsController Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            
            Initialize();
        }

        private void Initialize()
        {
            SetCoins(GameDataManager.Instance.currentGameData.Coins);
        }
        
        public void AddCoins(int numberOfCoins)
        {
            SetCoins(Coins + numberOfCoins);
        }

        public void SubtractCoins(int numberOfCoins)
        {
            SetCoins(math.max(0, Coins - numberOfCoins));
        }

        public void SetCoins(int numberOfCoins)
        {
            Coins = numberOfCoins;
            counterText.text = Coins.ToString();
        }

        public bool HasEnoughCoins(int numberOfCoins) => numberOfCoins >= Coins;
    }
}