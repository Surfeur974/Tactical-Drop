using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField] int minNumberForVerticalConnection = 3;
    //[SerializeField] GridManager gridManager;

    Block currentSearchBlock;
    List<Block> alreadyCheckedNodes = new List<Block>();
    Queue<Block> nodeToExplored = new Queue<Block>();


    public List<Block> GetFirstMatch(Dictionary<Vector2Int, Block> grid, Vector2Int gridSize) //Check throught all block in Grid and get first match connection
    {//IF bug maybe use REF //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                if (grid[coordinates]!=null && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Block block = grid[coordinates];
                    List<Block> blockList = new List<Block> { };

                    blockList.AddRange(GetAllVerticalConnectionNodesWithSameColor(block)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(blockList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        blockList.Clear();
                        blockList.AddRange(GetAllConnectionNodesWithSameColor(block));
                        return blockList;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return null;
    }

    public bool IsThereAConnection(Dictionary<Vector2Int, Block> grid, Vector2Int gridSize, int startLineToCheck)
    {
        for (int x = 0; x < gridSize.x; x += 1)
        {
            for (int y = startLineToCheck + (x % 2); y < gridSize.y - 1; y += 2)
            {
                Vector2Int coordinates = new Vector2Int(x, y);
                if (grid[coordinates] != null && !grid[coordinates].isMatched) //Check tous les node sauf ceux déja matched
                {
                    Block block = grid[coordinates];
                    List<Block> blockList = new List<Block> { };

                    blockList.AddRange(GetAllVerticalConnectionNodesWithSameColor(block)); //Met dans une liste un node avec ses connection vertical de la meme couleur
                    if (IsListMinNumber(blockList)) //Si Il a 2+ connection vertical on clear la list et on prend toutes ses connection
                    {
                        return true;  //Remove return to handle all matched at once
                    }
                }
            }
        }
        return false;
    }
    private List<Block> GetAllVerticalConnectionNodesWithSameColor(Block block) 
    {
        alreadyCheckedNodes = new List<Block>();
        bool isRunning = true;
        if (block.connectedToVertical.Count == 0) { return alreadyCheckedNodes; }
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(block);
        alreadyCheckedNodes.Add(block);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchBlock = nodeToExplored.Dequeue();
            isRunning = CheckSameColorNodes(currentSearchBlock.connectedToVertical);//Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Block>
            if (alreadyCheckedNodes.Count > 2) { return alreadyCheckedNodes; }
        }
        return alreadyCheckedNodes;
    }
    public List<Block> GetAllVerticalConnection(Block block)
    {
        alreadyCheckedNodes.Clear();
        bool isRunning = true;
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(block);
        alreadyCheckedNodes.Add(block);
        if (block.connectedToVertical.Count == 0) { return alreadyCheckedNodes; }
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchBlock = nodeToExplored.Dequeue();
            isRunning = CheckAnyColorBlocks(currentSearchBlock.connectedToVertical);//Check a list of node, Add it to queue "nodeToExplored", and to alreadyCheckedNodes List<Block>
        }
        return alreadyCheckedNodes;
    }
    private bool CheckSameColorNodes(List<Block> blockToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Block>
    {
        for (int i = 0; i < blockToCheck.Count; i++)
        {
            Block connectedBlock = blockToCheck[i];
            if (!alreadyCheckedNodes.Contains(connectedBlock))
            {
                alreadyCheckedNodes.Add(connectedBlock);

                if (!IsSameColor(alreadyCheckedNodes))
                {
                    alreadyCheckedNodes.Remove(connectedBlock);
                }
                else
                {
                    nodeToExplored.Enqueue(connectedBlock);
                }
            }
        }
        return true;
    }
    private bool CheckAnyColorBlocks(List<Block> BlockToCheck) //Check a list of node, Add it to queue "nodeToExplored" if same color, and to alreadyCheckedNodes List<Block>
    {
        for (int i = 0; i < BlockToCheck.Count; i++)
        {
            Block connectedBlock = BlockToCheck[i];
            if (!alreadyCheckedNodes.Contains(connectedBlock))
            {
                alreadyCheckedNodes.Add(connectedBlock);
                nodeToExplored.Enqueue(connectedBlock);
            }
        }
        return true;
    }
    private List<Block> GetAllConnectionNodesWithSameColor(Block block) //return for a node, all his connections of same color, tobe used after 3 vertical detected
    {

        if (block.connectedToVertical.Count == 0 && block.connectedToHorizontal.Count == 0) { return alreadyCheckedNodes; }

        alreadyCheckedNodes = new List<Block>();
        bool isRunning = true;
        nodeToExplored.Clear();
        nodeToExplored.Enqueue(block);
        alreadyCheckedNodes.Add(block);
        while (nodeToExplored.Count > 0 && isRunning)
        {
            currentSearchBlock = nodeToExplored.Dequeue();

            List<Block> nodesToChecked = new List<Block>();
            nodesToChecked.AddRange(currentSearchBlock.connectedToVertical);
            nodesToChecked.AddRange(currentSearchBlock.connectedToHorizontal);

            isRunning = CheckSameColorNodes(nodesToChecked);
        }
        return alreadyCheckedNodes;
    }
    private bool IsSameColor(List<Block> blocks)
    {
        Color testColor = new Color();

        for (int i = blocks.Count-1; i < blocks.Count; i++)
        {
            testColor = blocks[0].GetBlockColor();

            if (testColor != blocks[i].GetBlockColor())
            {
                return false;
            }
        }
        return true;
    }

    private bool IsListMinNumber(List<Block> blockList)//Si liste.Count >= minNumberForVerticalCOmbo return true
    {
        if (blockList.Count >= minNumberForVerticalConnection)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void MatchedNodeList(List<Block> blocks)
    {
        foreach (Block block in blocks)
        {
            block.isMatched = true;
        }
    }//Set all isMatched bool at true for the list


    public void DematchAll(Dictionary<Vector2Int, Block> grid, Vector2Int gridSize)//TODO a refaire avec des events pour updates les collisiton
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int coordinates = new Vector2Int(x, y);

                if (grid[coordinates] != null)
                {
                    grid[coordinates].isMatched = false;
                }

            }
        }
    }

}
