using System;
using _2___Scripts.Global.Events;
using TMPro;
using UnityEngine;

namespace _2___Scripts.Global
{
    public class GameDataManager : MonoBehaviour
    {
        private GameDataManager()
        {
        }

        public static GameDataManager Instance { get; } = new();
        public static event Action<IBaseEvent> OnCollected;
    
        private TextMeshProUGUI CoinText;

        public int CurrentHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public int CurrentConcentrationSlots { get; private set; }
        public int MaxConcentrationSlots { get; private set; }
        public int CurrentEffort { get; private set; }
        public int MaxEffort { get; private set; }
        public int Coins { get; private set; }
        public string LastSavePoint  { get; private set; }
	
        private void Awake()
        {
            // if (Instance == null)
            // {
            //     Instance = this;
            // }
            /*else*/ if (Instance != this)
            {
                Destroy(gameObject);
            }

            CoinText = GameObject.Find("CoinCounter").GetComponent<TextMeshProUGUI>();
            DontDestroyOnLoad (gameObject);
            OnPlayerEnteredEvent.OnPlayerEntered += OnPlayer_Entered;
        }
    
        private void OnPlayer_Entered(IOnPlayerEnteredEvent obj)
        {
            switch (obj.EventName)
            {
                case "CoinCollected":
                    GainCoins(obj.NumericData);
                    obj.Remove();
                    // Debug.Log("COINY: " + Coins);
                    CoinText.text = Coins.ToString();
                    break;
            }
        }

        public void LoadFromSave2(SaveDataSchema save)
        {
            MaxHealth = save.MaxHealth;
            CurrentHealth = save.CurrentHealth;
        }
    
        public void LoadFromSave(SaveDataSchema save)
        {
            MaxHealth = CurrentHealth = save.MaxHealth;
            CurrentEffort = MaxEffort = save.MaxMana;
            CurrentConcentrationSlots = MaxConcentrationSlots = save.MaxConcentrationSlots;
            LastSavePoint = save.SavePoint;
            Coins = save.Coins;
        }

        public bool TakeDamage(int amount)
        {
            if (amount >= CurrentHealth)
            {
                CurrentHealth = 0;
                return false;
            }
            // CurrentHealth = Math.Min(0, CurrentHealth - amount);

            CurrentHealth -= amount;
            return true;
        }

        public void GainCoins(int coins = 1) => Coins += coins;
    }
}
