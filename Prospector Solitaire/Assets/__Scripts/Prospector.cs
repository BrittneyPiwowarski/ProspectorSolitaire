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
    public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
    public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
    public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
    public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
    public float reloadDelay = 2f; // 2 sec delay between rounds

    [Header("Set Dynamically")]
    public Deck deck;
    public Layout layout;
    public List<CardProspector> drawPile;
    public Transform layoutAnchor;
    public CardProspector target;
    public List<CardProspector> tableau;
    public List<CardProspector> discardPile;
    public FloatingScore fsRun;

    void Awake(){
        S = this; // Set up a Singleton for Prospector    
    }

    void Start()
    {
        Scoreboard.S.score = ScoreManager.SCORE;

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
    void LayoutGame() {

        if (layoutAnchor == null){
            GameObject tGO = new GameObject("_LayoutAnchor");
            layoutAnchor = tGO.transform; 
            layoutAnchor.transform.position = layoutCenter;
        }

        CardProspector cp;
        //Follow the layout
        foreach (SlotDef tSD in layout.slotDefs) {
            cp = Draw(); // Pull a card from the top (beginning) of the draw 
            cp.faceUp = tSD.faceUp; // Set its faceUp to the value in SlotDef
            cp.transform.parent = layoutAnchor;
            // Set the localPosition of the card based on slotDef
            cp.transform.localPosition = new Vector3(
                layout.multiplier.x * tSD.x, 
                layout.multiplier.y * tSD.y, 
                -tSD.layerID); 

            cp.layoutID = tSD.id;
            cp.slotDef = tSD;
            // CardProspectors in the tableau have the state CardState.tableau
            cp.state = eCardState.tableau; 

            cp.SetSortingLayerName(tSD.layerName); // Set the sorting layers
            tableau.Add(cp); // Add this CardProspector to the List<> tableau
        }

        //Set which cards are hiding others
        foreach (CardProspector tCP in tableau)  {
            foreach (int hid in tCP.slotDef.hiddenBy){
                cp = FindCardByLayoutID(hid);
                tCP.hiddenby.Add(cp);
            }
        }

        MoveToTarget(Draw()); // Set up the initial target card
        UpdateDrawPile(); // Set up the Draw pile
    }

    private CardProspector FindCardByLayoutID(int layoutID) {
        foreach (CardProspector tCP in tableau) {
            if (tCP.layoutID == layoutID) {
                return tCP;
            }
        }
        //If not found, return null
        return null;
    }

    // This turns cards in the mine face up or face down
    private void SetTableauFaces(){
        foreach (CardProspector cd in tableau){
            bool faceUp = true; // Assume the card will be faceup
            foreach (CardProspector cover in cd.hiddenby){
                // If either of the covering cards are in the tableau
                if (cover.state == eCardState.tableau){
                    faceUp = false; // Then this card is face down
                }
            }
            cd.faceUp = faceUp; // Set the value on the card
        }
    }

    // Moves the current target to this discardPile
    void MoveToDiscard(CardProspector cd){
        // Set the state of the card to discard
        cd.state = eCardState.discard;
        discardPile.Add(cd); // Add to the discard pile List<>
        cd.transform.parent = layoutAnchor; // Update its transform parent

        // Position this card on the discard pike
        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID + 0.5f);

        cd.faceUp = true;

        // Place it on top of the pile for depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(-100 + discardPile.Count);
    }

    // Make cd the new target card
    void MoveToTarget(CardProspector cd){
        // If there is currently a target card, move it to discardPile
        if (target != null) MoveToDiscard(target);
        target = cd; // cd is the new target
        cd.state = eCardState.target;
        cd.transform.parent = layoutAnchor;

        // Move to the target position
        cd.transform.localPosition = new Vector3(
            layout.multiplier.x * layout.discardPile.x,
            layout.multiplier.y * layout.discardPile.y,
            -layout.discardPile.layerID);

        cd.faceUp = true;
        // Set the depth sorting
        cd.SetSortingLayerName(layout.discardPile.layerName);
        cd.SetSortOrder(0);
    }

    // Arranges all of the cards in the drawPile to show how many are left
    void UpdateDrawPile(){
        CardProspector cd;
        // Go through all of the cards of the draw pile
        for (int i = 0; i < drawPile.Count; i++){
            cd = drawPile[i];
            cd.transform.parent = layoutAnchor;

            // Position it correctly with the layout.drawpile.stagger
            Vector2 dpStagger = layout.drawPile.stagger;

            cd.transform.localPosition = new Vector3(
                layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x), 
                layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y), 
                -layout.drawPile.layerID + 0.1f * i);

            cd.faceUp = false;
            cd.state = eCardState.drawpile;
            // Set depth of sorting
            cd.SetSortingLayerName(layout.discardPile.layerName);
            cd.SetSortOrder(-10*i);
        }
    }

    // Called any time a card is clicked in the game
    public void CardClicked(CardProspector cd){
        // The reaction is determined by the state of the clicked card
        switch (cd.state){
            case eCardState.target:
                // Clicking the target card does nothing
                break;

            case eCardState.drawpile:
                // Clicking any card in the drawpile will draw the next card
                MoveToDiscard(target);
                MoveToTarget(Draw());
                UpdateDrawPile();
                ScoreManager.EVENT(eScoreEvent.draw);
                FloatingScoreHandler(eScoreEvent.draw);
                break;

            case eCardState.tableau:
                // Clicking a card in the tableau will check if it's a valid play
                bool validMatch = true;
                // If the card is faced down, its not valid
                if (!cd.faceUp) { validMatch = false; }

                if (!AdjacentRank(cd, target)){
                    // If it's not an adjacent rank, its not valid
                    validMatch = false;
                }

                if (!validMatch) return; // Return if not valid

                tableau.Remove(cd); //  Remove it from the tableau List
                MoveToTarget(cd); // Make it the target card
                SetTableauFaces(); // Update tableau card face-ups
                ScoreManager.EVENT(eScoreEvent.mine);
                FloatingScoreHandler(eScoreEvent.mine);
                break;

        }

        // Check to see wether game is over or not
        CheckForGameOver();
    }

    // Check whether game is over
    private void CheckForGameOver(){
        // If tableau is empty, the game is over
        if (tableau.Count == 0){
            // Call game over with a win
            GameOver(true);
            return;
        }

        // If there are still cards in the drawpile, the game's not over
        if (drawPile.Count > 0){
            return;
        }

        // Check for remaining valid plays
        foreach (CardProspector cd in tableau){
            if (AdjacentRank(cd, target)){
                // If there's a valid play, the game is not over
                return;
            }
        }
        // Since there are no valid plays, the game is over
        GameOver(false);
    }

    // Called when the game is over
    void GameOver(bool won){
        if (won){
            // print("You won");
            ScoreManager.EVENT(eScoreEvent.gameWin);
            FloatingScoreHandler(eScoreEvent.gameWin);
        }
        else {
            //print("You lost");
            ScoreManager.EVENT(eScoreEvent.gameLose);
            FloatingScoreHandler(eScoreEvent.gameLose);
        }
        // Reload the scene, resetting the game
        //SceneManager.LoadScene("Prospector_Scene0");

        Invoke("ReloadLevel", reloadDelay);
    }

    void ReloadLevel(){
        // Reload the scene, resetting the game
        SceneManager.LoadScene("Prospector_Scene0");
    }

    // Return true if the two cards are adjacent in rank (A & K wraparound)
    public bool AdjacentRank(CardProspector c0, CardProspector c1) {
        //If either card is face down, it's not adjacent
        if (!c0.faceUp || !c1.faceUp) return (false);

        // If they are one apart, they are adjacent
        if (Mathf.Abs(c0.rank - c1.rank) == 1) {
            return (true);
        }

        // If one is an Ace and the other King, they are adjacent
        if (c0.rank == 1 && c1.rank == 13) return (true);
        if (c0.rank == 13 && c1.rank == 1) return (true);

        //Otherwise, return false
        return (false);
    }

    // Handle FLoatingScore movement
    void FloatingScoreHandler(eScoreEvent evt){
        List<Vector2> fsPts;

        switch (evt){
            // Smae things need to happen wether its a draw, a win, or a loss
            case eScoreEvent.draw:
            case eScoreEvent.gameWin:
            case eScoreEvent.gameLose:
                // Add fsRun to the Scoreboard score
                if (fsRun != null){
                    // Create points for the Bezier curve
                    fsPts = new List<Vector2>();
                    fsPts.Add(fsPosRun);
                    fsPts.Add(fsPosMid2);
                    fsPts.Add(fsPosEnd);
                    fsRun.reportFinishTo = Scoreboard.S.gameObject;
                    fsRun.Init(fsPts, 0, 1);
                    // Also adjust the fontSize
                    fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
                    fsRun = null;
                }
                break;

            case eScoreEvent.mine:
                // Create a FloatingScore for this score
                FloatingScore fs;
                // Move it from the mosePosition to fsPosRun
                Vector2 p0 = Input.mousePosition;

                p0.x /= Screen.width;
                p0.y /= Screen.height;
                fsPts = new List<Vector2>();
                fsPts.Add(p0);
                fsPts.Add(fsPosMid);
                fsPts.Add(fsPosRun);
                fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);
                fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });
                if (fsRun == null){
                    fsRun = fs;
                    fsRun.reportFinishTo = null;
                }
                else{
                    fs.reportFinishTo = fsRun.gameObject;
                }
                break;
        }
    }
}
