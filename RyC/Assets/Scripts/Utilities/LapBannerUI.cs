using System.Collections;
using UnityEngine;
using TMPro;

public class LapBannerUI : MonoBehaviour
{
    public static LapBannerUI Instance { get; private set; }

    [Header("Referencia al texto")]
    [SerializeField] private TextMeshProUGUI lapText;

    [Header("Duración en pantalla (segundos)")]
    [SerializeField] private float showTime = 2.0f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (lapText != null)
            lapText.gameObject.SetActive(false);
    }

    public void ShowLapMessage(string message)
    {
        if (lapText == null) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(message));
    }

    private IEnumerator ShowRoutine(string msg)
    {
        lapText.text = msg;
        lapText.gameObject.SetActive(true);

        // Si quieres, aquí podrías hacer animaciones de escala/alpha, etc.
        yield return new WaitForSeconds(showTime);

        lapText.gameObject.SetActive(false);
    }
}