namespace VoiceRecorder.Views;

public partial class HomeView : ContentPage
{
    public HomeView(IAudioPlayerService audioPlayerService, IRecordAudioService recordAudioService)
    {
        InitializeComponent();
        BindingContext = new ViewModels.HomeViewModel(audioPlayerService, recordAudioService);
    }
}