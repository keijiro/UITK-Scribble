using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Scribble : VisualElement
{
    #region Public UI properties

    [UxmlAttribute]
    public ScribbleSettings scribbleSettings { get; set; } = null;

    #endregion

    #region USS class names

    public static readonly string ussClassName = "scribble";

    #endregion

    #region Visual element implementation

    public Scribble()
    {
        AddToClassList(ussClassName);
        this.AddManipulator(new ScribbleManipulator(this));
        generateVisualContent += GenerateVisualContent;
    }

    #endregion

    #region Render callback

    ScribbleBackend _backend;

    static readonly Vertex[] _vertices
      = new Vertex[] { TempVertex(0, 0), TempVertex(0, 1),
                       TempVertex(1, 1), TempVertex(1, 0) };

    static readonly ushort[] _indices = {0, 1, 2, 2, 3, 0};

    static Vertex TempVertex(float u, float v)
    {
        var temp = new Vertex();
        temp.tint = Color.white;
        temp.uv = new Vector2(u, v);
        return temp;
    }

    void GenerateVisualContent(MeshGenerationContext context)
    {
        Debug.Log(scribbleSettings);

        if (_backend == null && scribbleSettings != null)
            _backend = ScribbleBackend.Create(scribbleSettings);

        var rect = contentRect;
        var (w, h, z) = (rect.width, rect.height, Vertex.nearZ);

        _vertices[0].position = new Vector3(0, h, z);
        _vertices[1].position = new Vector3(0, 0, z);
        _vertices[2].position = new Vector3(w, 0, z);
        _vertices[3].position = new Vector3(w, h, z);

        var data = context.Allocate(4, 6);//, Texture2D.redTexture);
        data.SetAllVertices(_vertices);
        data.SetAllIndices(_indices);
    }

    #endregion
}
