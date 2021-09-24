using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    [SerializeField] Block blockPrefab;
    [SerializeField] Color[] colors;
    Dictionary<Vector2, Node> grid = new Dictionary<Vector2, Node>();
    Vector2Int gridSize = new Vector2Int(4, 4);
    ItemCollider itemCollider;
    [SerializeField] Camera mainCamera;
    int blankLinesOnBottom = 10;

    public Dictionary<Vector2, Node> Grid { get { return grid; } }
    public Vector2Int GridSize { get { return gridSize; } }

    void Start()
    {
        gridSize.y += blankLinesOnBottom;
        SetCameraToMiddleOfGrid();
        ClearGrid();
        CreateGrid();
        IsMatch3Vertical();

    }
    void Update()
    {
        PushSpaceToResetGrid();
    }

    public Node GetNode(Vector2 coordinates)
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
            for (int y = blankLinesOnBottom; y < gridSize.y; y++)
            {
                Vector2 coordinates = new Vector2(x, y);
                Color blockColor = colors[Random.Range(0, colors.Length)];


                    Block block_ = Instantiate(blockPrefab, coordinates, Quaternion.identity, transform);
                    block_.GetComponentInChildren<MeshRenderer>().material.color = blockColor;
                    //Node node_ = new Node(coordinates, blockColor);
                    Node node_ = ScriptableObject.CreateInstance<Node>();
                    node_.Init(coordinates, blockColor);
                    grid.Add(coordinates, node_);





            }
        }
    }
    void IsMatch3Vertical() //Chek throught all block in Grid to see if one has 2+ vertical connection and stock the connected nodes in a list
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2 coordinates = new Vector2(x, y);
                if (grid.ContainsKey(coordinates))
                {

                    Node node = grid[coordinates];
                    if (node.connectedToVertical.Count >= 2)
                    {
                        List<Node> testNodes = new List<Node>();
                        testNodes.Add(node);
                        testNodes.AddRange(node.connectedToVertical);
                        if (IsSameColor(testNodes))
                        {
                            ColorNodeList(testNodes, Color.black);
                            Debug.Log("MATCH3 GOTTEM : Item : " + node.name + "has : " + node.NumberOfVerticalConnection() + "Vertical connection");
                        }
                    }
                }

            }
        }
    }

    public bool IsSameColor(List<Node> nodes) //Check if a list of block are of the same color
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

    public void ColorNodeList(List<Node> nodes, Color color)
    {
        foreach (Node node in nodes)
        {
            node.color = color; ;
        }
    }






    void PushSpaceToResetGrid() //Press Space to reset Grid et instanciate new blocks
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClearGrid();
            CreateGrid();
            foreach (Transform child in transform)
            {
                child.GetComponentInChildren<ItemCollider>().UpdateConnectedTo();
            }
            IsMatch3Vertical();

        }
    }//TODO a refaire avec des events pour updates les collisiton

    private void SetCameraToMiddleOfGrid()
    {
        mainCamera.transform.position = new Vector3(gridSize.x / 2, (gridSize.y) / 2, -13.33f);
    }//Set camera position to middle of the Grid

}
