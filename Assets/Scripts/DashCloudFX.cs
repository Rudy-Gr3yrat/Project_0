using UnityEngine;

public class DashCloudFX : MonoBehaviour
{
    public Color smokeColor = new Color(0.55f, 0.2f, 0.9f, 1f);
    public Color glowColor = new Color(1f, 0.6f, 1f, 1f);

    Renderer[] meshes;
    ParticleSystem smoke;
    ParticleSystem glow;

    void Awake()
    {
        meshes = GetComponentsInChildren<Renderer>();

        Texture2D dot = MakeSoftDot();
        smoke = BuildSystem("DashSmoke", smokeColor, dot, size: 0.8f, life: 0.35f, rate: 80f);
        glow = BuildSystem("DashGlow", glowColor, dot, size: 0.35f, life: 0.25f, rate: 50f);
    }

    public void Begin(Vector3 direction)
    {
        foreach (Renderer r in meshes) if (r != null) r.enabled = false;
        smoke.Play();
        glow.Play();
        smoke.Emit(15); 
        glow.Emit(10);
    }

    public void End()
    {
        smoke.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        glow.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        foreach (Renderer r in meshes) if (r != null) r.enabled = true;
    }

    ParticleSystem BuildSystem(string objName, Color color, Texture2D tex, float size, float life, float rate)
    {
        GameObject go = new GameObject(objName);
        go.transform.SetParent(transform, false);

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop = true;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = life;
        main.startSpeed = 0.15f;
        main.startSize = size;
        main.startColor = color;

        var emission = ps.emission;
        emission.rateOverTime = rate;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.25f;

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        colorOverLifetime.color = g;

        Shader shader = Shader.Find("Custom/FX/DashGlowAdditive");
        if (shader == null) shader = Shader.Find("Sprites/Default"); // fallback if the .shader file isn't in the project

        Material mat = new Material(shader);
        mat.mainTexture = tex;

        go.GetComponent<ParticleSystemRenderer>().material = mat;
        return ps;
    }

    Texture2D MakeSoftDot()
    {
        int size = 32;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float half = size / 2f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half;
                float a = Mathf.Clamp01(1f - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        tex.Apply();
        return tex;
    }

    void OnDisable()
    {
        if (meshes != null) foreach (Renderer r in meshes) if (r != null) r.enabled = true;
    }
}