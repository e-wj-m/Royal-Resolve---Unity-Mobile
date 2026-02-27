using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MenuController : MonoBehaviour
{
    private UIDocument uiDoc;
    private readonly Dictionary<MenuStates, BaseMenu> menuDictonary = new Dictionary<MenuStates, BaseMenu>();
    private readonly Stack<MenuStates> menuStack = new Stack<MenuStates>();
    private BaseMenu currentState;

    [SerializeField] private string gameSceneName = "GameScene";

    private void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    private void Start()
    {
        VisualElement root = uiDoc.rootVisualElement;

        // Loop through all enum values and try to load Resources/Menu/[ENUM NAME].uxml
        foreach (MenuStates menuState in Enum.GetValues(typeof(MenuStates)))
        {
            string path = "Menu/" + menuState;

            VisualTreeAsset newMenu = Resources.Load<VisualTreeAsset>(path);
            if (newMenu == null)
            {
                Debug.LogError($"Could not find UXML doc at Resources/{path}.uxml");
                continue;
            }

            // Clone the UXML tree and add it to the UIDocument root
            VisualElement menuElement = newMenu.CloneTree();
            menuElement.name = menuState.ToString();
            root.Add(menuElement);

            // Instantiate the concrete menu
            BaseMenu menuInstance = CreateMenuInstance(menuState, menuElement);

            if (menuInstance != null)
            {
                menuDictonary.Add(menuState, menuInstance);
                menuInstance.Hide();
            }
        }

        // Initial menu push
        if (menuDictonary.ContainsKey(MenuStates.MainMenu))
            PushMenu(MenuStates.MainMenu);
        else
            Debug.LogError("MainMenu state not found. Make sure Resources/Menu/MainMenu.uxml exists.");
    }

    private BaseMenu CreateMenuInstance(MenuStates menuState, VisualElement menuElement)
    {
        return menuState switch
        {
            MenuStates.MainMenu => new MainMenu(this, menuElement),
            MenuStates.SettingsMenu => new SettingsMenu(this, menuElement),
            MenuStates.CreditsMenu => new CreditsMenu(this, menuElement),
            _ => null
        };
    }

    public void PushMenu(MenuStates newMenu)
    {
        if (!menuDictonary.ContainsKey(newMenu))
        {
            Debug.LogError($"PushMenu failed: No menu registered for state {newMenu}.");
            return;
        }

        if (currentState != null)
            currentState.Hide();

        currentState = menuDictonary[newMenu];
        currentState.Show();
        menuStack.Push(newMenu);
    }

    public void PopMenu()
    {
        if (currentState != null)
            currentState.Hide();

        if (menuStack.Count > 1)
        {
            menuStack.Pop();
            currentState = menuDictonary[menuStack.Peek()];
        }

        if (currentState != null)
            currentState.Show();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Makes Quit work in editor play mode too
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}