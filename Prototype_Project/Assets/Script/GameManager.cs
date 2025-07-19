using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI.Table;

public class GameManager : MonoBehaviour
{

    //public 
    public static GameManager Instance;
    public List<Sprite> cardFaceSprites;
    public Transform cardGrid;
    public GameObject cardPrefab;
    public int rows = 2;
    public int cols = 3;
    public Text scoreText;
    public Text TurnsText;
    public GameObject GameOverPanel;


    // private
    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();

    private int score = 0;
    private int Turns = 0;

    private GridLayoutGroup gridLayoutGroup;

    // combo 
    private int comboCount = 0;
    private int comboMultiplier = 1;


    void Awake() 
    { 
        Instance = this; 
    }

    void Start() 
    {
        gridLayoutGroup = cardGrid.GetComponent<GridLayoutGroup>();
        ConfigureGridToFit();
        SetupCards(); 
    }

    public void ConfigureGridToFit()
    {
        RectTransform rt = cardGrid.GetComponent<RectTransform>();

        float spacing = 10f; // desired spacing between cards
        float padding = 20f; // optional padding inside the grid

        float totalSpacingX = spacing * (cols - 1);
        float totalSpacingY = spacing * (rows - 1);

        float availableWidth = rt.rect.width - padding * 2 - totalSpacingX;
        float availableHeight = rt.rect.height - padding * 2 - totalSpacingY;

        float cellWidth = availableWidth / cols;
        float cellHeight = availableHeight / rows;

        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayoutGroup.constraintCount = cols;

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }

    void SetupCards()
    {
      


        int totalPairs = (rows * cols)/2 ;
       

        // Safety check: Ensure you have enough sprites to match pair
        if (cardFaceSprites.Count < totalPairs)
        {
            int count = cardFaceSprites.Count;
            print("Not enough card face sprites assigned!");
            // Expand sprite list 
            int i = 0;
            while (cardFaceSprites.Count < totalPairs)
            {
                cardFaceSprites.Add(cardFaceSprites[i % count]);
                i++;
            }
        }

        // Generate card IDs
        List<int> cardIds = new List<int>();
        for (int i = 0; i < totalPairs; i++)
        {
            cardIds.Add(i);
            cardIds.Add(i);
        }

        // Shuffle the list
        cardIds = cardIds.OrderBy(x => Random.value).ToList();

        // Create sprite mapping
        Dictionary<int, Sprite> idToSprite = new Dictionary<int, Sprite>();
        for (int i = 0; i < totalPairs; i++)
        {
            idToSprite[i] = cardFaceSprites[i];
        }
      
        // Instantiate and assign sprite to card
        foreach (int id in cardIds)
        {
            GameObject obj = Instantiate(cardPrefab, cardGrid);
            Card card = obj.GetComponent<Card>();
            card.cardId = id;
            card.SetCardFace(idToSprite[id]);
            cards.Add(card);
        }
    }
    public void OnCardFlipped(Card flipped)
    {
        if ( flippedCards.Contains(flipped) || flipped.isMatched)
            return;

        flippedCards.Add(flipped);

        // Wait until 2 cards are flipped before checking
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch(flippedCards[0], flippedCards[1]));
        }
    }

    IEnumerator CheckMatch(Card first, Card second)
    {
       
        flippedCards.Remove(first);
        flippedCards.Remove(second);
        yield return new WaitForSeconds(0.5f);

        if (first.cardId == second.cardId)
        {
            // Match
            first.isMatched = true;
            second.isMatched = true;

            // combo count 
            comboCount++;
           

            score +=  10 * comboMultiplier; // e.g., 1st combo = x2, 2nd = x3...


            comboMultiplier = 1 + comboCount;
        }
        else
        {
            // Not match
            first.Unflip();
            second.Unflip();

            //reset value
            comboCount = 0;
            comboMultiplier = 1;

            //calculate turns
            Turns++;

        }

        TurnsText.text = "Turns: " + Turns;
        scoreText.text = "Score: " + score;
       

        if (cards.All(c => c.isMatched))
        {
            GameOverPanel.SetActive(true);
            Debug.Log("Game Over: All pairs matched.");
        }

    }


    public void RestartGame()
    {


        // Clear existing cards from scene
        foreach (Transform child in cardGrid)
        {
            Destroy(child.gameObject);
        }

        cards.Clear();
        flippedCards.Clear();
      

        // Reset stats
        score = 0;
        Turns = 0;
        comboCount = 0;
        comboMultiplier = 1;

        // Update UI
        scoreText.text = "Score: 0";
        TurnsText.text = "Turns: 0";

        // Re-setup grid and cards
        SetupCards();
        GameOverPanel.SetActive(false); 
        Debug.Log("Game Restarted");
    }
}
