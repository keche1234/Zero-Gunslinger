using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Properties")]
    [Range(0.01f, 255)]
    [SerializeField] protected float speed;
    [Tooltip("Set to 0 for infinite range")]
    [Range(0, 255)]
    [SerializeField] protected float range;
    [SerializeField] protected WallBehavior wallBehavior;

    // Internal properties
    protected float lifeTime;
    protected float currentTime;
    protected Rigidbody projectileRb;
    private TrailRenderer trailRenderer;

    void Start()
    {
        currentTime = 0;
        lifeTime = range / speed;

        projectileRb = GetComponent<Rigidbody>();
        if (projectileRb == null)
            Debug.LogError("Rigidbody is missing!");

        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= lifeTime)
                this.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        projectileRb.velocity = transform.forward * speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            if (wallBehavior == WallBehavior.Destroy)
                gameObject.SetActive(false);
            else if (wallBehavior == WallBehavior.Bounce)
                transform.Rotate(0, 180, 0);
        }
    }

    /*
     * Sets the speed of the projectile, if `s` is positive
     * Returns true on success, false on failure
     */
    public bool SetSpeed(float s)
    {
        if (s <= 0)
            return false;
        else
        {
            speed = s;
            lifeTime = range / speed;
            return true;
        }
    }

    /*
     * Sets the range of the projectile
     */
    public bool SetRange(float r)
    {
        range = r;
        lifeTime = range / speed;
        return true;
    }

    public void OnEnable()
    {
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
            if (lifeTime > 0)
                trailRenderer.time = lifeTime;
            else
                trailRenderer.time = 3;
        }
    }

    public void OnDisable()
    {
        currentTime = 0;
        if (trailRenderer != null)
            trailRenderer.emitting = false;
    }

    public enum WallBehavior
    {
        Destroy,
        Bounce,
        Custom // Use this with other scripts
    }
}
