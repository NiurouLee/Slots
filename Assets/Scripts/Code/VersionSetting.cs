
public class VersionSetting
{
#if UNITY_ANDROID
    public const string Version = "1.0.1";        //ProjectSetting/Player/Version*
    public const string VersionCode = "3";        //ProjectSetting/Player/Bundle Version Code
    public const string BundleVersion = "v39";      //AssetBundle Version
#elif UNITY_IOS
    public const string Version = "1.0.1";        //ProjectSetting/Player/Version*
    public const string VersionCode = "2";        //ProjectSetting/Player/Bundle Version Code
    public const string BundleVersion = "v39";      //AssetBundle Version
#endif

}