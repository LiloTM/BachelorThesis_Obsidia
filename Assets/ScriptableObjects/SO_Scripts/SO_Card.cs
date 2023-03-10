using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType { Humanoid, Elemental, Celestial, Fiend, Gate, Fey }
public enum CardColour { Blue, Green, Red, Purple, Special }

public class SO_Card : ScriptableObject
{
    public new string name = "Card";
    public int index;
    public int value = 0;
    public CardType cardType;
    public CardColour colour;
}
