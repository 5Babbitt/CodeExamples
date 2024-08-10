using HeathenEngineering.SteamworksIntegration;
using UnityEngine;

public class LobbySlot : MonoBehaviour
{
    [SerializeField] private string _playerName;
    [SerializeField] private Texture2D _playerAvatar;

    [SerializeField] private GameObject _placeholderPlayerModel;
    [SerializeField] private SpriteRenderer _placeholderSprite;
    [SerializeField] private PlayerPortraitPopup _playerPortraitPopup;

    private void Start()
    {
        _playerPortraitPopup = GetComponentInChildren<PlayerPortraitPopup>();
        _placeholderPlayerModel.SetActive(false);
    }

    /// <summary>
    /// Initialize the lobby slot using the SteamID. Leave ID blank only if false.
    /// </summary>
    /// <param name="filled"></param>
    /// <param name="userID">Steam ID</param>
    public void SetFilled(bool filled, ulong userID = 0)
    {
        _placeholderSprite.enabled = !filled;
        _placeholderPlayerModel.SetActive(filled);
        
        if (filled)
        {
            UserData user = UserData.Get(userID);

            user.LoadAvatar((result) => { 
                _playerAvatar = user.Avatar;
                _playerName = user.Name;
                _playerPortraitPopup.SetPortrait(_playerName, _playerAvatar);
            });
        }
        else
        {
            _playerName = null;
            _playerAvatar = null;
            _playerPortraitPopup.SetPortrait(null);
        }
    }
}
