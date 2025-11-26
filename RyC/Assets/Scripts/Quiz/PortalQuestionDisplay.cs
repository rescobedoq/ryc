using UnityEngine;
using TMPro;   // <-- IMPORTANTE

// Este script ahora trabaja con TextMeshPro (3D), no con TextMesh legacy
[RequireComponent(typeof(TextMeshPro))]
public class PortalQuestionDisplay : MonoBehaviour
{
    [Tooltip("1 para Portal1, 2 para Portal2")]
    public int portalId = 1;

    private TextMeshPro tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        if (QuizManager.Instance != null)
        {
            QuizManager.Instance.RegisterDisplay(portalId, this);
        }
    }

    public void SetQuestion(string text)
    {
        if (tmp != null)
            tmp.text = text;
    }

    public void Clear()
    {
        if (tmp != null)
            tmp.text = string.Empty;
    }
}
