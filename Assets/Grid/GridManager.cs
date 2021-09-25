using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int(4,4);
    ItemCollider itemCollider;
    [SerializeField] Camera mainCamera;
    int blankLinesOnBottom = 10;
    int minNumberForVerticalCOmbo = 3;

    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    void Start()
    {
        gridSize.y += blankLinesOnBottom; //Pour avoir des lignes vides en bas
        SetCameraToMiddleOfGrid();
        ClearGrid();
        CreateGrid();
        UpdateAllNodeConnection();

    }
    void Update()
    {
        PushSpaceToResetGrid();
        PushEnterToTestFotMatched3();
    }
    public Node GetNode(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            return grid[coordinates];
        }
        return null;
    }

    void ClearGrid() //Destroy All child of Grid
    {
        grid.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    void CreateGrid()  //Instantiate block with 3 random color set in inspector, create a node for set (position and color), Add pair coordinate:node in dictionnary
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                Color blockColor = colors[Random.Range(0, colors.Length)];

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y,0), Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                }

                Node node_ = ScriptableObject.CreateInstance<Node>();
                node_.Init(coordinates);
                grid.Add(coordinates, node_);
            }
        }
    }

    List<Node> GetAllVerticalConnectionNodesWithSameColor(List<Node> linkedNodeOfSameColor, Node node) //return for a node, all his vertical connections of same color
    {
        if (node.connectedToVertical.Count == 0) { return linkedNodeOfSameColor; }

        Node linkedNode = node.connectedToVertical[0];
        node.RemoveVerticalConnection(linkedNode);

        if (!linkedNodeOfSameColor.Contains(linkedNode)) //Si list ne contient pas le linked node on l'ajoute
        {
            linkedNodeOfSameColor.Add(linkedNode);
        }
        if (!IsSameColor(linkedNodeOfSameColor)) //Si pas de la m�me couleur que le reste, on l'enleve et on return
        {
            linkedNodeOfSameColor.Remove(linkedNode);
            return linkedNodeOfSameColor;
        }

        GetAllVerticalConnectionNodesWithSameColor(linkedNodeOfSameColor, linkedNode); //On recall la fonction de mani�re recursive pour la node de la connection
        return linkedNodeOfSameColor;
    }

    void HandleFirstMatchedVerticalOfSameColor() //Check throught all block in Grid and handle first match connection
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid.ContainsKey(coordinates) && !grid[coordinates].isMacthed) //Check tous les node sauf ceux d�ja matched
                {
                    Node node = grid[coordinates];
                    List<Node> nodeList = new List<Node> { node };

                    GetAllVerticalConnectionNodesWithSameColor(nodeList, grid[coordinates]);
                    if (IsListMinCombo(nodeList))
                    {
                        //Debug.Log(nodeList.Count);
                        //Debug.Log(nodeList[0].name);
                        MatchedNodeList(nodeList);
                        return;  //Remove return to handle all matched at once
                    }
                }
            }
        }
    }

    private bool IsListMinCombo(List<Node> nodeList)//Si liste.Count >= minNumberForVerticalCOmbo return true
    {
        if (nodeList.Count >= minNumberForVerticalCOmbo)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsSameColor(List<Node> nodes) //Si tous les nodes de la liste sont de la m�me couleur, return true
    {
        Color testColor = new Color();

        for (int i = 0; i < nodes.Count; i++)
        {
            linkedNode.Add(node.connectedToVertical[i]);
            if (!IsSameColor(linkedNode))
            {
                linkedNode.Remove(node.connectedToVertical[i]);
            }
        }
        return linkedNode;
    }

    public void MatchedNodeList(List<Node> nodes)
    {
        if (node.connectedToVertical.Count == 0) { return linkedNodeOfSameColor; }

        Node linkedNode = node.connectedToVertical[0];
        node.RemoveVerticalConnection(linkedNode);

        if (!linkedNodeOfSameColor.Contains(linkedNode)) //Si list ne contient pas le linked node
        {
            node.IsMatched(true);
        }
    }//Set all isMatched bool at true for the list
    void PushSpaceToResetGrid() //Press Space to reset Grid et instanciate new blocks and UpdateAllNodeConnection
    {
        testColor = nodes[0].color;

        if (testColor != nodes[i].color)
        {
            ClearGrid();
            CreateGrid();
            UpdateAllNodeConnection();
        }
    }
    private void PushEnterToTestFotMatched3() //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateAllNodeConnection();
            HandleFirstMatchedVerticalOfSameColor();
        }
    }

    public void UpdateAllNodeConnection()//TODO a refaire avec des events pour updates les collisiton
    {
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<ItemCollider>().UpdateConnectedTo();
        }
        //for (int x = 0; x < gridSize.x; x++)
        //{
        //    for (int y = 0; y < gridSize.y; y++)
        //    {
        //        Vector2Int coordinates = new Vector2Int(x, y);
        //        if (grid.ContainsKey(coordinates))
        //        {
        //            grid[coordinates].UpdateConnectionFromNode();
        //        }

        //    }
        //}
    }
    private void SetCameraToMiddleOfGrid()
    {
        node.IsMatched(true);
    }
}
void PushSpaceToResetGrid() //Press Space to reset Grid et instanciate new blocks
{

    if (Input.GetKeyDown(KeyCode.Space))
    {
        ClearGrid();
        CreateGrid();
        UpdateAllNodeConnection();
        //GetFirstMatchedVerticalOfSameColor();
        //IsMatched3Vertical();

    }
}
private void PushEnterToTestFotMatched3()
{
    if (Input.GetKeyDown(KeyCode.Return))
    {
        UpdateAllNodeConnection();
        GetFirstMatchedVerticalOfSameColor();
        //IsMatched3Vertical();
    }
}

public void UpdateAllNodeConnection()//TODO a refaire avec des events pour updates les collisiton
{
    foreach (Transform child in transform)
    {
        child.GetComponentInChildren<ItemCollider>().UpdateConnectedTo();
    }
    //for (int x = 0; x < gridSize.x; x++)
    //{
    //    for (int y = 0; y < gridSize.y; y++)
    //    {
    //        Vector2 coordinates = new Vector2(x, y);
    //        if (grid.ContainsKey(coordinates))
    //        {
    //            grid[coordinates].UpdateConnectionFromNode();
    //        }

    //    }
    //}
}
private void SetCameraToMiddleOfGrid()
{
    mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -13.33f);
}//Set camera position to middle of the Grid

}
