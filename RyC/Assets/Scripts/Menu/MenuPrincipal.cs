using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void IniciarJuego()
    {
        SceneManager.LoadScene("F1TrackDisplayScene");
    }

    public void SalirJuego()
    {
        Debug.Log("SALIENDO!");
        Application.Quit();
    }
}
