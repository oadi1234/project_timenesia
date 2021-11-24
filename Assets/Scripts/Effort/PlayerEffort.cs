using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffort : MonoBehaviour
{
    public int maxEffort = 4;
    public int currentEffort = 0;
    public int spellCapacity = 2;

    public float regenInterval = 0.5f;
    public int effortPerInterval = 1;
    public EffortBar effortBar;

    private float currentTime = 0f;

    void Start()
    {
        effortBar.Initialize();
        effortBar.SetMaxMana(maxEffort, false);
        effortBar.SetSpellCapacity(spellCapacity);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UseEffort(1);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UseEffort(2);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UseEffort(3);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentEffort < maxEffort)
        {
            currentTime += Time.fixedDeltaTime;
            if(currentTime > regenInterval)
            {
                currentTime = 0f;
                RecoverEffort(effortPerInterval);
            }
        }
    }

    
    public void UseEffort(int cost)
    {
        if (currentEffort - cost < 0)
            currentEffort = 0;
        else
            currentEffort -= cost;
        effortBar.SetCurrentMana(currentEffort);
    }

    //Freeze mana will sometimes keep rightmost mana points coloured - they cannot be used and
    //are not recovering. Possibly used when a persistent effect is present (i.e. shield from force element).
    public void FreezeMana(int cost)
    {

    }

    public void RecoverEffort(int amount)
    {
        currentEffort += amount;
        if (currentEffort > maxEffort)
            currentEffort = maxEffort;
        effortBar.SetCurrentMana(currentEffort);
    }
}
