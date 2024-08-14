using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private List<Card> firstSelectedCard = null;
    [SerializeField] private List<Card> secondSelectedCard = null;
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
        if(firstSelectedCard.Count == secondSelectedCard.Count)
        {
            firstSelectedCard.Add(selectedCard);
        }
        else
        {
            secondSelectedCard.Add(selectedCard);
        }
        if(firstSelectedCard.Count == 1 && secondSelectedCard.Count == 1)
        {
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        isCheckingMatch = true;

        yield return new WaitForSeconds(0.5f);

        if (firstSelectedCard[0].GetCardImageName() == secondSelectedCard[0].GetCardImageName())
        {
            firstSelectedCard[0].SetMatched();
            secondSelectedCard[0].SetMatched();
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
            firstSelectedCard[0].Unmatch();
            secondSelectedCard[0].Unmatch();
        }
        firstSelectedCard.RemoveAt(0);
        secondSelectedCard.RemoveAt(0);
        isCheckingMatch = false;
        if(firstSelectedCard.Count > 0 && secondSelectedCard.Count > 0)
        {
            StartCoroutine(CheckMatch());
        }
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

    public void RestartScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}
