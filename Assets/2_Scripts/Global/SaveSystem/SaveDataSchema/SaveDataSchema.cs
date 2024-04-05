using System;


[Serializable]
public class SaveDataSchema
{
    public PreviewStatsDataSchema previewStatsDataSchema { get; set; }
    public GameStateSaveDataSchema gameStateSaveDataSchema { get; set; }

    public SaveDataSchema(PreviewStatsDataSchema previewStatsDataSchema, GameStateSaveDataSchema gameStateSaveDataSchema)
    {
        this.previewStatsDataSchema = previewStatsDataSchema;
        this.gameStateSaveDataSchema = gameStateSaveDataSchema;
    }
}
