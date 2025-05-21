using System;
using UnityEngine;
using FronkonGames.Retro.Pixelator;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PixelatorDemo))]
public class PixelatorDemoWarning : Editor
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

/// <summary> Retro: Pixelator demo. </summary>
/// <remarks>
/// This code is designed for a simple demo, not for production environments.
/// </remarks>
public class PixelatorDemo : MonoBehaviour
{
  [Space]
  
  [SerializeField]
  private Transform floor;

  [SerializeField, Range(0.0f, 10.0f)]
  private float angularVelocity = 5.0f;
  
  private Pixelator.Settings settings;
  
  private GUIStyle styleFont;
  private GUIStyle styleButton;
  private GUIStyle styleLogo;
  private Vector2 scrollView;

  private const float BoxWidth = 600.0f;
  private const float Margin = 20.0f;
  private const float LabelSize = 250.0f;
  private const float OriginalScreenWidth = 1920.0f;

  private void Awake()
  {
    if (Pixelator.IsInRenderFeatures() == false)
    {
      Debug.LogWarning($"Effect '{Constants.Asset.Name}' not found. You must add it as a Render Feature.");
#if UNITY_EDITOR
      if (EditorUtility.DisplayDialog($"Effect '{Constants.Asset.Name}' not found", $"You must add '{Constants.Asset.Name}' as a Render Feature.", "Quit") == true)
        EditorApplication.isPlaying = false;
#endif
    }

    styleFont = null;
    styleButton = null;
    styleLogo = null;

    this.enabled = Pixelator.IsInRenderFeatures();
  }

  private void Start()
  {
    settings = Pixelator.Instance.settings;
    settings?.ResetDefaultValues();
  }

  private void OnDestroy()
  {
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
      fontSize = 42
    };

    if (settings != null)
    {
      GUILayout.BeginVertical("box", GUILayout.Width(BoxWidth), GUILayout.ExpandHeight(true));
      {
        GUILayout.Space(Margin);
      
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label("Retro: Pixelator", styleLogo);
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        scrollView = GUILayout.BeginScrollView(scrollView);
        {
          GUILayout.Space(Margin * 0.5f);

          settings.intensity = Slider("Intensity", settings.intensity, 0.0f, 1.0f);

          settings.pixelationMode = EnumPopup("Shape", settings.pixelationMode);
          settings.pixelSize = Slider("Size", settings.pixelSize, 0.0f, 1.0f);

          settings.posterizeIntensity = Slider("Posterization", settings.posterizeIntensity, 0.0f, 1.0f);

          settings.ditherIntensity = Slider("Dither", settings.ditherIntensity, 0.0f, 1.0f);

          GUILayout.Space(Margin);
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

  private T EnumPopup<T>(string label, T value) where T : Enum
  {
    GUILayout.BeginHorizontal();
    {
      GUILayout.Space(Margin);

      GUILayout.Label(label, styleFont, GUILayout.Width(LabelSize));

      if (GUILayout.Button("<<", GUILayout.Width(30.0f)))
        value = (T)Enum.ToObject(typeof(T), Math.Max(0, Convert.ToInt32(value) - 1));
    
      GUILayout.Label(value.ToString(), styleFont);
    
      if (GUILayout.Button(">>", GUILayout.Width(30.0f)))
        value = (T)Enum.ToObject(typeof(T), Math.Min(Enum.GetValues(typeof(T)).Length - 1, Convert.ToInt32(value) + 1));
    
      GUILayout.Space(Margin);
    }
    GUILayout.EndHorizontal();

    return value;
    
  }  
}