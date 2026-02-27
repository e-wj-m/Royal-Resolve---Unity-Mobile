using UnityEngine;
using UnityEngine.UIElements;

public enum MenuStates
{
    MainMenu,
    SettingsMenu,
    CreditsMenu
}

public abstract class BaseMenu
{
    protected VisualElement menuRoot;
    protected MenuController context;

    public BaseMenu(MenuController context, VisualElement menuRoot)
    {
        this.context = context;
        this.menuRoot = menuRoot;

        BindButtons();
    }

    protected abstract void BindButtons();

    public virtual void Show()
    {
        menuRoot.style.display = DisplayStyle.Flex;
    }

    public virtual void Hide()
    {
        menuRoot.style.display = DisplayStyle.None;
    }

    public virtual void AddStyleSheet(StyleSheet sheet)
    {
        menuRoot.styleSheets.Clear();
        menuRoot.styleSheets.Add(sheet);
    }
}

public class MainMenu : BaseMenu
{
    public MainMenu(MenuController context, VisualElement menuRoot) : base(context, menuRoot) { }

    protected override void BindButtons()
    {
        Button startButton = menuRoot.Q<Button>("StartGameButton");
        Button settingsButton = menuRoot.Q<Button>("SettingsButton");
        Button creditsButton = menuRoot.Q<Button>("CreditsButton");
        Button quitButton = menuRoot.Q<Button>("QuitGameButton");

        if (startButton != null)
            startButton.clicked += context.StartGame;

        if (settingsButton != null)
            settingsButton.clicked += () => context.PushMenu(MenuStates.SettingsMenu);

        if (creditsButton != null)
            creditsButton.clicked += () => context.PushMenu(MenuStates.CreditsMenu);

        if (quitButton != null)
            quitButton.clicked += context.QuitGame;
    }
}

public class SettingsMenu : BaseMenu
{
    public SettingsMenu(MenuController context, VisualElement menuRoot)
        : base(context, menuRoot) { }

    protected override void BindButtons()
    {
        // Back to main menu
        Button mainMenuButton = menuRoot.Q<Button>("MainMenuButton");
        if (mainMenuButton != null)
            mainMenuButton.clicked += () => context.PushMenu(MenuStates.MainMenu);

        var audio = AudioSettingsManager.Instance;
        if (audio == null) return;

        // UI Toolkit Controls 

        Toggle musicToggle = menuRoot.Q<Toggle>("MusicToggle");
        Slider musicSlider = menuRoot.Q<Slider>("MusicSlider");

        Toggle playerToggle = menuRoot.Q<Toggle>("PlayerSFXToggle");
        Slider playerSlider = menuRoot.Q<Slider>("PlayerSFXSlider");

        Toggle enemyToggle = menuRoot.Q<Toggle>("EnemySfxToggle");
        Slider enemySlider = menuRoot.Q<Slider>("EnemySfxSlider");

        // Initialize UI from saved settings 

        if (musicToggle != null)
            musicToggle.value = audio.GetEnabled(AudioChannel.Music);

        if (musicSlider != null)
            musicSlider.value = audio.GetVolume(AudioChannel.Music);

        if (playerToggle != null)
            playerToggle.value = audio.GetEnabled(AudioChannel.PlayerSfx);

        if (playerSlider != null)
            playerSlider.value = audio.GetVolume(AudioChannel.PlayerSfx);

        if (enemyToggle != null)
            enemyToggle.value = audio.GetEnabled(AudioChannel.EnemySfx);

        if (enemySlider != null)
            enemySlider.value = audio.GetVolume(AudioChannel.EnemySfx);

        // Register change callbacks 

        if (musicToggle != null)
            musicToggle.RegisterValueChangedCallback(evt =>
                audio.SetEnabled(AudioChannel.Music, evt.newValue));

        if (musicSlider != null)
            musicSlider.RegisterValueChangedCallback(evt =>
                audio.SetVolume(AudioChannel.Music, evt.newValue));

        if (playerToggle != null)
            playerToggle.RegisterValueChangedCallback(evt =>
                audio.SetEnabled(AudioChannel.PlayerSfx, evt.newValue));

        if (playerSlider != null)
            playerSlider.RegisterValueChangedCallback(evt =>
                audio.SetVolume(AudioChannel.PlayerSfx, evt.newValue));

        if (enemyToggle != null)
            enemyToggle.RegisterValueChangedCallback(evt =>
                audio.SetEnabled(AudioChannel.EnemySfx, evt.newValue));

        if (enemySlider != null)
            enemySlider.RegisterValueChangedCallback(evt =>
                audio.SetVolume(AudioChannel.EnemySfx, evt.newValue));
    }
}

public class CreditsMenu : BaseMenu
{
    public CreditsMenu(MenuController context, VisualElement menuRoot) : base(context, menuRoot) { }

    protected override void BindButtons()
    {
        Button mainMenuButton = menuRoot.Q<Button>("MainMenuButton");

        if (mainMenuButton != null)
            mainMenuButton.clicked += () => context.PushMenu(MenuStates.MainMenu);
    }
}