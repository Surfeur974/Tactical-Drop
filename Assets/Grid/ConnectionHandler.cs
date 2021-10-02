using System.Collections;
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
    public List<Node> GetFirstMatch(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //Check throught all block in Grid and get first match connection
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
                        return nodeList;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return null;
    }

    public bool IsThereAConnection(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize, int startLineToCheck)
    {
        for (int x = 0; x < gridSize.x; x += 1)
        {
            for (int y = startLineToCheck + (x % 2); y < gridSize.y - 1; y += 2)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
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
    public bool IsThereAConnectionRandomizeColorTheOneNode(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize)
    {
        for (int x = 0; x < gridSize.x; x += 1)
        {
            for (int y = 1 + (x % 2); y < gridSize.y - 1; y += 2)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid.ContainsKey(coordinates) && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Node node = grid[coordinates];
                    List<Node> nodeList = new List<Node> { };

                    nodeList.AddRange(GetAllVerticalConnectionNodesWithSameColor(node)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(nodeList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        node.color = grid[new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y))].color;
                        return true;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return false;
    }

    private List<Node> GetAllVerticalConnectionNodesWithSameColor(Node node) 
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
            isRunning = CheckSameColorNodes(currentSearchNode.connectedToVertical);//Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Node>
            if (alreadyCheckedNodes.Count > 2) { return alreadyCheckedNodes; }
        }
        return alreadyCheckedNodes;
    }
    public List<Node> GetAllVerticalConnection(Node node)
    {
        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        if (node.connectedToVertical.Count == 0) { return alreadyCheckedNodes; }
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();
            isRunning = CheckAnyColorNodes(currentSearchNode.connectedToVertical);//Check a list of node, Add it to queue "nodeToExplored", and to alreadyCheckedNodes List<Node>
        }
        return alreadyCheckedNodes;
    }
    private bool CheckSameColorNodes(List<Node> nodeToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Node>
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
    private bool CheckAnyColorNodes(List<Node> nodeToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Node>
    {
        for (int i = 0; i < nodeToCheck.Count; i++)
        {
            Node connectedNode = nodeToCheck[i];
            if (!alreadyCheckedNodes.Contains(connectedNode))
            {
                alreadyCheckedNodes.Add(connectedNode);
                nodeToExplored.Enqueue(connectedNode);
            }
        }
        return true;
    }
    private List<Node> GetAllConnectionNodesWithSameColor(Node node) //return for a node, all his connections of same color, tobe used after 3 vertical detected
    {

        if (node.connectedToVertical.Count == 0 && node.connectedToHorizontal.Count == 0) { return alreadyCheckedNodes; }

        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();

            List<Node> nodesToChecked = new List<Node>();
            nodesToChecked.AddRange(currentSearchNode.connectedToVertical);
            nodesToChecked.AddRange(currentSearchNode.connectedToHorizontal);

            isRunning = CheckSameColorNodes(nodesToChecked);
        }
        return alreadyCheckedNodes;
    }
    private List<Node> GetAllHorizontalConnectionNodesWithSameColor(Node node) //return for a node, all his Horizontal connections of same color
    {
        if (node.connectedToHorizontal.Count == 0) { return alreadyCheckedNodes; }

        alreadyCheckedNodes = new List<Node>();
        bool isRunning = true;
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(node);
        alreadyCheckedNodes.Add(node);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchNode = nodeToExplored.Dequeue();
            isRunning = CheckSameColorNodes(currentSearchNode.connectedToHorizontal);
        }
        return alreadyCheckedNodes;
    }
    private bool IsSameColor(List<Node> nodes)
    {
        Color testColor = new Color();

        for (int i = nodes.Count-1; i < nodes.Count; i++)
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

    public void MatchedNodeList(List<Node> nodes)
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
                    grid[coordinates].isMatched = false;
                }

            }
        }
    }

}
