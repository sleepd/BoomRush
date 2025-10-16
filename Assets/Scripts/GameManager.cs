using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameObject _gameoverUI;
    public bool IsRunning { get; private set; }
    public override void Awake()
    {
        base.Awake();
        IsRunning = true;
    }

    public void Restart()
    {
        Scene current = SceneManager.GetActiveScene();
        if (current.IsValid())
        {
            SceneManager.LoadScene(current.buildIndex);
        }
    }

    public void Gameover()
    {
        IsRunning = false;
        PlayerController.Instance.Die();
        _gameoverUI.SetActive(true);
    }
}
