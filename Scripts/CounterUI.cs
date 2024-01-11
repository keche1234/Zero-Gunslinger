using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CounterUI : MonoBehaviour
{
    private int collected;
    private int total;

    [SerializeField] private string objectToCount;
    [SerializeField] private TextMeshProUGUI textObject;

    // Start is called before the first frame update
    void Start()
    {
        if (textObject == null)
        {
            textObject = GetComponent<TextMeshProUGUI>();
            if (textObject == null)
                Debug.LogError(this + ": Missing TextMeshProUGUI component!");
            else
                Debug.Log(this + ": Set TextMeshProUGUI component. It's recommended you do this yourself");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCollected(int c)
    {
        collected = c;
        textObject.text = objectToCount + ": " + collected + "/" + total;
    }

    public void SetTotal(int t)
    {
        total = t;
        textObject.text = objectToCount + ": " + collected + "/" + total;
    }
}
