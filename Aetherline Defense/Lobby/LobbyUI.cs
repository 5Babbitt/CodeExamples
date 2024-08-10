using HeathenEngineering.SteamworksIntegration;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    private static class UIClassNames
    {
        public const string LOBBY_COPY_CODE_BUTTON = "lobby-code__copy-button";
        public const string LOBBY_CODE_LABEL = "lobby-code__code";
        public const string LOBBY_BACK_BUTTON = "button--back";
        public const string LOBBY_READY_BUTTON = "button--ready";

        public const string LOBBY_INPUTS = "lobby-inputs";
        public const string LOBBY_INPUTS_HIDDEN = LOBBY_INPUTS + "--hidden";

        public const string LOBBY_INPUT_SECTION = "lobby-input-field";
        public const string LOBBY_INPUT_SECTION_HIDDEN = LOBBY_INPUT_SECTION + "--hidden";

        public const string JOIN_LOBBY_SECTION = "host-or-lobby";
        public const string JOIN_LOBBY_SECTION_HIDDEN = JOIN_LOBBY_SECTION + "--hidden";

        public const string LOBBY_CODE = "lobby-code";
        public const string LOBBY_CODE_HIDDEN = LOBBY_CODE + "--hidden";

        public const string MAIN_MENU = "main-menu";
        public const string MAIN_MENU_HIDDEN = MAIN_MENU + "--hidden";
        public const string MAIN_MENU_BUTTONS = "main-menu-buttons";

        public const string LOADING_POPUP = "lobby-loading-popup";
        public const string LOADING_POPUP_HIDDEN = LOADING_POPUP + "--hidden";
        public const string LOADING_POPUP_TEXT = LOADING_POPUP + "__text";
    }

    private static class UINames
    {
        public const string HOST_BUTTON = "HostButton";
        public const string JOIN_BUTTON = "JoinButton";
        public const string JOIN_LOBBY_BUTTON = "JoinLobbyButton";
        public const string CREDITS_BUTTON = "CreditsButton";
        public const string EXIT_BUTTON = "ExitButton";
        public const string LOBBY_CODE_INPUT = "LobbyCodeInput";
    }

    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private UIDocument _creditsScreenUIDocument;
    [SerializeField] private LobbySlot[] _lobbySlots;

    // UI
    private VisualElement _root;
    private VisualElement _mainMenu;
    private VisualElement _lobbyJoinSection;
    private VisualElement _copyCodeButton;
    private VisualElement _mainMenuButtons;
    private VisualElement _loadingPopup;
    private VisualElement _lobbyCodeSection;
    private Label _loadingPopupLabel;
    private Label _codeLabel;
    private Button _joinButton;
    private Button _joinLobbyButton;
    private Button _backButton;
    private Button _startButton; 
    private Button _hostButton;
    private Button _creditsButton;
    private Button _exitButton;
    private TextField _lobbyCodeInputField;

    // Credits Screen
    private VisualElement _creditsScreenRoot;
    private Button _creditsBackButton;
    private bool _canJoinLobby => _lobbyCodeInputField.text.Length == 8;
    
    private void Awake()
    {
        // Main Menu
        _root = _uiDocument.rootVisualElement;
        _mainMenu = _root.Q<VisualElement>(className: UIClassNames.MAIN_MENU);
        _copyCodeButton = _root.Q<VisualElement>(className: UIClassNames.LOBBY_COPY_CODE_BUTTON);
        _codeLabel = _root.Q<Label>(className: UIClassNames.LOBBY_CODE_LABEL);
        _joinButton = _root.Q<Button>(UINames.JOIN_BUTTON);
        _joinLobbyButton = _root.Q<Button>(UINames.JOIN_LOBBY_BUTTON);
        _backButton = _root.Q<Button>(className: UIClassNames.LOBBY_BACK_BUTTON);
        _startButton = _root.Q<Button>(className: UIClassNames.LOBBY_READY_BUTTON);
        _hostButton = _root.Q<Button>(UINames.HOST_BUTTON);
        _lobbyCodeInputField = _root.Q<TextField>(UINames.LOBBY_CODE_INPUT);
        _lobbyCodeSection = _root.Q<VisualElement>(className: UIClassNames.LOBBY_CODE);
        _lobbyCodeSection.EnableInClassList(UIClassNames.LOBBY_CODE_HIDDEN, true);
        _creditsButton = _root.Q<Button>(UINames.CREDITS_BUTTON);
        _exitButton = _root.Q<Button>(UINames.EXIT_BUTTON);
        _mainMenuButtons = _root.Q<VisualElement>(className: UIClassNames.MAIN_MENU_BUTTONS);
        _lobbyJoinSection = _root.Q<VisualElement>(className: UIClassNames.JOIN_LOBBY_SECTION);

        // Loading
        _loadingPopup = _root.Q<VisualElement>(className: UIClassNames.LOADING_POPUP);
        UIUtils.MakeDisableInputOnHide(_loadingPopup);
        UIUtils.RegisterChangePositioningOnTransitions(_loadingPopup);
        _loadingPopup.style.display = DisplayStyle.None;
        _loadingPopupLabel = _root.Q<Label>(className: UIClassNames.LOADING_POPUP_TEXT);

        // Credits Screen
        _creditsScreenRoot = _creditsScreenUIDocument.rootVisualElement;
        _creditsBackButton = _creditsScreenRoot.Q<Button>("CreditsBackButton");
        _creditsBackButton.clicked += OnCreditsBackButtonClicked;
        _creditsScreenRoot.style.display = DisplayStyle.None;

        // Register UI Events
        _lobbyCodeInputField.RegisterValueChangedCallback(OnLobbyCodeInputFieldUpdate);
        _copyCodeButton.AddManipulator(new Clickable(OnCopyCodeButtonClicked));
        _backButton.clicked += OnBackButtonClicked;
        _startButton.clicked += OnStartButtonClicked;
        _hostButton.clicked += OnHostButtonClicked;
        _joinButton.clicked += OnJoinbuttonClicked;
        _joinLobbyButton.clicked += OnJoinLobbyButtonClicked;
        _creditsButton.clicked += OnCreditsButtonClicked;
        _exitButton.clicked += OnExitButtonClicked;

        _startButton.style.display = DisplayStyle.None;
    }

    private void Start()
    {
        UpdateLobbyButtons();
    }

    private void OnEnable()
    {
        MultiplayerManager.Instance.OnPlayerJoined += OnPlayerJoined;
        MultiplayerManager.Instance.OnPlayerLeft += OnPlayerExit;
        MultiplayerManager.Instance.OnLeftLobby += OnLobbyLeft;
        MultiplayerManager.Instance.OnLobbyJoined += UpdateLobbyInfo;
    }

    private void OnDisable()
    {
        if (MultiplayerManager.Instance == null)
        {
            return;
        }

        MultiplayerManager.Instance.OnPlayerJoined -= OnPlayerJoined;
        MultiplayerManager.Instance.OnPlayerLeft -= OnPlayerExit;
        MultiplayerManager.Instance.OnLeftLobby -= OnLobbyLeft;
        MultiplayerManager.Instance.OnLobbyJoined -= UpdateLobbyInfo;
    }

    #region UI Elements Logic
    void OnCopyCodeButtonClicked()
    {
        UIUtils.CopyToClipboard(_codeLabel.text);
    }

    void OnBackButtonClicked()
    {
        // Return to main menu?
        if (MultiplayerManager.Instance.IsConnectedToLobby)
        {
            LeaveLobby();
        }
        else
        {
            SetMainMenuVisibility(true);
        }
    }

    void OnStartButtonClicked()
    {
        _loadingPopup.style.display = DisplayStyle.Flex;
        MultiplayerManager.Instance.StartHost();
    }
        
    void OnHostButtonClicked()
    {
        MultiplayerManager.Instance.CreateLobby();

        _lobbyJoinSection.EnableInClassList(UIClassNames.JOIN_LOBBY_SECTION_HIDDEN, true);

        SetMainMenuVisibility(false);
        _backButton.text = "Disband";
        UpdateLobbyInfo();
    }

    void OnJoinbuttonClicked()
    {
        SetMainMenuVisibility(false);
        _lobbyJoinSection.EnableInClassList(UIClassNames.JOIN_LOBBY_SECTION_HIDDEN, false);
        _backButton.text = "Leave";
    }

    void OnLobbyCodeInputFieldUpdate(ChangeEvent<string> evt)
    {
        UpdateLobbyButtons();
    }

    void OnJoinLobbyButtonClicked()
    {
        MultiplayerManager.Instance.JoinLobby(_lobbyCodeInputField.text);
    }

    void OnCreditsButtonClicked()
    {
        _creditsScreenRoot.style.display = DisplayStyle.Flex;
    }

    void OnExitButtonClicked()
    {
        Application.Quit();
    }

    void OnCreditsBackButtonClicked()
    {
        _creditsScreenRoot.style.display = DisplayStyle.None;
    }
    #endregion

    #region Lobby Logic
    private void LeaveLobby()
    {
        MultiplayerManager.Instance.LeaveLobby();
        UpdateLobbyInfo();
        _startButton.style.display = DisplayStyle.None;

        // _lobbyInputs.EnableInClassList(UIClassNames.LOBBY_INPUTS_HIDDEN, false);
        _lobbyCodeSection.EnableInClassList(UIClassNames.LOBBY_CODE_HIDDEN, true);
        SetMainMenuVisibility(true);
    }

    private void UpdateLobbyInfo()
    {
        if (MultiplayerManager.Instance.IsLobbyOwner)
        {
            _startButton.style.display = DisplayStyle.Flex;
            _startButton.SetEnabled(MultiplayerManager.Instance.GetPlayerCount >= 1);
        }

        _lobbyJoinSection.EnableInClassList(UIClassNames.JOIN_LOBBY_SECTION_HIDDEN, MultiplayerManager.Instance.IsConnectedToLobby);
        _lobbyCodeSection.EnableInClassList(UIClassNames.LOBBY_CODE_HIDDEN, false);
        _codeLabel.text = MultiplayerManager.Instance.GetJoinCode;

        ToggleLobbySlots();
    }

    private void ToggleLobbySlots()
    {
        var players = MultiplayerManager.Instance.GetLobbyMembers;
        if (players != null)
        {
            for (int i = 0; i < _lobbySlots.Length; i++)
            {
                if (i < players.Length)
                {
                    Debug.Log($"{players[i].user.Name}'s ID: {players[i].user.SteamId}");
                    _lobbySlots[i].SetFilled(true, players[i].user.SteamId);
                }
                else
                {
                    _lobbySlots[i].SetFilled(false);
                }
            }
        }
        else
        {
            foreach (var slot in _lobbySlots)
            {
                slot.SetFilled(false);
            }
        }
    }
    #endregion

    #region Lobby Events
    private void OnPlayerJoined(UserData player)
    {
        print($"On {player} Joined");
        UpdateLobbyInfo();
    }

    private void OnPlayerExit(UserData player)
    {
        print($"On {player} Exit");
        UpdateLobbyInfo();
    }

    private void OnLobbyLeft()
    {
        print("On Lobby Left");
        SetMainMenuVisibility(true);
    }
    #endregion

    private void UpdateLobbyButtons()
    {
        _joinLobbyButton.SetEnabled(_canJoinLobby);
    }

    private void SetMainMenuVisibility(bool isVisible)
    {
        _mainMenu.EnableInClassList(UIClassNames.MAIN_MENU_HIDDEN, !isVisible);
        _mainMenuButtons.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
