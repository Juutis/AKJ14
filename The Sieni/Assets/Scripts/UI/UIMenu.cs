using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIMenu : MonoBehaviour
{
    public static UIMenu main;
    private void Awake()
    {
        main = this;
    }

    [SerializeField]
    private List<UIMenuSelection> selections = new List<UIMenuSelection>();

    private int selectorIndex = 0;

    private UIMenuSelection CurrentSelection;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private bool isMainMenu;

    void Start()
    {
#if UNITY_WEBGL || UNITY_EDITOR
        UIMenuSelection quitSelection = selections.FirstOrDefault(selection => selection.SelectionType == UISelectionType.Exit);
        selections.Remove(quitSelection);
        quitSelection.gameObject.SetActive(false);
#endif
        if (isMainMenu)
        {
            string previousMode = PlayerPrefs.GetString("previousMode", "story");
            if (previousMode == "endless")
            {
                selectorIndex = 1;
            }
            else if (previousMode == "story")
            {
                selectorIndex = 0;
            }
            UIMenuSelection mmSelection = selections.FirstOrDefault(selection => selection.SelectionType == UISelectionType.MainMenu);
            UIMenuSelection restartSelection = selections.FirstOrDefault(selection => selection.SelectionType == UISelectionType.Restart);
            selections.Remove(mmSelection);
            mmSelection.gameObject.SetActive(false);
            selections.Remove(restartSelection);
            restartSelection.gameObject.SetActive(false);
            if (MusicPlayer.main != null)
            {
                MusicPlayer.main.Kill();
            }
        }
        else
        {
            UIMenuSelection storySelection = selections.FirstOrDefault(selection => selection.SelectionType == UISelectionType.Story);
            UIMenuSelection endlessSelection = selections.FirstOrDefault(selection => selection.SelectionType == UISelectionType.Endless);
            selections.Remove(storySelection);
            storySelection.gameObject.SetActive(false);
            selections.Remove(endlessSelection);
            endlessSelection.gameObject.SetActive(false);
        }
        int index = 0;
        foreach (UIMenuSelection selection in selections)
        {
            selection.SetSelected(selectorIndex == index);
            index += 1;
        }
        CurrentSelection = selections[selectorIndex];
    }

    public void MoveSelector(bool directionDown)
    {
        if (directionDown)
        {
            if (selectorIndex < selections.Count - 1)
            {
                selectorIndex += 1;
            }
            else
            {
                return;
            }
        }
        else
        {
            if (selectorIndex > 0)
            {
                selectorIndex -= 1;
            }
            else
            {
                return;
            }
        }

        CurrentSelection = selections[selectorIndex];
        foreach (UIMenuSelection selection in selections)
        {
            if (selection.IsSelected)
            {
                selection.SetSelected(false);
            }
        }
        CurrentSelection.SetSelected(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveSelector(true);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveSelector(false);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Select();
        }
    }

    public void Select()
    {
        selectorIndex = 0;
        UISelectionType selectionType = CurrentSelection.SelectionType;

        if (selectionType == UISelectionType.Story)
        {
            PlayerPrefs.SetString("previousMode", "story");
            Time.timeScale = 1f;
            SceneManager.LoadScene(1);

        }
        if (selectionType == UISelectionType.Endless)
        {
            PlayerPrefs.SetString("previousMode", "endless");
            Time.timeScale = 1f;
            SceneManager.LoadScene(2);
        }
        if (selectionType == UISelectionType.Exit)
        {
            Application.Quit();
        }
        if (selectionType == UISelectionType.MainMenu)
        {
            MusicPlayer.main.ResetEffects();
            Time.timeScale = 1f;
            SceneManager.LoadScene(0);
        }
        if (selectionType == UISelectionType.Restart)
        {
            MusicPlayer.main.ResetEffects();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

}


public enum UISelectionType
{
    Story,
    Endless,
    Exit,
    MainMenu,
    Restart
}