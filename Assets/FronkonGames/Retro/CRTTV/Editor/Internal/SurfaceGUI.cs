////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>. All rights reserved.
//
// THIS FILE CAN NOT BE HOSTED IN PUBLIC REPOSITORIES.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using UnityEngine;
using UnityEditor;
using static FronkonGames.Retro.CRTTV.Inspector;

namespace FronkonGames.Retro.CRTTV.Editor
{
  /// <summary> ShaderGUI base. </summary>
  public abstract class SurfaceGUI : ShaderGUI
  {
    private GUIStyle styleLogo;
    private MaterialProperty[] properties;

    protected abstract void InspectorGUI(MaterialEditor materialEditor, MaterialProperty[] properties);

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
      if (styleLogo == null)
      {
        Font font = null;
        string[] ids = AssetDatabase.FindAssets("FronkonGames-Black");
        for (int i = 0; i < ids.Length; ++i)
        {
          string fontPath = AssetDatabase.GUIDToAssetPath(ids[i]);
          if (fontPath.Contains(".otf") == true)
          {
            font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            break;
          }
        }

        if (font != null)
        {
          styleLogo = new GUIStyle(EditorStyles.boldLabel)
          {
            font = font,
            alignment = TextAnchor.LowerLeft,
            fontSize = 24
          };
        }
      }

      Material targetMat = materialEditor.target as Material;
      this.properties = properties;

      BeginVertical();
      {
        /////////////////////////////////////////////////
        // Description.
        /////////////////////////////////////////////////
        if (styleLogo != null)
        {
          Separator();

          EditorGUILayout.BeginHorizontal();
          {
            FlexibleSpace();

            GUILayout.Label(Constants.Asset.Name, styleLogo);
          }
          EditorGUILayout.EndHorizontal();

          EditorGUILayout.BeginHorizontal();
          {
            FlexibleSpace();

            GUILayout.Label(Constants.Asset.Description, EditorStyles.miniLabel);
          }
          EditorGUILayout.EndHorizontal();

          Separator();
        }

        /////////////////////////////////////////////////
        // Common.
        /////////////////////////////////////////////////
        Separator();

        TextureProperty("_MainTex");
        ColorProperty("_BaseColor");

        Separator();

        SliderProperty("_Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", 1.0f);

        /////////////////////////////////////////////////
        // Custom.
        /////////////////////////////////////////////////
        Separator();

        InspectorGUI(materialEditor, properties);

        /////////////////////////////////////////////////
        // Misc.
        /////////////////////////////////////////////////
        Separator();

        BeginHorizontal();
        {
          if (MiniButton(" documentation", "Online documentation") == true)
            Application.OpenURL(Constants.Support.Documentation);

          if (MiniButton("support", "Do you have any problem or suggestion?") == true)
            SupportWindow.ShowWindow();

          FlexibleSpace();
          // if (Button("Reset") == true)
          //   settings.ResetDefaultValues();
        }
        EndHorizontal();
      }
      EndVertical();
    }

    protected void SliderProperty(string propertyName, float reset) => SliderProperty(propertyName, "", reset);

    protected void SliderProperty(string propertyName, string tooltip, float reset)
    {
      MaterialProperty property = FindProperty(propertyName, properties, true);
      if (property != null)
      {
        EditorGUILayout.BeginHorizontal();
        {
          property.floatValue = EditorGUILayout.Slider(new GUIContent(property.displayName, tooltip), property.floatValue, property.rangeLimits.x, property.rangeLimits.y);

          if (ResetButton(reset) == true)
            property.floatValue = reset;
        }
        EditorGUILayout.EndHorizontal();
      }
    }

    protected void VectorProperty(string propertyName, Vector3 reset) => VectorProperty(propertyName, "", reset); 

    protected void VectorProperty(string propertyName, string tooltip, Vector3 reset)
    {
      MaterialProperty property = FindProperty(propertyName, properties, true);
      if (property != null)
      {
        EditorGUILayout.BeginHorizontal();
        {
          property.vectorValue = EditorGUILayout.Vector3Field(new GUIContent(property.displayName, tooltip), property.vectorValue);

          if (ResetButton(reset) == true)
            property.vectorValue = reset;
        }
        EditorGUILayout.EndHorizontal();
      }
    }

    protected void VectorProperty(string propertyName, Vector2 reset) => VectorProperty(propertyName, "", reset);

    protected void VectorProperty(string propertyName, string tooltip, Vector2 reset)
    {
      MaterialProperty property = FindProperty(propertyName, properties, true);
      if (property != null)
      {
        EditorGUILayout.BeginHorizontal();
        {
          property.vectorValue = EditorGUILayout.Vector2Field(new GUIContent(property.displayName, tooltip), property.vectorValue);

          if (ResetButton(reset) == true)
            property.vectorValue = reset;
        }
        EditorGUILayout.EndHorizontal();
      }
    }

    protected void ColorProperty(string propertyName, Color? reset = null) => ColorProperty(propertyName, "", reset);

    protected void ColorProperty(string propertyName, string tooltip, Color? reset = null, bool showAlpha = false, bool hdrEnabled = true)
    {
      MaterialProperty property = FindProperty(propertyName, properties, true);
      if (property != null)
      {
        EditorGUI.showMixedValue = property.hasMixedValue;
        EditorGUI.BeginChangeCheck();

        EditorGUILayout.BeginHorizontal();

        Color color = EditorGUILayout.ColorField(new GUIContent(property.displayName, tooltip), property.colorValue, false, showAlpha, hdrEnabled);

        if (reset != null && ResetButton(reset) == true)
          property.colorValue = reset.Value;

        EditorGUILayout.EndHorizontal();

        EditorGUI.showMixedValue = false;

        if (EditorGUI.EndChangeCheck() == true)
          property.colorValue = color;
      }
    }

    protected void TextureProperty(string propertyName, string tooltip = "")
    {
      MaterialProperty property = FindProperty(propertyName, properties, true);
      if (property != null)
        property.textureValue = (Texture)EditorGUILayout.ObjectField(new GUIContent(property.displayName, tooltip), property.textureValue, typeof(Texture), true);
    }
  }
}
