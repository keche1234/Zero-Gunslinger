using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OrbitalZone : MonoBehaviour
{
    [SerializeField] private List<PortalKey> keys;
    [SerializeField] private Portal portal;
    //[SerializeField] private CounterUI starKeyCounter;
    private int keyCount;
    // Start is called before the first frame update
    void Start()
    {
        keyCount = keys.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    public int GetCollectedKeyCount()
    {
        int collected = 0;
        foreach (PortalKey pk in keys)
            if (!pk.gameObject.activeSelf)
                collected++;

        return collected;
    }

    public Portal GetPortal()
    {
        return portal;
    }

    /*
     * Calculates the requested bound of the Orbital Zone,
     * and returns a float corresponding to either
     * an x position (direction is left or right) or
     * a y position (direction is up or down)
     */
    public float GetBound(BoundDirection direction)
    {
        if (GetComponent<BoxCollider>() == null)
            return 0;
        else
        {
            Vector3 worldBox = transform.TransformPoint(GetComponent<BoxCollider>().center);
            float length = GetComponent<BoxCollider>().size.x;
            float height = GetComponent<BoxCollider>().size.y;
            switch (direction)
            {
                case BoundDirection.Left:
                    return worldBox.x - (length / 2);
                case BoundDirection.Right:
                    return worldBox.x + (length / 2);
                case BoundDirection.Up:
                    return worldBox.y + (height / 2);
                case BoundDirection.Down:
                    return worldBox.x - (height / 2);
                default:
                    return 0;
            }
        }
    }

    public enum BoundDirection
    {
        Left = 1 << 0,
        Right = 1 << 1,
        Up = 1 << 2,
        Down = 1 << 3,
    }
}
