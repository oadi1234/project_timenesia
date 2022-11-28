using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance = null;

    public int CurrentHealth { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentConcentrationSlots { get; private set; }
    public int MaxConcentrationSlots { get; private set; }
    public int CurrentMana { get; private set; }
    public int MaxMana { get; private set; }
    public string LastSavePoint  { get; private set; }
	
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
        DontDestroyOnLoad (gameObject);
    }

    public void LoadFromSave2(SaveDataSchema save)
    {
        MaxHealth = save.MaxHealth;
        CurrentHealth = save.CurrentHealth;
    }
    
    public void LoadFromSave(SaveDataSchema save)
    {
        MaxHealth = CurrentHealth = save.MaxHealth;
        CurrentMana = MaxMana = save.MaxMana;
        CurrentConcentrationSlots = MaxConcentrationSlots = save.MaxConcentrationSlots;
        LastSavePoint = save.SavePoint;
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
}
