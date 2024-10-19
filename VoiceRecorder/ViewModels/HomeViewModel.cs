using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VoiceRecorder.Helper;

namespace VoiceRecorder.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    IRecordAudioService? recordAudioService;
    IAudioPlayerService? audioPlayerService;
    IDispatcherTimer? recordTimer;
    IDispatcherTimer? playTimer;

    private TimeSpan? timerValue;
    private bool isRecord;

    [ObservableProperty]
    private string? recentAudioFilePath;

    [ObservableProperty]
    private bool isRecordingAudio;

    [ObservableProperty]
    private string? timerLabel;

    [ObservableProperty]
    private Audio? audioFile;

    [ObservableProperty]
    private bool isRecordButtonVisible;

    [ObservableProperty]
    private bool isPauseButtonVisible;

    [ObservableProperty]
    private bool isResumeButtonVisible;

    [ObservableProperty]
    private ObservableCollection<Audio>? audios;

    public HomeViewModel(IAudioPlayerService audioPlayerService, IRecordAudioService recordAudioService)
    {
        this.audioPlayerService = audioPlayerService;
        this.recordAudioService = recordAudioService;
        Audios = new ObservableCollection<Audio>();
        IsRecordButtonVisible = true;
        IsRecordingAudio = false;
        IsResumeButtonVisible = false;

        Load();
    }

    public void CreateTimer()
    {
        recordTimer = Application.Current?.Dispatcher.CreateTimer();

        //timer start
        if (recordTimer == null) return;

        recordTimer.Interval = new TimeSpan(0, 0, 1);
        recordTimer.Tick += (s, e) =>
        {
            if (isRecord)
            {
                timerValue += new TimeSpan(0, 0, 1);
                TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
            }
        };
    }

    [RelayCommand]
    //start record command
    public async Task Record()
    {
        try
        {
            if (!IsRecordingAudio)
            {
                //var permissionStatus = await RequestandCheckPermission();
                PermissionStatus status = await Permissions.RequestAsync<ReadWriteStoragePerms>();
                if (status == PermissionStatus.Granted)
                {
                    IsRecordingAudio = true;
                    IsPauseButtonVisible = true;
                    recordAudioService?.StartRecord();
                    IsRecordButtonVisible = false;
                    isRecord = true;
                    timerValue = new TimeSpan(0, 0, -1);
                    if (recordTimer == null)
                        CreateTimer();
                    recordTimer?.Start();
                }
                else
                {
                    IsRecordingAudio = false;
                    IsPauseButtonVisible = false;
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    [RelayCommand]
    //Pause recording command
    public async Task Pause()
    {
        try
        {
            isRecord = false;
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = true;
            recordAudioService?.PauseRecord();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    private void ResumeRecording()
    {
        recordAudioService?.StartRecord();
        IsResumeButtonVisible = false;
        IsPauseButtonVisible = true;
        isRecord = true;
    }

    [RelayCommand]
    //Reset recording command
    public async Task Reset()
    {
        try
        {
            recordAudioService?.ResetRecord();
            timerValue = new TimeSpan();
            TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
            IsRecordingAudio = false;
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = false;
            await Record();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    [RelayCommand]
    //stop recording command
    public async Task Stop()
    {
        try
        {
            IsPauseButtonVisible = false;
            IsResumeButtonVisible = false;
            IsRecordingAudio = false;
            IsRecordButtonVisible = true;
            timerValue = new TimeSpan();
            recordTimer?.Stop();
            RecentAudioFilePath = recordAudioService?.StopRecord();
            await Shell.Current.DisplayAlert("Alert", "Audio has been recorded", "OK");
            TimerLabel = string.Format("{0:mm\\:ss}", timerValue);
            await Send();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    [RelayCommand]
    //Send recording command
    public async Task Send()
    {
        try
        {
            Audio? recordedFile = new Audio() { AudioURL = RecentAudioFilePath + "" };
            if (recordedFile != null)
            {
                recordedFile.AudioName = Path.GetFileNameWithoutExtension(RecentAudioFilePath);
                Audios?.Insert(0, recordedFile);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    [RelayCommand]
    //PlayAudio command
    public async Task PlayAudio(object obj)
    {
        try
        {
            if (AudioFile != null && AudioFile != (Audio)obj)
            {
                AudioFile.IsPlayVisible = true;
                StopAudio();
            }
            if (obj is Audio)
            {
                AudioFile = (Audio)obj;
                AudioFile.IsPlayVisible = false;
                string? audioFilePath = AudioFile?.AudioURL;
                audioPlayerService?.PlayAudio(audioFilePath);
                SetCurrentAudioPosition();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    [RelayCommand]
    //pause audio command
    public async Task PauseAudio(object obj)
    {
        try
        {
            if (obj is Audio)
            {
                var audioFile = (Audio)obj;
                audioFile.IsPlayVisible = true;
                audioPlayerService?.Pause();
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    public void StopAudio()
    {
        if (AudioFile != null)
        {
            audioPlayerService?.Stop();
            playTimer?.Stop();
        }
    }

    private void SetCurrentAudioPosition()
    {
        if (playTimer == null)
            playTimer = Application.Current?.Dispatcher.CreateTimer();

        if (playTimer != null)
        {
            playTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            playTimer.Tick += (s, e) =>
            {
                if (AudioFile != null)
                {
                    AudioFile.CurrentAudioPosition = audioPlayerService?.GetCurrentPlayTime();
                    bool isAudioCompleted = false;
                    if (audioPlayerService != null)
                        isAudioCompleted = audioPlayerService.CheckFinishedPlayingAudio();
                    if (isAudioCompleted)
                    {
                        AudioFile.IsPlayVisible = true;
                        audioPlayerService?.Stop();
                        playTimer.Stop();
                    }
                }
            };
            playTimer.Start();
        }
    }

    [RelayCommand]
    //delete audio command
    public async Task Delete(Audio? obj)
    {
        try
        {
            var alert = await Shell.Current.DisplayAlert("Alert", "Are you sure you want to delete the audio?", "Yes", "No");
            if (alert && obj != null && File.Exists(obj.AudioURL))
            {
                Audios?.Remove(obj);
                File.Delete(obj.AudioURL);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("", ex.Message, "OK");
        }
    }

    //public async Task<PermissionStatus> RequestandCheckPermission()
    //{
    //    PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
    //    if (status != PermissionStatus.Granted)
    //        await Permissions.RequestAsync<Permissions.StorageWrite>();

    //    status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
    //    if (status != PermissionStatus.Granted)
    //        await Permissions.RequestAsync<Permissions.Microphone>();

    //    PermissionStatus storagePermission = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
    //    PermissionStatus microPhonePermission = await Permissions.CheckStatusAsync<Permissions.Microphone>();
    //    if (storagePermission == PermissionStatus.Granted && microPhonePermission == PermissionStatus.Granted)
    //    {
    //        return PermissionStatus.Granted;
    //    }
    //    return PermissionStatus.Denied;
    //}

    public async void Load()
    {
        //var permissionStatus = await RequestandCheckPermission();
        PermissionStatus status = await Permissions.RequestAsync<ReadWriteStoragePerms>();
        //await Shell.Current.DisplayAlert("", status+"","OK");
        if (status == PermissionStatus.Granted)
        {
            //string dir = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            string dir = MauiProgram.ySRecorderFolder();
            foreach (string item in Directory.GetFiles(dir))
            {
                if (Path.GetExtension(item).ToLower() == ".mp3")
                {
                    Audio recordedFile = new Audio() { AudioURL = item };
                    if (recordedFile != null)
                    {
                        recordedFile.AudioName = Path.GetFileNameWithoutExtension(item);//+" | "+Path.GetExtension(item);
                        Audios?.Insert(0, recordedFile);
                    }
                }
            }
        }//end if
        else
        {
            await Shell.Current.DisplayAlert("", "سيتم الخروج من البرنامج لأنه لا يملك صلاحية الوصول إلى إلى الميكروفون وإدارة وحدة التخزين ", "OK");
            App.Current?.Quit();
        }//end else
    }

}
