using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, IPointerClickHandler
{
    public string m_ButtonType;

    private UIManager m_UIManager;

    void Start()
    {
        m_UIManager = FindObjectOfType<UIManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (m_ButtonType)
        {
            case "Quit":
                m_UIManager.QuitGame();
                break;
            case "Pause":
                m_UIManager.PauseButton();
                break;
            case "Load":
                m_UIManager.StartScene();
                break;
            default:
                break;
        }
    }
}
