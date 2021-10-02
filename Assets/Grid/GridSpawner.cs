using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    [SerializeField] Vector2Int gridSize = new Vector2Int(8, 18);
    [SerializeField] int blankLinesOnBottom = 10;

    [SerializeField] Block[] blockSpawned;

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();

    public Dictionary<Vector2Int, Node> GetGrid1()
    { return grid; }
    public void InitGrid()
    {
        //gridSize.y += blankLinesOnBottom; //Pour avoir des lignes vides en bas
        ClearGrid();
        CreateGrid();
    }
    void ClearGrid() //Destroy All child of Grid
    {
        grid.Clear();

        for (int i = 0; i < blockSpawned.Length; i++)
        {
            if (blockSpawned[i] != null)
            {
                Destroy(blockSpawned[i].transform.gameObject);
            }
        }

        blockSpawned.Initialize();
    }
    void CreateGrid()  //Instantiate block with 3 random color set in inspector, create a node for set (position and color), Add pair coordinate:node in dictionnary
    {
        int i = 0;
        blockSpawned = new Block[gridSize.x * (gridSize.y + blankLinesOnBottom)];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {

                Vector2Int coordinates = new Vector2Int(x, y);
                Color blockColor = colors[Random.Range(0, colors.Length)];

                //Color blockColor = colors[Random.Range(0, colors.Length-1)];
                //if (x == gridSize.x - 1 || x == gridSize.x - 3 || x == gridSize.x - 5 || x == 0) { blockColor = colors[2]; }
                //if (y == gridSize.y-1) { blockColor = colors[2]; }
                //if (coordinates == new Vector2Int(2, 12)) { blockColor = colors[0]; }
                //if (coordinates == new Vector2Int(2, 11)) { blockColor = colors[0]; }
                //if (coordinates == new Vector2Int(2, 10)) { blockColor = colors[1]; }


                //Color blockColor = colors[0];
                //if (x == 0) { blockColor = colors[1]; }
                //if (y == gridSize.y - 7) { blockColor = colors[2]; }

                //Color blockColor = colors[0];
                //if (x == 0) { blockColor = colors[1]; }
                //if (y == gridSize.y - 7 || y == gridSize.y-3 || y == gridSize.y - 1 || y == gridSize.y - 2) { blockColor = Color.cyan; }

                Node node_ = ScriptableObject.CreateInstance<Node>();
                node_.Init(coordinates);
                grid.Add(coordinates, node_);

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                    node_.Init(coordinates, blockColor); //Si on instancie un block on init la couleur du nodes aussi
                    blockSpawned[i] = block_;
                    i++;
                    
                }


            }
        }
    }
    public void AddTopLine(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //TODO add function to add line without match 3
    {
        int blockSpawnedNullIndex = 0; //Variable to store the index of the last block added and no to redo all the array search
        for (int x = 0; x < gridSize.x; x++)
        {
            int y = gridSize.y-1;
            Vector2Int coordinates = new Vector2Int(x, y);
            Color blockColor = colors[Random.Range(0, colors.Length)];

            Node node_ = grid[coordinates];
            Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);

            block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
            node_.Init(coordinates, blockColor); //Si on instancie un block on init la couleur du nodes aussi

            for (int i = blockSpawnedNullIndex; i < blockSpawned.Length; i++)
            {
                if(blockSpawned[i] == null)
                {
                    blockSpawnedNullIndex = i;
                    blockSpawned[i] = block_;
                    break;
                }
            }
        }
    }

    public void RandomizeAllBlockColor(Block[] blocksToRandomize)
    {
        for (int i = 0; i < blocksToRandomize.Length; i++)
        {
            Color blockColor = colors[Random.Range(0, colors.Length)];

            if (blocksToRandomize[i] != null)
            {
                blocksToRandomize[i].GetComponentInChildren<MeshRenderer>().material.color = blockColor;
            }
        }
    }

    public Vector2Int GetGridSize()
    {
        return gridSize;
    }
    public Dictionary<Vector2Int, Node> GetGrid()
    {
        return grid;
    }
    public Block[] GetBlockSpawned()
    {
        return blockSpawned;
    }

}
