using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private string endSceneName = "EndScene";


    // Update is called once per frame
    void Update()
    {
        if (Timer.remainingTime <= 0)
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
