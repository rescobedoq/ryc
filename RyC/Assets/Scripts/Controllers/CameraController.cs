using UnityEngine;

public class CameraController : MonoBehaviour
{
  [Header("Camera Settings")]
  [SerializeField] private Transform target;
  [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -5f);
  [SerializeField] private float smoothSpeed = 0.125f;
  [SerializeField] private float rotationSpeed = 2f;

  private void LateUpdate()
  {
    if (target == null) return;

    Vector3 desiredPosition = target.position + target.TransformDirection(offset);
    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

    transform.position = smoothedPosition;

    Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
  }

  public void SetTarget(Transform newTarget)
  {
    target = newTarget;
  }
}