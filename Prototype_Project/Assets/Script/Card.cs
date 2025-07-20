using System.Collections;
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

    private bool IscoroutineRunning;
    private bool facedUp;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnCardClicked);
        IscoroutineRunning = true;
        facedUp = true;
        isFlipped = true;
    }

    private void OnCardClicked()
    {
        if (isFlipped || isMatched) return;

        Flip();
        GameManager.Instance.OnCardFlipped(this);

    }

    public void Flip()
    {
        isFlipped = true;
        if (IscoroutineRunning)
        {
            StartCoroutine(RotateCard());
        }


    }

    public void Unflip()
    {
        isFlipped = false;
        if (IscoroutineRunning)
        {
            StartCoroutine(RotateCard());
        }
    }

    public void SetCardFace(Sprite faceSprite)
    {
        front.GetComponent<Image>().sprite = faceSprite;
    }

    private IEnumerator RotateCard()
    {
        IscoroutineRunning = false;

        if (!facedUp)
        {
            for (float i = 0f; i <= 180f; i += 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    front_CardActive(true);
                    back.SetActive(false);
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        else if (facedUp)
        {
            for (float i = 180f; i >= 0f; i -= 10f)
            {
                transform.rotation = Quaternion.Euler(0f, i, 0f);
                if (i == 90f)
                {
                    front_CardActive(false);
                    back.SetActive(true);
                }
                yield return new WaitForSeconds(0.01f);
            }
        }

        IscoroutineRunning = true;

        facedUp = !facedUp;
    }

    public void FlipInstant()
    {

        front_CardActive(false);
        back.SetActive(false);

    }
    public void UnFlipInstant()
    {
        facedUp = false;
        isFlipped = false;
        front_CardActive(false);
        back.SetActive(true);

    }
    public void UnflipImmediate()
    {
        facedUp = true;
        isFlipped = true;
        isMatched = false; ;
        front_CardActive(true);
        back.SetActive(false);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void front_CardActive(bool Istrue)
    {
        front.SetActive(Istrue);
        front.transform.parent.gameObject.SetActive(Istrue);
    }
}
