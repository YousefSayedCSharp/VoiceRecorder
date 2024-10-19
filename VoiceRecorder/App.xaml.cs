namespace VoiceRecorder
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (App.Current != null)
                App.Current.UserAppTheme = AppTheme.Dark;

            MainPage = new AppShell();
        }
    }
}