using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    Dictionary<Vector2, Node> grid = new Dictionary<Vector2, Node>();
    Vector2Int gridSize = new Vector2Int(4, 4);
    ItemCollider itemCollider;
    [SerializeField] Camera mainCamera;
    int blankLinesOnBottom = 10;

    public Dictionary<Vector2, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    void Start()
    {
        gridSize.y += blankLinesOnBottom;
        SetCameraToMiddleOfGrid();
        ClearGrid();
        CreateGrid();
        UpdateAllNodeConnection();
        IsMatched3Vertical();

    }
    void Update()
    {
        PushSpaceToResetGrid();
        PushEnterToTestFotMatched3();
    }



    public Node GetNode(Vector2 coordinates)
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
                Vector2 coordinates = new Vector2(x, y);
                Color blockColor = colors[Random.Range(0, colors.Length)];

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, coordinates, Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                }

                Node node_ = ScriptableObject.CreateInstance<Node>();
                node_.Init(coordinates);
                grid.Add(coordinates, node_);
            }
        }
    }
    void IsMatched3Vertical() //Check throught all block in Grid to see if one has 2+ vertical connection and stock the connected nodes in a list
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 coordinates = new Vector2(x, y);
                if (grid.ContainsKey(coordinates))
                {

                    Node node = grid[coordinates];
                    if (node.connectedToVertical.Count >= 2)
                    {
                        List<Node> testNodes = new List<Node>();
                        testNodes.Add(node);
                        testNodes.AddRange(node.connectedToVertical);
                        if (IsSameColor(testNodes))
                        {
                            Debug.Log("MATCH3 GOTTEM : Item : " + node.name + "has : " + node.NumberOfVerticalConnection() + "Vertical connection");
                        }
                    }
                }

            }
        }
    }
    List<Node> CheckBothVerticalConnectionForColor(Node node)
    {
        List<Node> linkedNode = new List<Node>();
        linkedNode.Add(node);
        for (int i = 0; i < node.connectedToVertical.Count; i++) //Pour chaque node stocké dans connectedToVertical, si il est de la même couleur on l'ajout dans la liste
        {
            linkedNode.Add(node.connectedToVertical[i]);
            if (!IsSameColor(linkedNode))
            {
                linkedNode.Remove(node.connectedToVertical[i]);
            }
        }
        return linkedNode;
    }

    List<Node> GetVerticalConnectedWithSameColor(List<Node> linkedNodeOfSameColor, Node node) //Check if first vertical connection is of same color to add it in a list, if not remove connection
    {
        if (node.connectedToVertical.Count == 0) { return linkedNodeOfSameColor; }

        Node linkedNode = node.connectedToVertical[0];
        node.RemoveVerticalConnection(linkedNode);

        if (!linkedNodeOfSameColor.Contains(linkedNode)) //Si list ne contient pas le linked node
        {
            linkedNodeOfSameColor.Add(linkedNode);
        }
        if(!IsSameColor(linkedNodeOfSameColor))
        {
            linkedNodeOfSameColor.Remove(linkedNode);
            return linkedNodeOfSameColor;
        }

        GetVerticalConnectedWithSameColor(linkedNodeOfSameColor, linkedNode);
        return linkedNodeOfSameColor;
    }

List<Node> GetFirstMatchedVerticalOfSameColor() //Check throught all block in Grid to see if one has 2+ vertical connection and stock the connected nodes in a list
{
    for (int x = 0; x < gridSize.x; x++)
    {
        for (int y = 0; y < gridSize.y; y++)
        {
            Vector2 coordinates = new Vector2(x, y);
            if (grid.ContainsKey(coordinates) && !grid[coordinates].isMacthed) //Check tous les node sauf ceux déja matched
            {
                Node node = grid[coordinates];
                List<Node> nodeList = new List<Node>();

                    nodeList.Add(node);
                    GetVerticalConnectedWithSameColor(nodeList, grid[coordinates]);

                if (nodeList.Count >= 3) //Si on trouve une liste ave cau moins 3 de la même couleur => return
                {
                    Debug.Log(nodeList.Count);
                    Debug.Log(nodeList[0].name);
                    MatchedNodeList(nodeList);
                    return nodeList;
                }
            }
        }
    }
    return null;
}

public bool IsSameColor(List<Node> nodes) //Check if a list of blocks are of the same color
{
    Color testColor = new Color();

    for (int i = 0; i < nodes.Count; i++)
    {
        testColor = nodes[0].color;

        if (testColor != nodes[i].color)
        {
            return false;
        }
    }
    return true;
}

public void MatchedNodeList(List<Node> nodes)
{
    foreach (Node node in nodes)
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
