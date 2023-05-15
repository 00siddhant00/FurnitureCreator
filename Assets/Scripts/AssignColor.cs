using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class AssignColor : MonoBehaviour
{
    private Button colorButton;

    private void Start()
    {
        colorButton = GetComponent<Button>();
        colorButton.onClick.AddListener(() => ItemManager.Instance.ColorChange(GetComponent<RawImage>()));
    }
}
