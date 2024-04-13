public enum TextTransitionType
{
    Simple = 0,         //Text just appears
    EnlargeReduce = 1,  //Text gets larger, from a set size to a set size
    Fade = 2,           //Text increases its alpha from original color maximum to a minimum of magnitude.
    Chaotic = 3,        //Text randomly switches between other transition types. Might be a bit goofy but could fit more chaotic characters.
    ScrollDown = 4,     //Text appears from above. TODO Probably unused - seems a bit goofy, but stays as an idea for now.
    ScrollUp = 5,       //Text appears from below. TODO Probably unused - seems a bit goofy, but stays as an idea for now.
    FadeReduce = 6,     //Text starts big but invisible, gets reduced to normal size while slowly becoming visible
    FadeEnlarge = 7,    //Text starts small but invisible, gets enlarged to normal size while slowly becoming visible
}
