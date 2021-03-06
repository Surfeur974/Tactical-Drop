using System.Collections;
using UnityEngine;

public class HandMover : MonoBehaviour
{
    [SerializeField] bool enableLoopMovement = false;
    [SerializeField] GridManager gridManager;

    bool isMoving;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] float HandSpeed = 200f;
    Transform spriteScale;
    bool allowMove;

    void Start()
    {
        PlaceHandInMiddle();
        isMoving = false;
        gridSize = gridManager.GridSize;
        spriteScale = transform.GetComponentInChildren<HandSprite>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        gridSize = gridManager.GridSize;

        MoveHandAxis();
    }
    private void MoveHand() //Move hand by block size steps
    {
        if (gridSize == Vector2Int.zero)
        {
            gridSize = gridManager.GridSize;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isMoving)
        {

            StartCoroutine(Move(Vector3Int.right));

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !isMoving)
        {
            StartCoroutine(Move(Vector3Int.left));
        }
    }
    private void MoveHandAxis() //Move hand by block size steps
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        if (gridSize == Vector2Int.zero)
        {
            gridSize = gridManager.GridSize;
        }
        if (Input.GetAxisRaw("Horizontal") >= .8f && !isMoving && allowMove)
        {
            StartCoroutine(Move(Vector3Int.right));
            allowMove = false;
        }
        if (Input.GetAxisRaw("Horizontal") <= -.8f && !isMoving && allowMove)
        {
            StartCoroutine(Move(Vector3Int.left));
            allowMove = false;
        }
        if (Mathf.Abs(horizontalInput) <= .7f)
        {
            allowMove = true;
        }
    }


    private void PlaceHandInMiddle()
    {

    }

    IEnumerator Move(Vector3Int direction)
    {
        isMoving = true;
        float t = 0;
        float timeToTravel = .1f/ HandSpeed;
        Vector3 startposition = transform.position;
        Vector3 endposition = transform.position + direction;
        int maxHandPosition = gridSize.x - 1;
        int minHandPosition = 0;
        endposition = ConstraintMove(endposition, maxHandPosition, minHandPosition);

        while (t <= timeToTravel)
        {
            spriteScale.localScale = new Vector3(1.2f, .8f, 1);
            transform.position = Vector3.Lerp(startposition, endposition, t / timeToTravel);
            t += .01f * Time.deltaTime;
            yield return null;
        }
        spriteScale.localScale = new Vector3Int(1, 1, 1);
        isMoving = false;
        yield return new WaitForEndOfFrame();
    }

    private Vector3 ConstraintMove(Vector3 endposition, int maxHandPosition, int minHandPosition) //Limit end position by min, max, enable looping
    {
        if (enableLoopMovement)
        {
            if (endposition.x < minHandPosition)
            {
                endposition.x = maxHandPosition;
            }
            if (endposition.x > maxHandPosition)
            {
                endposition.x = minHandPosition;
            }
        }
        else
        {
            endposition.x = Mathf.Clamp(endposition.x, minHandPosition, maxHandPosition);
        }
        endposition.x = Mathf.RoundToInt(endposition.x);
        return endposition;
    }
}
