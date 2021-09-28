using System.Collections.Generic;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField] int minNumberForVerticalConnection = 3;
    //[SerializeField] GridManager gridManager;

    Node currentSearchNode;
    List<Node> linkedNodeOfSameColor = new List<Node>();
    List<Node> alreadyCheckedNodes = new List<Node>();
    Queue<Node> nodeToExplored = new Queue<Node>();

    private void Start()
    {
        //gridManager = GetComponent<GridManager>();
        //grid = gridManager.Grid;
        //gridSize = gridManager.GridSize;
    }
    public List<Node> HandleFirstMatched(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //Check throught all block in Grid and handle first match connection
    {//IF bug maybe use REF //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid.ContainsKey(coordinates) && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Node node = grid[coordinates];
                    List<Node> nodeList = new List<Node> { };

                    nodeList.AddRange(GetAllVerticalConnectionNodesWithSameColor(node)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(nodeList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        nodeList.Clear();
                        nodeList.AddRange(GetAllConnectionNodesWithSameColor(node));

                        MatchedNodeList(nodeList);

                        return nodeList;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return null;
    }

    public bool IsThereAConnection(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize)
    {
        for (int x = 0; x < gridSize.x; x += 1)
        {
            for (int y = 1 + (x % 2); y < gridSize.y - 1; y += 2)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                //grid[coordinates].isMatched = true;

                if (grid.ContainsKey(coordinates) && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Node node = grid[coordinates];
                    List<Node> nodeList = new List<Node> { };

                    nodeList.AddRange(GetAllVerticalConnectionNodesWithSameColor(node)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(nodeList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        return true;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return false;
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
    private bool CheckConnectedNode(List<Node> nodeToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Node>
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

            List<Node> nodesToChecked = new List<Node>();
            nodesToChecked.AddRange(currentSearchNode.connectedToVertical);
            nodesToChecked.AddRange(currentSearchNode.connectedToHorizontal);

            isRunning = CheckConnectedNode(nodesToChecked);
        }
        return alreadyCheckedNodes;
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
    private bool IsSameColor(List<Node> nodes) //Si tous les nodes de la liste sont de la meme couleur, return true
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

    private void MatchedNodeList(List<Node> nodes)
    {
        foreach (Node node in nodes)
        {
            node.isMatched = true;
        }
    }//Set all isMatched bool at true for the list


    public void DematchAll(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize)//TODO a refaire avec des events pour updates les collisiton
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

}
