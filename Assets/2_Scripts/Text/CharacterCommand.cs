
public class CharacterCommand
{
    public TextCommandType type { get; set; }
    public int index { get; set; }
    public float magnitude { get; set; } //for shake variance, wave length, pulse length or gradient length?

    public CharacterCommand(TextCommand textCommand, int index)
    {
        type = textCommand.type;
        this.index = index;
        magnitude = textCommand.magnitude;
    }
}
