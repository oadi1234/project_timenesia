using System;

namespace _2_Scripts.Global.SaveSystem.SaveDataSchemas
{
    [Serializable]
    public class PreviewDataSchema
    {
        public PreviewDataSchema()
        {

        }
        //for json magic
        public PreviewDataSchema(PreviewDataSchema preview)
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
        public float savePointX;
        public float savePointY;
        public ZoneEnum zone; //might actually replace scene name if a proper dictionary with ZoneEnum to Scene Name is implemented. No clue if it's really necessary though.
        public int MaxHealth;
        public int MaxEffort;
        public int SpellCapacity;
        public string gameVersion;
    }
}
