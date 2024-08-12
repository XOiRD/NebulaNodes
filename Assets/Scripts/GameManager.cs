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
    public GameObject cardPrefab; //the card prefab

    [Header("Mainmenu page")]
    [Space(5)]
    public TMP_Dropdown columnsDropdown;
    public TMP_Dropdown rowsDropdown;

    [Header("Gameplay page")]
    [Space(5)]
    public GridLayoutGroup gridLayout;
    public RectTransform gameArea; //the grid container
    public Sprite[] cardImages; // an array of the front images of the cards

    [Header("Mechanics")]
    [Space(5)]
    [SerializeField] private int columns;
    [SerializeField] private int rows;
    private List<GameObject> cards = new List<GameObject>();
    private GameObject firstSelectedCard;
    private GameObject secondSelectedCard;
    private bool isCheckingMatch = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame()
    {
        UpdateGrid();
        animator.Play("Gameplay");
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

        //calculating the width and height of the cell
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

        for (int i = 0; i < totalCards; i++)
        {
            GameObject card = Instantiate(cardPrefab, gridLayout.transform);
            //card.GetComponent<Card>().Initialize(selectedImages[i], this);
            cards.Add(card);
        }
    }
}
