using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string endSceneName = "EndScene";
    [SerializeField] private float endGameThreshold = 0.9f;

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
    }

    private void EndGameWithWinner()
    {
        GameData.Instance.SetGameResult(Map.normalicedProgress);
        SceneManager.LoadScene(endSceneName);
    }
}
