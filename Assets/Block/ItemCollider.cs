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
    List<Block> blocksToMove = new List<Block>();
    [SerializeField]  float blockFallSpeed = 0.1f;

    Block block;
    Hand hand;
    ConnectionHandler connectionHandler;
    BoolCollision boolCollision;
    Vector2Int coordinates;
    Dictionary<Vector2Int, Block> grid = new Dictionary<Vector2Int, Block>();
    List<Vector2Int> directions = new List<Vector2Int>() { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
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
            gridManager.moveDownCollumnEvent += DownCollumn;
        }
    }
    private void OnDisable()
    {
        if (!gridManager)
        {
            return;
        }
        gridManager.updateConnectionEvent -= UpdateBlock;
        gridManager.moveDownCollumnEvent -= DownCollumn;
    }
    public void UpdateBlock()
    {
        StartCoroutine(UpdateMoveBlock());
    }
    public void DownCollumn()
    {
        //Debug.Log("DownCollumn called");
        StartCoroutine(MoveDownCollumn());
    }
    IEnumerator UpdateMoveBlock() //Coroutine called by grid manager Event update
    {
        //Debug.Log("UpdateMoveBlock called");
        block = GetComponent<Block>();

        if (block == null) { yield break; }
        yield return StartCoroutine(UpdateConnectedTo()); //On update les connection et on attend la fin grace au yield return

        bool isTopHitRaycatHit = TopHitRaycast(10); //On test si le bloc a un espace libre au dessus

        while (isTopHitRaycatHit) //true if need to move
        {
            yield return StartCoroutine(MoveUpCollumnBlockIfVoid()); //On le move et on attend la fin

            isTopHitRaycatHit = TopHitRaycast(10);  //On test si le bloc a un espace libre au dessus
            StopAllCoroutines();    //On stop toutes les coroutines de tous le monde comme ça le dernier va call gridManager.TestFor3Match(); et permettre un update de la grid unique "Singlemerde"
            gridManager.TestFor3Match();
        }
    }
    public void UpdateBoolCollisionState()
    {
        //Debug.Log("UpdateBoolCollisionState is called");
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
        //Debug.Log("UpdateConnectedTo called");
        coordinates = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        grid = gridManager.Grid;
        block = GetComponent<Block>();

        if (block == null) {yield break;}
        //if (node.isInHand == true) { node.ClearConnection(); return; }

        //UpdateRaycastOrigins();
        //UpdateBoolCollisionState();
        block.ClearConnectionUpdateGridPosition();

        foreach(Vector2Int dir in directions)
        {
            AddConnectedBlock(block,dir);
        }
        yield return null;
    }
    private void AddConnectedBlock(Block block,Vector2Int dir)
    {
        if(grid.ContainsKey(coordinates + dir) && grid[coordinates + dir] != null)
        {
            if(dir == Vector2Int.up || dir == Vector2Int.down)
            {
            block.AddVerticalConnection(grid[coordinates + dir]);
            }
            else if (dir == Vector2Int.left || dir == Vector2Int.right)
            {
                block.AddHorizontalConnection(grid[coordinates + dir]);
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
    public bool TopHitRaycast(int raylenght) //Used for detecting if bloc has something up him
    {
        //Debug.Log("TopHitRaycast is called from " + this.name);
        UpdateRaycastOrigins();
        raycastOrigins.up.y -= .1f;

        Physics.Raycast(raycastOrigins.up, Vector2.up, out topHit, raylenght);
        //Debug.DrawRay(raycastOrigins.up, Vector2.up * topHit.distance, Color.red);

        if (block != null && (topHit.distance > 1f || topHit.transform == null) && hand == null && block.Coordinates.y < gridManager.GridSize.y-1 )
        {
            block.ClearConnectionUpdateGridPosition();
            //Debug.Log(block.name);
            return true;
        }
        return false;
    }
    public IEnumerator MoveUpCollumnBlockIfVoid() //Find node without up block group all collumn move it into game object move game object remove from game object detroy it 
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement
        GameObject objectToMove = this.gameObject;
        Transform oldParent = objectToMove.transform.parent;
        Block blockToMove = this.gameObject.GetComponent<Block>();
        blocksToMove.Clear();

        yield return StartCoroutine(UpdateConnectedTo());

        blocksToMove.AddRange(connectionHandler.GetAllVerticalConnection(blockToMove));   ///PROBLEME ici le node n'a plus de connection, car on les efface pour eviter bug

        GameObject CollumnToMove = new GameObject("CollumnToMove");
        CollumnToMove.transform.position = objectToMove.transform.position;
        for (int i = 0; i < blocksToMove.Count; i++)
        {
            blocksToMove[i].ClearConnectionUpdateGridPosition(); //Need to clear connection before moving
            blocksToMove[i].transform.parent = CollumnToMove.transform;
        }

        objectToMove = CollumnToMove;

        float maxYPosition = gridManager.GridSize.y;
        Vector3 startPosition = Vector3Int.RoundToInt(transform.position);
        Vector3 endPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(maxYPosition), 0);

        if (topHit.transform != null)
        {
            endPosition = Vector3Int.RoundToInt(topHit.transform.position);
        }
        endPosition += Vector3Int.down;

        float maxChildLocalPositionY = 0;
        foreach (Transform child in objectToMove.transform)//FIX for times xhen bock go up the limit because the objectToMove is sometimes set to a lower block in a collum du coup des blocks depassent
        {
            if (child.transform.localPosition.y > maxChildLocalPositionY)
            {
                maxChildLocalPositionY = child.transform.localPosition.y;
            }
        }
        endPosition.y = Mathf.Clamp(endPosition.y, 0, (gridManager.GridSize.y - 1) - Mathf.RoundToInt(maxChildLocalPositionY));
        float t = 0;
        while (t < 1)
        {
            t += blockFallSpeed;
            objectToMove.transform.position = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }
        for (int i = CollumnToMove.transform.childCount - 1; i > -1; i--)
        {
            CollumnToMove.transform.GetChild(i).transform.SetParent(oldParent);
        }
        Destroy(CollumnToMove); //TODO some are not destroyed
        yield return null;

    }
    public IEnumerator MoveDownCollumn() //Find node without up block group all collumn move it into game object move game object remove from game object detroy it 
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement

        GameObject objectToMove = this.gameObject;
        Transform oldParent = objectToMove.transform.parent;
        Block blockToMove = this.gameObject.GetComponent<Block>();
        List<Block> blocksToMove = new List<Block>();

        yield return StartCoroutine(UpdateConnectedTo());

        blocksToMove.AddRange(connectionHandler.GetAllVerticalConnection(blockToMove));   ///PROBLEME ici le node n'a plus de connection, car on les efface pour eviter bug

        GameObject groupToMoveDownCollumn = new GameObject("groupToMoveDownCollumn");
        groupToMoveDownCollumn.transform.position = objectToMove.transform.position;
        for (int i = 0; i < blocksToMove.Count; i++)
        {
            blocksToMove[i].ClearConnectionUpdateGridPosition(); //Need to clear connection before moving
            blocksToMove[i].transform.parent = groupToMoveDownCollumn.transform;
        }

        objectToMove = groupToMoveDownCollumn;
        

        int maxYPosition = gridManager.GridSize.y-1;
        Vector3Int startPosition = Vector3Int.RoundToInt(transform.position);
        Vector3Int endPosition = startPosition + Vector3Int.down;
        endPosition.y = Mathf.Clamp(endPosition.y, 0, maxYPosition);

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

        for (int i = groupToMoveDownCollumn.transform.childCount - 1; i > -1; i--)
        {
            groupToMoveDownCollumn.transform.GetChild(i).transform.SetParent(oldParent);
            //Debug.Log(i);
            //Debug.Log(groupToMove.transform.GetChild(i));
        }
        Destroy(groupToMoveDownCollumn);
    }

}
