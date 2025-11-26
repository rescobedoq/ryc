using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class PortalAnswerDisplay : MonoBehaviour
{
    [Tooltip("1 para Portal1, 2 para Portal2")]
    public int portalId = 1;

    [Tooltip("True = LEFT, False = RIGHT")]
    public bool isLeft = true;

    private TextMeshPro tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        if (QuizManager.Instance != null)
        {
            QuizManager.Instance.RegisterAnswerDisplay(portalId, isLeft, this);
        }
    }

    public void SetAnswer(string text)
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
