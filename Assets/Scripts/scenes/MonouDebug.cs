using UnityEngine;
using Monou;
using UnityEngine.SceneManagement;
public class MonouDebug : MonoBehaviour
{
    public string gameOverScene;
    public int score;

    public KeyCode scoreKey, gameOverKey;
    
    void Update()
    {
       //if (Input.GetKeyDown(scoreKey))
        //{
         //  score += 1;
         //   GameScraper.Advance(score);
        //}

        if (Input.GetKeyDown(gameOverKey))
        {
            GameScraper.Finish(score);
            SceneManager.LoadScene(gameOverScene);
        }
    }
}
