using UnityEngine;
using FronkonGames.Retro.CRTTV;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(RetroCRTTVDemo))]
public class RetroCRTTVDemoWarning : Editor
{
  private GUIStyle Style => style ??= new GUIStyle(GUI.skin.GetStyle("HelpBox")) { richText = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
  private GUIStyle style;
  public override void OnInspectorGUI()
  {
    EditorGUILayout.TextArea($"\nThis code is only for the demo\n\n<b>DO NOT USE</b> it in your projects\n\nIf you have any questions,\ncheck the <a href='{Constants.Support.Documentation}'>online help</a> or use the <a href='mailto:{Constants.Support.Email}'>support email</a>,\n<b>thanks!</b>\n", Style);
    DrawDefaultInspector();
  }
}
#endif

/// <summary> Retro: CRT TV demo. </summary>
/// <remarks> This code is designed for a simple demo, not for production environments. </remarks>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public sealed class RetroCRTTVDemo : MonoBehaviour
{
  [Space]
  
  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity;
  
  private CRTTV.Settings settings;
  
  private GUIStyle styleFont;
  private GUIStyle styleButton;
  private GUIStyle styleLogo;
  private Vector2 scrollView;

  private const float BoxWidth = 700.0f;
  private const float Margin = 20.0f;
  private const float LabelSize = 250.0f;
  private const float OriginalScreenWidth = 1920.0f;

  private int Slider(string label, int value, int left, int right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = (int)GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private float Slider(string label, float value, float left, float right)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = GUILayout.HorizontalSlider(value, left, right, GUILayout.ExpandWidth(true));
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private bool Toggle(string label, bool value)
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);
    
      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));
    
      value = GUILayout.Toggle(value, string.Empty);
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
  }

  private void Awake()
  {
    if (CRTTV.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
    }

    this.enabled = CRTTV.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = CRTTV.Instance.settings;
    settings?.ResetDefaultValues();
  }

  private void Update()
  {
    if (floor != null && angularVelocity > 0.0f)
      floor.rotation = Quaternion.Euler(0.0f, floor.rotation.eulerAngles.y + Time.deltaTime * angularVelocity * 10.0f, 0.0f);
  }

  private void OnGUI()
  {
    Matrix4x4 guiMatrix = GUI.matrix;
    GUI.matrix = Matrix4x4.Scale(Vector3.one * (Screen.width / OriginalScreenWidth));

    styleFont ??= new GUIStyle(GUI.skin.label)
      {
        alignment = TextAnchor.UpperLeft,
        fontStyle = FontStyle.Bold,
        fontSize = 28
      };

    styleButton ??= new GUIStyle(GUI.skin.button)
      {
        fontStyle = FontStyle.Bold,
        fontSize = 28
      };

    styleLogo ??= new GUIStyle(GUI.skin.label)
      {
        alignment = TextAnchor.MiddleCenter,
        fontStyle = FontStyle.Bold,
        fontSize = 36
      };

    if (settings != null)
    {
      GUILayout.BeginVertical("box", GUILayout.Width(BoxWidth), GUILayout.ExpandHeight(true));
      {
        GUILayout.Space(Margin);
      
        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label("RETRO: CRT TV", styleLogo);
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(Margin * 0.5f);
        
        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          settings.intensity = Slider("Intensity", settings.intensity, 0.0f, 1.0f);
          settings.shadowmaskStrength = Slider("Shadowmask", settings.shadowmaskStrength, 0.0f, 1.0f);
          settings.fishEyeStrength = Slider("Fisheye", settings.fishEyeStrength, 0.0f, 1.0f);
          settings.vignetteSmoothness = Slider("Vignette", settings.vignetteSmoothness, 0.0f, 2.0f);
          settings.shineStrength = Slider("Shine", settings.shineStrength, 0.0f, 1.0f);
          settings.rgbOffsetStrength = Slider("RGB offset", settings.rgbOffsetStrength, 0.0f, 1.0f);
          settings.colorBleedingStrength = Slider("Color bleeding", settings.colorBleedingStrength, 0.0f, 1.0f);
          settings.scanlines = Slider("Scanlines", settings.scanlines, 0.0f, 2.0f);
          settings.interferenceStrength = Slider("Interference", settings.interferenceStrength, 0.0f, 1.0f);
          settings.shakeRate = Slider("Shaking", settings.shakeRate, 0.0f, 1.0f);
          settings.movementRate = Slider("Movement", settings.movementRate, 0.0f, 1.0f);
          settings.grain = Slider("Grain", settings.grain, 0.0f, 1.0f);
          settings.staticNoise = Slider("Noise", settings.staticNoise, 0.0f, 1.0f);
          settings.barStrength = Slider("Bar", settings.barStrength, 0.0f, 1.0f);
          settings.flickerStrength = Slider("Flicker", settings.flickerStrength, 0.0f, 1.0f);
        }
        GUILayout.EndScrollView();

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("RESET", styleButton) == true)
          settings?.ResetDefaultValues();

        GUILayout.Space(4.0f);

        if (GUILayout.Button("ONLINE DOCUMENTATION", styleButton) == true)
          Application.OpenURL(Constants.Support.Documentation);

        GUILayout.Space(4.0f);

        if (GUILayout.Button("❤️ LEAVE A REVIEW ❤️", styleButton) == true)
          Application.OpenURL(Constants.Support.Store);

        GUILayout.Space(Margin * 2.0f);
      }
      GUILayout.EndVertical();
    }
    else
      GUILayout.Label($"URP not available or '{Constants.Asset.Name}' is not correctly configured, please consult the documentation", styleLogo);

    GUI.matrix = guiMatrix;
  }
}