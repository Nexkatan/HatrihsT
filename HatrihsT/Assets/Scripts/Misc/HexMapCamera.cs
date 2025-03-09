using Unity.VisualScripting;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{

    Transform swivel, stick;

    public float zoom = 0f;

    public float stickMinZoom, stickMaxZoom, swivelMinZoom, swivelMaxZoom, moveSpeedMinZoom, moveSpeedMaxZoom, minPitch, maxPitch, currentPitch, currentYaw, rotationSpeed, rotationAngle, swivelSpeed, swivelAngle, dragRotationSpeed;

    public HexGrid grid;

    static HexMapCamera instance;


    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
        instance = this;
    }

    void OnEnable()
    {
        instance = this;
    }

    private void Start()
    {
        Debug.Log(zoom);
    }


    void Update()
    {
        float zoomDelta = 0f; // Initialize zoom change

        // Read scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            zoomDelta = scrollInput; // Use scroll input if available
        }

        // Read Q & E input
        if (Input.GetKey(KeyCode.Q))
        {
            zoomDelta -= 0.001f; // Adjust for smooth zoom
        }
        if (Input.GetKey(KeyCode.E))
        {
            zoomDelta += 0.001f;
        }

        // Apply zoom only if there's a change
        if (zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        
        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0f || mouseY != 0f)
            {
                AdjustPositionDrag(mouseX, mouseY);
            }
        }

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0f || mouseY != 0f)
            {
                AdjustRotDrag(mouseX, mouseY);
            }
        }

    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        Debug.Log(zoom);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        float speed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom);
        CalculateMovement(xDelta, zDelta,speed);
    }


    void AdjustRotDrag(float mouseX, float mouseY)
    {

        // Adjust yaw (left-right rotation) and pitch (up-down rotation)
        currentYaw += mouseX * dragRotationSpeed;
        currentPitch -= mouseY * dragRotationSpeed;

        // Clamp the pitch to prevent the camera from going too far up or down
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

        // Apply the rotation to the camera
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    void AdjustPositionDrag(float mouseX, float mouseY)
    {
        float speed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * 15f;
        CalculateMovement(-mouseX,-mouseY,speed);
    }

    void CalculateMovement(float x, float y, float speed)
    {
        Vector3 camRight = Camera.main.transform.right;
        Vector3 camForwards = Camera.main.transform.up;

        Vector3 rightInput = x * camRight;
        Vector3 forwardInput = y * camForwards;


        Vector3 direction = (rightInput + forwardInput).normalized;
        direction.y = 0f;

        float damping = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
        float distance = speed * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax =
            (grid.cellCountX - 0.5f) *
            (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax =
            (grid.cellCountZ * HexMetrics.chunkSizeZ - 1) *
            (0.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);


        Debug.Log("x: " + xMax);
        Debug.Log("z: " + zMax);
        return position;
    }

    public static bool Locked
    {
        set
        {
            instance.enabled = !value;
        }
    }

    public static void ValidatePosition(HexMapCamera instance1)
    {
        instance1.AdjustPosition(0f, 0f);
    }
}