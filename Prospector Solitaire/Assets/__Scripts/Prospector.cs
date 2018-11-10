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

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;

    void Awake(){
        S = this; // Set up a Singleton for Prospector    
    }

    void Start()
    {
        deck = GetComponent<Deck>(); // Get the Deck
        deck.InitDeck(deckXML.text); //Pass DeckXML to it
        Deck.Shuffle(ref deck.cards); // This shuffles the deck by reference

        //Card c;
        //for (int cNum = 0; cNum< deck.cards.Count; cNum++){
        //    c = deck.cards[cNum];
        //    c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        //}

        layout = GetComponent<Layout>();
        layout.ReadLayout(layoutXML.text);
    }
}
