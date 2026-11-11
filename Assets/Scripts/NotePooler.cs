using System.Collections.Generic;
using UnityEngine;

public class NotePooler : MonoBehaviour
{
    public static NotePooler Instance;

    [Header("Configuración del Pool Múltiple")]
    public GameObject[] notePrefabs; // Aquí arrastras tus 5 prefabs en el mismo orden que en el Spawner
    public int poolSizePerPrefab = 15; // Cuántas copias guardamos de cada tipo

    // Diccionario o lista de listas para manejar las piscinas separadas por índice
    private List<List<GameObject>> pooledNotesLists = new List<List<GameObject>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Creamos una piscina independiente para cada prefab diferente
        for (int i = 0; i < notePrefabs.Length; i++)
        {
            List<GameObject> pool = new List<GameObject>();

            for (int j = 0; j < poolSizePerPrefab; j++)
            {
                GameObject obj = Instantiate(notePrefabs[i]);
                obj.SetActive(false);
                pool.Add(obj);
            }

            pooledNotesLists.Add(pool);
        }
    }

    // Pide una nota prestada del pool correspondiente al carril / tipo exacto
    public GameObject GetPooledNote(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= pooledNotesLists.Count) return null;

        List<GameObject> pool = pooledNotesLists[prefabIndex];

        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Si se saturan las de la piscina, creamos una de emergencia para ese índice específico
        GameObject extraObj = Instantiate(notePrefabs[prefabIndex]);
        pool.Add(extraObj);
        extraObj.SetActive(true);
        return extraObj;
    }
}