using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
	// This will be defined later
}

[System.Serializable] // A serializable class is able to be edited in the Inspector
public class Decorator {
    // THis class stores information about each decorator or pip from DeckXML
    public string type; // For card pops, type = "pip"
    public Vector3 loc; // The location of the Sprite on the Card
    public bool flip = false; // Wether to flip the sprite vertically
    public float scale = 1f; // The scale of the sprite
}

[System.Serializable]
public class CardDefinition{
    // This class stores information for each rank of card
    public string face; // Sprite to use for each face card
    public int rank; // The rank (1-13) of this card
    public List<Decorator> pips = new List<Decorator>(); // Pips used
}

