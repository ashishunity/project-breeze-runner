using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardId;
    public GameObject front;
    public GameObject back;
    private Button button;

    public bool isFlipped = false;
    public bool isMatched = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
    }

    private void OnCardClicked()
    {
        if (isFlipped || isMatched) return;

        Flip();
        GameManager.Instance.OnCardFlipped(this);
     //   AudioManager.Instance.Play("flip");
    }

    public void Flip()
    {
        isFlipped = true;
        front.SetActive(true);
        back.SetActive(false);
    }

    public void Unflip()
    {
        isFlipped = false;
        front.SetActive(false);
        back.SetActive(true);
    }

    public void SetCardFace(Sprite faceSprite)
    {
        front.GetComponent<Image>().sprite = faceSprite;
    }
}
