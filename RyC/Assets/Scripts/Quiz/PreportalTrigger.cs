using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PreportalTrigger : MonoBehaviour
{
    [Tooltip("1 para Portal1, 2 para Portal2")]
    public int portalId = 1;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // LOG de depuración
        Debug.Log($"[PreportalTrigger] portalId={portalId} Trigger con {other.name}", this);

        var car = other.GetComponentInParent<CarController>();
        if (car == null)
        {
            Debug.Log("[PreportalTrigger] No encontré CarController en el objeto que entró", this);
            return;
        }

        if (QuizManager.Instance != null)
        {
            Debug.Log("[PreportalTrigger] Llamando a QuizManager.OnPreportalEnter", this);
            QuizManager.Instance.OnPreportalEnter(portalId, car);
        }
        else
        {
            Debug.LogWarning("[PreportalTrigger] QuizManager.Instance es null", this);
        }
    }
}
