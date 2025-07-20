using System.Collections.Generic;
using UnityEngine;

public class CardPoolManager : MonoBehaviour
{

    public Transform cardGrid;

    private int initialPoolSize;

   
    private GameObject cardPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        cardPrefab = Resources.Load<GameObject>("Prefabs/Card");
        
        // Pre-instantiate cards
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject card = Instantiate(cardPrefab, cardGrid);
            card.SetActive(false);
            pool.Enqueue(card);
        }

      
    }
    int i;
    public GameObject GetCard()
    {
        i++;
       
        if (pool.Count > 0)
        {
            GameObject card = pool.Dequeue();
            card.SetActive(true);
            return card;
        }
        else
        {
            // Expand pool if needed
            GameObject card = Instantiate(cardPrefab, cardGrid);
            return card;
        }
    }

    public void ReturnCard(GameObject card)
    {
        card.SetActive(false);
        pool.Enqueue(card);
    }

    public void ReturnAllCards()
    {
        pool.Clear();
        
        foreach (Transform child in cardGrid)
        {
            ReturnCard(child.gameObject);
         
        }
    }
}
