using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
    [Header("Set Dynamically")]
    public string suit; // SUit of the card (C, D, H, or S)
    public int rank; // Rank of the card (1-14)
    public Color color = Color.black; // color to tink pips
    public string colS = "Black"; // or "Red". Name of the color

    // This list holds all of the Decorator GaeObjects
    public List<GameObject> decoGOs = new List<GameObject>();
    // This list holds all of the Pip GameObjects
    public List<GameObject> pipGOs = new List<GameObject>();

    public GameObject back; // The GameObject of the back of the card
    public CardDefinition def; // Parsed from the DeckXML.xml
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

