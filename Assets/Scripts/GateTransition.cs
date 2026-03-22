using UnityEngine;
using UnityEngine.SceneManagement;

public class GateTransition : MonoBehaviour
{
    [SerializeField] private string targetScene = "GameScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}
