using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VoiceRecorder.Models;

public class Audio : INotifyPropertyChanged
{
    private bool isPlayVisible;
    public bool IsPlayVisible
    {
        get { return isPlayVisible; }
        set
        {
            isPlayVisible = value;
            OnPropertyChanged();
            IsPauseVisble = !value;
        }
    }

    private bool isPauseVisible;
    public bool IsPauseVisble
    {
        get { return isPauseVisible; }
        set { isPauseVisible = value; OnPropertyChanged(); }
    }

    private string? currentAudioPostion;
    public string? CurrentAudioPosition
    {
        get { return currentAudioPostion; }
        set
        {
            if (string.IsNullOrEmpty(currentAudioPostion))
            {
                currentAudioPostion = string.Format("{0:mm\\:ss}", new TimeSpan());
            }
            else
            {
                currentAudioPostion = value;
            }
            OnPropertyChanged();
        }
    }

    public Audio()
    {
        IsPlayVisible = true;
    }

    public string? AudioName { get; set; }
    public string? AudioURL { get; set; }
    public string? Caption { get; set; }
    
    

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = PropertyChanged;
        if (changed == null)
            return;

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    
}
