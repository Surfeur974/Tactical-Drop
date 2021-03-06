using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


[ExecuteAlways]
public class ItemCoordinatesLabeler : MonoBehaviour
{
    [SerializeField] Color labelColor;
    Vector2Int itemCoordinates;
    TextMeshPro label;
    public Vector2Int ItemCoordinates { get { return itemCoordinates; } }

    // Start is called before the first frame update
    void Awake()
    {
        label = GetComponentInChildren<TextMeshPro>();
        DisplayCoordinates();
        UpdateNameObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(!Application.isPlaying)
        {
            DisplayCoordinates();
            UpdateNameObject();
        }
        DisplayCoordinates();
        UpdateNameObject();

    }
    private void DisplayCoordinates()
    {
        itemCoordinates.x = Mathf.RoundToInt(transform.position.x);
        itemCoordinates.y = Mathf.RoundToInt(transform.position.y);
        label.text = "(" + itemCoordinates.x + "," + itemCoordinates.y + ")";
        //label.color = labelColor;
    }
    private void UpdateNameObject()
    {
        this.transform.name = label.text;
    }

    public Vector2Int GetCoordinates()
    {
        return itemCoordinates;

    }


}
