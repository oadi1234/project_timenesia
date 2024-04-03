using System;

[Serializable]
public enum ZoneEnum
{
    //A zone enum for the UI, so the saves know where they were done.
    //Mostly used to load correct save + background graphic.
    //In the future might be more granulated, i.e. done per save instead of per zone.
    None = 0,
    OldDunheim = 10,
    //OldDunheim_Wartweed = 11
    //OldDunheim_Fountain = 12
    //OldDunheim_TowerOfRuin = 13 etc.
    Bloodbloom = 20,
    Ackerland = 30, //dunheim floating gardens and fields
    DunheimSlum = 40, //lower city + canals
    DunheimPromenade = 50, //comercial district, aristocratic district + hub
    DunheimMines = 60, //floating mines
    Mechanicarium = 70,
    MemoransWorkshop = 80,
    HavonsTower = 90,
    Corpse = 100,
    Vulnus = 110,
    LowDunheim = 120,
    DwarvenCity = 130,
    Embassy = 140,
    Rift = 150, //Giant hole between OldDunheim and Bloodbloom. Might not be a zone in the final game.
    LibraryOfMemories = 160

}
