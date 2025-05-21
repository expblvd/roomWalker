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
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#if UNITY_6000_0_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
#endif

namespace FronkonGames.Retro.LoFi
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class LoFi
  {
    [DisallowMultipleRendererFeature]
    private sealed class RenderPass : ScriptableRenderPass
    {
      // Internal use only.
      internal Material material { get; set; }

      private readonly Settings settings;

#if UNITY_6000_0_OR_NEWER
#else
      private RenderTargetIdentifier colorBuffer;
      private RenderTextureDescriptor renderTextureDescriptor;

      private readonly int renderTextureHandle0 = Shader.PropertyToID($"{Constants.Asset.AssemblyName}.RTH0");

      private const string CommandBufferName = Constants.Asset.AssemblyName;

      private ProfilingScope profilingScope;
      private readonly ProfilingSampler profilingSamples = new(Constants.Asset.AssemblyName);
#endif

      private Texture2D gradientTex;

      private static class ShaderIDs
      {
        internal static readonly int Intensity = Shader.PropertyToID("_Intensity");

        internal static readonly int ColorThreshold = Shader.PropertyToID("_ColorThreshold");
        internal static readonly int ColorSamples   = Shader.PropertyToID("_ColorSamples");
        internal static readonly int LuminancePow   = Shader.PropertyToID("_LuminancePow");
        internal static readonly int RangeMin       = Shader.PropertyToID("_RangeMin");
        internal static readonly int RangeMax       = Shader.PropertyToID("_RangeMax");

        internal static readonly int Pixelate   = Shader.PropertyToID("_Pixelate");
        internal static readonly int PixelSize  = Shader.PropertyToID("_PixelSize");
        internal static readonly int Samples    = Shader.PropertyToID("_Samples");

        internal static readonly int PixelSobel               = Shader.PropertyToID("_PixelSobel");
        internal static readonly int PixelSobelPower          = Shader.PropertyToID("_PixelSobelPower");
        internal static readonly int PixelSobelLight          = Shader.PropertyToID("_PixelSobelLight");
        internal static readonly int PixelSobelLightIntensity = Shader.PropertyToID("_PixelSobelLightIntensity");
        internal static readonly int PixelSobelAmbient        = Shader.PropertyToID("_PixelSobelAmbient");

        internal static readonly int PixelRound = Shader.PropertyToID("_PixelRound");
        internal static readonly int PixelBlend = Shader.PropertyToID("_PixelBlend");
        internal static readonly int PixelTint  = Shader.PropertyToID("_PixelTint");
        internal static readonly int Bevel      = Shader.PropertyToID("_Bevel");

        internal static readonly int Scanline = Shader.PropertyToID("_Scanline");
        internal static readonly int ScanlineCount     = Shader.PropertyToID("_ScanlineCount");
        internal static readonly int ScanlineSpeed     = Shader.PropertyToID("_ScanlineSpeed");

        internal static readonly int Dither       = Shader.PropertyToID("_Dither");
        internal static readonly int Quantization = Shader.PropertyToID("_Quantization");
        internal static readonly int Vignette     = Shader.PropertyToID("_Vignette");

        internal static readonly int ChromaticAberration = Shader.PropertyToID("_ChromaticAberration");

        internal static readonly int Shine     = Shader.PropertyToID("_Shine");
        internal static readonly int ShineSize = Shader.PropertyToID("_ShineSize");

        internal static readonly int Aperture  = Shader.PropertyToID("_Aperture");
        internal static readonly int Curvature = Shader.PropertyToID("_Curvature");

        internal static readonly int BorderColor   = Shader.PropertyToID("_BorderColor");
        internal static readonly int BorderSmooth  = Shader.PropertyToID("_BorderSmooth");
        internal static readonly int BorderNoise   = Shader.PropertyToID("_BorderNoise");
        internal static readonly int BorderMargins = Shader.PropertyToID("_BorderMargins");

        internal static readonly int Brightness = Shader.PropertyToID("_Brightness");
        internal static readonly int Contrast   = Shader.PropertyToID("_Contrast");
        internal static readonly int Gamma      = Shader.PropertyToID("_Gamma");
        internal static readonly int Hue        = Shader.PropertyToID("_Hue");
        internal static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Textures
      {
        internal static readonly int Gradient = Shader.PropertyToID("_GradientTex");
      }

      private static class Keywords
      {
        internal const string UsePalette         = "USE_PALETTE";
        internal const string UseLuminanceSample = "USE_LUMINANCE_SAMPLE";
        internal const string UseDistanceSample  = "USE_DISTANCE_SAMPLE";
        internal const string UseHSVSample       = "USE_HSV_SAMPLE";
        internal const string UseDominantSample  = "USE_DOMINANT_SAMPLE";
        internal const string UsePixelate        = "USE_PIXELATE";
        internal const string UseSobel           = "USE_SOBEL";
        internal const string UseQuantization    = "USE_QUANTIZATION";
        internal const string UseBorder          = "USE_BORDER";
      }

      /// <summary> Render pass constructor. </summary>
      public RenderPass(Settings settings) : base()
      {
        this.settings = settings;
#if UNITY_6000_0_OR_NEWER
        profilingSampler = new ProfilingSampler(Constants.Asset.AssemblyName);
#endif
      }

      /// <summary> Destroy the render pass. </summary>
      ~RenderPass() => material = null;

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        if (settings.palette == true && settings.profile != null)
        {
          material.EnableKeyword(Keywords.UsePalette);
#if UNITY_EDITOR
          gradientTex = settings.profile.ToTexture(settings.mode, settings.invert, (int)settings.resolution);
#else
          if (settings.mustUpdateTexture == true)
          {
            gradientTex = settings.profile.ToTexture(settings.mode, settings.invert, (int)settings.resolution);
            settings.mustUpdateTexture = false;
          }
#endif
          material.SetTexture(Textures.Gradient, gradientTex);

          switch (settings.sampleMethod)
          {
            case SampleMethod.Luminance:
              material.EnableKeyword(Keywords.UseLuminanceSample);
              material.SetFloat(ShaderIDs.LuminancePow, 2.0f - settings.luminancePow);
              material.SetFloat(ShaderIDs.RangeMin, Mathf.Min(settings.rangeMin, settings.rangeMax));
              material.SetFloat(ShaderIDs.RangeMax, Mathf.Max(settings.rangeMax, settings.rangeMin));
              break;
            case SampleMethod.Distance:
              material.EnableKeyword(Keywords.UseDistanceSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)settings.resolution);
              material.SetFloat(ShaderIDs.ColorThreshold, settings.colorThreshold * 0.1f);
              break;
            case SampleMethod.HSV:
              material.EnableKeyword(Keywords.UseHSVSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)settings.resolution);
              material.SetFloat(ShaderIDs.ColorThreshold, settings.colorThreshold * 0.1f);
              break;
            case SampleMethod.Dominant:
              material.EnableKeyword(Keywords.UseDominantSample);
              material.SetFloat(ShaderIDs.ColorSamples, (int)settings.resolution);
              material.SetFloat(ShaderIDs.ColorThreshold, settings.colorThreshold * 0.1f);
              break;
            case SampleMethod.Similarity:
              material.SetFloat(ShaderIDs.ColorSamples, (int)settings.resolution);
              material.SetFloat(ShaderIDs.ColorThreshold, settings.colorThreshold * 25.0f);
              break;
          }
        }

        if (settings.pixelate == true)
        {
          material.EnableKeyword(Keywords.UsePixelate);

          material.SetFloat(ShaderIDs.PixelSize, Mathf.Max(1.0f, settings.pixelSize));
          material.SetFloat(ShaderIDs.Samples, Mathf.Max(1.0f, settings.pixelSamples));
          material.SetColor(ShaderIDs.PixelTint, settings.pixelTint);

          if (settings.pixelSobel > 0.0f)
          {
            material.EnableKeyword(Keywords.UseSobel);
            material.SetFloat(ShaderIDs.PixelSobel, settings.pixelSobel);
            material.SetFloat(ShaderIDs.PixelSobelPower, settings.pixelSobelPower);

            Vector3 light = new(-Mathf.Sin(settings.pixelSobelAngle * Mathf.Deg2Rad),
                                Mathf.Cos(settings.pixelSobelAngle * Mathf.Deg2Rad),
                                1.0f);
            material.SetVector(ShaderIDs.PixelSobelLight, light.normalized);
            material.SetFloat(ShaderIDs.PixelSobelLightIntensity, settings.pixelSobelLightIntensity);
            material.SetFloat(ShaderIDs.PixelSobelAmbient, settings.pixelSobelAmbient);
          }

          material.SetFloat(ShaderIDs.PixelRound, Mathf.Lerp(0.25f, 0.7f, settings.pixelRound));
          material.SetInt(ShaderIDs.PixelBlend, (int)settings.pixelBlend);
          material.SetFloat(ShaderIDs.Bevel, settings.pixelBevel);
        }

        if (settings.quantization == true)
        {
          material.EnableKeyword(Keywords.UseQuantization);
          material.SetFloat(ShaderIDs.Quantization, Mathf.Max(2, settings.colors - 1));
        }

        material.SetFloat(ShaderIDs.Vignette, settings.vignette);

        material.SetFloat(ShaderIDs.ChromaticAberration, settings.chromaticAberration * 25.0f);

        material.SetFloat(ShaderIDs.Shine, settings.shine);
        material.SetFloat(ShaderIDs.ShineSize, settings.shineSize);

        material.SetFloat(ShaderIDs.Aperture, Mathf.SmoothStep(10.0f, 1.0f, settings.aperture));
        material.SetFloat(ShaderIDs.Curvature, settings.curvature);

        if (settings.border == true)
        {
          material.EnableKeyword(Keywords.UseBorder);
          material.SetColor(ShaderIDs.BorderColor, settings.borderColor);
          material.SetFloat(ShaderIDs.BorderSmooth, settings.borderSmooth * 0.01f);
          material.SetFloat(ShaderIDs.BorderNoise, settings.borderNoise * 0.1f);
          material.SetVector(ShaderIDs.BorderMargins, settings.borderMargins);
        }

        material.SetFloat(ShaderIDs.Scanline, settings.scanline);
        material.SetInt(ShaderIDs.ScanlineCount, settings.scanlineCount);
        material.SetFloat(ShaderIDs.ScanlineSpeed, settings.scanlineSpeed);

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);
      }

#if UNITY_6000_0_OR_NEWER
      /// <inheritdoc/>
      public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
      {
        if (material == null || settings.intensity == 0.0f)
          return;

        UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
        if (resourceData.isActiveTargetBackBuffer == true)
          return;

        UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
        if (cameraData.camera.cameraType == CameraType.SceneView && settings.affectSceneView == false || cameraData.postProcessEnabled == false)
          return;

        TextureHandle source = resourceData.activeColorTexture;
        TextureHandle destination = renderGraph.CreateTexture(source.GetDescriptor(renderGraph));

        UpdateMaterial();

        RenderGraphUtils.BlitMaterialParameters pass = new(source, destination, material, 0);
        renderGraph.AddBlitPass(pass, $"{Constants.Asset.AssemblyName}.Pass");

        resourceData.cameraColor = destination;
      }
#elif UNITY_2022_3_OR_NEWER
      /// <inheritdoc/>
      public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
      {
        renderTextureDescriptor = renderingData.cameraData.cameraTargetDescriptor;
        renderTextureDescriptor.depthBufferBits = 0;

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
      {
        if (material == null ||
            renderingData.postProcessingEnabled == false ||
            settings.intensity <= 0.0f ||
            settings.affectSceneView == false && renderingData.cameraData.isSceneViewCamera == true)
          return;

        CommandBuffer cmd = CommandBufferPool.Get(CommandBufferName);

        if (settings.enableProfiling == true)
          profilingScope = new ProfilingScope(cmd, profilingSamples);

        UpdateMaterial();

        cmd.Blit(colorBuffer, renderTextureHandle0, material);
        cmd.Blit(renderTextureHandle0, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd) => cmd.ReleaseTemporaryRT(renderTextureHandle0);
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}
