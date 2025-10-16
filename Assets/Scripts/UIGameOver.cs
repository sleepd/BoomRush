using System.Collections;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] GameObject _restartLabel;
    void OnEnable()
    {
        if (_restartLabel != null)
        {
            _restartLabel.SetActive(false);
            StartCoroutine(ShowRestartLabelDelayed());
        }
    }

    private IEnumerator ShowRestartLabelDelayed()
    {
        yield return new WaitForSeconds(1f);
        if (_restartLabel != null)
        {
            _restartLabel.SetActive(true);
        }
    }
}
