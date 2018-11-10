using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eCardState{
    drawpile,
    tableau,
    target,
    discard
}

public class CardProspector : Card { // CardProspector extends Card

    [Header("Set Dynamically: CardProspector")]
    public eCardState state = eCardState.drawpile;
    // This hiddenBy list stores which other cards will keep this one face down
    public List<CardProspector> hiddenby = new List<CardProspector>();
    // This layoutID matches this card to the tablear XML if its a tableau card
    public int layoutID;
    // The SlotDef class stores info pulled from the Layout XML <slot>
    public SlotDef slotDef;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
