using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour {
    static public Prospector S;

    [Header("Set in Inspector")]
    public TextAsset deckXML;
    public TextAsset layoutXML;
    public float xOffset = 3;
    public float yOffset = -2.5f;
    public Vector3 layoutCenter;

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;

    void Awake(){
        S = this; // Set up a Singleton for Prospector    
    }

    void Start()
    {
        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); //Pass DeckXML to it
        Deck.Shuffle(ref deck.cards); // This shuffles the deck by reference

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);

        drawPile = ConvertListCardsTOListCardProspectors(deck.cards);
        LayoutGame();
    }

    List <CardProspector> ConvertListCardsTOListCardProspectors(List<Card> lCD){
        List<CardProspector> lCP = new List<CardProspector>();
        CardProspector tCP;
        foreach (Card tCD in lCD){
            tCP = tCD as CardProspector;
            lCP.Add(tCP);
        }
        return lCP;
    }

    // The Draw function will pull a single card from the drawPile and return it
    CardProspector Draw(){
        CardProspector cd = drawPile[0];
        drawPile.RemoveAt(0);
        return cd;
    }

    // LayoutGame() positions the initial tableau of cards, AKA the "Mine"
    void LayoutGame(){
        // Create an empty GameObject to serve as an anchor for the tableau
        if (layoutAnchor == null){
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform;
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        // Follow the layout
        foreach (SlotDef tSD in layout.slotDefs){ 
            cp = Draw(); // Pull a card from the top (beginning) of the draw pile
            cp.faceUp = tSD.faceUp; // Set its faceUp to the value in SlotDef
            cp.transform.parent = layoutAnchor; // make its parent layoutAnchor
            // Set the localPosition of the card based on slotDef
            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x,
                layout.multiplier.y * tSD.y,
                -tSD.layerID);
            cp.layoutID = tSD.id;
            cp.slotDef = tSD;

            // CardProspectors in the tableau have the state CardState.tableau
            cp.state = eCardState.tableau;

            // CardProspectors in the tableau have the state CardState.tableau
            cp.SetSortingLayerName(tSD.layerName);

            tableau.Add(cp);
        }
    }
}
