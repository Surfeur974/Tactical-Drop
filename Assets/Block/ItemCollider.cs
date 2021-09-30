using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollider : MonoBehaviour
{
    Bounds bounds;
    BoxCollider boxCollider;
    RaycastOrigin raycastOrigins;
    float skinWidth = 0.1f;
    [SerializeField] int defaultraycastDistance = 1;
    int currentRaycastDistance = 1;
    RaycastHit rightHitInfo, leftHitInfo, upHitInfo, downHitInfo;
    RaycastHit topHit;
    GridManager gridManager;
    Block[] blockSpawned;
    Block block;
    Hand hand;
    ConnectionHandler connectionHandler;
    Node node;
    BoolCollision boolCollision;
    Vector2Int coordinates;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public int DefaultraycastDistance
    {  //GETTER SETTER
        get { return defaultraycastDistance; }
        set { defaultraycastDistance = value; }
    }

    struct RaycastOrigin
    {
        public Vector2 up, down;
        public Vector2 left, right;
    }
    struct BoolCollision
    {
        public bool isUpHit, isDownHit;
        public bool isLeftHit, isRightHit;

        public BoolCollision(bool initBool)
        {
            isUpHit = isDownHit = isLeftHit = isRightHit = initBool;
        }
    }

    void OnEnable()
    {
        boxCollider = GetComponent<BoxCollider>();
        gridManager = FindObjectOfType<GridManager>();
        blockSpawned = gridManager.GetBlockSpawned();
        connectionHandler = gridManager.GetComponent<ConnectionHandler>();
        block = GetComponent<Block>();
        hand = GetComponent<Hand>();
        //grid = gridManager.Grid;
        //node = gridManager.GetNode(coordinates);

        if (!transform.GetComponent<Hand>())
        {
            gridManager.updateConnectionEvent += UpdateBlock;
        }
    }
    private void OnDisable()
    {
        if (!gridManager)
        {
            return;
        }
        gridManager.updateConnectionEvent -= UpdateBlock;

    }
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Debug.Log("TopHitRaycast(10); called");
        //    TopHitRaycast(10);
        //}
    }
    public void UpdateBlock()
    {
        StartCoroutine(MoveBlock());
    }
    IEnumerator MoveBlock() //Coroutine called by grid manager Event update
    {
        yield return StartCoroutine(UpdateConnectedTo()); //On update les connection et on attend la fin grace au yield return

        bool isTopHitRaycatHit = TopHitRaycast(10); //On test si le bloc a un espace libre au dessus

        while (isTopHitRaycatHit) //true if need to move
        {
            yield return StartCoroutine(MoveUpCollumnBlockIfVoid()); //On le move et on attend la fin

            //isTopHitRaycatHit = TopHitRaycast(10);  //On test si le bloc a un espace libre au dessus
            StopAllCoroutines();    //On stop toutes les coroutines de tous le monde comme �a le dernier va call gridManager.TestFor3Match(); et permettre un update de la grid unique "Singlemerde"
            gridManager.TestFor3Match();

        }
    }

    public void UpdateBoolCollisionState()
    {
        boolCollision = new BoolCollision(false);
        currentRaycastDistance = defaultraycastDistance;

        bool isRightHit = Physics.Raycast(raycastOrigins.right, Vector2.right, out rightHitInfo, currentRaycastDistance);
        bool isLefttHit = Physics.Raycast(raycastOrigins.left, Vector2.left, out leftHitInfo, currentRaycastDistance);
        bool isUpHit = Physics.Raycast(raycastOrigins.up, Vector2.up, out upHitInfo, currentRaycastDistance);
        bool isDownHit = Physics.Raycast(raycastOrigins.down, Vector2.down, out downHitInfo, currentRaycastDistance);

        if (isUpHit)
        {
            //Debug.DrawRay(raycastOrigins.up, Vector2.up * upHitInfo.distance, Color.red);
            boolCollision.isUpHit = true;
        }
        if (isDownHit)
        {
            //Debug.DrawRay(raycastOrigins.down, Vector2.down * downHitInfo.distance, Color.red);
            boolCollision.isDownHit = true;

        }
        if (isLefttHit)
        {
            //Debug.DrawRay(raycastOrigins.left, Vector2.left * leftHitInfo.distance, Color.red);
            boolCollision.isLeftHit = true;

        }
        if (isRightHit)
        {
            //Debug.DrawRay(raycastOrigins.right, Vector2.right * rightHitInfo.distance, Color.red);
            boolCollision.isRightHit = true;
        }
    }
    private void UpdateRaycastOrigins()
    {
        bounds = boxCollider.bounds;
        bounds.Expand(-skinWidth);
        raycastOrigins.up = new Vector2(bounds.center.x, bounds.max.y);
        raycastOrigins.down = new Vector2(bounds.center.x, bounds.min.y);
        raycastOrigins.right = new Vector2(bounds.center.x + bounds.extents.x, bounds.center.y);
        raycastOrigins.left = new Vector2(bounds.center.x - bounds.extents.x, bounds.center.y);
    }


    IEnumerator UpdateConnectedTo()
    {
        coordinates = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        grid = gridManager.Grid;
        node = gridManager.GetNode(coordinates);

        if (node == null) { yield return null; }
        //if (node.isInHand == true) { node.ClearConnection(); return; }

        UpdateRaycastOrigins();
        UpdateBoolCollisionState();
        node.ClearConnection();


        if (boolCollision.isUpHit)
        {
            {
                Vector2Int upHitPosition = (Vector2Int)Vector3Int.RoundToInt(upHitInfo.transform.position);
                if (grid.ContainsKey(upHitPosition))
                {
                    node.AddVerticalConnection(grid[upHitPosition]);
                }
            }
        }
        if (boolCollision.isDownHit)
        {
            {
                Vector2Int downHitPosition = (Vector2Int)Vector3Int.RoundToInt(downHitInfo.transform.position);
                if (grid.ContainsKey(downHitPosition))
                {
                    node.AddVerticalConnection(grid[downHitPosition]);
                }
            }
        }
        if (boolCollision.isLeftHit)
        {
            {
                Vector2Int leftHitPosition = (Vector2Int)Vector3Int.RoundToInt(leftHitInfo.transform.position);
                if (grid.ContainsKey(leftHitPosition))
                {
                    node.AddHorizontalConnection(grid[leftHitPosition]);
                }
            }
        }
        if (boolCollision.isRightHit)
        {
            {
                Vector2Int rightHitPosition = (Vector2Int)Vector3Int.RoundToInt(rightHitInfo.transform.position);
                if (grid.ContainsKey(rightHitPosition))
                {
                    node.AddHorizontalConnection(grid[rightHitPosition]);
                }
            }
        }
        yield return null;

    }

    public RaycastHit GetUpHitInfo()
    {
        UpdateRaycastOrigins();
        UpdateBoolCollisionState();
        return upHitInfo;
    }
    public void DrawRaycast()
    {
        UpdateRaycastOrigins();
        UpdateBoolCollisionState();
    }
    private bool TopHitRaycast(int raylenght) //Used for detecting if bloc has something up him
    {
        //Debug.Log("TopHitRaycast is called from " + this.name);
        UpdateRaycastOrigins();
        raycastOrigins.up.y -= .1f;

        Physics.Raycast(raycastOrigins.up, Vector2.up, out topHit, raylenght);
        ////Debug.DrawRay(raycastOrigins.up, Vector2.up * topHit.distance, Color.red);

        if ((topHit.distance > 1f || topHit.transform == null) && hand == null && block.Coordinates.y < gridManager.GridSize.y - 1)
        {
            block.GetCurrentNode().ClearConnection();
            return true;
        }
        return false;
    }

    IEnumerator MoveUpCollumnBlockIfVoid() //Group 
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement

        GameObject objectToMove = this.gameObject;
        Transform oldParent = objectToMove.transform.parent;
        Node nodeToMove = block.GetCurrentNode();
        List<Node> nodesToMove = new List<Node>();

        yield return StartCoroutine(UpdateConnectedTo());

        nodesToMove.AddRange(connectionHandler.GetAllVerticalConnection(nodeToMove));   ///PROBLEME ici le node n'a plus de connection, car on les efface pour eviter bug

        GameObject groupToMove = new GameObject("GroupToMove");
        groupToMove.transform.position = objectToMove.transform.position;
        for (int i = 0; i < nodesToMove.Count; i++)
        {
            nodesToMove[i].ClearConnection(); //Need to clear connection before moving
            nodesToMove[i].CurrentBlock.transform.parent = groupToMove.transform;
        }

        objectToMove = groupToMove;


        float maxYPosition = gridManager.GridSize.y;
        Vector3Int startPosition = Vector3Int.RoundToInt(transform.position);
        Vector3Int endPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(maxYPosition), 0);

        if (topHit.transform != null)
        {
            endPosition = Vector3Int.RoundToInt(topHit.transform.position);
        }
        endPosition += Vector3Int.down;

        float t = 0;
        float blockFallSpeed = 0.1f;
        while (t < 1)
        {
            t += blockFallSpeed;
            //objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(.8f, 1.2f, 1);
            objectToMove.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        //objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);

        for (int i = groupToMove.transform.childCount-1; i > -1; i--)
        {
            groupToMove.transform.GetChild(i).transform.SetParent(oldParent);
            //Debug.Log(i);
            //Debug.Log(groupToMove.transform.GetChild(i));
        }
        Destroy(groupToMove);
    }
    IEnumerator MoveUpSingleBlockIfVoid()
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement

        GameObject objectToMove = this.gameObject;
        float maxYPosition = gridManager.GridSize.y;
        Vector3Int startPosition = Vector3Int.RoundToInt(transform.position);
        Vector3Int endPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(maxYPosition), 0);

        if (topHit.transform != null)
        {
            endPosition = Vector3Int.RoundToInt(topHit.transform.position);
        }
        endPosition += Vector3Int.down;

        float t = 0;
        float timeToTravel = 0.1f;
        while (t < 1)
        {
            t += timeToTravel;
            objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(.8f, 1.2f, 1);
            objectToMove.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);
    }
}
