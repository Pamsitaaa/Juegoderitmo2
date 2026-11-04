using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Estadísticas de Juego")]
    public int score = 0;
    public int combo = 0;
    public int multiplier = 1;
    public int missCount = 0;

    [Header("Referencia de Texto (UI Clásico)")]
    public Text ratingText;
    public Text scoreText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void EvaluateHit(float distanceToTarget)
    {
        if (distanceToTarget < 0.2f)
        {
            RegisterHit(1000, "¡Perfecto!", new Color(1f, 0.4f, 0.7f));
        }
        else if (distanceToTarget < 0.4f)
        {
            RegisterHit(700, "¡Bien!", new Color(0.3f, 0.6f, 1f));
        }
        else
        {
            RegisterHit(300, "¡Regular!", new Color(1f, 0.9f, 0.2f));
        }
    }

    void RegisterHit(int basePoints, string ratingMessage, Color ratingColor)
    {
        combo++;
        UpdateMultiplier();

        int earnedPoints = basePoints * multiplier;
        score += earnedPoints;

        ShowRating(ratingMessage, ratingColor);
    }

    // Cuando dejas pasar una nota (Miss) -> Rompe combo y BAJA PUNTUACIÓN
    public void RegisterMiss()
    {
        combo = 0;
        multiplier = 1;
        missCount++;

        score -= 500; // Penalización de puntos por dejar pasar la nota
        if (score < 0) score = 0; // Evita que baje de 0

        ShowRating("¡Mal!", new Color(1f, 0.2f, 0.2f));
        UpdateUIOnly();
    }

    // Cuando presionas la tecla por error sin ninguna nota en el carril (Ghost Tap)
    public void RegisterGhostMiss()
    {
        combo = 0;
        multiplier = 1;
        score -= 200; // Castigo leve por presionar a lo loco
        if (score < 0) score = 0;

        ShowRating("¡Fallo!", new Color(1f, 0.5f, 0.2f));
        UpdateUIOnly();
    }

    void UpdateMultiplier()
    {
        if (combo >= 50) multiplier = 8;
        else if (combo >= 25) multiplier = 4;
        else if (combo >= 10) multiplier = 2;
        else multiplier = 1;
    }

    void ShowRating(string message, Color color)
    {
        if (ratingText != null)
        {
            ratingText.text = message;
            ratingText.color = color;
        }

        UpdateUIOnly();
    }

    void UpdateUIOnly()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }
}