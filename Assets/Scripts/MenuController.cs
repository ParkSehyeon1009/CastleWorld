using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject dropdownPanel;

    private void Start()
    {
        var btn = GetComponentInChildren<Button>();
        if (btn != null)
            btn.onClick.AddListener(ToggleMenu);

        if (dropdownPanel != null)
            dropdownPanel.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (dropdownPanel != null)
            dropdownPanel.SetActive(!dropdownPanel.activeSelf);
    }
}
