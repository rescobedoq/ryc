using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalAnswerTrigger : MonoBehaviour
{
    [Tooltip("1 para Portal1, 2 para Portal2")]
    public int portalId = 1;

    [Tooltip("Marca true si este objeto es la opción LEFT, false si es RIGHT")]
    public bool isLeft = true;

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
            QuizManager.Instance.OnPortalAnswer(portalId, isLeft, car);
    }
}