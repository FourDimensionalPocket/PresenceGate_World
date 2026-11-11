using UnityEngine;
using UnityEngine.SceneManagement;

namespace PocketWorld
{
    // Keep the authored scene intact while providing a player when it is opened directly.
    public static class TownWalkBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Setup()
        {
            if (SceneManager.GetActiveScene().path != "Assets/Scenes/Town_Sunny.unity") return;
            if (Object.FindFirstObjectByType<FirstPersonWalker>() != null) return;

            Vector3 start = new Vector3(0f, 100f, -10f);
            if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 200f))
                start = hit.point + Vector3.up * 0.05f;
            else
                start = new Vector3(0f, 0.05f, -10f);

            var player = new GameObject("Player (runtime)");
            player.transform.position = start;
            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0f, 0.9f, 0f);
            controller.stepOffset = 0.3f;
            controller.skinWidth = 0.03f;

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.SetParent(player.transform, false);
            cameraObject.transform.localPosition = new Vector3(0f, 1.65f, 0f);
            var camera = cameraObject.GetComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            camera.fieldOfView = 70f;
            player.AddComponent<FirstPersonWalker>();
        }
    }
}
