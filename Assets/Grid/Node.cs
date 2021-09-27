using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //to see public value in inspector
public class Node : ScriptableObject
{
    public Vector2Int coordinates;
    public Color color;
    public List<Node> connectedToVertical = new List<Node>();
    public List<Node> connectedToHorizontal = new List<Node>();
    public bool isMatched;
    public bool isInHand;
    public bool isCHecked;

    public bool IsMatched { get { return isMatched; } }
    public bool IsInHand { get { return isInHand; } }
    public bool IsCHecked { get { return isCHecked; } }


    public void Init(Vector2Int coordinates)
    {
        this.coordinates = coordinates;
        UpdateNameObject();
        ClearConnection();
        isMatched = false;
        isInHand = false;
    }
    public void Init(Vector2Int coordinates, Color colorToInit)
    {
        this.coordinates = coordinates;
        this.color = colorToInit;
        UpdateNameObject();
        ClearConnection();
        isMatched = false;
        isInHand = false;
    }
    public void Init()
    {
        UpdateNameObject();
        ClearConnection();
        isMatched = false;
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
    public void RemoveConnection(Node nodeToRemove)
    {

        if (connectedToVertical.Contains(nodeToRemove))
        {
            connectedToVertical.Remove(nodeToRemove);
        }
        else if (connectedToHorizontal.Contains(nodeToRemove))
        {
            connectedToHorizontal.Remove(nodeToRemove);
        }
        else
        {
            Debug.Log("Cannot remove Node");
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
    public void updateNodeColor(Color colorToUpdateTo)
    {
        this.color = colorToUpdateTo;
    }
    public void UpdateConnectionFromNode()
    {

    }

}


