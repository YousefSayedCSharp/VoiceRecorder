using Android.Media;
using Android.Renderscripts;
using VoiceRecorder.InterFaces;

namespace VoiceRecorder.Platforms.Service;

public partial class RecordAudioService : IRecordAudioService
{
    private MediaRecorder? mediaRecorder;
    private string? storagePath;
    private bool isRecordStarted = false;

    public void StartRecord()
    {
        if (mediaRecorder == null)
        {
            SetAudioFilePath();
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            mediaRecorder = new MediaRecorder(MainActivity.context);
else
            mediaRecorder = new MediaRecorder();


            mediaRecorder.Reset();
            mediaRecorder.SetAudioSource(AudioSource.Mic);
            mediaRecorder.SetOutputFormat(OutputFormat.AacAdts);
            mediaRecorder.SetAudioEncoder(AudioEncoder.Aac);

            // ضبط معدل البت (bit rate) ومعدل العينة (sample rate)
            mediaRecorder.SetAudioEncodingBitRate(192000);  // 256 kbps لزيادة جودة الصوت
            mediaRecorder.SetAudioSamplingRate(44100);  // 44.1 kHz لمعدل العينة

            mediaRecorder.SetOutputFile(storagePath);
            mediaRecorder.Prepare();
            mediaRecorder.Start();
        }
        else
        {
            mediaRecorder.Resume();
        }
        isRecordStarted = true;
    }

    public void PauseRecord()
    {
        if (mediaRecorder == null)
        {
            return;
        }
        mediaRecorder.Pause();
        isRecordStarted = false;
    }
    
    public void ResetRecord()
    {
        if (mediaRecorder != null)
        {
            mediaRecorder.Resume();
            mediaRecorder.Reset();
        }
        mediaRecorder = null;
        isRecordStarted = false;
    }
    
    public string StopRecord()
    {
        if (mediaRecorder == null)
        {
            return string.Empty;
        }
        mediaRecorder.Resume();
        mediaRecorder.Stop();
        mediaRecorder = null;
        isRecordStarted = false;
        return storagePath+"";
    }

    private void SetAudioFilePath()
    {
        string fileName = "/Record_" + DateTime.UtcNow.ToString("ddMMM_hhmmss") + ".mp3";
        //var path = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        var path = MauiProgram.ySRecorderFolder();
        storagePath = path + fileName;
    }
}
