using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //to see public value in inspector
public class Node : ScriptableObject
{
    public Vector2 coordinates;
    public Color color;
    public List<Node> connectedToVertical = new List<Node>();
    public List<Node> connectedToHorizontal = new List<Node>();
    GridManager gridManager;

    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
    }
    public void Init(Vector2 coordinates, Color color)
    {
        this.coordinates = coordinates;
        this.color = color;
        UpdateNameObject();
        ClearConnection();
    }
    public void AddVerticalConnection(Node nodeToAdd)
    {
        if (!connectedToVertical.Contains(nodeToAdd))
        {
            connectedToVertical.Add(nodeToAdd);
        }
    }
    public void AddHorizontalConnection(Node nodeToAdd)
    {
        if (!connectedToHorizontal.Contains(nodeToAdd))
        {
            connectedToHorizontal.Add(nodeToAdd);
        }
    }
    public void ClearConnection()
    {
        if(connectedToHorizontal != null)
        {
            connectedToHorizontal.Clear();
        }
        if (connectedToVertical != null)
        {
            connectedToVertical.Clear();
        }
    }
    public int NumberOfVerticalConnection()
    {
        int numberOfVerticalConnection = 0;
        return numberOfVerticalConnection = connectedToVertical.Count;
    }

    private void UpdateNameObject()
    {
        this.name = "(" + coordinates.x + "," + coordinates.y + ")";
    }
}


