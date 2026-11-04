using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        float currentSpeed = speed;

        // Si este prefab es la flecha amarilla (arrows_4), le duplicamos la velocidad a la fuerza
        if (gameObject.name.Contains("arrows_4"))
        {
            currentSpeed = speed * 2f;
        }

        transform.position += Vector3.down * currentSpeed * Time.deltaTime;

        // Si la nota se pasa de largo sin ser tocada:
        if (transform.position.y < -7f)
        {
            // ¡Avisamos al ScoreManager que hubo un fallo (Miss)!
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.RegisterMiss();
            }

            Destroy(gameObject);
        }
    }
}