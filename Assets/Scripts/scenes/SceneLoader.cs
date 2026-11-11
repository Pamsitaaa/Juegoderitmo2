using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Configuración de Fade")]
    [Tooltip("Arrastra aquí el objeto que tiene el CanvasGroup")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    private void Start()
    {
        // Al iniciar la escena, la pantalla se aclara (Fade In)
        if (fadeCanvasGroup != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    public void QuitGame()
    {
        StartCoroutine(FadeOutAndQuit());
    }

    private IEnumerator FadeIn()
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true; // Corregido: blocksRaycasts en plural

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false; // Permite hacer clic de nuevo
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true; // Bloquea clics durante la transición
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOutAndQuit()
    {
        fadeCanvasGroup.blocksRaycasts = true;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}