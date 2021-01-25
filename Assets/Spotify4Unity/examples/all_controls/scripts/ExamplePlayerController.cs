using Spotify4Unity;
using Spotify4Unity.Dtos;
using Spotify4Unity.Enums;
using Spotify4Unity.Events;
using Spotify4Unity.Helpers;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Spotify4Unity
/// Example Controller script for a Simple Player with media controls
/// (Make sure you read the Wiki documentation for all information & help!)
/// </summary>
public class ExamplePlayerController : SpotifyUIBase
{
    [SerializeField, Tooltip("Text to display the current track's artists")]
    private TextMeshProUGUI m_artistText = null;

    [SerializeField, Tooltip("Text to display the current track name")]
    private TextMeshProUGUI m_trackText = null;

    //[SerializeField, Tooltip("Text to display the current track's album name")]
    //private TextMeshProUGUI m_albumText = null;

    [SerializeField, Tooltip("Image to display the current track's album art")]
    public Image m_albumArt = null;


    [SerializeField, Tooltip("Sprite icon for when advert is playing. Has media for Tracks and Episodes")]
    private Sprite AdvertSprite = null;

    [SerializeField, Tooltip("The resolution to load album arts at")]
    private Spotify4Unity.Enums.Resolution m_albumArtResolution = Spotify4Unity.Enums.Resolution.Original;

    public songChange songChange;

    const string TIME_SPAN_FORMAT = @"mm\:ss";

    #region MonoBehavious
    protected override void Awake()
    {
        base.Awake();

        

    }

    private void Start()
    {
        // Hide Connect & Connecting UI
        OnConnect();
    }
    #endregion

    private void OnConnect()
    {
        if (!SpotifyService.IsConnected)
        {


            bool didAttempt = SpotifyService.Connect();
            // If an attempt to authorize failed, show connect btn, hide spinner
            if (!didAttempt)
            {

            }
        }
    }

    protected async void OnNextMedia()
    {
        await SpotifyService.NextSongAsync();
    }

    protected async void OnPreviousMedia()
    {
        await SpotifyService.PreviousSongAsync();
    }

    protected async void OnPauseMedia()
    {
        await SpotifyService.PauseAsync();
    }

    protected async void OnPlayMedia()
    {
        await SpotifyService.PlayAsync();
    }

    protected override void OnConnectingChanged(ConnectingChanged e)
    {
        base.OnConnectingChanged(e);

        // Enable Connect Canvas when NOT connecting and NOT connected
        // Enable Spinner when connecting and NOT connected

    }

    protected override void OnConnectedChanged(ConnectedChanged e)
    {
        base.OnConnectedChanged(e);

        // Re-enable connect UI if not connected to Spotify


        // Always disable spinner

    }

    protected override void OnTrackTimeChanged(TrackTimeChanged e)
    {
        base.OnTrackTimeChanged(e);



    }

    protected override void OnPlayStatusChanged(PlayStatusChanged e)
    {
        base.OnPlayStatusChanged(e);


    }

    protected override void OnTrackChanged(TrackChanged e)
    {
        if(e != null)
        {
            // Load the Album Art for the new Track
            LoadAlbumArt(e.NewTrack, m_albumArtResolution);
            SetTrackInfo(e.NewTrack.Title, e.NewTrack.Album, String.Join(", ", e.NewTrack.Artists.Select(x => x.Name)));

            UpdateSaveTrackBtn();
            songChange.changeSong();
        }
    }

    private void LoadAlbumArt(Track t, Spotify4Unity.Enums.Resolution resolution)
    {
        // Get the URL and load on a Coroutine
        string url = t.GetAlbumArtUrl();
        if (!string.IsNullOrEmpty(url))
        {
            if (this.isActiveAndEnabled)
                StartCoroutine(Utility.LoadImageFromUrl(url, resolution, sprite => OnAlbumArtLoaded(sprite)));
            else
                Utility.RunCoroutineEmptyObject(Utility.LoadImageFromUrl(url, resolution, sprite => OnAlbumArtLoaded(sprite)));
        }
    }

    private void OnAlbumArtLoaded(Sprite s)
    {
        if (m_albumArt != null)
        {
            m_albumArt.sprite = s;
        }
    }

    protected override void OnVolumeChanged(VolumeChanged e)
    {
        base.OnVolumeChanged(e);


    }

    protected void OnSetVolumeChanged(float value)
    {
    }

    protected async void OnUnmuteSound()
    {
        if (SpotifyService.IsMuted)
        {
            await SpotifyService.SetMuteAsync(false);
        }
    }

    protected async void OnMuteSound()
    {
        if (!SpotifyService.IsMuted)
        {
            await SpotifyService.SetMuteAsync(true);
        }
    }

    protected override void OnMuteChanged(MuteChanged e)
    {
        base.OnMuteChanged(e);
    }



    public void OnMouseDownTrackTimeSlider()
    {

    }

    public async void OnMouseUpTrackTimeSlider()
    {

    }

    protected async void OnClickShuffle()
    {
        Shuffle state = SpotifyService.ShuffleState;
        if (state == 0)
            state = (Shuffle)1;
        else
            state = (Shuffle)0;

        await SpotifyService.SetShuffleAsync(state);
    }

    protected async void OnClickRepeat()
    {
        //Repeat button acts as a toggle through 3 items
        Repeat state = SpotifyService.RepeatState;
        if (SpotifyService.RepeatState == Repeat.Disabled)
            state = Repeat.Playlist;
        else if (SpotifyService.RepeatState == Repeat.Playlist)
            state = Repeat.Track;
        else if (SpotifyService.RepeatState == Repeat.Track)
            state = Repeat.Disabled;

        await SpotifyService.SetRepeatAsync(state);
    }

    protected override void OnRepeatChanged(RepeatChanged e)
    {
        base.OnRepeatChanged(e);

        
    }

    protected override void OnShuffleChanged(ShuffleChanged e)
    {
        base.OnShuffleChanged(e);


    }

    protected override void OnMediaTypeChanged(MediaTypeChanged e)
    {
        if(e.MediaType == MediaType.Advert)
        {
            if (m_albumArt != null)
                m_albumArt.sprite = AdvertSprite;

            SetTrackInfo("Advert", "Unknown", "Unknown");
        }
    }

    protected override void OnUserInformationLoaded(UserInfoLoaded e)
    {
        base.OnUserInformationLoaded(e);

        // If the user is't Premium, show red error when trying to set playback
        // (Spotify doesn't allow Free users to control their playback through the WebAPI)
        if (!e.Info.IsPremium)
        {

        }
    }

    private void SetBtnPressedTint(ref Button btn)
    {
        if (btn == null)
            return;
        ColorBlock colors = btn.colors;
        btn.colors = colors;
    }

    /// <summary>
    /// Set the correct sprite in the UI button
    /// </summary>
    /// <param name="stateIndex">The state as an int</param>
    /// <param name="spritesArray">The array of sprites for that state</param>
    /// <param name="image">The image inside the button that will change and display the current state</param>
    /// <param name="errorMsg">The output error message if stateIndex is too high</param>
    protected void SetSprite(int stateIndex, Sprite[] spritesArray, Image image, string errorMsg)
    {
        if (stateIndex >= spritesArray.Length)
        {
            Analysis.LogError(errorMsg, Analysis.LogLevel.All);
            return;
        }

        if (image != null)
            image.sprite = spritesArray[stateIndex];
    }

    private void SetTrackPositionText(float currentMs, float maxMs)
    {
        string currentPositionFormat = TimeSpan.FromMilliseconds(currentMs).ToString(TIME_SPAN_FORMAT);
        string totalTimeFormat = TimeSpan.FromMilliseconds(maxMs).ToString(TIME_SPAN_FORMAT);
    }





    /// <summary>
    /// Sets the Track, Album and Artist name
    /// </summary>
    /// <param name="track"></param>
    /// <param name="artist"></param>
    /// <param name="album"></param>
    private void SetTrackInfo(string track, string artist, string album)
    {
        if (m_artistText != null)
            m_artistText.text = artist;

        if (m_trackText != null)
            m_trackText.text = track;

        //if (m_albumText != null)
            //m_albumText.text = album;
    }

    protected override void OnSavedTracksLoaded(SavedTracksLoaded e)
    {
        base.OnSavedTracksLoaded(e);

        UpdateSaveTrackBtn();
    }

    private void UpdateSaveTrackBtn()
    {
        if (SpotifyService.CurrentTrack == null)
            return;

        Track currentTrack = SpotifyService.CurrentTrack;

    }

    private void OnSaveTrack()
    {
        if (SpotifyService.CurrentTrack == null)
            return;

        bool hasTrackSaved = SpotifyService.SavedTracks.Exists(x => x.TrackId == SpotifyService.CurrentTrack.TrackId);
        bool result = false;
        if (hasTrackSaved)
        {
            result = SpotifyService.UnsaveTracks(SpotifyService.CurrentTrack);
        }
        else
        {
            result = SpotifyService.SaveTracks(SpotifyService.CurrentTrack);
        }

        UpdateSaveTrackBtn();
    }

    private void OnDisconnect()
    {
        SpotifyService.Disconnect();
    }
}
