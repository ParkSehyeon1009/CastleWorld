using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        GetComponent<Button>()?.onClick.AddListener(() => panel.SetActive(false));
    }
}
