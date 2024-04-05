using System;

[Serializable]
public class PreviewStatsDataSchema
{
    public PreviewStatsDataSchema()
    {

    }

    public PreviewStatsDataSchema(PreviewStatsDataSchema preview)
    {
        sceneName = preview.sceneName;
        savePointX = preview.savePointX;
        savePointY = preview.savePointY;
        zone = preview.zone;
        MaxHealth = preview.MaxHealth;
        MaxEffort = preview.MaxEffort;
        gameVersion = preview.gameVersion;
    }
    
    public string sceneName { get; set; } //scene into which load the player.
    public float savePointX { get; set; }
    public float savePointY { get; set; }
    public ZoneEnum zone { get; set; } //might actually replace scene name if a proper dictionary with ZoneEnum to Scene Name is implemented. No clue if it's really necessary though.
    public int MaxHealth { get; set; }
    public int MaxEffort { get; set; }
    public int gameVersion { get; set; }
}
