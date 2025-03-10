using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    Transform swivel, stick;
    Camera mainCamera;  // Cache Camera.main
    public float zoom = 0f;
    public float stickMinZoom, stickMaxZoom, swivelMinZoom, swivelMaxZoom, moveSpeedMinZoom, moveSpeedMaxZoom, minPitch, maxPitch, currentPitch, currentYaw, rotationSpeed, rotationAngle, swivelSpeed, swivelAngle, dragRotationSpeed;
    public HexGrid grid;
    static HexMapCamera instance;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
        mainCamera = Camera.main;  // Cache the main camera
        instance = this;
    }

    void Update()
    {
        float zoomDelta = 0f;


        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f && IsMouseOnScreen())
        {
            zoomDelta = scrollInput;
        }
        else if (Input.GetKey(KeyCode.Q))
            zoomDelta -= 0.001f;
        else if (Input.GetKey(KeyCode.E))
            zoomDelta += 0.001f;

        if (zoomDelta != 0f)
            AdjustZoom(zoomDelta);

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");

        if (xDelta != 0f || zDelta != 0f)
            AdjustPosition(xDelta, zDelta);

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0f || mouseY != 0f)
                AdjustPositionDrag(mouseX, mouseY);
        }

        if (Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX != 0f || mouseY != 0f)
                AdjustRotDrag(mouseX, mouseY);
        }
    }
    bool IsMouseOnScreen()
    {
        Vector3 mousePosition = Input.mousePosition;

        return mousePosition.x >= 0 && mousePosition.x <= Screen.width &&
               mousePosition.y >= 0 && mousePosition.y <= Screen.height;
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);
        stick.localPosition = new Vector3(0f, 0f, Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom));
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        float speed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom);
        CalculateMovement(xDelta, zDelta, speed);
    }

    void AdjustRotDrag(float mouseX, float mouseY)
    {
        currentYaw += mouseX * dragRotationSpeed;
        currentPitch -= mouseY * dragRotationSpeed;
        currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    void AdjustPositionDrag(float mouseX, float mouseY)
    {
        float speed = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * 15f;
        CalculateMovement(-mouseX, -mouseY, speed);
    }

    void CalculateMovement(float x, float y, float speed)
    {
        Vector3 rightInput = x * mainCamera.transform.right;
        Vector3 forwardInput = y * mainCamera.transform.up;

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
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountZ * HexMetrics.chunkSizeZ - 1) * (0.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);
        return position;
    }

    public static bool Locked
    {
        set { instance.enabled = !value; }
    }

    public static void ValidatePosition(HexMapCamera instance1)
    {
        instance1.AdjustPosition(0f, 0f);
    }
}