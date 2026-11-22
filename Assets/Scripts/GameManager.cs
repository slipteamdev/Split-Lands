using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string endSceneName = "EndScene";
    [SerializeField] private float endGameThreshold = 0.9f;
    [SerializeField] private AnimalSpawner animalSpawner;

    [SerializeField] private float increasedSpawnSpeedThreshold = 60f;
    [SerializeField] private float spawnSpeedBonus = 1f;

    private bool speedIncreased = false;

    // Update is called once per frame
    void Update()
    {
        if (Timer.remainingTime <= 0)
        {
            EndGameWithWinner();
        }
        if (Map.normalicedProgress >= endGameThreshold || Map.normalicedProgress <= (1 - endGameThreshold))
        {
            EndGameWithWinner();
        }
        if (Timer.remainingTime <= increasedSpawnSpeedThreshold && speedIncreased == false)
        {
            IncreaseSpawnSpeed();
        }
    }

    private void IncreaseSpawnSpeed()
    {
        if (animalSpawner != null)
        {
            animalSpawner.IncreaseSpawnSpeed(spawnSpeedBonus);
            speedIncreased = true;
        }
    }

    private void EndGameWithWinner()
    {
        GameData.Instance.SetGameResult(Map.normalicedProgress);
        SceneManager.LoadScene(endSceneName);
    }
}
