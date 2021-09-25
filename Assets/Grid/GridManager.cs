using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int(8, 8);
    ItemCollider itemCollider;
    [SerializeField] Camera mainCamera;
    int blankLinesOnBottom = 10;
    int minNumberForVerticalConnection = 3;
    Node currentSearchNode;
    List<Node> linkedNodeOfSameColor = new List<Node>();
    List<Node> alreadyCheckedNodes = new List<Node>();
    Queue<Node> nodeToExplored = new Queue<Node>();


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
        PushBackSpaceToDematch();
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

                //Color blockColor = colors[0];
                //if (x == gridSize.x - 1 || x == gridSize.x - 3 || x == gridSize.x - 5) { blockColor = colors[1]; }
                //if (y == gridSize.y - 4 || y == gridSize.y - 2 || y == gridSize.y - 6) { blockColor = colors[1]; }

                //Color blockColor = colors[0];
                //if (x == 0) { blockColor = colors[1]; }
                //if (y == gridSize.y - 7) { blockColor = colors[2]; }

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                }

                Node node_ = ScriptableObject.CreateInstance<Node>();
                node_.Init(coordinates);
                grid.Add(coordinates, node_);
            }
        }
    }


    private List<Node> GetAllVerticalConnectionNodesWithSameColor(Node node) //return for a node, all his vertical connections of same color
    {
        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        if (node.connectedToVertical.Count == 0) { return alreadyCheckedNodes; }
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();
            isRunning = CheckConnectedNode(currentSearchNode.connectedToVertical);
        }
        return alreadyCheckedNodes;
    }
    private List<Node> GetAllConnectionNodesWithSameColor(Node node) //return for a node, all his connections of same color, tobe used after 3 vertical detected
    {
        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        if (node.connectedToVertical.Count == 0 && node.connectedToHorizontal.Count == 0) { return alreadyCheckedNodes; }
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();

            List<Node> nodesToChecked = currentSearchNode.connectedToVertical;
            nodesToChecked.AddRange(currentSearchNode.connectedToHorizontal);

            isRunning = CheckConnectedNode(nodesToChecked);
        }
        return alreadyCheckedNodes;
    }
    bool CheckConnectedNode(List<Node> nodeToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Node>
    {
        for (int i = 0; i < nodeToCheck.Count; i++)
        {
            Node connectedNode = nodeToCheck[i];
            if (!alreadyCheckedNodes.Contains(connectedNode))
            {
                alreadyCheckedNodes.Add(connectedNode);

                if (!IsSameColor(alreadyCheckedNodes))
                {
                    alreadyCheckedNodes.Remove(connectedNode);
                }
                else
                {
                    nodeToExplored.Enqueue(connectedNode);
                }
            }
        }
        return true;
    }


    private List<Node> GetAllHorizontalConnectionNodesWithSameColor(Node node) //return for a node, all his Horizontal connections of same color
    {
        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        if (node.connectedToHorizontal.Count == 0) { return alreadyCheckedNodes; }
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();
            isRunning = CheckConnectedNode(currentSearchNode.connectedToHorizontal);
        }
        return alreadyCheckedNodes;
    }

    void HandleFirstMatched() //Check throught all block in Grid and handle first match connection
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid.ContainsKey(coordinates) && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Node node = grid[coordinates];
                    List<Node> nodeList = new List<Node> {};

                    nodeList.AddRange(GetAllVerticalConnectionNodesWithSameColor(node)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(nodeList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        nodeList.Clear();
                        nodeList.AddRange(GetAllConnectionNodesWithSameColor(node)); 

                        MatchedNodeList(nodeList);
                        //return;  //Remove return to handle all matched at once
                    }
                }
            }
        }
    }

    private bool IsListMinNumber(List<Node> nodeList)//Si liste.Count >= minNumberForVerticalCOmbo return true
    {
        if (nodeList.Count >= minNumberForVerticalConnection)
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
            node.isMatched = true;
        }
    }//Set all isMatched bool at true for the list
    void PushSpaceToResetGrid() //Press Space to reset Grid et instanciate new blocks and UpdateAllNodeConnection
    {

        if (Input.GetKeyDown(KeyCode.Space))
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
            HandleFirstMatched();
        }
    }
    private void PushBackSpaceToDematch() //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DematchAll();
        }
    }

    public void UpdateAllNodeConnection()//TODO a refaire avec des events pour updates les collisiton
    {
        foreach (Transform child in transform)
        {
            child.GetComponentInChildren<ItemCollider>().UpdateConnectedTo();
        }
    }
    public void DematchAll()//TODO a refaire avec des events pour updates les collisiton
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid.ContainsKey(coordinates))
                {
                    grid[coordinates].Init();
                }

            }
        }
    }
    private void SetCameraToMiddleOfGrid()
    {
        mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -13.33f);
    }//Set camera position to middle of the Grid
    //test github
}
