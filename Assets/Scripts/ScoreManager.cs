using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton para llamarlo fácil desde cualquier script

    [Header("Estadísticas de Juego")]
    public int score = 0;
    public int combo = 0;
    public int multiplier = 1;
    public int missCount = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Método que se llama cuando presionas la tecla y evalúas qué tan preciso fuiste
    public void EvaluateHit(float distanceToTarget)
    {
        // Aquí defines el margen de error en distancia o tiempo (ejemplos en unidades de Unity)
        if (distanceToTarget < 0.2f)
        {
            RegisterHit(1000, "¡Perfecto!");
        }
        else if (distanceToTarget < 0.4f)
        {
            RegisterHit(700, "¡Bien!");
        }
        else
        {
            RegisterHit(300, "¡Podría ser mejor!");
        }
    }

    void RegisterHit(int basePoints, string rating)
    {
        combo++;
        UpdateMultiplier();

        int earnedPoints = basePoints * multiplier;
        score += earnedPoints;

        Debug.Log($"{rating} +{earnedPoints} pts | Combo: {combo} (x{multiplier})");
    }

    // Método para cuando fallas o la nota se pasa (Miss)
    public void RegisterMiss()
    {
        combo = 0;
        multiplier = 1;
        missCount++;

        Debug.Log("¡Mal! Combo roto. Errores: " + missCount);
    }

    // Actualiza el multiplicador según las reglas de tu combo
    void UpdateMultiplier()
    {
        if (combo >= 50)
        {
            multiplier = 8;
        }
        else if (combo >= 25)
        {
            multiplier = 4;
        }
        else if (combo >= 10)
        {
            multiplier = 2;
        }
        else
        {
            multiplier = 1;
        }
    }
}
