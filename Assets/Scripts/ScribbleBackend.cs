using UnityEngine;
using UnityEngine.UIElements;

namespace UITKScribble {

public class ScribbleBackend : MonoBehaviour
{
    #region Public properties

    [field:SerializeField] public string ElementID { get; set; } = "scribble";
    [field:SerializeField] public Color StrokeColor { get; set; } = Color.red;
    [field:SerializeField] public float StrokeSize { get; set; } = 1;

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

    public void Stroke(Vector3 p0, Vector3 p1)
    {
        var prevRT = RenderTexture.active;
        RenderTexture.active = _rt;

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

    Scribble _ui;
    RenderTexture _rt;
    Material _material;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _ui = root.Q<Scribble>();

        _rt = new RenderTexture(1920, 1080, 0);
        _rt.Create();

        _material = new Material(_shader);
        _material.hideFlags = HideFlags.DontSave;

        Clear();

        _ui.style.backgroundImage = Background.FromRenderTexture(_rt);
    }

    void OnDestroy()
    {
        Destroy(_material);
        _material = null;
    }

    void Update()
    {
        if (!_ui.HasInput) return;

        var seg = _ui.DequeueInput();
        Stroke(seg.p1 * 0.001f, seg.p2 * 0.001f);
    }

    #endregion
}

} // namespace UITKScribble
