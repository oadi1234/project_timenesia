using System;

[Serializable]
public enum TextCommandType
{ 
    None = 0,       //Text stays simple. By default does not need command.
    Shake = 1,      //Text shakes a little
    Wave = 2,       //Text waves in a sinusoidal pattern. TODO most likely unused - seems a bit goofy.
    FadeWave = 3,   //Text has a wave of reduced alpha value travelling through it.
    Pulse = 4,      //Text pulses in size. TODO might be unused, but might fit mechanical entities?
    Gradient = 5    //Text cycles through a list of colours.
}
