using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class LaneController : MonoBehaviour
{
    [Header("Configuración de Carril")]
    public int laneIndex;
    public KeyCode pcKey;

    private List<GameObject> notesInZone = new List<GameObject>();
    private Vector3 originalScale; // Para guardar el tamaño exacto que les pusiste

    void Start()
    {
        // Guardamos el tamaño original con el que acomodaste las flechas en la escena
        originalScale = transform.localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        // Efecto de rebote basado en su escala original (se encoge un poquito al tocar)
        transform.localScale = originalScale * 0.85f;

        if (notesInZone.Count > 0)
        {
            GameObject noteToDestroy = notesInZone[0];
            notesInZone.RemoveAt(0);
            Destroy(noteToDestroy);
            Debug.Log("¡PERFECT! Carril: " + laneIndex);
        }
        else
        {
            Debug.Log("¡MISS!");
        }
    }

    void LateUpdate()
    {
        // Regresa suavemente a su escala original, no a un Vector3.one genérico
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 10f);
    }
}
