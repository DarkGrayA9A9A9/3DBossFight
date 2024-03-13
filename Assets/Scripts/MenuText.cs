using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuText : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type { Start, Option, Exit, GoBack, WindowMode, Resume, PausedOption, PausedOptionExit, GoTitle }
    public Type type;

    Text text;

    void Awake()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        
    }

    void OnEnable()
    {
        text.color = new Color(1, 1, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (type)
        {
            case Type.Start:
                SceneManager.LoadScene("Main");
                break;
            case Type.Option:
                TitleManager.instance.Option();
                break;
            case Type.Exit:
                Application.Quit();
                break;
            case Type.GoBack:
                TitleManager.instance.OptionExit();
                break;
            case Type.WindowMode:
                TitleManager.instance.SetWindow();
                break;
            case Type.Resume:
                GameManager.instance.PausedMenuExit();
                break;
            case Type.PausedOption:
                GameManager.instance.Option();
                break;
            case Type.PausedOptionExit:
                GameManager.instance.OptionExit();
                break;
            case Type.GoTitle:
                GameManager.instance.ReturnToTitle();
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = new Color(1, 1, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = new Color(1, 1, 1);
    }
}
