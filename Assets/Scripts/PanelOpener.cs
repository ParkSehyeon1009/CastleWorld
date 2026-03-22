using UnityEngine;
using UnityEngine.UI;

public class PanelOpener : MonoBehaviour
{
    [SerializeField] private GameObject targetPanel;

    private void Start()
    {
        GetComponent<Button>()?.onClick.AddListener(() => targetPanel.SetActive(true));
    }
}
