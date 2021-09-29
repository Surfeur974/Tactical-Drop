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
    public Vector2Int Coordinates { get { return coordinates; } }
    public bool IsInHand { get { return isInHand; } }

    private void OnEnable()
    {
        gridManager = FindObjectOfType<GridManager>();

        material = GetComponentInChildren<MeshRenderer>().material;
        coordinates.x = Mathf.RoundToInt(transform.position.x);
        coordinates.y = Mathf.RoundToInt(transform.position.y);

        initColor = material.color;
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
        coordinates.x = Mathf.RoundToInt(transform.position.x);
        coordinates.y = Mathf.RoundToInt(transform.position.y);
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
        //node.Init();
        node.HandleMatched();
        this.transform.gameObject.SetActive(false);
    }

    public Node GetCurrentNode()
    {
        return node;
    }

}
