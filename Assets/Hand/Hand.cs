using System.Collections;
using UnityEngine;


public class Hand : MonoBehaviour
{
    ItemCollider itemCollider;
    RaycastHit UpHit;
    HandPuller handPuller;
    
    void Start()
    {
        handPuller = GetComponent<HandPuller>();
        itemCollider = GetComponent<ItemCollider>();
    }
    // Update is called once per frame
    void Update()
    {
        itemCollider.DrawRaycast();
        handPuller.PullDownItem();
        handPuller.PushItem();

    }

}
