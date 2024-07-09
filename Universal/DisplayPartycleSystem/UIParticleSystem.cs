using UnityEngine;
using UnityEngine.UI;

public class UIParticleSystem : MaskableGraphic
{
    [SerializeField] private ParticleSystemRenderer _particleSystemRenderer;
    [SerializeField] private Texture _texture;
    private Camera _bakeCamera;

    public override Texture mainTexture => _texture ?? base.mainTexture;

    protected override void Awake()
    {
        base.Awake();
        _bakeCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(Mesh mesh)
    {
        mesh.Clear();
        if (_particleSystemRenderer != null && _bakeCamera != null)
            _particleSystemRenderer.BakeMesh(mesh, _bakeCamera);
    }
}
