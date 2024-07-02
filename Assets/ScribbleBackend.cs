using UnityEngine;

public class ScribbleBackend : MonoBehaviour
{
    public RenderTexture CanvasTexture { get; private set; }

    Shader _shader;

    public static ScribbleBackend Create(ScribbleSettings settings)
    {
        var go = new GameObject("Scribble (temp)");
        go.hideFlags = HideFlags.DontSave;
        var comp = go.AddComponent<ScribbleBackend>();
        comp._shader = settings.ScribbleShader;
        return comp;
    }

    void Initialize()
    {
        CanvasTexture = new RenderTexture(1920, 1080, 0);
    }

    void Update()
    {
        if (CanvasTexture == null) Initialize();
    }
}

