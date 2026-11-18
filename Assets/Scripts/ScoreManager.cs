using UnityEngine;
using UnityEngine.UI;
using Monou;
using UnityEngine.SceneManagement;

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
            // Rosa Neón brillante y saturado
            RegisterHit(1000, "CRUSHING IT!", new Color(1f, 0.07f, 0.57f));
        }
        else if (distanceToTarget < 0.4f)
        {
            // Azul Cian Neón
            RegisterHit(700, "CHUNKY!", new Color(0f, 1f, 1f));
        }
        else
        {
            // Amarillo Neón eléctrico
            RegisterHit(300, "ALMOST...", new Color(0.9f, 1f, 0f));
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

    // Cuando dejas pasar una nota (Miss) -> Ahora dice MISFIRE! en Neón
    public void RegisterMiss()
    {
        combo = 0;
        multiplier = 1;
        missCount++;

        score -= 500;
        if (score < 0) score = 0;

        // Rojo Sangre Neón súper agresivo para el Miss
        ShowRating("MISFIRE!", new Color(1f, 0.05f, 0.1f));
        UpdateUIOnly();
    }

    // Cuando presionas la tecla por error sin ninguna nota (Ghost Tap)
    public void RegisterGhostMiss()
    {
        combo = 0;
        multiplier = 1;
        score -= 200;
        if (score < 0) score = 0;

        // Naranja Neón brillante
        ShowRating("GHOST!", new Color(1f, 0.4f, 0f));
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
        score += 1;
        GameScraper.Advance(score);
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }
}