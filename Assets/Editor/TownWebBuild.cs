using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

[InitializeOnLoad]
public static class TownWebBuild
{
    static TownWebBuild() { EditorApplication.delayCall += ApplyRequestedSettings; }

    private static void ApplyRequestedSettings()
    {
        const string request = "Temp/ConfigureWebGL1.request";
        if (!System.IO.File.Exists(request)) return;
        System.IO.File.Delete(request);
        ConfigureWebGL1();
        System.IO.File.WriteAllText("Temp/ConfigureWebGL1.result", "Configured: " + string.Join(", ", PlayerSettings.GetGraphicsAPIs(BuildTarget.WebGL)) + "; color space: " + PlayerSettings.colorSpace);
    }

    [MenuItem("Tools/Pocket World/Configure WebGL 1")]
    public static void ConfigureWebGL1()
    {
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
#pragma warning disable 618
        PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new[] { GraphicsDeviceType.OpenGLES2 });
#pragma warning restore 618
        PlayerSettings.colorSpace = ColorSpace.Gamma;
        AssetDatabase.SaveAssets();
        Debug.Log("WebGL 1 configured with Gamma color space.");
    }

    public static void BuildWebGL1Batch()
    {
        ConfigureWebGL1();
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/Town_Sunny.unity" },
            locationPathName = System.IO.Path.GetFullPath("../Town_WebGL1_Build"),
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded || report.summary.totalErrors != 0 || !System.IO.File.Exists(System.IO.Path.GetFullPath("../Town_WebGL1_Build/index.html")))
            throw new System.Exception("WebGL 1 build failed: " + report.summary.result + ", errors: " + report.summary.totalErrors);
    }
    [MenuItem("Tools/Pocket World/Build Town for Web")]
    public static void Build()
    {
        const string scene = "Assets/Scenes/Town_Sunny.unity";
        if (!System.IO.File.Exists(scene))
        {
            Debug.LogError("Town_Sunny scene was not found.");
            return;
        }
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
        {
            Debug.LogError("Install WebGL Build Support for the current Unity version from Unity Hub first.");
            return;
        }
        string folder = EditorUtility.SaveFolderPanel("Choose an empty folder for the Web build", "", "TownWeb");
        if (string.IsNullOrEmpty(folder)) return;
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { scene },
            locationPathName = folder,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        });
        if (report.summary.result == BuildResult.Succeeded && System.IO.File.Exists(System.IO.Path.Combine(folder, "index.html")))
            Debug.Log("Web build completed: " + folder);
        else
            Debug.LogError("Web build did not produce index.html (result: " + report.summary.result + "). Restart Unity after installing Web Build Support, then build again. Check the Console for earlier errors.");
    }
}
