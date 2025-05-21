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
using static FronkonGames.Retro.Pixelator.Inspector;

namespace FronkonGames.Retro.Pixelator
{
  /// <summary> Retro Pixelator inspector. </summary>
  [CustomPropertyDrawer(typeof(Pixelator.Settings))]
  public class NoirFeatureSettingsDrawer : Drawer
  {
    private Pixelator.Settings settings;

    public bool filtersFoldout = false;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    private void OnEnable()
    {
      settings = GetSettings<Pixelator.Settings>();

      filtersFoldout = EditorPrefs.GetBool($"{Constants.Asset.AssemblyName}.FiltersFoldout", false);
    }

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<Pixelator.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // Pixelator.
      /////////////////////////////////////////////////
      Separator();

      settings.pixelationMode = (Pixelator.PixelationModes)EnumPopup("Pixelator", "The mode of the pixelation. Default Quad.", settings.pixelationMode, Pixelator.PixelationModes.Rectangle);
      IndentLevel++;
      settings.pixelSize = Slider("Size", "The size of the pixels [0, 1]. Default 0.75.", settings.pixelSize, 0.0f, 1.0f, 0.75f);

      switch (settings.pixelationMode)
      {
        case Pixelator.PixelationModes.Rectangle:
        case Pixelator.PixelationModes.Triangle:
        case Pixelator.PixelationModes.Hexagon:
        case Pixelator.PixelationModes.Leaf:
          settings.pixelScale = Vector2Field("Scale", "The scale of the pixels [0.2, 5.0]. Default (1, 1).", settings.pixelScale, Vector2.one);
          settings.screenAspectRatio = Toggle("Aspect ratio", "Use the screen aspect ratio to calculate the pixel scale. Default true.", settings.screenAspectRatio);
          IndentLevel++;
          GUI.enabled = settings.screenAspectRatio == false;
          settings.aspectRatio = Slider("Custom", "The a custom aspect ratio [0.2, 5.0]. Default 1.", settings.aspectRatio, 0.2f, 5.0f, 1.0f);
          GUI.enabled = true;
          IndentLevel--;
          break;
        case Pixelator.PixelationModes.Circle:
        case Pixelator.PixelationModes.Led:
          settings.radius = Slider("Radius", "The radius of the circle [0, 1]. Default 0.5.", settings.radius, 0.0f, 1.0f, 0.5f);
          settings.background = ColorField("Background", "The background color. Default Black.", settings.background, Color.black);
          break;
        case Pixelator.PixelationModes.Knitted:
          settings.threads = Slider("Threads", "The number of threads [1, 8]. Default 3.", settings.threads, 1, 8, 3);
          settings.pixelScale = Vector2Field("Scale", "The scale of the pixels [0.2, 5.0]. Default (1, 1).", settings.pixelScale, Vector2.one);
          break;
        case Pixelator.PixelationModes.Diamond:
          break;
      }

      IndentLevel--;

      settings.gradientIntensity = Slider("Gradient", "The intensity of the gradient [0, 1]. Default 0.", settings.gradientIntensity, 0.0f, 1.0f, 0.0f);
      IndentLevel++;
      BeginHorizontal();
      {
        settings.gradient = EditorGUILayout.GradientField(new GUIContent("Gradient", "Color gradient mode"), settings.gradient);

        if (GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), EditorStyles.miniLabel, GUILayout.Width(20.0f), GUILayout.Height(20.0f)) == true)
          PaletteTool.ShowTool();

        if (ResetButton(Pixelator.Settings.DefaultGradient) == true)
          settings.gradient = new Gradient() { colorKeys = Pixelator.Settings.DefaultGradient.colorKeys };
      }
      EndHorizontal();

      settings.gradientMappingMode = (Pixelator.GradientMappingMode)EnumPopup("Mapping Mode", "How the gradient is mapped. Luminance uses brightness, CIELAB uses color similarity.",
          settings.gradientMappingMode, Pixelator.GradientMappingMode.CIELAB);

      IndentLevel++;
      if (settings.gradientMappingMode == Pixelator.GradientMappingMode.Luminance)
      {
        BeginHorizontal();
        {
          float luminanceMin = settings.luminanceMin;
          float luminanceMax = settings.luminanceMax;

          EditorGUILayout.MinMaxSlider("Luminance", ref luminanceMin, ref luminanceMax, 0.0f, 1.0f);

          settings.luminanceMin = luminanceMin;
          settings.luminanceMax = luminanceMax;

          if (ResetButton() == true)
          {
            settings.luminanceMin = 0.0f;
            settings.luminanceMax = 1.0f;
          }
        }
        EndHorizontal();
      }
      else
      {
        settings.gradientCIELabSamples = Slider("Samples", "Number of samples along the gradient for CIELAB mode. Higher is more accurate but slower.",
            settings.gradientCIELabSamples, 2, 64, 16);
      }
      IndentLevel--;
      settings.gradientApplyLuminance = Toggle("Apply Luminance", "Apply original luminance to the final color.", settings.gradientApplyLuminance);
      IndentLevel--;

      settings.ditherIntensity = Slider("Dither", "The intensity of the dither effect [0, 1]. Default 0.5.", settings.ditherIntensity, 0.0f, 1.0f, 0.5f);
      IndentLevel++;
      settings.ditherPatternScale = IntPopup("Bayer Pattern", settings.ditherPatternScale, new string[] { "2x2", "4x4", "8x8" }, new int[] { 2, 4, 8 }, 4);
      settings.ditherThresholdScale = Slider("Threshold Scale", "Adjusts dither pattern influence [0, 1]. Default 0.75.", settings.ditherThresholdScale, 0.0f, 1.0f, 0.75f);
      settings.ditherColorSteps = Slider("Color Steps", "Number of color steps for dithering [2, 16]. Default 8.", settings.ditherColorSteps, 2, 16, 8);
      IndentLevel--;

      settings.posterizeIntensity = Slider("Posterize", "Overall intensity of the posterize effect [0, 1]. 0 means disabled.", settings.posterizeIntensity, 0.0f, 1.0f, 0.5f);
      IndentLevel++;
      settings.posterizeLuminanceSteps = Slider("Luminance", "Number of steps for the luminance channel [2, 256]. Default 24.", settings.posterizeLuminanceSteps, 2, 256, 24);
      settings.posterizeGamma = Slider("Gamma", "Gamma value for posterization [0.1, 3.0]. Default 1.0 (no correction).", settings.posterizeGamma, 0.1f, 3.0f, 1.0f);

      settings.posterizeRGBSteps = Vector3IntField("RGB", "Number of steps for the RGB channels [2, 256]. Default (24, 24, 24).", settings.posterizeRGBSteps, Pixelator.Settings.DefaultPosterizeStepsRGB);
      settings.posterizeRGBSteps.x = Mathf.Clamp(settings.posterizeRGBSteps.x, 2, 256);
      settings.posterizeRGBSteps.y = Mathf.Clamp(settings.posterizeRGBSteps.y, 2, 256);
      settings.posterizeRGBSteps.z = Mathf.Clamp(settings.posterizeRGBSteps.z, 2, 256);
      
      settings.posterizeHSVSteps = Vector3IntField("HSV", "Number of steps for the HSV channels [2, 64]. Default (24, 24, 24).", settings.posterizeHSVSteps, Pixelator.Settings.DefaultPosterizeStepsHSV);
      settings.posterizeHSVSteps.x = Mathf.Clamp(settings.posterizeHSVSteps.x, 2, 64);
      settings.posterizeHSVSteps.y = Mathf.Clamp(settings.posterizeHSVSteps.y, 2, 64);
      settings.posterizeHSVSteps.z = Mathf.Clamp(settings.posterizeHSVSteps.z, 2, 64);
      IndentLevel--;

      settings.bevel = Slider("Bevel", "Strength of the bevel effect [0, 10]. Default 1.", settings.bevel, 0.0f, 10.0f, 1.0f);

      settings.chromaticAberrationIntensity = Slider("Chromatic aberration", "The intensity of the chromatic aberration [0, 10]. Default 1.", settings.chromaticAberrationIntensity, 0.0f, 10.0f, 1.0f);
      IndentLevel++;
      settings.chromaticAberrationOffset = Vector3Field("Offset", "The offset of the chromatic aberration. Default (1.0, 2.0, -1.0).", settings.chromaticAberrationOffset, Pixelator.Settings.DefaultChromaticAberrationOffset);
      IndentLevel--;

      filtersFoldout = EditorGUILayout.Foldout(filtersFoldout, "Filters");
      if (filtersFoldout == true)
      {
        IndentLevel++;
        settings.filtersIntensity = Slider("Global", "Global intensity for all color filters [0, 1]. Default 0.0 (off).", settings.filtersIntensity, 0.0f, 1.0f, 0.0f);
        Separator();
        settings.sepiaIntensity = Slider("Sepia", "Intensity for Sepia filter [0, 1]. Default 0.0.", settings.sepiaIntensity, 0.0f, 1.0f, 0.0f);
        settings.coolBlueIntensity = Slider("Cool Blue", "Intensity for Cool Blue filter [0, 1]. Default 0.0.", settings.coolBlueIntensity, 0.0f, 1.0f, 0.0f);
        settings.warmFilterIntensity = Slider("Warm", "Intensity for Warm filter [0, 1]. Default 0.0.", settings.warmFilterIntensity, 0.0f, 1.0f, 0.0f);
        Separator();
        settings.hudsonIntensity = Slider("Hudson", "Intensity for Hudson filter [0, 1]. Default 0.0.", settings.hudsonIntensity, 0.0f, 1.0f, 0.0f);
        settings.hefeIntensity = Slider("Hefe", "Intensity for Hefe filter [0, 1]. Default 0.0.", settings.hefeIntensity, 0.0f, 1.0f, 0.0f);
        settings.xproIntensity = Slider("X-Pro", "Intensity for X-Pro filter [0, 1]. Default 0.0.", settings.xproIntensity, 0.0f, 1.0f, 0.0f);
        settings.riseIntensity = Slider("Rise", "Intensity for Rise filter [0, 1]. Default 0.0.", settings.riseIntensity, 0.0f, 1.0f, 0.0f);
        settings.toasterIntensity = Slider("Toaster", "Intensity for Toaster filter [0, 1]. Default 0.0.", settings.toasterIntensity, 0.0f, 1.0f, 0.0f);
        settings.duotoneIntensity = Slider("Duotone", "Intensity for Duotone filter [0, 1]. Default 0.0.", settings.duotoneIntensity, 0.0f, 1.0f, 0.0f);
        IndentLevel++;
        settings.duotoneColorA = ColorField("Color A", "First color for Duotone. Default Dark Blue.", settings.duotoneColorA, Pixelator.Settings.DefaultDuotoneColorA);
        settings.duotoneColorB = ColorField("Color B", "Second color for Duotone. Default Bright Yellow.", settings.duotoneColorB, Pixelator.Settings.DefaultDuotoneColorB);
        IndentLevel--;
        Separator();
        settings.thermalFilterIntensity = Slider("Thermal", "Intensity for Thermal filter [0, 1]. Default 0.0.", settings.thermalFilterIntensity, 0.0f, 1.0f, 0.0f);
        settings.nightVisionIntensity = Slider("Night Vision", "Intensity for Night Vision filter [0, 1]. Default 0.0.", settings.nightVisionIntensity, 0.0f, 1.0f, 0.0f);
        settings.irFilterIntensity = Slider("Infrared", "Intensity for Infrared (IR) filter [0, 1]. Default 0.0.", settings.irFilterIntensity, 0.0f, 1.0f, 0.0f);
        settings.invertColorIntensity = Slider("Invert Color", "Intensity for Invert Color filter [0, 1]. Default 0.0.", settings.invertColorIntensity, 0.0f, 1.0f, 0.0f);
        settings.popArtIntensity = Slider("Pop Art", "Intensity for Pop Art filter [0, 1]. Default 0.0.", settings.popArtIntensity, 0.0f, 1.0f, 0.0f);
        settings.blueprintIntensity = Slider("Blueprint", "Intensity for Blueprint filter [0, 1]. Default 0.0.", settings.blueprintIntensity, 0.0f, 1.0f, 0.0f);
        IndentLevel++;
        settings.blueprintEdgeColor = ColorField("Edge Color", "Edge color for Blueprint. Default Light Blue.", settings.blueprintEdgeColor, Pixelator.Settings.DefaultBlueprintEdgeColor);
        settings.blueprintBackgroundColor = ColorField("Background Color", "Background color for Blueprint. Default Dark Blue.", settings.blueprintBackgroundColor, Pixelator.Settings.DefaultBlueprintBackgroundColor);
        settings.blueprintEdgeThreshold = Slider("Edge Threshold", "Edge detection threshold [0.05, 0.5]. Default 0.1.", settings.blueprintEdgeThreshold, 0.05f, 0.5f, 0.1f);
        IndentLevel--;
        IndentLevel--;
      }

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
      settings ??= GetSettings<Pixelator.Settings>();

      settings.forceGradientTextureUpdate = true;

      EditorPrefs.SetBool($"{Constants.Asset.AssemblyName}.FiltersFoldout", filtersFoldout);
    }    
  }
}
