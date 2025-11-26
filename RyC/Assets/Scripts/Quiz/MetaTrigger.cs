using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MetaTrigger : MonoBehaviour
{
    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var car = other.GetComponentInParent<CarController>();
        if (car == null) return;

        if (QuizManager.Instance != null)
            QuizManager.Instance.OnMeta(car);
    }
}
