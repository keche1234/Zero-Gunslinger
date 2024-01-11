using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PlayerZoneManager playerZoneManager;
    private OrbitalZone orbitalZone;

    private float horizontalSpace = 2f / 3;
    private float verticalSpace = 1f / 3;
    private float minHSpeed = 12;
    private float minVSpeed = 12;
    private Camera cam;

    private Vector3 initOffset;
    void Start()
    {
        if (playerTransform == null)
            Debug.LogError(this + ": objTransform is null!");
        else
        {
            initOffset = transform.position - playerTransform.position;
            initOffset = new Vector3(initOffset.x, initOffset.y, 0);
        }
        cam = GetComponent<Camera>();
        if (playerZoneManager == null)
            Debug.LogError(this + ": Missing Player Zone Manager!");
        else
            orbitalZone = playerZoneManager.GetZone();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SetPosition();

        transform.rotation = Quaternion.LookRotation(Vector3.forward, playerTransform.up);
        transform.Rotate(new Vector3(5, 0, 0));
    }

    private void SetPosition()
    {

        // Calculate new camera position
        Vector3 horizontalOffset = playerTransform.forward * (horizontalSpace * -transform.position.z);
        Vector3 verticalOffset = playerTransform.up * (verticalSpace * -transform.position.z);
        //Vector3 newPosition = playerTransform.position + offset + (Vector3.forward * transform.position.z);

        if (orbitalZone.gameObject == playerZoneManager.GetZone().gameObject)
        {
            // Slerp camera towards it. Travels speed * Time.deltaTime units, how much of the total distance is that.

            /* Horizontal */
            Vector3 tempPosition = transform.position;
            Vector3 newPosition = playerTransform.position + horizontalOffset + (Vector3.forward * transform.position.z);
            float thisSpeed = Mathf.Max(minHSpeed, (newPosition - transform.position).magnitude * 2);
            float t = thisSpeed * Time.deltaTime / (newPosition - transform.position).magnitude;
            transform.position = Vector3.Lerp(transform.position, newPosition, t);

            //Check Bounds on Camera
            if (!CheckHorizontalBounds())
            {
                // Undo Lerp
                transform.position = Vector3.Lerp(transform.position, tempPosition, t);
            }

            /* Vertical */
            tempPosition = transform.position;
            newPosition = playerTransform.position + verticalOffset + (Vector3.forward * transform.position.z);
            thisSpeed = Mathf.Max(minVSpeed, (newPosition - transform.position).magnitude * 2);
            t = thisSpeed * Time.deltaTime / (newPosition - transform.position).magnitude;
            transform.position = Vector3.Lerp(transform.position, newPosition, t);

            if (!CheckVerticalBounds())
            {
                // Undo Lerp
                transform.position = Vector3.Lerp(transform.position, tempPosition, t);
            }
        }
        else // A transition happened!
        {
            Vector3 offset = (Vector3.right * 5f) + (Vector3.up * 2f);
            transform.position = playerTransform.position + offset + (Vector3.forward * transform.position.z);
            orbitalZone = playerZoneManager.GetZone();
        }
    }

    private bool CheckHorizontalBounds()
    {
        if (orbitalZone == null)
            return true;
        else
        {
            Vector3 lowLeftCorner = cam.ScreenToWorldPoint(new Vector3(0, 0, playerTransform.position.z - transform.position.z));
            Vector3 upperRightCorner = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, playerTransform.position.z - transform.position.z));

            if (lowLeftCorner.x < orbitalZone.GetBound(OrbitalZone.BoundDirection.Left))
            {
                Debug.Log("Too far left (" + lowLeftCorner.x + " vs " + orbitalZone.GetBound(OrbitalZone.BoundDirection.Left) + ")");
                return false;
            }

            if (upperRightCorner.x > orbitalZone.GetBound(OrbitalZone.BoundDirection.Right))
            {
                Debug.Log("Too far right (" + upperRightCorner.x + " vs " + orbitalZone.GetBound(OrbitalZone.BoundDirection.Right) + ")");
                return false;
            }

            return true;
        }
    }

    private bool CheckVerticalBounds()
    {
        if (orbitalZone == null)
            return true;
        else
        {
            Vector3 lowLeftCorner = cam.ScreenToWorldPoint(new Vector3(0, 0, playerTransform.position.z - transform.position.z));
            Vector3 upperRightCorner = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, playerTransform.position.z - transform.position.z));

            if (upperRightCorner.y > orbitalZone.GetBound(OrbitalZone.BoundDirection.Up))
            {
                Debug.Log("Too up high (" + upperRightCorner.y + " vs " + orbitalZone.GetBound(OrbitalZone.BoundDirection.Up) + ")");
                return false;
            }

            if (lowLeftCorner.y < orbitalZone.GetBound(OrbitalZone.BoundDirection.Down))
            {
                Debug.Log("Too down low (" + lowLeftCorner.y + " vs " + orbitalZone.GetBound(OrbitalZone.BoundDirection.Down) + ")");
                return false;
            }

            return true;
        }
    }
}
