using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HandPuller : MonoBehaviour
{
    ItemCollider itemCollider;
    RaycastHit UpHit;
    Queue<GameObject> blockStoredInHand = new Queue<GameObject>();
   [SerializeField ]GridManager gridManager;
    bool isMoving = true;

    void Start()
    {
        itemCollider = GetComponent<ItemCollider>();
        isMoving = false;
    }

    public void PullDownItem() //Pull object touched by raycast and stocked them in a List<GameObject> and disable them
                               //TODO check if block are same color before authorizing pulling
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();
            if (UpHit.collider == null) { return; } //Si rien n'est Hit return

            GameObject gameObjectHitted = UpHit.transform.gameObject; //Recup gameObject hitted
            Vector2Int nodePosition = Vector2Int.RoundToInt(gameObjectHitted.transform.position);
            Node ObjectHittedNode = gridManager.GetNode(nodePosition);


            Vector3Int startPosition = new Vector3Int(nodePosition.x, nodePosition.y, 0);
            Vector3Int endPosition = Vector3Int.RoundToInt(this.transform.position);

            gameObjectHitted.GetComponent<Block>().isInHand = true; //Si inHand == true, on clear connections
            ObjectHittedNode.Init(); //Clear connection, isMatched==false, update node name

            StartCoroutine(Move(gameObjectHitted, startPosition, endPosition, false));
            gameObjectHitted.transform.parent = this.transform;
            blockStoredInHand.Enqueue(gameObjectHitted.gameObject);

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

            if (UpHit.transform != null)
            {
                endPosition = Vector3Int.RoundToInt(UpHit.transform.position);
            }

            endPosition += Vector3Int.down * offset;
            while (blockStoredInHand.Count > 0)
            {
                GameObject currentBlockToPush = blockStoredInHand.Dequeue();
                Node ObjectToPushdNode = gridManager.GetNode(currentBlockToPush.GetComponent<Block>().Coordinates);


                currentBlockToPush.GetComponent<Block>().isInHand = false; //Si inHand == true, on clear connections
                StartCoroutine(Move(currentBlockToPush, startPosition, endPosition, true));

                currentBlockToPush.transform.parent = gridManager.transform;
                currentBlockToPush.SetActive(true);
                endPosition += Vector3Int.down;
            }
        }

    }
    IEnumerator Move(GameObject objectToMove, Vector3Int startposition, Vector3Int endposition, bool enableAtEndPosition)
    {//Disabling the BoxCOllider when in movement, solve an issue where the collision were updated bizzarement
        //Debug.Log(endposition);
        float t = 0;
        float timeToTravel = 0.1f;

        isMoving = true;
        objectToMove.GetComponent<BoxCollider>().enabled = false;
        while (t < 1)
        {
            t += timeToTravel;
            objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(.8f, 1.2f, 1);
            objectToMove.transform.position = Vector3.Lerp(startposition, endposition, t);

            //Debug.Log(t);
            yield return null;
        }
        itemCollider.UpdateBoolCollisionState();

        yield return null;
        objectToMove.GetComponent<BoxCollider>().enabled = true;

        isMoving = false;

        objectToMove.gameObject.SetActive(enableAtEndPosition);
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);

        if(enableAtEndPosition)
        {
            gridManager.TestFor3Match();
        }
    }


}
