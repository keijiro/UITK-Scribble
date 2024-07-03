using UnityEngine;
using UnityEngine.UIElements;

namespace UITKScribble {

public class ScribbleBackend : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public string ElementID { get; set; } = "scribble";
    [field:SerializeField] public int Resolution { get; set; } = 1024;
    [field:SerializeField] public Color StrokeColor { get; set; } = Color.red;
    [field:SerializeField] public float StrokeSize { get; set; } = 8;

    #endregion

    #region Project asset references

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Public drawer methods

    public void Clear()
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = _rt;
        GL.Clear(false, true, Color.clear);
        RenderTexture.active = prevRT;
    }

    public void DrawLineSegment((Vector3 p0, Vector3 p1) seg)
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = _rt;

        _material.SetVector("_Point0", seg.p0);
        _material.SetVector("_Point1", seg.p1);
        _material.SetColor("_Color", Color.red);
        _material.SetFloat("_Width", StrokeSize / Resolution);
        _material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 12, 1);

        RenderTexture.active = prevRT;
    }

    #endregion

    #region Private members

    Scribble _ui;
    RenderTexture _rt;
    Material _material;

    void LazyInitialize()
    {
        var rect = _ui.contentRect;
        var aspect = (float)rect.width / rect.height;
        _rt = new RenderTexture(Resolution, (int)(Resolution / aspect), 0);
        _rt.Create();
        Clear();
        _ui.style.backgroundImage = Background.FromRenderTexture(_rt);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _ui = root.Q<Scribble>();

        _material = new Material(_shader);
        _material.hideFlags = HideFlags.DontSave;
    }

    void OnDestroy()
    {
        Destroy(_material);
        _material = null;

        if (_rt != null)
        {
            Destroy(_rt);
            _rt = null;
        }
    }

    void Update()
    {
        if (!_ui.HasInput) return;
        if (_rt == null) LazyInitialize();
        DrawLineSegment(_ui.DequeueInput());
    }

    #endregion
}

} // namespace UITKScribble
