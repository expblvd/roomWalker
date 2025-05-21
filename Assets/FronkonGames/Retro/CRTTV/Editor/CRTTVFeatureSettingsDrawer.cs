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
  /// <summary> Retro CRT TV inspector. </summary>
  [CustomPropertyDrawer(typeof(CRTTV.Settings))]
  public class CRTTVFeatureSettingsDrawer : Drawer
  {
    private CRTTV.Settings settings;

    protected override void ResetValues() => settings?.ResetDefaultValues();

    protected override void InspectorGUI()
    {
      settings ??= GetSettings<CRTTV.Settings>();

      /////////////////////////////////////////////////
      // Common.
      /////////////////////////////////////////////////
      settings.intensity = Slider("Intensity", "Controls the intensity of the effect [0, 1]. Default 1.", settings.intensity, 0.0f, 1.0f, 1.0f);

      /////////////////////////////////////////////////
      // CRT TV.
      /////////////////////////////////////////////////
      Separator();

      settings.shadowmaskStrength = Slider("Shadowmask", "Shadowmask strength [0.0, 1.0]. Default 1.0.", settings.shadowmaskStrength, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.shadowmaskLuminosity = Slider("Luminosity", "Shadowmask luminosity [0.0, 2.0]. Default 1.0.", settings.shadowmaskLuminosity, 0.0f, 2.0f, 1.0f);
      settings.shadowmaskScale = Slider("Scale", "Shadowmask cells scale [0.0, 1.0]. Default 0.5.", settings.shadowmaskScale, 0.0f, 1.0f, 0.5f);
      settings.shadowmaskColorOffset = Vector3Field("Color offset", "Shadowmask cells color channels X-axis offset. Default (0.0, -0.3, -0.6).", settings.shadowmaskColorOffset, CRTTV.Settings.DefaultShadowmaskColorOffset);
      settings.shadowmaskHorizontalGapHardness = Slider("Horizontal hardness", "Shadowmask cells horizontal gap hardness [0.0, 1.0]. Default 0.8.", settings.shadowmaskHorizontalGapHardness, 0.0f, 1.0f, 0.8f);
      settings.shadowmaskVerticalGapHardness = Slider("Vertical hardness", "Shadowmask cells vertical gap hardness [0.0, 1.0]. Default 0.1.", settings.shadowmaskVerticalGapHardness, 0.0f, 1.0f, 0.1f);
      IndentLevel--;

      settings.fishEyeStrength = Slider("Fisheye", "Fisheye strength [-1.0, 5.0]. Default 0.2.", settings.fishEyeStrength, -1.0f, 5.0f, 0.2f);
      IndentLevel++;
      settings.fishEyeZoom = Vector2Field("Zoom", "Fisheye zoom.", settings.fishEyeZoom, Vector2.one);
      IndentLevel--;

      settings.distortion = Slider("Distortion", "Sinusoidal distortion of the vertical axis [0, 2.0]. Default 0.17.", settings.distortion, 0.0f, 2.0f, 0.17f);
      IndentLevel++;
      settings.distortionSpeed = Slider("Speed", "Distortion speed [-10, 10]. Default 1.", settings.distortionSpeed, -10.0f, 10.0f, 1.0f);
      settings.distortionAmplitude = Slider("Amplitude", "Wave amplitude of the distortion [0, 10]. Default 2.", settings.distortionAmplitude, 0.0f, 10.0f, 2.0f);
      IndentLevel--;

      settings.vignetteSmoothness = Slider("Vignette smoothness", "Vignette smoothness [0.0, 2.0]. Default 0.7.", settings.vignetteSmoothness, 0.0f, 2.0f, 0.7f);
      IndentLevel++;
      settings.vignetteRounding = Slider("Rounding", "Vignette rounding [0.0, 1.0]. Default 0.5.", settings.vignetteRounding, 0.0f, 1.0f, 0.5f);
      settings.vignetteBorders = Vector2Field("Borders", "Vignette borders.", settings.vignetteBorders, CRTTV.Settings.DefaultVignetteBorders);
      IndentLevel--;

      settings.shineStrength = Slider("Shine", "Screen shine strength [0.0, 1.0]. Default 0.3.", settings.shineStrength, 0.0f, 1.0f, 0.3f);
      IndentLevel++;
      settings.shineColor = ColorField("Color", "Screen shine color.", settings.shineColor, CRTTV.Settings.DefaultShineColor);
      settings.shinePosition = Vector2Field("Position", "Screen shine position. Default (0.5, 1.0).", settings.shinePosition, CRTTV.Settings.DefaultShinePosition);
      IndentLevel--;

      Vector4 gammaColor = settings.gammaColor;
      gammaColor.w = Slider("Gamma color", "Gamma color tint.", gammaColor.w, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      gammaColor.x = Slider("Red", "Red channel.", gammaColor.x, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.x);
      gammaColor.y = Slider("Green", "Green channel", gammaColor.y, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.y);
      gammaColor.z = Slider("Blue", "Blue channel", gammaColor.z, 0.0f, 2.0f, CRTTV.Settings.DefaultGammaColor.z);
      IndentLevel--;
      settings.gammaColor = gammaColor;

      settings.colorCurves = ColorField("Color curves", "Color curves adjustment.", settings.colorCurves, CRTTV.Settings.DefaultColorCurves);
      settings.rgbOffsetStrength = Slider("RGB offset", "Color channels offset strength [0.0, 1.0]. Default 0.25.", settings.rgbOffsetStrength, 0.0f, 1.0f, 0.25f);

      settings.colorBleedingStrength = Slider("Color bleeding", "Color bleeding strength [0.0, 1.0]. Default 0.25.", settings.colorBleedingStrength, 0.0f, 1.0f, 0.25f);
      IndentLevel++;
      settings.colorBleedingDistance = Slider("Distance", "Color bleeding distance [-1.0, 1.0]. Default 0.5.", settings.colorBleedingDistance, -1.0f, 1.0f, 0.5f);
      IndentLevel--;

      settings.scanlines = Slider("Scanlines", "Scanlines strength [0.0, 1.0]. Default 1.", settings.scanlines, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.scanlinesCount = Slider("Density", "Scanlines density [0.0, 2.0]. Default 1.25.", settings.scanlinesCount, 0.0f, 2.0f, 1.25f);
      settings.scanlinesVelocity = Slider("Velocity", "Scanlines velocity [-10.0, 10.0]. Default 3.5.", settings.scanlinesVelocity, -10.0f, 10.0f, 3.5f);
      IndentLevel--;

      settings.interferenceStrength = Slider("Signal interference", "Signal interference strength [0.0, 1.0]. Default 0.2.", settings.interferenceStrength, 0.0f, 1.0f, 0.2f);
      IndentLevel++;
      settings.interferencePeakStrength = Slider("Peak strength", "Peak interference strength [0.0, 1.0]. Default 0.3.", settings.interferencePeakStrength, 0.0f, 1.0f, 0.3f);
      settings.interferencePeakPosition = Slider("Peak position", "Peak interference y position [0.0, 1.0]. Default 0.2.", settings.interferencePeakPosition, 0.0f, 1.0f, 0.2f);
      IndentLevel--;

      settings.shakeStrength = Slider("Frame shaking", "Vertical shake strength [0.0, 1.0]. Default 1.0.", settings.shakeStrength, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.shakeRate = Slider("Rate", "Vertical shake rate [0.0, 1.0]. Default 0.2.", settings.shakeRate, 0.0f, 1.0f, 0.2f);
      IndentLevel--;

      settings.movementStrength = Slider("Frame movement", "Vertical frame movement strength [0.0, 1.0]. Default 1.0.", settings.movementStrength, 0.0f, 1.0f, 1.0f);
      IndentLevel++;
      settings.movementRate = Slider("Rate", "Vertical frame movement rate [0.0, 1.0]. Default 0.2.", settings.movementRate, 0.0f, 1.0f, 0.2f);
      settings.movementSpeed = Slider("Speed", "Vertical frame movement speed [0.0, 1.0]. Default 0.4.", settings.movementSpeed, 0.0f, 1.0f, 0.4f);
      IndentLevel--;

      settings.grain = Slider("Grain", "Signal noise [0.0, 1.0]. Default 0.", settings.grain, 0.0f, 1.0f, 0.0f);
      settings.staticNoise = Slider("Static noise", "Static noise [0.0, 1.0]. Default 0.", settings.staticNoise, 0.0f, 1.0f, 0.0f);

      settings.barStrength = Slider("Bar effect", "Bar effect strength [0.0, 1.0]. Default 0.1.", settings.barStrength, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.barHeight = Slider("Height", "Bar height [0.0, 10.0]. Default 6.", settings.barHeight, 0.0f, 10.0f, 6.0f);
      settings.barSpeed = Slider("Speed", "Bar speed [-10.0, 10.0]. Default 4.", settings.barSpeed, -10.0f, 10.0f, 4.0f);
      settings.barOverflow = Slider("Overflow", "Bar overflow [0.0, 4.0]. Default 1.2.", settings.barOverflow, 0.0f, 4.0f, 1.2f);
      IndentLevel--;

      settings.flickerStrength = Slider("Flicker", "Flicker strength [0.0, 1.0]. Default 0.1.", settings.flickerStrength, 0.0f, 1.0f, 0.1f);
      IndentLevel++;
      settings.flickerSpeed = Slider("Rate", "Flicker speed [0.0, 100.0]. Default 50.", settings.flickerSpeed, 0.0f, 100.0f, 50.0f);
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
  }
}
