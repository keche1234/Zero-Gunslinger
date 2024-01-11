using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKeyManager : MonoBehaviour
{
    private List<PortalKey> tempCollect;
    [SerializeField] private CounterUI starKeyCounter;

    // Start is called before the first frame update
    void Start()
    {
        tempCollect = new List<PortalKey>();

        if (starKeyCounter == null)
            Debug.LogWarning(this + ": UI is missing for Star Key Counter!");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal Key") && other.GetComponent<PortalKey>() != null)
        {
            tempCollect.Add(other.GetComponent<PortalKey>());
            other.gameObject.SetActive(false);
        }    
    }

    public void TransferTemp()
    {
        for (int i = 0; i < tempCollect.Count; i++)
        {
            PortalKey k = tempCollect[i];
            tempCollect.RemoveAt(i--);
            k.GetPortal().RemoveLock(k);
            if (starKeyCounter != null)
            {
                starKeyCounter.SetCollected(k.GetPortal().TotalLocks() - k.GetPortal().LocksRemaining());
                starKeyCounter.SetTotal(k.GetPortal().TotalLocks());
            }
        }
    }

    public void DiscardTemp()
    {
        for (int i = 0; i < tempCollect.Count; i++)
        {
            PortalKey k = tempCollect[i];
            tempCollect.RemoveAt(i--);
            k.gameObject.SetActive(true);
            if (starKeyCounter != null)
            {
                starKeyCounter.SetCollected(k.GetPortal().TotalLocks() - k.GetPortal().LocksRemaining());
                starKeyCounter.SetTotal(k.GetPortal().TotalLocks());
            }
        }
    }
}
