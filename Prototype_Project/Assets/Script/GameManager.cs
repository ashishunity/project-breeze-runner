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


    // private
    private List<Card> cards = new List<Card>();
    private Card firstFlippedCard = null;
  
    private int score = 0;
   
    private GridLayoutGroup gridLayoutGroup;
                                          

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
        if (firstFlippedCard == null)
        {
            firstFlippedCard = flipped;
            return;
        }

        StartCoroutine(CheckMatch(flipped));
    }

    IEnumerator CheckMatch(Card second)
    {
        yield return new WaitForSeconds(0.5f);

        if (firstFlippedCard.cardId == second.cardId)
        {
            // Match
            firstFlippedCard.isMatched = true;
            second.isMatched = true;
            score += 10;
          
        }
        else
        {
            // Not match
            firstFlippedCard.Unflip();
            second.Unflip();
            if(score>0)
            score -= 1;
         
        }

        scoreText.text = "Score: " + score;
        firstFlippedCard = null;
    }
}
