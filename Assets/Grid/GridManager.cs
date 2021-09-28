using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraZoom = 7.29f;


    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int();
    Block[] blockSpawned;


    GridSpawner gridspawner;
    ConnectionHandler connectionHandler;

    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    public delegate void UpdateConnectionDelegate();
    public event UpdateConnectionDelegate updateConnectionEvent;

    void Start()
    {
        gridspawner = GetComponent<GridSpawner>();
        connectionHandler = GetComponent<ConnectionHandler>();
        //ResetGrid();
    }

    private void ResetGrid()
    {
        gridspawner.InitGrid();
        gridSize = gridspawner.GetGridSize();
        grid = gridspawner.GetGrid();
        blockSpawned = gridspawner.GetBlockSpawned();
        UpdateAllNodeConnection();
        int i = 0;
        bool connectionpresent = true;

        while (connectionpresent && i < 100) //Boucle pour generer une grille sans match
        {
            gridspawner.InitGrid();
            gridSize = gridspawner.GetGridSize();
            grid = gridspawner.GetGrid();
            blockSpawned = gridspawner.GetBlockSpawned();

            //gridspawner.RandomizeAllBlockColor();


            UpdateAllNodeConnection();
            connectionpresent = connectionHandler.IsThereAConnection(grid, gridSize); //True if match is present
            i++;
        }
        Debug.Log(connectionpresent + "After " + i + " itération");

        SetCameraToMiddleOfGrid();

    }

    void Update()
    {
        SetCameraToMiddleOfGrid();
        PushSpaceToResetGrid();
        PushEnterToTestFotMatched3(grid, gridSize);
        PushBackSpaceToDematch(grid, gridSize);


    }
    public Node GetNode(Vector2Int coordinates)
    {
        if (grid.ContainsKey(coordinates))
        {
            return grid[coordinates];
        }
        return null;
    }
    public Block[] GetBlockSpawned()
    {
        return blockSpawned;
    }

    private void PushBackSpaceToDematch(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            connectionHandler.DematchAll(grid, gridSize);
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
    private void PushEnterToTestFotMatched3(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TestFor3Match();
        }
    }
    public void UpdateAllNodeConnection() //Call UpdateConnectedTo() on all itemCollider subscribte to the event
    {
        if (updateConnectionEvent != null)
        {
            updateConnectionEvent();
        }
    }

    private void SetCameraToMiddleOfGrid()
    {
        mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -10);
        mainCamera.orthographicSize = cameraZoom;
    }//Set camera position to middle of the Grid

    public void TestFor3Match() //Update all connection and check for one combo
    {
        List<Node> matchedNodeList = new List<Node> { };
        UpdateAllNodeConnection();
        matchedNodeList = connectionHandler.GetFirstMatch(grid, gridSize);
        if (matchedNodeList != null)
        {
            connectionHandler.MatchedNodeList(matchedNodeList);
        }

    }


}
