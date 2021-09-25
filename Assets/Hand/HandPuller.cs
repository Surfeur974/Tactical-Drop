using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class HandPuller : MonoBehaviour
{
    float handCollisionDistance = 20f;
    ItemCollider itemCollider;
    RaycastHit UpHit;
    List<GameObject> blockstocked = new List<GameObject>();
    GridManager gridManager;
    bool isMoving = true;

    void Start()
    {
        itemCollider = GetComponent<ItemCollider>();
        itemCollider.DefaultraycastDistance = handCollisionDistance;
        gridManager = FindObjectOfType<GridManager>();
        isMoving = false;
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void PullDownItem() //Pull object touched by raycast and stocked them in a List<GameObject> and disable them
        //TODO check if block are same color before authorizing pulling
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) && !isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();

            GameObject gameObjectHitted = UpHit.transform.gameObject;
            Node ObjectHittedNode = gridManager.GetNode(gameObjectHitted.transform.position);
            ObjectHittedNode.Init(); //Reset State of node at pull position
            StartCoroutine(Move(gameObjectHitted, gameObjectHitted.transform.position, this.transform.position, false));
            gameObjectHitted.transform.parent = this.transform;
            blockstocked.Add(gameObjectHitted.gameObject);
            ObjectHittedNode.ClearConnection();

            gridManager.UpdateAllNodeConnection();
        }
    }

    public void PushItem() //Send every stocked object at directed position
    {
        float offset = 1f;
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isMoving)
        {
            UpHit = itemCollider.GetUpHitInfo();
            Vector3 PushPosition = UpHit.transform.position;
            PushPosition += Vector3.down * offset;
            int numberOfBlockStocked = blockstocked.Count;
            for (int i = 0; i < numberOfBlockStocked; i++)
            {
                StartCoroutine(Move(blockstocked[i], transform.position,PushPosition, true));
                blockstocked[i].transform.parent = gridManager.transform;
                blockstocked[i].SetActive(true);
                PushPosition += Vector3.down;
            }
            blockstocked.Clear();

            gridManager.UpdateAllNodeConnection();
        }
    }
    IEnumerator Move(GameObject objectToMove, Vector3 startposition, Vector3 endposition, bool enableAtEndPosition)
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
        objectToMove.GetComponentInChildren<BlockSprite>().transform.localScale = new Vector3(1, 1, 1);
    }






}
