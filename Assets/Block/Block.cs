using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    //public Vector2 coordinates;
    Material material;
    //[SerializeField] public List<Block> connectedToVertical;
    //[SerializeField] public List<Block> connectedToHorizontal;
    [SerializeField] public Node node;
    GridManager gridManager;
    [SerializeField] Vector2 coordinates;



    private void Start()
    {
        material = GetComponentInChildren<MeshRenderer>().material;
        gridManager = FindObjectOfType<GridManager>();
        coordinates = GetComponent<ItemCoordinatesLabeler>().ItemCoordinates;

        if(gridManager.Grid.ContainsKey(coordinates))
        {
            node = gridManager.GetNode(coordinates);
        }
    }

    private void Update()
    {
        material.color = node.color;
    }

}
