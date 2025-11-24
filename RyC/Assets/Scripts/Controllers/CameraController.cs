using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("Camera Settings")]
  [SerializeField] private Transform target;
  [SerializeField] private Vector3 offset = new Vector3(0f, 1.5f, -3.5f);
  [SerializeField] private float smoothSpeed = 1.5f;
  [SerializeField] private float rotationSpeed = 2f;

  private PlayerIndex playerIndex;
  private Camera cam;
  
  private void LateUpdate()
  {
    if (target == null) return;

    Vector3 desiredPosition = target.position + target.TransformDirection(offset);
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

    transform.position = smoothedPosition;

    Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
  }

  public void SetTarget(Transform newTarget)
  {
    target = newTarget;
  }
}