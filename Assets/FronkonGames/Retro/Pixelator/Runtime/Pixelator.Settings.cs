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
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace FronkonGames.Retro.Pixelator
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Pixelator
  {
    /// <summary> Settings. </summary>
    [System.Serializable]
    public sealed class Settings
    {
      #region Common settings.
      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;
      #endregion

      #region Pixelator settings.

      /// <summary> The mode of the pixelation. Default Quad. </summary>
      public PixelationModes pixelationMode = PixelationModes.Rectangle;

      /// <summary> The size of the pixels [0, 1]. Default 0.75. </summary>
      public float pixelSize = 0.75f;

      /// <summary> Use the screen aspect ratio to calculate the pixel scale. Default true. </summary>
      public bool screenAspectRatio = true;

      /// <summary> Custom aspect ratio [0.2, 5.0]. Default 1. </summary>
      /// <remarks> Only used if screenAspectRatio is false. </remarks>
      public float aspectRatio = 1.0f;

      /// <summary> The scale of the pixels. Default (1, 1). </summary>
      public Vector2 pixelScale = Vector2.one;

      /// <summary> The radius of the circle [0, 1]. Default 0.5. </summary>
      /// <remarks> Only used if pixelationMode is Circle. </remarks>
      public float radius = 0.5f;

      /// <summary> The background color. Default Black. </summary>
      public Color background = Color.black;

      /// <summary> The number of threads [1, 8]. Default 3. </summary>
      /// <remarks> Only used if pixelationMode is Knitted. </remarks>
      public int threads = 3;

      /// <summary> The chromatic aberration intensity [0, 10]. Default 1. </summary>
      /// <remarks> Only used if chromaticAberration is true. </remarks>
      public float chromaticAberrationIntensity = 1.0f;

      /// <summary> The chromatic aberration offset. Default (1.0, 2.0, -1.0). </summary>
      /// <remarks> Only used if chromaticAberration is true. </remarks>
      public Vector3 chromaticAberrationOffset = DefaultChromaticAberrationOffset;

      /// <summary> The gradient intensity [0, 1]. Default 0. </summary>
      public float gradientIntensity = 0.0f;

      /// <summary> Color gradient. </summary>
      /// <remarks> Used in ColorModes.Gradient color mode. </remarks>
      public Gradient gradient = DefaultGradient;

      /// <summary> Minimum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 0. </summary>
      /// <remarks> Should be less than LuminanceMax. Used in ColorModes.Gradient color mode. </remarks>
      public float luminanceMin = 0.0f;

      /// <summary> Maximum luminance value that is taken into account in the color mode Gradient [0, 1]. Default 1. </summary>
      /// <remarks> It must be greater than LuminanceMin. Used in ColorModes.Gradient color mode. </remarks>
      public float luminanceMax = 1.0f;

      /// <summary> How the gradient is mapped. Default CIELAB. </summary>
      public GradientMappingMode gradientMappingMode = GradientMappingMode.CIELAB;

      /// <summary> Number of samples along the gradient to check when using CIELAB mapping mode [2, 64]. Default 16. </summary>
      /// <remarks> Higher values are more accurate but slower. </remarks>
      public int gradientCIELabSamples = 16;

      /// <summary> Apply original luminance to the final color. </summary>
      public bool gradientApplyLuminance = true;

      /// <summary> Strength of the bevel effect [0, 10]. Default 1. </summary>
      public float bevel = 1.0f;

      /// <summary> Dither intensity [0, 1]. Default 0.5. </summary>
      public float ditherIntensity = 0.5f;

      /// <summary> Scale of the dither pattern (2 for 2x2, 4 for 4x4, 8 for 8x8). Default 4. </summary>
      public int ditherPatternScale = 4;

      /// <summary> Threshold scale for dithering [0, 1]. Adjusts dither pattern influence. Default 0.75. </summary>
      public float ditherThresholdScale = 0.75f;

      /// <summary> Number of color steps for dithering [2, 16]. Default 8. </summary>
      public int ditherColorSteps = 8;

      /// <summary> Overall intensity of the posterize effect [0, 1]. Default 0.0. An intensity of 0 effectively disables posterization. </summary>
      public float posterizeIntensity = 0.5f;

      /// <summary> Number of color steps per channel for RGB mode [2, 256]. Default (8,8,8). </summary>
      public Vector3Int posterizeRGBSteps = DefaultPosterizeStepsRGB;

      /// <summary> Number of color steps for Luminance mode [2, 256]. Default 24. </summary>
      public int posterizeLuminanceSteps = 24;

      /// <summary> Number of color steps per channel for HSV mode [H:2-64, S:2-32, V:2-32]. Default (8,8,8). </summary>
      public Vector3Int posterizeHSVSteps = DefaultPosterizeStepsHSV;

      /// <summary> Gamma value for posterization. Default 1.0 (no correction). </summary>
      public float posterizeGamma = 1.0f;

      // Filters settings
      /// <summary> Global intensity for color filters [0, 1]. Default 0.0 (off). </summary>
      public float filtersIntensity = 0.0f;

      /// <summary> Intensity for Sepia filter [0, 1]. Default 0.0. </summary>
      public float sepiaIntensity = 0.0f;

      /// <summary> Intensity for Cool Blue filter [0, 1]. Default 0.0. </summary>
      public float coolBlueIntensity = 0.0f;

      /// <summary> Intensity for Warm filter [0, 1]. Default 0.0. </summary>
      public float warmFilterIntensity = 0.0f;

      /// <summary> Intensity for Invert Color filter [0, 1]. Default 0.0. </summary>
      public float invertColorIntensity = 0.0f;

      /// <summary> Intensity for Hudson filter [0, 1]. Default 0.0. </summary>
      public float hudsonIntensity = 0.0f;

      /// <summary> Intensity for Hefe filter [0, 1]. Default 0.0. </summary>
      public float hefeIntensity = 0.0f;

      /// <summary> Intensity for X-Pro filter [0, 1]. Default 0.0. </summary>
      public float xproIntensity = 0.0f;

      /// <summary> Intensity for Rise filter [0, 1]. Default 0.0. </summary>
      public float riseIntensity = 0.0f;

      /// <summary> Intensity for Toaster filter [0, 1]. Default 0.0. </summary>
      public float toasterIntensity = 0.0f;

      /// <summary> Intensity for Infrared (IR) filter [0, 1]. Default 0.0. </summary>
      public float irFilterIntensity = 0.0f;

      /// <summary> Intensity for Thermal filter [0, 1]. Default 0.0. </summary>
      public float thermalFilterIntensity = 0.0f;

      /// <summary> Intensity for Duotone filter [0, 1]. Default 0.0. </summary>
      public float duotoneIntensity = 0.0f;

      /// <summary> First color for Duotone filter. Default Dark Blue. </summary>
      public Color duotoneColorA = DefaultDuotoneColorA;

      /// <summary> Second color for Duotone filter. Default Bright Yellow. </summary>
      public Color duotoneColorB = DefaultDuotoneColorB;

      /// <summary> Intensity for Night Vision filter [0, 1]. Default 0.0. </summary>
      public float nightVisionIntensity = 0.0f;

      /// <summary> Intensity for Pop Art filter [0, 1]. Default 0.0. </summary>
      public float popArtIntensity = 0.0f;

      /// <summary> Intensity for Blueprint filter [0, 1]. Default 0.0. </summary>
      public float blueprintIntensity = 0.0f;

      /// <summary> Edge color for Blueprint filter. Default Light Blue. </summary>
      public Color blueprintEdgeColor = DefaultBlueprintEdgeColor;

      /// <summary> Background color for Blueprint filter. Default Dark Blue. </summary>
      public Color blueprintBackgroundColor = DefaultBlueprintBackgroundColor;

      /// <summary> Edge detection threshold for Blueprint filter [0.05, 0.5]. Default 0.1. </summary>
      public float blueprintEdgeThreshold = 0.1f;

      public static Vector3 DefaultChromaticAberrationOffset = new(1.0f, 2.0f, -1.0f);

      public static readonly Gradient DefaultGradient = new()
      {
        colorKeys = new GradientColorKey[]
        {
          new(Color.white * 0.0f, 0.0f),
          new(Color.white * 0.33f, 0.2f),
          new(Color.white * 0.66f, 0.5f),
          new(Color.white * 1.0f, 1.0f)
        }
      };

      public static readonly Vector3Int DefaultPosterizeStepsRGB = new(24, 24, 24);
      public static readonly Vector3Int DefaultPosterizeStepsHSV = new(24, 24, 24);

      public static readonly Color DefaultDuotoneColorA = new(0.1f, 0.2f, 0.5f, 1.0f); // Dark Blue
      public static readonly Color DefaultDuotoneColorB = new(0.9f, 0.9f, 0.2f, 1.0f); // Bright Yellow

      public static readonly Color DefaultBlueprintEdgeColor = new(0.6f, 0.8f, 1.0f, 1.0f);
      public static readonly Color DefaultBlueprintBackgroundColor = new(0.1f, 0.2f, 0.4f, 1.0f);

      #endregion

      #region Color settings.
      /// <summary> Brightness [-1, 1]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0, 10]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary>Gamma [0.1, 10]. Default 1. </summary>      
      public float gamma = 1.0f;

      /// <summary> The color wheel [0, 1]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>      
      public float saturation = 1.0f;
      #endregion      

      #region Advanced settings.
      /// <summary> Does it affect the Scene View? </summary>
      public bool affectSceneView = false;

#if !UNITY_6000_0_OR_NEWER
      /// <summary> Enable render pass profiling. </summary>
      public bool enableProfiling = false;

      /// <summary> Filter mode. Default Bilinear. </summary>
      public FilterMode filterMode = FilterMode.Bilinear;
#endif

      /// <summary> Render pass injection. Default BeforeRenderingPostProcessing. </summary>
      public RenderPassEvent whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      #endregion

      // Internal use.
      public bool forceGradientTextureUpdate;

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        pixelationMode = PixelationModes.Rectangle;
        pixelSize = 0.75f;
        screenAspectRatio = true;
        aspectRatio = 1.0f;
        pixelScale = Vector2.one;
        radius = 0.5f;
        threads = 3;
        background = Color.black;
        chromaticAberrationIntensity = 1.0f;
        chromaticAberrationOffset = DefaultChromaticAberrationOffset;
        gradientIntensity = 0.0f;
        gradient = DefaultGradient;
        gradientApplyLuminance = true;
        luminanceMin = 0.0f;
        luminanceMax = 1.0f;
        gradientMappingMode = GradientMappingMode.CIELAB;
        gradientCIELabSamples = 16;
        bevel = 1.0f;

        ditherIntensity = 0.5f;
        ditherPatternScale = 4;
        ditherThresholdScale = 0.75f;
        ditherColorSteps = 8;

        posterizeIntensity = 0.5f;
        posterizeRGBSteps = DefaultPosterizeStepsRGB;
        posterizeLuminanceSteps = 24;
        posterizeHSVSteps = DefaultPosterizeStepsHSV;
        posterizeGamma = 1.0f;

        filtersIntensity = 0.0f;
        sepiaIntensity = 0.0f;
        coolBlueIntensity = 0.0f;
        warmFilterIntensity = 0.0f;
        invertColorIntensity = 0.0f;
        hudsonIntensity = 0.0f;
        hefeIntensity = 0.0f;
        xproIntensity = 0.0f;
        riseIntensity = 0.0f;
        toasterIntensity = 0.0f;
        irFilterIntensity = 0.0f;
        thermalFilterIntensity = 0.0f;
        duotoneIntensity = 0.0f;
        duotoneColorA = DefaultDuotoneColorA;
        duotoneColorB = DefaultDuotoneColorB;
        nightVisionIntensity = 0.0f;
        popArtIntensity = 0.0f;
        blueprintIntensity = 0.0f;
        blueprintEdgeColor = DefaultBlueprintEdgeColor;
        blueprintBackgroundColor = DefaultBlueprintBackgroundColor;
        blueprintEdgeThreshold = 0.1f;

        brightness = 0.0f;
        contrast = 1.0f;
        gamma = 1.0f;
        hue = 0.0f;
        saturation = 1.0f;

        affectSceneView = false;
#if !UNITY_6000_0_OR_NEWER
        enableProfiling = false;
        filterMode = FilterMode.Bilinear;
#endif
        whenToInsert = RenderPassEvent.BeforeRenderingPostProcessing;
      }

      public Texture gradientTexture = null;
      public float gradientLuminanceMin = 0.0f;
      public float gradientLuminanceMax = 1.0f;

      // Filters.
      public bool filtersEnable = false;

      private void ResetGradient()
      {
        gradientTexture = null;
        gradientIntensity = 1.0f;
        gradientLuminanceMin = 0.0f;
        gradientLuminanceMax = 1.0f;
        gradientMappingMode = GradientMappingMode.Luminance;
        gradientCIELabSamples = 16;
      }
    }    
  }
}
