
public class CharacterCommand
{
    public TextCommandType type { get; set; }
    public int index { get; set; }
    public float magnitude { get; set; } //for shake strength, wave length, pulse length or gradient length?
    public float operationTime; //used for per command calculations for commands like Pulse, FadeWave or Gradient. Usually calculates up to magnitude.
    public bool commandSwitch; //as above, allows for counting down.

    public CharacterCommand(TextCommand textCommand, int index)
    {
        type = textCommand.type;
        this.index = index;
        magnitude = textCommand.magnitude;
        operationTime = 0f;
    }
}
