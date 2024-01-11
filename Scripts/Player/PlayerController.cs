using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(PlayerMover))]
[RequireComponent(typeof(PlayerShooter))]
[RequireComponent(typeof(PlayerZoneManager))]
[RequireComponent(typeof(PlayerKeyManager))]
public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private KeyCode jumpButton = KeyCode.Space;
    //[SerializeField] private MouseInt gBubbleButton = MouseInt.Left; // Mouse buttons
    [SerializeField] private MouseInt axisshotButton = MouseInt.Right; // Mouse Buttons
    [SerializeField] private MouseInt gravityResetButton = MouseInt.Middle; // Mouse buttons
    [SerializeField] private KeyCode portalButton = KeyCode.W;

    [Header("Components")]
    [SerializeField] private Camera cam;
    // Components to Control
    private PlayerMover playerMover;
    private PlayerShooter playerShooter;
    private PlayerZoneManager playerZM;

    [SerializeField] private TextMeshProUGUI thankYou;

    // Portal usage
    private Portal currentPortal;

    // Start is called before the first frame update
    void Start()
    {
        if (axisshotButton == gravityResetButton)
            Debug.LogWarning(this + ": axisshot and Reset Axis are set to the same button!");

        playerMover = GetComponent<PlayerMover>();
        playerShooter = GetComponent<PlayerShooter>();
        playerZM = GetComponent<PlayerZoneManager>();

        if (cam == null)
            Debug.LogError("Camera is missing!");
    }

    // Update is called once per frame
    void Update()
    {
        OrbitalZone temp;
        if (currentPortal != null && Input.GetKeyDown(portalButton) && (temp = currentPortal.Activate(playerZM)) != null)
        {
            if (temp.gameObject.name == "Orbital Zone 3")
                thankYou.gameObject.SetActive(true);
            // Don't fall through on a successful activation
        }
        else
        {
            // Camera Zoom
            if (cam.GetComponent<CameraZoom>() != null)
            {
                cam.GetComponent<CameraZoom>().Zoom(Input.mouseScrollDelta.y);
            }

            // Move
            playerMover.Move(Input.GetAxis("Horizontal"));

            // Jump
            if (Input.GetKeyDown(jumpButton))
                playerMover.Jump();

            Vector3 aim = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.position.z));
            // Aim, Charge, Fire Zero Gun
            //if (Input.GetMouseButtonDown((int)gBubble))
            //{
            //    //playerShooter.Fire(0, mouseToWorld)
            //} else

            if (Input.GetMouseButton((int)axisshotButton))
            {
                Debug.DrawRay(transform.position, aim - transform.position, Color.green);
                playerShooter.ChargeAxisshot();
            }
            else if (Input.GetMouseButtonUp((int)axisshotButton))
            {
                playerShooter.Fire(1, new Vector3(aim.x, aim.y, transform.position.z));
                playerShooter.ResetASCharge();
            }

            if (Input.GetMouseButtonDown((int)gravityResetButton))
            {
                Physics.gravity = Vector3.down * Physics.gravity.magnitude;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal") && other.gameObject.GetComponent<Portal>() != null)
        {
            currentPortal = other.GetComponent<Portal>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal") && other.gameObject.GetComponent<Portal>() != null)
        {
            currentPortal = null;
        }
    }

    public enum MouseInt
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }
}
