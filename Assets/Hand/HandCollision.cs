using System.Collections.Generic;
using UnityEngine;

public class HandCollision : MonoBehaviour
{
    Bounds bounds;
    BoxCollider boxCollider;
    RaycastOrigin raycastOrigins;
    float skinWidth = 0.1f;
    float defaultraycastDistance = 2f;
    float currentRaycastDistance = 2f;
    RaycastHit rightHitInfo, leftHitInfo, upHitInfo, downHitInfo;
    GridManager gridManager;
    Node node;
    BoolCollision boolCollision;
    Vector2Int coordinates;
    Dictionary<Vector2Int, Node> grid = new Dictionary<Vector2Int, Node>();
    public float DefaultraycastDistance
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
        gridManager = FindObjectOfType<GridManager>();
        grid = gridManager.Grid;
        boxCollider = GetComponentInChildren<BoxCollider>();
    }
    void Start()
    {

    }
    void Update()
    {

    }


    private void TestFourSideCollision()
    {
        boolCollision = new BoolCollision(false);
        currentRaycastDistance = defaultraycastDistance;

        bool isRightHit = Physics.Raycast(raycastOrigins.right, Vector2.right, out rightHitInfo, currentRaycastDistance);
        bool isLefttHit = Physics.Raycast(raycastOrigins.left, Vector2.left, out leftHitInfo, currentRaycastDistance);
        bool isUpHit = Physics.Raycast(raycastOrigins.up, Vector2.up, out upHitInfo, currentRaycastDistance);
        bool isDownHit = Physics.Raycast(raycastOrigins.down, Vector2.down, out downHitInfo, currentRaycastDistance);

        if (isUpHit)
        {
            Debug.DrawRay(raycastOrigins.up, Vector2.up * currentRaycastDistance, Color.red);
            boolCollision.isUpHit = true;
        }
        if (isDownHit)
        {
            Debug.DrawRay(raycastOrigins.down, Vector2.down * currentRaycastDistance, Color.red);
            boolCollision.isDownHit = true;

        }
        if (isLefttHit)
        {
            Debug.DrawRay(raycastOrigins.left, Vector2.left * currentRaycastDistance, Color.red);
            boolCollision.isLeftHit = true;

        }
        if (isRightHit)
        {
            Debug.DrawRay(raycastOrigins.right, Vector2.right * currentRaycastDistance, Color.red);
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
        //coordinates = transform.position;
        //node = gridManager.GetNode(coordinates);

        UpdateRaycastOrigins();
        TestFourSideCollision();

        //node.ClearConnection();

        //if (boolCollision.isUpHit && grid.ContainsKey(upHitInfo.transform.position))
        //{
        //    node.AddVerticalConnection(grid[upHitInfo.transform.position]);
        //}
        //if (boolCollision.isDownHit && grid.ContainsKey(downHitInfo.transform.position))
        //{
        //    node.AddVerticalConnection(grid[downHitInfo.transform.position]);
        //}
        //if (boolCollision.isLeftHit && grid.ContainsKey(leftHitInfo.transform.position))
        //{
        //    node.AddHorizontalConnection(grid[leftHitInfo.transform.position]);
        //}
        //if (boolCollision.isRightHit && grid.ContainsKey(rightHitInfo.transform.position))
        //{
        //    //Debug.Log("isRightHit is " + boolCollision.isRightHit +" from " + this.name);
        //    node.AddHorizontalConnection(grid[rightHitInfo.transform.position]);
        //}
    }
}
