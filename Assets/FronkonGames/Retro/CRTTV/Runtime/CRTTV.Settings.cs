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

namespace FronkonGames.Retro.CRTTV
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Settings. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class CRTTV
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

#region CRT TV settings.
      /// <summary> Shadowmask strength [0.0, 1.0]. Default 1.0. </summary>
      public float shadowmaskStrength = 1.0f;

      /// <summary> Shadowmask luminosity [0.0, 2.0]. Default 1.0. </summary>
      public float shadowmaskLuminosity = 1.0f;

      /// <summary> Shadowmask cells scale [0.0, 1.0]. Default 0.5. </summary>
      public float shadowmaskScale = 0.5f;
      
      /// <summary> Shadowmask cells vertical gap hardness [0.0, 1.0]. Default 0.1. </summary>
      public float shadowmaskVerticalGapHardness = 0.1f;

      /// <summary> Shadowmask cells horizontal gap hardness [0.0, 1.0]. Default 0.8. </summary>
      public float shadowmaskHorizontalGapHardness = 0.8f;
      
      /// <summary> Shadowmask cells color channels X-axis offset. Default (0.0, -0.3, -0.6). </summary>
      public Vector3 shadowmaskColorOffset = DefaultShadowmaskColorOffset;
      
      /// <summary> Fisheye strength [-1.0, 5.0]. Default 0.3. </summary>
      public float fishEyeStrength = 0.2f;
      
      /// <summary> Fisheye zoom. </summary>
      public Vector2 fishEyeZoom = Vector2.one;

      /// <summary> Sinusoidal distortion of the vertical axis [0, 2.0]. Default 0.17. </summary>
      public float distortion = 0.17f;

      /// <summary> Distortion speed [-10, 10]. Default 1. </summary>
      public float distortionSpeed = 1.0f;

      /// <summary> Wave amplitude of the distortion [0, 10]. Default 2. </summary>
      public float distortionAmplitude = 2.0f;

      /// <summary> Vignette smoothness [0.0, 2.0]. Default 0.7. </summary>
      public float vignetteSmoothness = 0.7f;

      /// <summary> Vignette rounding [0.0, 1.0]. Default 0.5. </summary>
      public float vignetteRounding = 0.5f;

      /// <summary> Screen borders. </summary>
      public Vector2 vignetteBorders = DefaultVignetteBorders;
      
      /// <summary> Screen shine strength [0.0, 1.0]. Default 0.3. </summary>
      public float shineStrength = 0.3f;
      
      /// <summary> Screen shine color.</summary>
      public Color shineColor = DefaultShineColor;
      
      /// <summary> Screen shine position. Default (0.5, 1.0). </summary>
      public Vector2 shinePosition = DefaultShinePosition;
      
      /// <summary> Color channels offset strength [0.0, 1.0]. Default 0.25. </summary>
      public float rgbOffsetStrength = 0.25f;

      /// <summary> Color bleeding strength [0.0, 1.0]. Default 0.25. </summary>
      public float colorBleedingStrength = 0.25f;

      /// <summary> Color bleeding distance [-1.0, 1.0]. Default 0.5. </summary>
      public float colorBleedingDistance = 0.5f;
      
      /// <summary> Color curves adjustment. </summary>
      public Color colorCurves = DefaultColorCurves;

      /// <summary> Gamma color tint. </summary>
      public Vector4 gammaColor = DefaultGammaColor;
      
      /// <summary> Scanlines strength [0.0, 1.0]. Default 1.0. </summary>
      public float scanlines = 1.0f;

      /// <summary> Scanlines density [0.0, 2.0]. Default 1.25. </summary>
      public float scanlinesCount = 1.25f;

      /// <summary> Scanlines velocity [-10.0, 10.0]. Default 3.5. </summary>
      public float scanlinesVelocity = 3.5f;

      /// <summary> Signal interference strength [0.0, 1.0]. Default 0.2. </summary>
      public float interferenceStrength = 0.2f;
      
      /// <summary> Peak interference strength [0.0, 1.0]. Default 0.3. </summary>
      public float interferencePeakStrength = 0.3f;
      
      /// <summary> Peak interference y position [0.0, 1.0]. Default 0.2. </summary>
      public float interferencePeakPosition = 0.2f;
      
      /// <summary> Vertical shake strength [0.0, 1.0]. Default 1.0. </summary>
      public float shakeStrength = 1.0f;

      /// <summary> Vertical shake rate [0.0, 1.0]. Default 0.2. </summary>
      public float shakeRate = 0.2f;
      
      /// <summary> Vertical frame movement strength [0.0, 1.0]. Default 1.0. </summary>
      public float movementStrength = 1.0f;

      /// <summary> Vertical frame movement rate [0.0, 1.0]. Default 0.2. </summary>
      public float movementRate = 0.2f;
      
      /// <summary> Vertical frame movement speed [0.0, 1.0]. Default 0.4. </summary>
      public float movementSpeed = 0.4f;

      /// <summary> Signal noise [0.0, 1.0]. Default 0. </summary>
      public float grain = 0.0f;
      
      /// <summary> Static noise [0.0, 1.0]. Default 0. </summary>
      public float staticNoise = 0.0f;
      
      /// <summary> Bar effect strength [0.0, 1.0]. Default 0.1. </summary>
      public float barStrength = 0.1f;

      /// <summary> Bar height [0.0, 10.0]. Default 6. </summary>
      public float barHeight = 6.0f;
      
      /// <summary> Bar speed [-10.0, 10.0]. Default 4. </summary>
      public float barSpeed = 4.0f;

      /// <summary> Bar overflow [0.0, 4.0]. Default 1.2. </summary>
      public float barOverflow = 1.2f;
      
      /// <summary> Flicker strength [0.0, 1.0]. Default 0.1. </summary>
      public float flickerStrength = 0.1f;

      /// <summary> Flicker speed [0.0, 100.0]. Default 50. </summary>
      public float flickerSpeed = 50.0f;
      
      public static Color DefaultShineColor = new(0.5f, 0.5f, 0.5f, 0.5f);
      public static Color DefaultColorCurves = new(0.95f, 1.05f, 0.95f, 1.0f);
      public static Vector4 DefaultGammaColor = new(0.9f, 0.7f, 1.2f, 1.0f);
      public static Vector2 DefaultShinePosition = new(0.5f, 1.0f);
      public static Vector3 DefaultShadowmaskColorOffset = new(0.0f, -0.3f, -0.6f);
      public static Vector2 DefaultVignetteBorders = new(0.0f, 0.5f);
#endregion

#region Color settings.
      /// <summary> Brightness [-1.0, 1.0]. Default 0. </summary>
      public float brightness = 0.0f;

      /// <summary> Contrast [0.0, 10.0]. Default 1. </summary>
      public float contrast = 1.0f;

      /// <summary>Gamma [0.1, 10.0]. Default 1. </summary>      
      public float gamma = 1.0f;

      /// <summary> The color wheel [0.0, 1.0]. Default 0. </summary>
      public float hue = 0.0f;

      /// <summary> Intensity of a colors [0.0, 2.0]. Default 1. </summary>      
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

      /// <summary> Reset to default values. </summary>
      public void ResetDefaultValues()
      {
        intensity = 1.0f;

        shadowmaskStrength = 1.0f;
        shadowmaskLuminosity = 1.0f;
        shadowmaskColorOffset = DefaultShadowmaskColorOffset;
        shadowmaskScale = 0.5f;
        shadowmaskVerticalGapHardness = 0.1f;
        shadowmaskHorizontalGapHardness = 0.8f;
        fishEyeStrength = 0.2f;
        fishEyeZoom = Vector2.one;
        distortion = 0.17f;
        distortionSpeed = 1.0f;
        distortionAmplitude = 2.0f;
        vignetteSmoothness = 0.7f;
        vignetteRounding = 0.5f;
        vignetteBorders = DefaultVignetteBorders;
        shineStrength = 0.3f;
        shineColor = DefaultShineColor;
        shinePosition = DefaultShinePosition;
        rgbOffsetStrength = 0.25f;
        colorBleedingStrength = 0.25f;
        colorBleedingDistance = 0.5f;
        scanlines = 1.0f;
        scanlinesCount = 1.25f;
        scanlinesVelocity = 3.5f;
        colorCurves = DefaultColorCurves;
        gammaColor = DefaultGammaColor;
        interferenceStrength = 0.2f;
        interferencePeakStrength = 0.3f;
        interferencePeakPosition = 0.2f;
        shakeStrength = 1.0f;
        shakeRate = 0.2f;
        movementStrength = 1.0f;
        movementRate = 0.2f;
        movementSpeed = 0.4f;
        grain = 0.0f;
        staticNoise = 0.0f;
        barStrength = 0.1f;
        barHeight = 6.0f;
        barSpeed = 4.0f;
        barOverflow = 1.2f;
        flickerStrength = 0.1f;
        flickerSpeed = 50.0f;

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
