using UnityEngine;

public class DoomBillboardEnemy : MonoBehaviour
{
    private Transform _mainCameraTransform;

    void Start()
    {
        if (Camera.main != null)
            _mainCameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (_mainCameraTransform == null)
        {
            if (Camera.main != null) _mainCameraTransform = Camera.main.transform;
            return;
        }

        // 1. Get direction to camera (ignoring Y for Y-axis only rotation)
        Vector3 dir = _mainCameraTransform.position - transform.position;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            // 2. Calculate Look Rotation
            Quaternion lookRot = Quaternion.LookRotation(-dir); // Negative because we want the front to face camera
            float angle = lookRot.eulerAngles.y;

            // 3. Snap to 8 increments (45 degrees)
            angle = Mathf.Round(angle / 45f) * 45f;

            // 4. Apply rotation
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
    }
}
