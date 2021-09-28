using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Node node;
    [SerializeField] Vector2Int coordinates;
    [SerializeField] Color matchedColor;

    GridManager gridManager;

    Color initColor;
    Material material;
    ItemCoordinatesLabeler itemCoordinatesLabeler;
    public Vector2Int Coordinates {get{ return coordinates;}}
    
    
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
        if (node.isInHand == false)
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
            material.color = matchedColor;
        }
        else
        {
            material.color = initColor;
        }
    }

}
