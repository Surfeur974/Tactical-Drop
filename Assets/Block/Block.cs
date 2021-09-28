using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Node node;
    [SerializeField] Vector2Int coordinates;
    [SerializeField] Color matchedColor;
    public bool isInHand;
    GridManager gridManager;

    Color initColor;
    Material material;
    ItemCoordinatesLabeler itemCoordinatesLabeler;
    public Vector2Int Coordinates { get { return coordinates; } }
    public bool IsInHand { get { return isInHand; } }


    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        material = GetComponentInChildren<MeshRenderer>().material;
        itemCoordinatesLabeler = GetComponent<ItemCoordinatesLabeler>();

        initColor = material.color;
        coordinates = itemCoordinatesLabeler.GetCoordinates();

        if (gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
        }
    }

    private void Update()
    {
        if (IsInHand == false)
        {
            UpdateSelfNode();
            ChangeBlockState();
        }
    }

    private void UpdateSelfNode()
    {
        coordinates = itemCoordinatesLabeler.GetCoordinates();
        if (gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
            node.updateNodeColor(material.color);
        }
    }

    private void ChangeBlockState()
    {
        if (node.isMatched == true)
        {
            HandleMatched();
        }
    }

    private void HandleMatched()
    {
        node.Init();
        this.transform.gameObject.SetActive(false);
    }

    public Node GetCurrentNode()
    {
        return node;
    }

}
