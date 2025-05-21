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

namespace FronkonGames.Retro.CRTTV
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class CRTTV
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

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");

        public static readonly int Frame = Shader.PropertyToID("_Frame");
        public static readonly int ShadowmaskStrength = Shader.PropertyToID("_ShadowmaskStrength");
        public static readonly int ShadowmaskLuminosity = Shader.PropertyToID("_ShadowmaskLuminosity");
        public static readonly int ShadowmaskScale = Shader.PropertyToID("_ShadowmaskScale");
        public static readonly int ShadowmaskColorOffset = Shader.PropertyToID("_ShadowmaskColorOffset");
        public static readonly int ShadowmaskVerticalGapHardness = Shader.PropertyToID("_ShadowmaskVerticalGapHardness");
        public static readonly int ShadowmaskHorizontalGapHardness = Shader.PropertyToID("_ShadowmaskHorizontalGapHardness");
        public static readonly int FishEyeStrength = Shader.PropertyToID("_FishEyeStrength");
        public static readonly int FishEyeZoom = Shader.PropertyToID("_FishEyeZoom");
        public static readonly int Distortion = Shader.PropertyToID("_Distortion");
        public static readonly int DistortionSpeed = Shader.PropertyToID("_DistortionSpeed");
        public static readonly int DistortionAmplitude = Shader.PropertyToID("_DistortionAmplitude");
        public static readonly int VignetteSmoothness = Shader.PropertyToID("_VignetteSmoothness");
        public static readonly int VignetteRounding = Shader.PropertyToID("_VignetteRounding");
        public static readonly int VignetteBorders = Shader.PropertyToID("_VignetteBorders");
        public static readonly int ShineStrength = Shader.PropertyToID("_ShineStrength");
        public static readonly int ShineColor = Shader.PropertyToID("_ShineColor");
        public static readonly int ShinePosition = Shader.PropertyToID("_ShinePosition");
        public static readonly int RGBOffsetStrength = Shader.PropertyToID("_RGBOffsetStrength");
        public static readonly int ColorBleedingStrength = Shader.PropertyToID("_ColorBleedingStrength");
        public static readonly int ColorBleedingDistance = Shader.PropertyToID("_ColorBleedingDistance");
        public static readonly int ColorCurves = Shader.PropertyToID("_ColorCurves");
        public static readonly int GammaColor = Shader.PropertyToID("_GammaColor");
        public static readonly int Scanlines = Shader.PropertyToID("_Scanlines");
        public static readonly int ScanlinesCount = Shader.PropertyToID("_ScanlinesCount");
        public static readonly int ScanlinesVelocity = Shader.PropertyToID("_ScanlinesVelocity");
        public static readonly int InterferenceStrength = Shader.PropertyToID("_InterferenceStrength");
        public static readonly int InterferencePeakStrength = Shader.PropertyToID("_InterferencePeakStrength");
        public static readonly int InterferencePeakPosition = Shader.PropertyToID("_InterferencePeakPosition");
        public static readonly int ShakeStrength = Shader.PropertyToID("_ShakeStrength");
        public static readonly int ShakeRate = Shader.PropertyToID("_ShakeRate");
        public static readonly int MovementStrength = Shader.PropertyToID("_MovementStrength");
        public static readonly int MovementRate = Shader.PropertyToID("_MovementRate");
        public static readonly int MovementSpeed = Shader.PropertyToID("_MovementSpeed");
        public static readonly int Grain = Shader.PropertyToID("_Grain");
        public static readonly int StaticNoise = Shader.PropertyToID("_StaticNoise");
        public static readonly int BarStrength = Shader.PropertyToID("_BarStrength");
        public static readonly int BarHeight = Shader.PropertyToID("_BarHeight");
        public static readonly int BarSpeed = Shader.PropertyToID("_BarSpeed");
        public static readonly int BarOverflow = Shader.PropertyToID("_BarOverflow");

        public static readonly int FlickerStrength = Shader.PropertyToID("_FlickerStrength");
        public static readonly int FlickerSpeed = Shader.PropertyToID("_FlickerSpeed");
        
        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");      
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

        material.SetInt(ShaderIDs.Frame, Time.frameCount);
        material.SetFloat(ShaderIDs.ShadowmaskStrength, settings.shadowmaskStrength);
        material.SetFloat(ShaderIDs.ShadowmaskLuminosity, settings.shadowmaskLuminosity * 300.0f);
        material.SetFloat(ShaderIDs.ShadowmaskScale, settings.shadowmaskScale);
        material.SetVector(ShaderIDs.ShadowmaskColorOffset, settings.shadowmaskColorOffset);
        material.SetFloat(ShaderIDs.ShadowmaskVerticalGapHardness, settings.shadowmaskVerticalGapHardness);
        material.SetFloat(ShaderIDs.ShadowmaskHorizontalGapHardness, settings.shadowmaskHorizontalGapHardness);
        material.SetFloat(ShaderIDs.FishEyeStrength, settings.fishEyeStrength);
        material.SetVector(ShaderIDs.FishEyeZoom, settings.fishEyeZoom);
        material.SetFloat(ShaderIDs.Distortion, settings.distortion * 0.01f);
        material.SetFloat(ShaderIDs.DistortionSpeed, settings.distortionSpeed);
        material.SetFloat(ShaderIDs.DistortionAmplitude, settings.distortionAmplitude * 10.0f);
        material.SetFloat(ShaderIDs.VignetteSmoothness, settings.vignetteSmoothness);
        material.SetFloat(ShaderIDs.VignetteRounding, settings.vignetteRounding);
        material.SetVector(ShaderIDs.VignetteBorders, settings.vignetteBorders);
        material.SetFloat(ShaderIDs.ShineStrength, settings.shineStrength);
        material.SetColor(ShaderIDs.ShineColor, settings.shineColor);
        material.SetVector(ShaderIDs.ShinePosition, settings.shinePosition);
        material.SetFloat(ShaderIDs.RGBOffsetStrength, settings.rgbOffsetStrength);
        material.SetFloat(ShaderIDs.ColorBleedingStrength, settings.colorBleedingStrength);
        material.SetFloat(ShaderIDs.ColorBleedingDistance, settings.colorBleedingDistance);
        material.SetColor(ShaderIDs.ColorCurves, settings.colorCurves);
        material.SetVector(ShaderIDs.GammaColor, settings.gammaColor);
        material.SetFloat(ShaderIDs.Scanlines, settings.scanlines);
        material.SetFloat(ShaderIDs.ScanlinesCount, settings.scanlinesCount);
        material.SetFloat(ShaderIDs.ScanlinesVelocity, settings.scanlinesVelocity);
        material.SetFloat(ShaderIDs.InterferenceStrength, settings.interferenceStrength);
        material.SetFloat(ShaderIDs.InterferencePeakStrength, settings.interferencePeakStrength);
        material.SetFloat(ShaderIDs.InterferencePeakPosition, settings.interferencePeakPosition);
        material.SetFloat(ShaderIDs.ShakeStrength, settings.shakeStrength);
        material.SetFloat(ShaderIDs.ShakeRate, settings.shakeRate);
        material.SetFloat(ShaderIDs.MovementStrength, settings.movementStrength);
        material.SetFloat(ShaderIDs.MovementRate, settings.movementRate);
        material.SetFloat(ShaderIDs.MovementSpeed, settings.movementSpeed);
        material.SetFloat(ShaderIDs.Grain, settings.grain);
        material.SetFloat(ShaderIDs.StaticNoise, settings.staticNoise);
        material.SetFloat(ShaderIDs.BarStrength, settings.barStrength);
        material.SetFloat(ShaderIDs.BarHeight, settings.barHeight);
        material.SetFloat(ShaderIDs.BarSpeed, settings.barSpeed);
        material.SetFloat(ShaderIDs.BarOverflow, settings.barOverflow);

        material.SetFloat(ShaderIDs.FlickerStrength, settings.flickerStrength);
        material.SetFloat(ShaderIDs.FlickerSpeed, settings.flickerSpeed);

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
