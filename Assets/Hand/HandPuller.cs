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
    private void Update()
    {
        //Debug.Log(Input.GetAxisRaw("Vertical"));
    }
    public void PullDownItem() //Pull object touched by raycast and stocked them in a List<GameObject> and disable them
                               //TODO check if block are same color before authorizing pulling
    {
        if (!isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();
            if (UpHit.collider == null) { return; } //Si rien n'est Hit return

            GameObject gameObjectHitted = UpHit.transform.gameObject; //Recup gameObject hitted
            Block blockHitted = gameObjectHitted.GetComponent<Block>();
            Vector2Int blockPosition = Vector2Int.RoundToInt(gameObjectHitted.transform.position);


            Vector3Int startPosition = new Vector3Int(blockPosition.x, blockPosition.y, 0);
            Vector3Int endPosition = Vector3Int.RoundToInt(this.transform.position);

            blockHitted.InHand(true); //Si inHand == true, on clear connections
            blockHitted.ClearConnectionUpdateGridPosition(); //Clear connection, isMatched==false, update node name
            StartCoroutine(Move(gameObjectHitted, startPosition, endPosition, false));
            gameObjectHitted.transform.parent = this.transform;
            blockStoredInHand.Enqueue(gameObjectHitted.gameObject);

        }
    }

    public void PushItem() //Send every stocked object at directed position
    {
        int offset = 1;
        if (!isMoving)
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
                GameObject currentGameObjectToPush = blockStoredInHand.Dequeue();
                Block currentBlockToPush = currentGameObjectToPush.GetComponent<Block>();

                StartCoroutine(Move(currentGameObjectToPush, startPosition, endPosition, true));

                currentGameObjectToPush.transform.parent = gridManager.transform;
                currentGameObjectToPush.SetActive(true);
                currentBlockToPush.Init(new Vector2Int(endPosition.x, endPosition.y)); //Si inHand == true, on clear connections

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

        objectToMove.SetActive(enableAtEndPosition);
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3Int(1, 1, 1);

        if(enableAtEndPosition)
        {
            objectToMove.GetComponent<Block>().ClearConnectionUpdateGridPosition();
            gridManager.TestFor3Match();
        }
    }


}
