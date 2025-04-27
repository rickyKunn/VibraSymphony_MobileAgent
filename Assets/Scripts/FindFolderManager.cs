using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

[RequireComponent(typeof(RectTransform))]
public class FindFolderManager : MonoBehaviour
{
    [SerializeField] private GameObject MusicBox;
    [SerializeField] private GameObject MusicsListPanel;
    [SerializeField] private GameObject ExitButton;
    [SerializeField] private GameObject PermissionErrors;
    [SerializeField] private Sprite AdmittedImage;

    private bool hasPermissionButtonPressed;
    private bool permissionAdmitted;
    private List<string> FilesPath = new List<string>();
    private List<string> FilesName = new List<string>();

    async void Start()
    {
        permissionAdmitted = CheckAndRequestPermission();
        if (!permissionAdmitted)
        {
            PermissionErrors.SetActive(true);
        }
        // ユーザーが権限を許可するまで待機
        await UniTask.WaitUntil(() => permissionAdmitted);
        FindFolders();
    }

    private bool CheckAndRequestPermission()
    {
#if PLATFORM_ANDROID
        int sdk = GetAndroidSDKInt();

        // Android 13 以上は granular media permission
        if (sdk >= 33)
        {
            const string audioPerm = "android.permission.READ_MEDIA_AUDIO";
            if (!Permission.HasUserAuthorizedPermission(audioPerm))
            {
                Permission.RequestUserPermission(audioPerm);
                return false;
            }
        }
        // Android 6～12 (API23～32) は従来のストレージ読み取り権限
        else if (sdk >= 23)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                return false;
            }
        }
        // Android 5 以下はインストール時に付与済み
        return true;
#else
        return true;
#endif
    }

    // ボタンから再リクエスト
    public void SetPermissionAdmitAgain()
    {
#if PLATFORM_ANDROID
        if (!hasPermissionButtonPressed)
        {
            hasPermissionButtonPressed = true;
            permissionAdmitted = false;
            PermissionErrors.transform
                .GetChild(2)
                .GetComponent<Image>()
                .sprite = AdmittedImage;
            CheckAndRequestPermission();
            return;
        }
        else
        {
            permissionAdmitted = CheckAndRequestPermission();
            if (!permissionAdmitted)
            {
                var text = PermissionErrors.transform
                    .GetChild(1)
                    .GetComponent<TextMeshProUGUI>();
                text.text = "設定 → アプリ → “この作品の名前” →\n“音楽とオーディオ” の権限を許可してください";
                PermissionErrors.SetActive(true);
            }
            else
            {
                PermissionErrors.SetActive(false);
            }
        }
#endif
    }

    private int GetAndroidSDKInt()
    {
#if PLATFORM_ANDROID
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
#else
        return 0;
#endif
    }

    private async void FindFolders()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        string downloadPath = GetAndroidDownloadPath();
        if (string.IsNullOrEmpty(downloadPath)) return;

        var files = new DirectoryInfo(downloadPath).GetFiles();
        foreach (var file in files)
        {
            if (file.Extension.Equals(".mp3", StringComparison.OrdinalIgnoreCase)
                && !file.Name.StartsWith(".trashed", StringComparison.OrdinalIgnoreCase)
                && !file.Name.EndsWith(".trashed", StringComparison.OrdinalIgnoreCase))
            {
                FilesName.Add(file.Name);
                FilesPath.Add(file.FullName);
            }
        }
        InstantiateMusicBoxes();
    }

    private void InstantiateMusicBoxes()
    {
        if (Application.platform != RuntimePlatform.Android) return;

        var panelRt = MusicsListPanel.GetComponent<RectTransform>();
        var safeSize = GetSafeAreaAnchor("size").x;
        panelRt.sizeDelta = new Vector2(
            Mathf.Clamp(panelRt.sizeDelta.x * FilesPath.Count, 0, safeSize),
            panelRt.sizeDelta.y
        );

        var child = MusicsListPanel.transform.GetChild(0)
            .GetChild(0).gameObject;
        var childRt = child.GetComponent<RectTransform>();
        childRt.sizeDelta = new Vector2(
            childRt.sizeDelta.x * FilesPath.Count,
            childRt.sizeDelta.y
        );

        float exitX = Mathf.Clamp(225 * FilesPath.Count, 225, safeSize / 2);
        ExitButton.GetComponent<RectTransform>()
            .anchoredPosition = new Vector3(exitX, -70);

        for (int i = 0; i < FilesPath.Count; i++)
        {
            var box = Instantiate(MusicBox, child.transform, false);
            var mgr = box.GetComponent<MusicBoxManager>();
            mgr.SetThisBox(FilesName[i], i + 1, FilesPath[i]);
        }
    }

    private string GetAndroidDownloadPath()
    {
#if PLATFORM_ANDROID
        using (var env = new AndroidJavaClass("android.os.Environment"))
        using (var dir = env.GetStatic<AndroidJavaObject>("DIRECTORY_DOWNLOADS"))
        using (var file = env.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", dir))
        {
            return file.Call<string>("getAbsolutePath");
        }
#else
        Debug.LogWarning("Not running on Android");
        return null;
#endif
    }

    private Vector2 GetSafeAreaAnchor(string kind)
    {
        var safe = Screen.safeArea;
        return kind switch
        {
            "min" => safe.position,
            "max" => safe.position + safe.size,
            "size" => safe.size,
            _ => Vector2.zero
        };
    }

    public static void Request_SettingsIntent()
    {
#if PLATFORM_ANDROID
        using var act = GetActivity();
        using var intent = new AndroidJavaObject(
            "android.content.Intent",
            "android.settings.MANAGE_ALL_FILES_ACCESS_PERMISSION"
        );
        act.Call("startActivity", intent);
#endif
    }

    private static AndroidJavaObject GetActivity()
    {
#if PLATFORM_ANDROID
        using var up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        return up.GetStatic<AndroidJavaObject>("currentActivity");
#else
        return null;
#endif
    }
}
