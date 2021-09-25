using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //public Vector2Int coordinates;
    Material material;
    //[SerializeField] public List<Block> connectedToVertical;
    //[SerializeField] public List<Block> connectedToHorizontal;
    [SerializeField] public Node node;
    GridManager gridManager;
    [SerializeField] Vector2Int coordinates;
    [SerializeField] Color matchedColor;
    Color initColor;





    private void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
        gridManager = FindObjectOfType<GridManager>();
        coordinates = GetComponent<ItemCoordinatesLabeler>().ItemCoordinates;

        initColor = material.color;


        if (gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
        }
    }

    private void Update()
    {
        ChangeBlockColor();

        coordinates = GetComponent<ItemCoordinatesLabeler>().ItemCoordinates;

        if (gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
            node.updateNodeColor(material.color);
        }


    }
    private void ChangeBlockColor()
    {
        if (node.isMacthed == true)
        {
            material.color = Color.black;
        }
        else
        {
            material.color = initColor;
        }
    }
}
