using System.Collections;
using UnityEngine;


public class Hand : MonoBehaviour
{
    ItemCollider itemCollider;
    RaycastHit UpHit;
    HandPuller handPuller;
    bool allowMove;
    
    void Start()
    {
        handPuller = GetComponent<HandPuller>();
        itemCollider = GetComponent<ItemCollider>();
        allowMove = true;
    }
    // Update is called once per frame
    void Update()
    {
        HandPushPull();


    }
    private void HandPushPull()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput <= -.8f && allowMove)
        {
            handPuller.PullDownItem();
            allowMove = false;

        }
        if (verticalInput >= .8f && allowMove)
        {
            handPuller.PushItem();
            allowMove = false;
        }

        if (Mathf.Abs(verticalInput) <= .7f) 
        {
            allowMove = true;
        }
    }


}
