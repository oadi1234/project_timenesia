using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveDataSchema
{
   // private static SaveDataSchema _instance = null;
   //
   // public static SaveDataSchema DataSchema
   // {
   //    get
   //    {
   //       if (_instance == null)
   //          _instance = SaveManager.Instance.Load();
   //
   //       return _instance;
   //    }
   // }
   
   public int CurrentHealth { get; set; }
   public int MaxHealth { get; set; }
   // public int CurrentMana { get; set; }
   public int MaxMana { get; set; }
   public int MaxConcentrationSlots { get; set; }
   
   public string SavePoint { get; set; }
   public int Coins { get; set; }

    //current scene (for manual saving)
    //current location

   // public SaveDataSchema(int t, int t2)
   // {
   //    CurrentHealth = t;
   //    testInt2 = t2;
   // }
   //
   // public SaveDataSchema()
   // {
   //    CurrentHealth = testInt2 = 0;
   // }
}
