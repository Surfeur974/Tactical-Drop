using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionHandler : MonoBehaviour
{
    [SerializeField] int minNumberForVerticalConnection = 3;
    [SerializeField] GridSpawner gridspawner;


    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    Vector2Int gridSize = new Vector2Int();

    Node currentSearchNode;
    List<Node> linkedNodeOfSameColor = new List<Node>();
    List<Node> alreadyCheckedNodes = new List<Node>();
    Queue<Node> nodeToExplored = new Queue<Node>();









}
