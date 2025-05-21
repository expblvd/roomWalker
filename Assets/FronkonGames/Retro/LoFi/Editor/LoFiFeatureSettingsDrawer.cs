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
using static FronkonGames.Retro.LoFi.Inspector;
using System;

namespace FronkonGames.Retro.LoFi.Editor
{
  /// <summary> Retro Lo-Fi inspector. </summary>
  [CustomPropertyDrawer(typeof(LoFi.Settings))]
  public class SparkFeatureSettingsDrawer : Drawer
  {
    private LoFi.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<LoFi.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 0.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Lo-Fi.
      /////////////////////////////////////////////////
      Separator();

      settings.palette = Toggle("Palette", "Activate color conversion using a palette.", settings.palette);
      IndentLevel++;

      EditorGUILayout.BeginHorizontal();
      {
        settings.profile = ObjectField("Profile", "Lo-Fi palette.", settings.profile);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), EditorStyles.miniLabel, GUILayout.Width(20.0f), GUILayout.Height(20.0f)) == true)
          PaletteBrowser.ShowWindow();
      }

      EditorGUILayout.EndHorizontal();
      settings.mode = (BlendModes)EnumPopup("Mode", "Interpolation, or not, of colors of the palette. Default BlendModes.Blend.", settings.mode, BlendModes.Blend);

      settings.sampleMethod = (SampleMethod)EnumPopup("Sample method", "Method used to sample color from the palette.", settings.sampleMethod, SampleMethod.Distance);
      switch (settings.sampleMethod)
      {
        case SampleMethod.Luminance:
          IndentLevel++;
          settings.luminancePow = Slider("Luminance", "Contrast between zones of different luminance [0, 2]. Default 1.5.", settings.luminancePow, 0.0f, 2.0f, 1.5f);

          float min = settings.rangeMin;
          float max = settings.rangeMax;
          MinMaxSlider("Remap luminance", "Luminance range used to change colors.", ref min, ref max, 0.0f, 1.0f, 0.0f, 1.0f);
          settings.rangeMin = min;
          settings.rangeMax = max;

          settings.invert = Toggle("Invert", "Inverts the order of the color palette. Default false.", settings.invert);
          IndentLevel--;
          break;
        case SampleMethod.Distance:
        case SampleMethod.HSV:
        case SampleMethod.Similarity:
        case SampleMethod.Dominant:
          IndentLevel++;
          settings.colorThreshold = Slider("Color threshold", "Maximum distance between colors [0, 1]. Small values result in colors more similar to the palette. Default 0.5.", settings.colorThreshold, 0.0f, 1.0f, 0.5f);
          IndentLevel--;
          break;
      }

      settings.resolution = (PaletteResolutions)EnumPopup("Resolution", "Texture resolution using by the palette. The higher the resolution, the less artifacts in the color gradients.", settings.resolution, PaletteResolutions._32);
      if (settings.profile != null)
      {
        GUILayout.Label(string.Empty);
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.xMin += EditorGUIUtility.labelWidth;

        EditorGUI.DrawPreviewTexture(rect, settings.profile.ToTexture(settings.mode, settings.invert, (int)settings.resolution));
      }
      IndentLevel--;

      settings.pixelate = Toggle("Pixelate", "Activates the pixelization effect.", settings.pixelate);
      IndentLevel++;
      settings.pixelSize = RoundToNearestEven(Math.Max(2, IntField("Size", "Pixel size [2, ...]. Even values only. Default 12.", settings.pixelSize, 12)));
      IndentLevel++;
      settings.pixelBlend = (ColorBlends)EnumPopup("Blend", "Operation used to blend the pixels with the original image. Default ColorBlends.Solid.", settings.pixelBlend, ColorBlends.Solid);
      settings.pixelTint = ColorField("Tint", "Pixel tinting. Default white.", settings.pixelTint, Color.white);
      IndentLevel--;

      settings.pixelSobel = Slider("Sobel", "Sobel effect strength [0, 2]. Default 0.", settings.pixelSobel, 0.0f);
      IndentLevel++;
      settings.pixelSobelPower = Slider("Power", "Sobel effect intensity [0.01, 10]. Default 1.", settings.pixelSobelPower, 0.01f, 10.0f, 1.0f);
      settings.pixelSobelAngle = Slider("Light angle", "Sobel effect intensity [0, 2]. Default 0.", settings.pixelSobelAngle, 0.0f, 360.0f, 45.0f);
      settings.pixelSobelLightIntensity = Slider("Light intensity", "Sobel effect intensity [0, 2]. Default 0.", settings.pixelSobelLightIntensity, 0.0f, 2.0f, 1.0f);
      settings.pixelSobelAmbient = Slider("Ambient", "Sobel effect intensity [0, 2]. Default 0.", settings.pixelSobelAmbient, 0.0f, 0.5f, 0.2f);
      IndentLevel--;

      settings.pixelRound = Slider("Round", "Roundness of the pixel. The more roundness, the more circular the pixel will be [0, 1]. Default 1.", settings.pixelRound, 0.0f);
      IndentLevel++;
      settings.pixelBevel = Slider("Bevel", "Flange enhancing the pixel shape [0, 1]. Default 0.", settings.pixelBevel, 0.0f);
      IndentLevel--;

      settings.pixelSamples = Slider("Samples", "Pixel smoothing [1, 8]. Default 2.", settings.pixelSamples, 1, 8, 2);
      IndentLevel--;

      settings.scanline = Slider("Scanline", "Scalines intensity [0, 1]. Default 1.", settings.scanline, 1.0f);
      IndentLevel++;
      settings.scanlineCount = IntField("Count", "Scanlines count [0, ...]. Default 500.", settings.scanlineCount, 500);
      settings.scanlineSpeed = Slider("Speed", "Scanlines speed [-25, 25]. Default 10.", settings.scanlineSpeed, -25.0f, 25.0f, 10.0f);
      IndentLevel--;

      settings.vignette = Slider("Vignette", "Darkens the image margins [0, 1]. Default 0.3.", settings.vignette, 0.3f);

      settings.quantization = Toggle("Quantization", "Activates the quantization effect.", settings.quantization);
      IndentLevel++;
      settings.colors = Slider("Colors", "Reduction of the number of colors [2, 64]. Default 32.", settings.colors, 2, 64, 32);
      IndentLevel--;

      settings.chromaticAberration = Slider("Chromatic aberration", "Chromatic aberration [0, 2]. Default 0.5.", settings.chromaticAberration, 0.0f, 2.0f, 0.5f);

      settings.shine = Slider("Glass shine", "Glass shine [0, 1]. Default 0.1.", settings.shine, 0.1f);
      IndentLevel++;
      settings.shineSize = Slider("Size", "Glass shine size [0, 1]. Default 0.4.", settings.shineSize, 0.4f);
      IndentLevel--;

      settings.aperture = Slider("Aperture", "Frame aperture [0, 1] Default 0.95.", settings.aperture, 0.95f);
      settings.curvature = Slider("Curvature", "Frame curvature [0, 1]. Default 0.5.", settings.curvature, 0.5f);
      IndentLevel++;
      IndentLevel--;

      settings.border = Toggle("Border", "Apply a monitor border.", settings.border);
      IndentLevel++;
      settings.borderColor = ColorField("Color", "Monitor border color.", settings.borderColor, LoFi.Settings.DefaultBorderColor);
      settings.borderSmooth = Slider("Smooth", "Monitor border smoothness [0, 1]. Default 0.5.", settings.borderSmooth, 0.5f);
      settings.borderNoise = Slider("Noise", "Texture of the border material [0, 1]. Default 0.2.", settings.borderNoise, 0.2f);
      settings.borderMargins = Vector2Field("Margins", "Border margins.", settings.borderMargins, Vector2.one);
      settings.borderMargins.x = Mathf.Clamp(settings.borderMargins.x, 1.0f, 2.0f);
      settings.borderMargins.y = Mathf.Clamp(settings.borderMargins.y, 1.0f, 2.0f);
      IndentLevel--;

      /////////////////////////////////////////////////
      // Color.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Color") == true)
      {
        IndentLevel++;

        settings.brightness = Slider("Brightness", "Brightness [-1.0, 1.0]. Default 0.", settings.brightness, -1.0f, 1.0f, 0.0f);
        settings.contrast = Slider("Contrast", "Contrast [0.0, 10.0]. Default 1.", settings.contrast, 0.0f, 10.0f, 1.0f);
        settings.gamma = Slider("Gamma", "Gamma [0.1, 10.0]. Default 1.", settings.gamma, 0.01f, 10.0f, 1.0f);
        settings.hue = Slider("Hue", "The color wheel [0.0, 1.0]. Default 0.", settings.hue, 0.0f, 1.0f, 0.0f);
        settings.saturation = Slider("Saturation", "Intensity of a colors [0.0, 2.0]. Default 1.", settings.saturation, 0.0f, 2.0f, 1.0f);

        IndentLevel--;
      }

      /////////////////////////////////////////////////
      // Advanced.
      /////////////////////////////////////////////////
      Separator();

      if (Foldout("Advanced") == true)
      {
        IndentLevel++;

#if !UNITY_6000_0_OR_NEWER
        settings.filterMode = (FilterMode)EnumPopup("Filter mode", "Filter mode. Default Bilinear.", settings.filterMode, FilterMode.Bilinear);
#endif
        settings.affectSceneView = Toggle("Affect the Scene View?", "Does it affect the Scene View?", settings.affectSceneView);
        settings.whenToInsert = (UnityEngine.Rendering.Universal.RenderPassEvent)EnumPopup("RenderPass event",
          "Render pass injection. Default BeforeRenderingPostProcessing.",
          settings.whenToInsert,
          UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingPostProcessing);
#if !UNITY_6000_0_OR_NEWER
        settings.enableProfiling = Toggle("Enable profiling", "Enable render pass profiling", settings.enableProfiling);
#endif

        IndentLevel--;
      }
    }

    protected override void InspectorChanged()
    {
      settings ??= GetSettings<LoFi.Settings>();

      settings.mustUpdateTexture = true;
    }

    private int RoundToNearestEven(int number) => (int)Math.Round(number / 2.0) * 2;
  }
}
