using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    [SerializeField] Vector2Int gridSize = new Vector2Int(4, 14);
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

                //Color blockColor = colors[0];
                //if (x == gridSize.x - 1 || x == gridSize.x - 3 || x == gridSize.x - 5) { blockColor = colors[1]; }
                //if (y == gridSize.y - 4 || y == gridSize.y - 2 || y == gridSize.y - 6) { blockColor = colors[1]; }

                //Color blockColor = colors[0];
                //if (x == 0) { blockColor = colors[1]; }
                //if (y == gridSize.y - 7) { blockColor = colors[2]; }

                //Color blockColor = colors[0];
                //if (x == 0) { blockColor = colors[1]; }
                //if (y == gridSize.y - 5 || y == gridSize.y - 4) { blockColor = colors[2]; }

                Node node_ = ScriptableObject.CreateInstance<Node>();
                node_.Init(coordinates);

                if (y >= blankLinesOnBottom)
                {
                    Block block_ = Instantiate(blockPrefab, new Vector3Int(coordinates.x, coordinates.y, 0), Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                    node_.Init(coordinates, blockColor); //Si on instancie un block on init la couleur du nodes aussi
                    blockSpawned[i] = block_;
                    i++;
                }


                grid.Add(coordinates, node_);
            }
        }
    }

    public void RandomizeAllBlockColor(Block[] blocksToRandomize)  //TODO DONT WORK
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
