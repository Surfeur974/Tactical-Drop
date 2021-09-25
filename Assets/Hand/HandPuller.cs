using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandPuller : MonoBehaviour
{
    int handCollisionDistance = 20;
    ItemCollider itemCollider;
    RaycastHit UpHit;
    List<GameObject> blockstored = new List<GameObject>();
    GridManager gridManager;
    bool isMoving = true;

    void Start()
    {
        itemCollider = GetComponent<ItemCollider>();
        itemCollider.DefaultraycastDistance = handCollisionDistance;
        gridManager = FindObjectOfType<GridManager>();
        isMoving = false;
    }

    public void PullDownItem() //Pull object touched by raycast and stocked them in a List<GameObject> and disable them
                               //TODO check if block are same color before authorizing pulling
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();
            if(UpHit.collider == null) { return; }
            GameObject gameObjectHitted = UpHit.transform.gameObject;

            Vector2Int nodePosition = Vector2Int.RoundToInt(gameObjectHitted.transform.position);
            Node ObjectHittedNode = gridManager.GetNode(nodePosition);


            Vector3Int startPosition = new Vector3Int(nodePosition.x, nodePosition.y, 0);
            Vector3Int endPosition = Vector3Int.RoundToInt(this.transform.position);
            //Debug.Log(startPosition);

            //bool testBool = Equals(startPosition, gridManager.GetNode(startPosition).coordinates);
            //Debug.Log(startPosition);


            ObjectHittedNode.Init(); //Reset State of node at pull position

            StartCoroutine(Move(gameObjectHitted, startPosition, endPosition, false));
            gameObjectHitted.transform.parent = this.transform;
            blockstored.Add(gameObjectHitted.gameObject);

            gameObjectHitted.GetComponent<Block>().HandleInHand(true);
            gridManager.UpdateAllNodeConnection();
        }
    }

    public void PushItem() //Send every stocked object at directed position
    {
        int offset = 1;
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();
            float maxYPosition = gridManager.GridSize.y;
            Vector3Int startPosition = Vector3Int.RoundToInt(transform.position);
            Vector3Int endPosition = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(maxYPosition), 0);
            //Debug.Log(endPosition);

            if (UpHit.transform != null)
            {
                endPosition = Vector3Int.RoundToInt(UpHit.transform.position);
            }

            endPosition += Vector3Int.down * offset; //TODO change this to reference real grid position and not generate one becaus epulling won't work after
            int numberOfBlockStocked = blockstored.Count;
            for (int i = 0; i < numberOfBlockStocked; i++)
            {
                StartCoroutine(Move(blockstored[i], startPosition, endPosition, true));
                blockstored[i].transform.parent = gridManager.transform;
                blockstored[i].SetActive(true);
                endPosition += Vector3Int.down;
                blockstored[i].GetComponent<Block>().HandleInHand(false);

            }
            blockstored.Clear();

            gridManager.UpdateAllNodeConnection();
        }
    }
    IEnumerator Move(GameObject objectToMove, Vector3Int startposition, Vector3Int endposition, bool enableAtEndPosition)
    {
        //Debug.Log(endposition);
        float t = 0;
        float timeToTravel = 0.1f;

        isMoving = true;
        while (t < 1)
        {
            t += timeToTravel;
            objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(.8f, 1.2f, 1);
            objectToMove.transform.position = Vector3.Lerp(startposition, endposition, t);

            //Debug.Log(t);
            yield return null;
        }
        yield return null;
        isMoving = false;

        objectToMove.gameObject.SetActive(enableAtEndPosition);
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);
    }






}
