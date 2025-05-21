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
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.LoFi
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class LoFi
  {
    /// <summary> Settings. </summary>
    [Serializable]
    public sealed class Settings
    {
      public Settings() => ResetDefaultValues();

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Common settings.

      /// <summary> Controls the intensity of the effect [0, 1]. Default 1. </summary>
      /// <remarks> An effect with Intensity equal to 0 will not be executed. </remarks>
      public float intensity = 1.0f;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region LoFi settings.

      /// <summary> Enables color palette conversion for authentic retro color schemes. </summary>
      public bool palette = true;

      /// <summary> Lo-Fi color palette profile containing the color set and configuration. </summary>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to regenerate the palette texture. </remarks>
      public LoFiProfile profile;

      /// <summary> Determines how colors are interpolated in the palette - from sharp transitions to smooth gradients. Default BlendModes.Blend. </summary>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      public BlendModes mode = BlendModes.Blend;

      /// <summary> Method used to sample color from the palette. </summary>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      public SampleMethod sampleMethod = SampleMethod.Distance;

      /// <summary>
      /// Resolution of the internal palette texture. Higher resolutions reduce color banding but use more memory.
      /// Default PaletteResolutions._512.
      /// </summary>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      public PaletteResolutions resolution = PaletteResolutions._32;

      /// <summary> Maximum distance between colors [0, 1]. Small values result in colors more similar to the palette. Default 0.5. </summary>
      /// <remarks> Only in sampleMethod = SampleMethod.Similarity or sampleMethod = SampleMethod.Distance. </remarks>
      public float colorThreshold = 0.5f;

      /// <summary> Controls contrast between areas of different brightness [0, 2]. Higher values create more pronounced color shifts. Default 1.5. </summary>
      /// <remarks> Only in sampleMethod = SampleMethod.Luminance. </remarks>
      public float luminancePow = 1.5f;

      /// <summary> Minimum luminance value for color remapping [0, 1]. Controls the darkest parts of the image. Default 0. </summary>
      /// <remarks> Must always be less than rangeMax. Adjusting this can eliminate crushed blacks. </remarks>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      /// <remarks> Only in sampleMethod = SampleMethod.LuminanceBased. </remarks>
      public float rangeMin = 0.0f;

      /// <summary> Maximum luminance value for color remapping [0, 1]. Controls the brightest parts of the image. Default 1. </summary>
      /// <remarks> Must always be greater than rangeMin. Adjusting this can prevent blown-out highlights. </remarks>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      /// <remarks> Only in sampleMethod = SampleMethod.LuminanceBased. </remarks>
      public float rangeMax = 1.0f;

      /// <summary> Inverts the color palette order for negative/film-like effects. Default false. </summary>
      /// <remarks> When updating this variable, set mustUpdateTexture to true to update the texture. </remarks>
      /// <remarks> Only in sampleMethod = SampleMethod.LuminanceBased. </remarks>
      public bool invert;

      /// <summary> Enables pixelation for that classic low-resolution look. </summary>
      public bool pixelate = true;

      /// <summary> Controls the size of each pixel block [2 and up]. Larger values create a more chunky, retro appearance. Even values only. Default 12. </summary>
      public int pixelSize = 12;

      /// <summary> Applies edge detection with Sobel filter [0, 2]. Creates a pseudo-3D effect on pixel boundaries. Default 0. </summary>
      public float pixelSobel = 0.0f;

      /// <summary> Controls the intensity of the Sobel edge effect [0.01, 10]. Higher values create more pronounced edges. Default 1. </summary>
      public float pixelSobelPower = 1.0f;

      /// <summary> Direction of the light source for Sobel effect [0, 360]. Affects shadow direction. Default 45. </summary>
      public float pixelSobelAngle = 45.0f;

      /// <summary> Brightness of the light source for Sobel effect [0, 2]. Controls contrast between lit and shadowed areas. Default 1. </summary>
      public float pixelSobelLightIntensity = 1.0f;

      /// <summary> Base lighting level for Sobel effect [0, 0.5]. Prevents shadows from becoming too dark. Default 0.2. </summary>
      public float pixelSobelAmbient = 0.2f;

      /// <summary> Controls pixel corner rounding [0, 1]. At 0, pixels are square; at 1, pixels become circular. Default 0. </summary>
      public float pixelRound = 0.0f;

      /// <summary> Number of samples per pixel for anti-aliasing [1, 8]. Higher values reduce jagged edges but impact performance. Default 2. </summary>
      /// <remarks> Value of 1 disables sampling (fastest but most aliased). </remarks>
      public int pixelSamples = 2;

      /// <summary> Blending method between pixelated and original image. Affects how colors are combined. Default ColorBlends.Solid. </summary>
      public ColorBlends pixelBlend = ColorBlends.Solid;

      /// <summary> Color tint applied to pixels. Useful for creating monochrome or duotone effects. Default white (no tinting). </summary>
      public Color pixelTint = Color.white;

      /// <summary> Creates a 3D-like bevel effect on pixel edges [0, 1]. Adds depth to the pixelated image. Default 0. </summary>
      /// <remarks> Only visible when pixelRound > 0. Higher values create more pronounced bevels. </remarks>
      public float pixelBevel = 0.0f;

      /// <summary> Enables color quantization to reduce the color depth. Creates that limited color palette look. </summary>
      public bool quantization = false;

      /// <summary> Maximum number of colors in the quantized image [2, 64]. Lower values create more pronounced banding. Default 32. </summary>
      /// <remarks> Only applied when quantization is enabled. Common retro values: 8, 16, 32. </remarks>
      public int colors = 32;

      /// <summary> Darkens the screen edges to simulate CRT monitor light falloff [0, 1]. Higher values create more pronounced darkening. Default 0.3. </summary>
      public float vignette = 0.3f;

      /// <summary> Horizontal scanline intensity [0, 1]. Creates those classic CRT monitor lines. Default 0. </summary>
      public float scanline = 0.0f;

      /// <summary> Number of scanlines across the screen [0, ...]. Higher values create finer lines. Default 500. </summary>
      public int scanlineCount = 500;

      /// <summary> Animation speed of scanlines [-25, 25]. Negative values move upward, positive move downward. Default 10. </summary>
      public float scanlineSpeed = 10.0f;

      /// <summary> Color fringing effect [0, 2]. Simulates lens imperfections or poor composite video. Default 0.5. </summary>
      public float chromaticAberration = 0.5f;

      /// <summary> Adds a reflective highlight to simulate glass screen [0, 1]. Creates that authentic CRT screen glare. Default 0.1. </summary>
      public float shine = 0.1f;

      /// <summary> Controls the size of the glass reflection effect [0, 1]. Larger values create a more diffuse reflection. Default 0.4. </summary>
      public float shineSize = 0.4f;

      /// <summary> Controls screen aperture/mask [0, 1]. Lower values create a more pronounced frame around the image. Default 0.95. </summary>
      public float aperture = 0.95f;

      /// <summary> Screen curvature amount [0, ...]. Simulates the curved surface of CRT displays. Default 0.5. </summary>
      public float curvature = 0.5f;

      /// <summary> Enables a decorative border around the screen to simulate a monitor bezel. </summary>
      public bool border;

      /// <summary> Color of the monitor border/bezel. Default is a vintage beige color. </summary>
      public Color borderColor = DefaultBorderColor;

      /// <summary> Controls the softness of the border edges [0, 1]. Higher values create a more gradual transition. Default 0.5. </summary>
      public float borderSmooth = 0.5f;

      /// <summary> Texture of the border material [0, 1]. Default 0.2. </summary>
      public float borderNoise = 0.2f;

      /// <summary> Border margins. </summary>
      public Vector2 borderMargins = Vector2.one;

      [NonSerialized]
      /// <summary> Flag to indicate palette texture needs regeneration. Set to true after changing palette-related settings. </summary>
      public bool mustUpdateTexture = false;

      public static Color DefaultBorderColor = new(0.30f, 0.25f, 0.15f);

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
      #region Color settings.

      /// <summary> Brightness [-1, 1]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0, 10]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary> Gamma [0.1, 10]. Default 1. </summary>
      public float gamma = 1.0f;

      /// <summary> The color wheel [0, 1]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0, 2]. Default 1. </summary>
      public float saturation = 1.0f;

      #endregion
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
      /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        palette = true;
        mode = BlendModes.Blend;
        sampleMethod = SampleMethod.Distance;
        resolution = PaletteResolutions._32;
        colorThreshold = 0.5f;
        luminancePow = 1.5f;
        rangeMin = 0.0f;
        rangeMax = 1.0f;
        invert = false;

        pixelate = true;
        pixelSamples = 2;
        pixelSize = 12;
        pixelRound = 0.0f;
        pixelBlend = ColorBlends.Solid;
        pixelTint = Color.white;
        pixelBevel = 0.0f;
        pixelSobel = 0.0f;
        pixelSobelPower = 1.0f;
        pixelSobelAngle = 45.0f;
        pixelSobelLightIntensity = 1.0f;
        pixelSobelAmbient = 0.2f;

        scanline = 1.0f;
        scanlineCount = 500;
        scanlineSpeed = 10.0f;

        quantization = false;
        colors = 32;
        vignette = 0.3f;

        chromaticAberration = 0.5f;

        shine = 0.1f;
        shineSize = 0.4f;

        aperture = 0.95f;
        curvature = 0.5f;

        border = true;
        borderColor = DefaultBorderColor;
        borderSmooth = 0.5f;
        borderNoise = 0.2f;
        borderMargins = Vector2.one;

        mustUpdateTexture = true;

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
    }
  }
}
