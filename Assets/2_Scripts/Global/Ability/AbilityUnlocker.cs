using _2___Scripts.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUnlocker : MonoBehaviour
{
    [SerializeField]
    public AbilityName abilityName;

    public static event Action<AbilityName> OnAbilityUnlocked;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnAbilityUnlocked != null && collision.CompareTag("Player"))
        {
            OnAbilityUnlocked(abilityName);
            Destroy(gameObject);
        }

    }
}
