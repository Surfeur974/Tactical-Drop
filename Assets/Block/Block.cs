using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] int blockScore;
    [SerializeField] Vector2Int coordinates;
    public bool isInHand;
    GridManager gridManager;
    UIDisplay uIDisplay;

    Color initColor;
    Material material;
    public Vector2Int Coordinates { get { return coordinates; } }
    public bool IsInHand { get { return isInHand; } }
    [SerializeField] Node node;

    private void OnEnable()
    {
        blockScore = 100;
        gridManager = FindObjectOfType<GridManager>();
        uIDisplay = FindObjectOfType<UIDisplay>();
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
            node.SetCurrentBlock(this);
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
        uIDisplay.AddScore(blockScore, initColor);
        node.HandleMatched();
        this.transform.gameObject.SetActive(false);
    }

    public Node GetCurrentNode()
    {
        return node;
    }

}
