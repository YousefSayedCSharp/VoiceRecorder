namespace VoiceRecorder.InterFaces;

public interface IRecordAudioService
{
    void StartRecord();
    string? StopRecord();
    void PauseRecord();
    void ResetRecord();
}
