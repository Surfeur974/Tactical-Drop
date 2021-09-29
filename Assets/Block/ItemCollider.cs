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
    public void UpdateBlock()
    {
        StartCoroutine(MoveBlock());
    }
    IEnumerator MoveBlock() //TODO Work pour monter les bloc dans les vide mais pasa tout le temps..
    {
        yield return StartCoroutine(UpdateConnectedTo()); //On attend la fin de cette coroutine
        TopHitRaycast(10); //if void au dessus bouge bloc jusqu'a pas vide
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
    private void Update()
    {
        //DrawRaycast();
    }
    public RaycastHit TopHitRaycast(int raylenght) //Used for detecting if bloc has something up him
    {

        UpdateRaycastOrigins();
        raycastOrigins.up.y -= .1f;

        Physics.Raycast(raycastOrigins.up, Vector2.up, out topHit, raylenght);
        ////Debug.DrawRay(raycastOrigins.up, Vector2.up * topHit.distance, Color.red);

        if ((topHit.distance > 1f || topHit.transform == null) && hand == null && block.Coordinates.y < gridManager.GridSize.y-1)
        {
            block.GetCurrentNode().ClearConnection();
            float maxYPosition = gridManager.GridSize.y;
            Vector3Int startPosition = Vector3Int.RoundToInt(transform.position);
            Vector3Int endPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(maxYPosition), 0);

            if (topHit.transform != null)
            {
                endPosition = Vector3Int.RoundToInt(topHit.transform.position);
            }

            endPosition += Vector3Int.down;

            StartCoroutine(MoveUpBlockIfVoid(block.gameObject, startPosition, endPosition));
        }
        return topHit;
    }

    IEnumerator MoveUpBlockIfVoid(GameObject objectToMove, Vector3Int startposition, Vector3Int endposition)
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement
        float t = 0;
        float timeToTravel = 0.1f;

        //objectToMove.GetComponent<BoxCollider>().enabled = false;
        //objectToMove .transform.position = endposition;

        while (t < 1)
        {
            t += timeToTravel;
            objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(.8f, 1.2f, 1);
            objectToMove.transform.position = Vector3.Lerp(startposition, endposition, t);

            //Debug.Log(t);
            yield return null;
        }
        //Needs to cal TopHitRaycast because if a colum of block the first already moves
        gridManager.TestFor3Match();

        yield return null;
        //objectToMove.GetComponent<BoxCollider>().enabled = true;
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);
        gridManager.TestFor3Match();

    }


}
