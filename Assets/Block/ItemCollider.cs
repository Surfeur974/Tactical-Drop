using System.Collections.Generic;
using UnityEngine;

public class ItemCollider : MonoBehaviour
{
    Bounds bounds;
    BoxCollider boxCollider;
    RaycastOrigin raycastOrigins;
    float skinWidth = 0.1f;
    int defaultraycastDistance = 1;
    int currentRaycastDistance = 1;
    RaycastHit rightHitInfo, leftHitInfo, upHitInfo, downHitInfo;
    GridManager gridManager;
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

    void Awake()
    {


    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        gridManager = FindObjectOfType<GridManager>();
        
        
        //grid = gridManager.Grid;
        //node = gridManager.GetNode(coordinates);

        if(!transform.GetComponent<Hand>())
        {
        gridManager.updateConnectionEvent += UpdateConnectedTo;
        }
    }
    private void OnDestroy()
    {
        gridManager.updateConnectionEvent -= UpdateConnectedTo;

    }


    private void UpdateBoolCollisionState()
    {
        boolCollision = new BoolCollision(false);
        currentRaycastDistance = defaultraycastDistance;

        bool isRightHit = Physics.Raycast(raycastOrigins.right, Vector2.right, out rightHitInfo, currentRaycastDistance);
        bool isLefttHit = Physics.Raycast(raycastOrigins.left, Vector2.left, out leftHitInfo, currentRaycastDistance);
        bool isUpHit = Physics.Raycast(raycastOrigins.up, Vector2.up, out upHitInfo, currentRaycastDistance);
        bool isDownHit = Physics.Raycast(raycastOrigins.down, Vector2.down, out downHitInfo, currentRaycastDistance);

        if (isUpHit)
        {
            Debug.DrawRay(raycastOrigins.up, Vector2.up * upHitInfo.distance, Color.red);
            boolCollision.isUpHit = true;
        }
        if (isDownHit)
        {
            Debug.DrawRay(raycastOrigins.down, Vector2.down * downHitInfo.distance, Color.red);
            boolCollision.isDownHit = true;

        }
        if (isLefttHit)
        {
            Debug.DrawRay(raycastOrigins.left, Vector2.left * leftHitInfo.distance, Color.red);
            boolCollision.isLeftHit = true;

        }
        if (isRightHit)
        {
            Debug.DrawRay(raycastOrigins.right, Vector2.right * rightHitInfo.distance, Color.red);
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


    public void UpdateConnectedTo()
    {
        coordinates = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        grid = gridManager.Grid;
        node = gridManager.GetNode(coordinates);

             if (node == null) { return; }
        if (node.isInHand == true)
        {
            node.ClearConnection();
            return;
        }


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
}
