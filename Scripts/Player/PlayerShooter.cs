using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("GBubble Parameters")]
    [Range(0, 255)]
    [SerializeField] private float gBRange;
    [Range(0.01f, 255)]
    [SerializeField] private float gBSpeed;
    [Tooltip("The max amount of GBubbles allowed on screen at once")]
    [Range(0, 10)]
    [SerializeField] private int gBMax = 2;
    //[SerializeField] private ObjectPool gBPool;
    //[SerializeField] private Projectile gBPrefab;

    [Header("Axisshot Parameters")]
    [Range(0.01f, 255)]
    [SerializeField] private float aSChargeMax;
    [Range(0, 255)]
    [SerializeField] private float aSRangeMin;
    [Range(0, 255)]
    [SerializeField] private float aSRangeMax;
    [Range(0, 255)]
    [SerializeField] private float aSBaseSpeed;
    private float chargeInternal = 0;
    [SerializeField] private ObjectPool aSPool;
    //[SerializeField] private Projectile aS;

    // Start is called before the first frame update
    void Start()
    {
        if (gBMax == 0)
            Debug.LogWarning("The player can have a max of 0 G-Bubbles!");
        //if (gBPool == null)
        //    Debug.LogError("The G-Bubble Object Pool is missing!");

        if (aSChargeMax <= 0)
            aSChargeMax = 1 / 60f;
        if (aSPool == null)
            Debug.LogError("The Axisshot Object Pool is missing!");
        if (aSRangeMin < 0)
            Debug.LogError("The Axisshot minimum range must be non-negative!");
        if (aSRangeMax < 0)
            Debug.LogError("The Axisshot maximum range must be positive!");
        if (aSRangeMax < aSRangeMin)
            Debug.LogError("The Axisshot maximum range must be greater than or equal to the minimum range!");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(chargeInternal);
    }

    public void ChargeAxisshot()
    {
        if (chargeInternal < aSChargeMax)
            chargeInternal += Time.deltaTime;
    }

    /*
     * Resets the Axisshot Charge
     */
    public void ResetASCharge()
    {
        chargeInternal = 0;
    }

    /*
     * Attempts to fire either a Gravity Bubble (shotType == 0)
     * or Axisshot (shotType == 1) from the given pool.
     * Returns true if successful, or false if otherwise.
     */
    public bool Fire(int shotType, Vector3 aim)
    {
        // Calculate
        switch (shotType)
        {
            case 0:
                return true;
            case 1:
                GameObject shot = aSPool.GetPooledObject();
                if (shot == null)
                    return false;

                Vector3 newForward = aim - transform.position;
                shot.transform.rotation = Quaternion.LookRotation(newForward, Quaternion.Euler(90, 0, 0) * newForward);
                shot.transform.position = transform.position + shot.transform.forward;
                shot.GetComponent<Projectile>().SetRange(aSRangeMin + ((aSRangeMax - aSRangeMin) * (chargeInternal / aSChargeMax)));
                shot.GetComponent<Projectile>().SetSpeed(aSBaseSpeed * (1 + ((chargeInternal / aSChargeMax) * 1.5f)));

                shot.SetActive(true);
                chargeInternal = 0;
                return true;
            default:
                return false;
        }
    }
}
