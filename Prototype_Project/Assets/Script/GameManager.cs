using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
   
   

    [Header("UI Element")]
    public Text scoreText;
    public Text TurnsText;
    public GameObject GameOverPanel;
    public GameObject MenuPanel;
    public ToggleGroup toggleGroup;


    // rows col
    private int rows = 2;
    private int cols = 3;

    // private
    private List<Card> cards = new List<Card>();
    private List<Card> flippedCards = new List<Card>();

    private int score = 0;
    private int Turns = 0;

    private GridLayoutGroup gridLayoutGroup;

    // combo 
    private int comboCount = 0;
    private int comboMultiplier = 1;
    private float CardRevealTime;

    // Select toggle group
    int selected_Level;

    public  AudioManager _AudioManager;

    void Awake() 
    { 
        Instance = this;

    }

    void Start() 
    {
        _AudioManager= GetComponent<AudioManager>();
        gridLayoutGroup = cardGrid.GetComponent<GridLayoutGroup>();
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

   public void SetupCards()
    {

        if (toggleGroup.IsActive())
            selected_Level = GetToggleLevel();

        // This logic could be implemented using a switch statement, but we've optimized the code for better performance and readability.
        //Set rows col according to radio button choose in menu 
        rows = selected_Level + 1;// eg. 2 *3 , 3*4, 4*5 , 5*6 
        cols = selected_Level + 2;
        CardRevealTime = 1.5f * selected_Level; // eg. 1.5f * 1 , 1.5f * 2 

        MenuPanel.SetActive(false);

        ConfigureGridToFit();

        int totalPairs = (rows * cols)/2 ;
       

        // Safety check: Ensure you have enough sprites to match pair
        if (cardFaceSprites.Count < totalPairs)
        {
            int count = cardFaceSprites.Count;
            print("Not enough card face sprites assigned!" + totalPairs);
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



        // To display all cards briefly at the start of each level  and have the reveal duration change based on level
        Invoke("InnokeAfterSomeTime", CardRevealTime);
    }


    
    void InnokeAfterSomeTime()
    {
        foreach (Card child in cards)
        {
            child.Unflip();
        }
      
      
    }

    public void OnCardFlipped(Card flipped)
    {
        if ( flippedCards.Contains(flipped) || flipped.isMatched)
            return;

       
        flippedCards.Add(flipped);

        _AudioManager.Play("flip");

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

            first.FlipInstant();
            second.FlipInstant();

            _AudioManager.Play("match");
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

            _AudioManager.Play("mismatch");

        }

        TurnsText.text = "Turns\n" + Turns;
        scoreText.text = "Score\n" + score;
       

        if (cards.All(c => c.isMatched))
        {
            GameOverPanel.SetActive(true);
            _AudioManager.Play("gameover");
            Debug.Log("Game Over: All pairs matched.");
           
        }

    }


     void Reset()
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
        TurnsText.text = "Turns\n0";
        scoreText.text = "Score\n0" ;
        

        Debug.Log("Game Data Reset");

    }


    public void RestartGame()
    {
        Reset();

        // Re-setup grid and cards
        SetupCards();
        GameOverPanel.SetActive(false);

        Debug.Log("Game Restarted");
    }

    public void Menu()
    {
        Reset();
        GameOverPanel.SetActive(false );
        MenuPanel.SetActive(true);

    }

 
    int GetToggleLevel()
    {
      
        Toggle activeToggle = toggleGroup.ActiveToggles().FirstOrDefault();
        return int.Parse(activeToggle.name); 
    }
  
}
