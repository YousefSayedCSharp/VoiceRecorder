namespace VoiceRecorder.Helper;

public class ReadWriteStoragePerms : Permissions.BasePlatformPermission
{
#if ANDROID
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions =>
            new List<(string androidPermission, bool isRuntime)>
            {
        (global::Android.Manifest.Permission.ReadExternalStorage, true),
        (global::Android.Manifest.Permission.WriteExternalStorage, true),
        (global::Android.Manifest.Permission.RecordAudio, true),
        //(global::Android.Manifest.Permission.ReadMediaAudio, true),
        //(global::Android.Manifest.Permission.ReadMediaImages, true),
        //(global::Android.Manifest.Permission.ReadMediaVideo, true),
        //(global::Android.Manifest.Permission.ManageExternalStorage, true)
        //(global::Android.Manifest.Permission.ManageDocuments, true),
            }.ToArray();
#endif
}