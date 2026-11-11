using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LaneController : MonoBehaviour
{
    [Header("Configuración de Carril")]
    public int laneIndex;
    public KeyCode pcKey;

    private List<GameObject> notesInZone = new List<GameObject>();
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Limpiamos referencias nulas por si acaso una nota se desactivó en el aire
        notesInZone.RemoveAll(item => item == null || !item.activeInHierarchy);

        if (collision.CompareTag("Note") || collision.gameObject.name.Contains("arrows") || collision.gameObject.name.Contains("Square"))
        {
            if (!notesInZone.Contains(collision.gameObject))
            {
                notesInZone.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (notesInZone.Contains(collision.gameObject))
        {
            notesInZone.Remove(collision.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pcKey) || CheckNewInputSystemKey())
        {
            HitNote();
        }
    }

    bool CheckNewInputSystemKey()
    {
        if (Keyboard.current == null) return false;

        if (laneIndex == 0 && Keyboard.current.qKey.wasPressedThisFrame) return true;
        if (laneIndex == 1 && Keyboard.current.eKey.wasPressedThisFrame) return true;
        if (laneIndex == 2 && Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        if (laneIndex == 3 && Keyboard.current.aKey.wasPressedThisFrame) return true;
        if (laneIndex == 4 && Keyboard.current.dKey.wasPressedThisFrame) return true;

        return false;
    }

    void HitNote()
    {
        // Efecto visual: se hace un poco más grande y cambia a blanco (brillo)
        transform.localScale = originalScale * 1.15f;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }

        // Limpiamos notas inactivas de la lista por seguridad
        notesInZone.RemoveAll(item => item == null || !item.activeInHierarchy);

        if (notesInZone.Count > 0)
        {
            GameObject noteToHit = notesInZone[0];
            notesInZone.RemoveAt(0);

            float distance = Vector3.Distance(noteToHit.transform.position, transform.position);

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.EvaluateHit(distance);
            }

            // --- REQUISITO DEL PROFE: OBJECT POOLING EN VEZ DE DESTROY ---
            // En lugar de destruir la nota, la desactivamos para que regrese a la piscina
            noteToHit.SetActive(false);
        }
        else
        {
            // Ghost Tap (Fallo por presionar sin nota en el carril)
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.RegisterGhostMiss();
            }
        }
    }

    void LateUpdate()
    {
        // Regresa suavemente a su escala original
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10f);

        // Regresa suavemente a su color original
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, originalColor, Time.deltaTime * 10f);
        }
    }
}