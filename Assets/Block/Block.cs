using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] int blockScore;
    [SerializeField] Vector2Int coordinates;
    bool isInHand;
    GridManager gridManager;
    UIDisplay uIDisplay;
    public bool isMatched;

    Material material;
    public Vector2Int Coordinates { get { return coordinates; } }
    public List<Block> connectedToVertical = new List<Block>();
    public List<Block> connectedToHorizontal = new List<Block>();
    public bool IsMatched { get { return isMatched; } }



    public void Init(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
        UpdateNameObject();
        ClearConnectionUpdateGridPosition();
        isMatched = false;
        //isInHand = false;
    }
    public void Init(Vector2Int coordinates, Color colorToInit)
    {
        material.color = colorToInit;
        this.coordinates = coordinates;

        UpdateNameObject();
        ClearConnectionUpdateGridPosition();
        isMatched = false;
        //isInHand = false;
    }


    private void OnEnable()
    {
        blockScore = 100;
        gridManager = FindObjectOfType<GridManager>();
        uIDisplay = FindObjectOfType<UIDisplay>();
        material = GetComponentInChildren<MeshRenderer>().material;
        coordinates.x = Mathf.RoundToInt(transform.position.x);
        coordinates.y = Mathf.RoundToInt(transform.position.y);
        FillGrid();
    }
    private void Update()
    {
        //FillGrid();
        //material.color = color;
        if (isInHand == false)
        {
            UpdateBlockCoordinates();
        }
        HandleMatched();
    }
    private void FillGrid()
    {
        UpdateBlockCoordinates();
        if (gridManager.Grid.ContainsKey(coordinates) && gridManager.Grid[coordinates] != this) //Si la place dans grille a ses coordinates n'est pas this block
        {
            gridManager.Grid[coordinates] = this;
        }
    }
    private void LeaveGrid()
    {
        if (gridManager.Grid.ContainsKey(coordinates) && gridManager.Grid[coordinates] == this)
        {
            gridManager.Grid[coordinates] = null;
        }
    }
    public void InHand(bool isInHand)
    {
        this.isInHand = isInHand;
        if (this.isInHand == false) //S'il n'est plus en main tu prend ta place dans la grille
        {
            UpdateBlockCoordinates();
            gridManager.Grid[coordinates] = this;
        }
        else //SInon tu laisse ta place dans la grille
        {
            gridManager.Grid[coordinates] = null;
        }
    }

    private void UpdateBlockCoordinates()
    {
        coordinates.x = Mathf.RoundToInt(transform.position.x);
        coordinates.y = Mathf.RoundToInt(transform.position.y);
        UpdateNameObject();
    }
    public Color GetBlockColor()
    {
        return material.color;
    }

    public void HandleMatched()
    {
        if (isMatched == true)
        {
            uIDisplay.AddScore(blockScore, material.color);
            UpdateNameObject();
            LeaveGrid();

        this.transform.gameObject.SetActive(false);
        }
    }
    public void AddVerticalConnection(Block blockToAdd)
    {
        if (!connectedToVertical.Contains(blockToAdd))
        {
            connectedToVertical.Add(blockToAdd);
        }


    }
    public void AddHorizontalConnection(Block blockToAdd)
    {
        if (!connectedToHorizontal.Contains(blockToAdd))
        {
            connectedToHorizontal.Add(blockToAdd);
        }
    }
    public void ClearConnectionUpdateGridPosition()
    {
        LeaveGrid();
        FillGrid();
        if (connectedToHorizontal != null)
        {
            connectedToHorizontal.Clear();
        }
        if (connectedToVertical != null)
        {
            connectedToVertical.Clear();
        }
    }
    private void UpdateNameObject()
    {
        this.name = "(" + coordinates.x + "," + coordinates.y + ")";
    }
    public void ChangeBlockColor(Color colorToUpdateTo)
    {
        this.material.color = colorToUpdateTo;
    }
}
