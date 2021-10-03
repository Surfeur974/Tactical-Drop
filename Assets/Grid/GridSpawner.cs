using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    [SerializeField] Vector2Int gridSize = new Vector2Int(8, 18);
    [SerializeField] int blankLinesOnBottom = 10;

    Dictionary<Vector2Int, Block> grid = new Dictionary<Vector2Int, Block>();
    [SerializeField] Block[] blockSpawned;

    public Dictionary<Vector2Int, Block> GetGrid(){ return grid; }
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
        blockSpawned = new Block[gridSize.x * gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {

                Vector2Int coordinates = new Vector2Int(x, y);
                Color blockColor = colors[Random.Range(0, colors.Length)];

                //Color blockColor = colors[Random.Range(0, colors.Length-1)];
                //if (x == gridSize.x - 1 || x == gridSize.x - 3 || x == gridSize.x - 5 || x == 0) { blockColor = colors[2]; }
                //if (y == gridSize.y-1) { blockColor = colors[2]; }


                grid.Add(coordinates, null);

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);
                    block_.Init(coordinates, blockColor); //Si on instancie un block on init la couleur du nodes aussi
                    grid[coordinates] = block_;


                    blockSpawned[i] = block_;
                    i++;

                }
            }
        }
    }
    public void AddTopLine(Dictionary<Vector2Int, Block> grid, Vector2Int gridSize) //On part du principoe que la top line est vide !!!!!!
    {
        int blockSpawnedNullIndex = 0; //Variable to store the index of the last block added and no to redo all the array search
        if(blockSpawned[blockSpawned.Length-1] != null) { return; }
        for (int x = 0; x < gridSize.x; x++)
        {
            int y = gridSize.y-1;
            Vector2Int coordinates = new Vector2Int(x, y);
            Color blockColor = colors[Random.Range(0, colors.Length)];
            Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);
            block_.Init(coordinates, blockColor); //Si on instancie un block on init la couleur du nodes aussi
            grid[coordinates] = block_;

        for (int i = blockSpawnedNullIndex; i < blockSpawned.Length; i++)
        {
            if (blockSpawned[i] == null)
            {
                blockSpawnedNullIndex = i;
                blockSpawned[i] = block_;
                break;
            }
        }
        }
    }
    public void RandomizeBlockColor(Block[] blocksToRandomize)
    {
        for (int i = 0; i < blocksToRandomize.Length; i++)
        {
            Color blockColor = colors[Random.Range(0, colors.Length)];

            if (blocksToRandomize[i] != null)
            {
                blocksToRandomize[i].ChangeBlockColor(blockColor);
            }
        }
    }

    public Vector2Int GetGridSize()
    {
        return gridSize;
    }
    public Block[] GetBlockSpawned()
    {
        return blockSpawned;
    }
}
