using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using PocketWorld;

// Creates the initial demo only on explicit request, and only in the untouched starter scene.
[InitializeOnLoad]
public static class WalkthroughSetup
{
    private const string RequestPath = "Temp/SetupWalkthrough.request";

    static WalkthroughSetup() => EditorApplication.delayCall += ProcessRequest;

    private static void ProcessRequest()
    {
        if (!File.Exists(RequestPath) || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete(RequestPath);
        Scene scene = SceneManager.GetActiveScene();
        GameObject[] roots = scene.GetRootGameObjects();
        if (scene.path != "Assets/Scenes/SampleScene.unity" || roots.Length != 2 || scene.isDirty
            || Camera.main == null || Object.FindFirstObjectByType<Light>() == null)
        {
            Debug.LogWarning("Walkthrough setup skipped: starter scene has been modified. Use Tools > Pocket World > Create Walkthrough Demo.");
            return;
        }
        CreateDemo();
    }

    [MenuItem("Tools/Pocket World/Create Walkthrough Demo")]
    public static void CreateDemo()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (Object.FindFirstObjectByType<FirstPersonWalker>() != null) return;
        Scene scene = SceneManager.GetActiveScene();
        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Create Walkthrough Demo");
        var world = new GameObject("Walkthrough World");
        Undo.RegisterCreatedObjectUndo(world, "Create walkthrough world");
        Material light = MaterialAsset("Tile Light", new Color(0.62f, 0.71f, 0.70f));
        Material dark = MaterialAsset("Tile Dark", new Color(0.48f, 0.58f, 0.58f));
        Material teal = MaterialAsset("Teal", new Color(0.14f, 0.48f, 0.50f));
        Material orange = MaterialAsset("Orange", new Color(0.94f, 0.48f, 0.20f));
        Material stone = MaterialAsset("Stone", new Color(0.70f, 0.73f, 0.70f));
        for (int x = 0; x < 10; x++)
        for (int z = 0; z < 10; z++)
            Box("Floor Tile", world.transform, new Vector3(x * 4 - 18, -0.25f, z * 4 - 18),
                new Vector3(4, 0.5f, 4), (x + z) % 2 == 0 ? light : dark);
        Box("North Wall", world.transform, new Vector3(0, 0.6f, 20), new Vector3(41, 1.2f, 0.5f), teal);
        Box("South Wall", world.transform, new Vector3(0, 0.6f, -20), new Vector3(41, 1.2f, 0.5f), teal);
        Box("East Wall", world.transform, new Vector3(20, 0.6f, 0), new Vector3(0.5f, 1.2f, 40), teal);
        Box("West Wall", world.transform, new Vector3(-20, 0.6f, 0), new Vector3(0.5f, 1.2f, 40), teal);
        Box("Orange Landmark", world.transform, new Vector3(-5, 1, 3), new Vector3(2, 2, 2), orange);
        Box("Teal Landmark", world.transform, new Vector3(5, 1.5f, 6), new Vector3(2, 3, 2), teal);
        Box("Low Jump Block", world.transform, new Vector3(0, 0.3f, 0), new Vector3(3, 0.6f, 2), orange);
        for (int i = 0; i < 5; i++)
            Box("Step " + (i + 1), world.transform, new Vector3(-9, (i + 1) * 0.125f, 3 + i),
                new Vector3(3, (i + 1) * 0.25f, 1), stone);
        Box("Step Platform", world.transform, new Vector3(-9, 0.625f, 9), new Vector3(3, 1.25f, 3), stone);

        var player = new GameObject("Player");
        Undo.RegisterCreatedObjectUndo(player, "Create player");
        player.transform.position = new Vector3(0, 0.05f, -10);
        var controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.3f;
        controller.center = new Vector3(0, 0.9f, 0);
        controller.stepOffset = 0.3f;
        controller.slopeLimit = 45;
        controller.skinWidth = 0.03f;
        Camera camera = Camera.main;
        if (camera == null)
        {
            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            Undo.RegisterCreatedObjectUndo(cameraObject, "Create camera");
            cameraObject.tag = "MainCamera";
            camera = cameraObject.GetComponent<Camera>();
        }
        Undo.SetTransformParent(camera.transform, player.transform, "Attach player camera");
        Undo.RecordObject(camera.transform, "Position player camera");
        camera.transform.localPosition = new Vector3(0, 1.65f, 0);
        camera.transform.localRotation = Quaternion.identity;
        Undo.RecordObject(camera, "Configure player camera");
        camera.nearClipPlane = 0.05f;
        camera.fieldOfView = 70;
        player.AddComponent<FirstPersonWalker>();
        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(scene.path, true) };
        Selection.activeGameObject = player;
        File.WriteAllText("Temp/WalkthroughSetup.result", "SUCCESS: scene saved, player and collidable world created.");
        Debug.Log("Walkthrough ready. Press Play, click Game view, then use WASD, mouse, Shift and Space. Esc releases the mouse.");
    }

    private static Material MaterialAsset(string name, Color color)
    {
        const string folder = "Assets/Materials";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Materials");
        string path = folder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        material = new Material(Shader.Find("Standard")) { name = name, color = color };
        material.SetFloat("_Glossiness", 0.15f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static void Box(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.position = position;
        box.transform.localScale = scale;
        box.GetComponent<Renderer>().sharedMaterial = material;
    }
}
