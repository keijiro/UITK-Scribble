using UnityEngine;

public class ScribbleBackend : MonoBehaviour
{
    #region Public properties

    public RenderTexture CanvasTexture { get; private set; }

    #endregion

    #region Public factory method

    public static ScribbleBackend Create
      (ScribbleSettings settings, ScribbleManipulator manipulator)
    {
        if (!Application.isPlaying) return null;
        var go = new GameObject("Scribble Backend");
        var instance = go.AddComponent<ScribbleBackend>();
        instance.Initialize(settings, manipulator);
        return instance;
    }

    #endregion

    #region Public drawer methods

    public void Clear()
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = CanvasTexture;
        GL.Clear(false, true, Color.clear);
        RenderTexture.active = prevRT;
    }

    public void Stroke(Vector3 p0, Vector3 p1)
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = CanvasTexture;

        _material.SetVector("_Point0", p0);
        _material.SetVector("_Point1", p1);
        _material.SetColor("_Color", Color.red);
        _material.SetFloat("_Width", 0.01f);
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 12, 1);

        RenderTexture.active = prevRT;
    }

    #endregion

    #region MonoBehaviour implementation

    void OnDestroy()
    {
        if (_material != null)
        {
            Destroy(_material);
            _material = null;
        }
    }

    void Update()
    {
        var queue = _manipulator.Points;
        if (queue.Count < 2) return;

        var p1 = queue.Dequeue();
        while (queue.Count > 1) queue.Dequeue();
        var p2 = queue.Peek();

        Stroke(p1 * 0.001f, p2 * 0.001f);
    }

    #endregion

    #region Private methods

    ScribbleManipulator _manipulator;
    Material _material;

    void Initialize(ScribbleSettings settings, ScribbleManipulator manipulator)
    {
        _manipulator = manipulator;

        _material = new Material(settings.ScribbleShader);
        _material.hideFlags = HideFlags.DontSave;

        CanvasTexture = new RenderTexture(1920, 1080, 0);
        CanvasTexture.Create();

        Clear();
    }

    #endregion
}
