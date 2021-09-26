using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField] Node node;
    [SerializeField] Vector2Int coordinates;
    [SerializeField] Color matchedColor;

    GridManager gridManager;
    Color initColor;
    Material material;

    public Vector2Int Coordinates {get{ return coordinates;}}
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        material = GetComponentInChildren<MeshRenderer>().material;
        coordinates = GetComponent<ItemCoordinatesLabeler>().ItemCoordinates;

        initColor = material.color;


        if (gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
        }
    }

    private void Update()
    {
        if (node.isInHand == false)
        {
            ChangeBlockColor();

            coordinates = GetComponent<ItemCoordinatesLabeler>().ItemCoordinates;

            if (gridManager.Grid.ContainsKey(coordinates))
            {
                node = gridManager.GetNode(coordinates);
                node.updateNodeColor(material.color);
            }
        }
    }
    private void ChangeBlockColor()
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
    public Vector2Int GetCoordinates()
    {
        return coordinates;
    }
}
