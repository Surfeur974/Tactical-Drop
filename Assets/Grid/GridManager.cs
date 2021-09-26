using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraZoom = 7.29f;

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int();

    GridSpawner gridspawner;
    int minNumberForVerticalConnection = 3;
    Node currentSearchNode;
    List<Node> linkedNodeOfSameColor = new List<Node>();
    List<Node> alreadyCheckedNodes = new List<Node>();
    Queue<Node> nodeToExplored = new Queue<Node>();




    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    public delegate void UpdateConnectionDelegate();
    public event UpdateConnectionDelegate updateConnectionEvent;

    void Start()
    {
        gridspawner = GetComponent<GridSpawner>();
        ResetGrid();
    }

    private void ResetGrid()
    {
        gridspawner.InitGrid();
        gridSize = gridspawner.GetGridSize();
        grid = gridspawner.GetGrid();
        SetCameraToMiddleOfGrid();
        UpdateAllNodeConnection();
    }

    void Update()
    {
        SetCameraToMiddleOfGrid();
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

    private void PushBackSpaceToDematch() //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DematchAll();
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
    void PushSpaceToResetGrid() //Press Space to reset Grid et instanciate new blocks and UpdateAllNodeConnection
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetGrid();
            //UpdateAllNodeConnection();
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

    public void  HandleFirstMatched() //Check throught all block in Grid and handle first match connection
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
                        return;  //Remove return to handle all matched at once
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


    public void UpdateAllNodeConnection() //Call UpdateConnectedTo() on all itemCollider subscribte to the event
    {
        if(updateConnectionEvent != null)
        {
            updateConnectionEvent();
        }
        //foreach (Transform child in transform)
        //{
        //    child.GetComponentInChildren<ItemCollider>().UpdateConnectedTo();
        //}
    }
    private void SetCameraToMiddleOfGrid()
    {
        mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -10);
        mainCamera.orthographicSize = cameraZoom;
    }//Set camera position to middle of the Grid
}
