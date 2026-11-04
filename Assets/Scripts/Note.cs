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
            currentSpeed = speed * 2f; // O el multiplicador exacto que necesite para emparejarse
        }

        transform.position += Vector3.down * currentSpeed * Time.deltaTime;

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
}