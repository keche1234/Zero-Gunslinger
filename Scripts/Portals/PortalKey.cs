using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalKey : MonoBehaviour
{
    [SerializeField] private Portal portal;

    // Start is called before the first frame update
    void Start()
    {
        if (portal == null)
            Debug.LogWarning(this + ": Key without a portal!");
        else if (portal.GetPortalKeys().IndexOf(this) == -1)
            Debug.LogWarning(this + ": The provided portal isn't unlocked with this key!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Portal GetPortal()
    {
        return portal;
    }
}
