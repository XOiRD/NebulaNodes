using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("General")]
    [Space(5)]
    public Animator animator;
    public GameObject cardPrefab; // The card prefab
    public int timerLimit = 60;
    bool isGameFinished = false;

    [Header("Mainmenu page")]
    [Space(5)]
    public TMP_Dropdown columnsDropdown;
    public TMP_Dropdown rowsDropdown;

    [Header("Gameplay page")]
    [Space(5)]
    public GridLayoutGroup gridLayout;
    public RectTransform gameArea; // The grid container
    public Sprite[] cardImages; // An array of the front images of the cards
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public int score;
    public GameObject endScene;

    [Header("Mechanics")]
    [Space(5)]
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    private List<GameObject> cards = new List<GameObject>();
    private Card firstSelectedCard;
    private Card secondSelectedCard;
    private bool isCheckingMatch = false;
    [SerializeField]private int totalMatches;
    [SerializeField]private int currentMatches = 0;

    private List<Sprite> selectedImages = new List<Sprite>();

    void Start()
    {
        
    }

    public void StartGame()
    {
        UpdateGrid();
        animator.Play("Gameplay");
        StartCoroutine(CountdownTimer());
    }

    public void UpdateGrid()
    {
        // Clear existing cards if any
        foreach (Transform child in gridLayout.transform)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();

        columns = int.Parse(columnsDropdown.options[columnsDropdown.value].text);
        rows = int.Parse(rowsDropdown.options[rowsDropdown.value].text);

        gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        gridLayout.constraintCount = rows;

        // Calculating the width and height of the cell
        float cellWidth = gameArea.rect.width / columns - gridLayout.spacing.x;
        float cellHeight = gameArea.rect.height / rows - gridLayout.spacing.y;

        // Maintain aspect ratio (150x210)
        float aspectRatio = 150f / 210f;
        if (cellWidth / cellHeight > aspectRatio)
        {
            cellWidth = cellHeight * aspectRatio;
        }
        else
        {
            cellHeight = cellWidth / aspectRatio;
        }

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);

        int totalCards = columns * rows;
        totalMatches = totalCards / 2;

        // Select and shuffle card images
        selectedImages = new List<Sprite>();
        for (int i = 0; i < totalCards / 2; i++)
        {
            selectedImages.Add(cardImages[i]);
            selectedImages.Add(cardImages[i]);
        }
        Shuffle(selectedImages);

        // Instantiate the cards
        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, gridLayout.transform);
            card.GetComponent<Card>().Initialize(selectedImages[i], this);
            cards.Add(card);
        }
    }

    private void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void OnCardSelected(Card selectedCard)
    {
        if (firstSelectedCard == null)
        {
            firstSelectedCard = selectedCard;
        }
        else if (secondSelectedCard == null)
        {
            secondSelectedCard = selectedCard;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isCheckingMatch = true;

        yield return new WaitForSeconds(1f);

        if (firstSelectedCard.GetCardImage() == secondSelectedCard.GetCardImage())
        {
            firstSelectedCard.SetMatched();
            secondSelectedCard.SetMatched();
            currentMatches++;
            if(currentMatches == totalMatches)
            {
                isGameFinished = true;
                endScene.SetActive(true);
                endScene.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            firstSelectedCard.Unmatch();
            secondSelectedCard.Unmatch();
        }

        firstSelectedCard = null;
        secondSelectedCard = null;
        isCheckingMatch = false;
    }

    public bool IsCheckingMatch()
    {
        return isCheckingMatch;
    }

    public void UpdateScore()
    {
        scoreText.text = score.ToString();
    }

    IEnumerator CountdownTimer()
    {
        yield return new WaitForSeconds (0.5f);
        timerText.text = timerLimit.ToString();
        while(timerLimit > 0 && !isGameFinished)
        {
            timerLimit--;
            timerText.text = timerLimit.ToString();
            yield return new WaitForSeconds(1f);
        }
        if (!isGameFinished)
        {
            endScene.SetActive(true);
            endScene.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

}
