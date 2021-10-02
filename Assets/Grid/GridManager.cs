using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] float cameraZoom = 7.29f;
    [SerializeField] bool gridWithoutMatchAtStart;

    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int();
    Block[] blockSpawned;


    GridSpawner gridspawner;
    ConnectionHandler connectionHandler;

    public Dictionary<Vector2Int, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    public delegate void UpdateConnectionDelegate();
    public event UpdateConnectionDelegate updateConnectionEvent;

    public delegate void MoveDownCollumnsDelegate();
    public event MoveDownCollumnsDelegate moveDownCollumnEvent;

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
        CallEventUpdateNodeConnection();

        if (gridWithoutMatchAtStart)
        {
            StartCoroutine(NoMatchGrid());
        }


        SetCameraToMiddleOfGrid();

    }
    IEnumerator NoMatchGrid() //TODO can be optimize
    {
        int i = 0;
        while (connectionHandler.IsThereAConnection(grid, gridSize))
        {
            gridspawner.RandomizeAllBlockColor(blockSpawned);
            yield return null;

            CallEventUpdateNodeConnection();
            i++;
        }
        Debug.Log("After " + i + " itération");

    }
    void Update()
    {
        SetCameraToMiddleOfGrid();
        PushSpaceToResetGrid();
        PushEnterToTest(grid, gridSize);
        PushBackSpaceToDematch(grid, gridSize);
        CallEventMovedownAllCollumn();
        AddTopLineTrigger();
        //IF match3

        //IF voidupBlock

    }
    public void TestFor3Match()
    {
        List<Node> matchedNodeList = new List<Node> { };
        CallEventUpdateNodeConnection();
        matchedNodeList = connectionHandler.GetFirstMatch(grid, gridSize);
        if (matchedNodeList != null)
        {
            connectionHandler.MatchedNodeList(matchedNodeList);
        }
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

    private void PushEnterToTest(Dictionary<Vector2Int, Node> grid, Vector2Int gridSize) //Update all connection and check for one combo
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TestFor3Match();
            //StartCoroutine(NoMatchGrid());
        }
    }
    public void CallEventUpdateNodeConnection() //Call UpdateConnectedTo() on all itemCollider subscribte to the event
    {
        if (updateConnectionEvent != null)
        {
            updateConnectionEvent();
        }
    }
    private void CallEventMovedownAllCollumn()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (moveDownCollumnEvent != null)
            {
                moveDownCollumnEvent();
            }
        }
    }
    private void AddTopLineTrigger()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            gridspawner.AddTopLine(grid, gridSize);
        }
    }
    private void SetCameraToMiddleOfGrid()
    {
        mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -10);
        mainCamera.orthographicSize = cameraZoom;
    }//Set camera position to middle of the Grid




}
