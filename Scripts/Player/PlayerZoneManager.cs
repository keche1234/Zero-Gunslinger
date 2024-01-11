using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMover))]
public class PlayerZoneManager : MonoBehaviour
{
    [SerializeField] private OrbitalZone currentZone;
    private PlayerMover playerMover;

    // Start is called before the first frame update
    void Start()
    {
        if (currentZone == null)
            Debug.LogWarning(this + ": It is recommended that you start the player in an orbital zone");
        playerMover = GetComponent<PlayerMover>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == currentZone.gameObject)
            playerMover.Respawn();
    }

    /*
     * Attempts to the set the zone
     * Returns true if the zone is
     */
    public bool SetZone(OrbitalZone zone)
    {
        if (zone == null || zone.CompareTag("Orbital Zone"))
        {
            currentZone = zone;
            return true;
        }
        else
            return false;
    }

    public OrbitalZone GetZone()
    {
        return currentZone;
    }
}
