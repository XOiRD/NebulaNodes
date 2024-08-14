using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public Image cardImage;    // The image component of the card
    private Sprite frontSprite; // The front image (to be set during initialization)
    private Sprite backSprite;  // The back image (default, already set in the prefab)
    private GameManager gameManager;
    private bool isFlipped = false;
    private bool isMatched = false;

    private void Awake()
    {
        cardImage = GetComponent<Image>();
    }

    public void Initialize(Sprite frontImage, GameManager manager)
    {
        frontSprite = frontImage;
        gameManager = manager;
        backSprite = cardImage.sprite;
    }

    public void FlipCard(bool showFront)
    {
        if (isMatched) return;

        isFlipped = showFront;
        cardImage.sprite = showFront ? frontSprite : backSprite;
    }

    public void OnCardClicked()
    {
        if (isFlipped || gameManager.IsCheckingMatch()) return;

        FlipCard(true);
        gameManager.OnCardSelected(this);
    }

    public void SetMatched()
    {
        isMatched = true;
        FlipCard(true);
    }

    public void Unmatch()
    {
        isFlipped = false;
        FlipCard(false); 
    }

    public Sprite GetCardImage()
    {
        return frontSprite;
    }
}
