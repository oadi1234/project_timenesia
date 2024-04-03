using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A simplified Save Data Schema, used only for the saves.
public class SavePreview
{
    public ZoneEnum zone { get; set; }
    public int MaxHealth { get; set; }
    public int MaxEffort { get; set; }
    public int Coins { get; set; }
}
