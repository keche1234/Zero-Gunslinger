using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    [Header("Travel Info")]
    [SerializeField] GameObject destination; // Can be another portal
    [SerializeField] OrbitalZone targetZone;

    [Header("Lock Info")]
    [Tooltip("Be sure there are no duplicates in this list, or your portal will never unlock.")]
    [SerializeField] private List<PortalKey> keys;
    private int locksRemaining;
    private int totalLocks;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private Material lockedMaterial;
    [SerializeField] private Material unlockedMaterial;
    [SerializeField] private CounterUI starKeyCounter;

    // Start is called before the first frame update
    void Start()
    {
        if (!GetComponent<Collider>().isTrigger)
        {
            GetComponent<Collider>().isTrigger = true;
            Debug.Log(this + ": Collider set to trigger!");
        }

        if (destination == null)
            Debug.LogError(this + ": Your portal must have a destination!");

        if (mesh == null)
            Debug.LogError(this + ": Missing Mesh Render for lock indication!");
        else
        {
            if (keys.Count == 0)
                mesh.material = unlockedMaterial;
            else
                mesh.material = lockedMaterial;
        }

        totalLocks = keys.Count;
        locksRemaining = totalLocks;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int LocksRemaining()
    {
        return locksRemaining;
    }

    public int TotalLocks()
    {
        return totalLocks;
    }

    public List<PortalKey> GetPortalKeys()
    {
        List<PortalKey> theKeys = new List<PortalKey>();
        foreach (PortalKey k in keys)
            theKeys.Add(k);
        return theKeys;
    }

    /*
     * @pre Assumes that pk only appears in the list once.
     */
    public void RemoveLock(PortalKey pk)
    {
        if (keys.IndexOf(pk) >= 0)
        {
            locksRemaining--;
        }

        if (locksRemaining <= 0)
            FullUnlock();
    }

    public void FullUnlock()
    {
        for (int i = 0; i < keys.Count; i++)
        {
            PortalKey k = keys[i];
            keys.RemoveAt(i--);
            Destroy(k.gameObject);
        }
        mesh.material = unlockedMaterial;
    }

    /*
     * Returns the Orbital Zone the player was sent to, or null
     */
    public OrbitalZone Activate(PlayerZoneManager user)
    {
        if (locksRemaining > 0)
            return null;
        else
        {
            OrbitalZone tempZone = user.GetZone();
            Vector3 tempPosition = user.transform.position;

            Vector3 finalDestination = new Vector3(destination.transform.position.x, destination.transform.position.y, user.gameObject.transform.position.z);
            user.SetZone(null);
            user.gameObject.transform.position = finalDestination;

            if (!user.SetZone(targetZone))
            {
                user.SetZone(tempZone);
                user.transform.position = tempPosition;
                return null;
            }
            Physics.gravity = Vector3.down * Physics.gravity.magnitude;
            user.transform.Rotate(new Vector3(-user.transform.eulerAngles.x, 0, 0));

            if (starKeyCounter != null)
            {
                if (targetZone == null || targetZone.GetPortal() == null || targetZone.GetPortal().TotalLocks() < 1)
                    starKeyCounter.gameObject.SetActive(false);
                else
                {
                    starKeyCounter.SetCollected(targetZone.GetPortal().TotalLocks() - targetZone.GetPortal().LocksRemaining());
                    starKeyCounter.SetTotal(targetZone.GetPortal().TotalLocks());
                }
            }
            return targetZone;
        }
    }
}
