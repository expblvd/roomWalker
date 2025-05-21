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

namespace FronkonGames.Retro.Pixelator
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Render Pass. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Pixelator
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

      private Gradient gradient = null;
      private Texture2D gradientTexture;

      private static class ShaderIDs
      {
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");

        public static readonly int PixelationMode = Shader.PropertyToID("_PixelationMode");
        public static readonly int PixelSize = Shader.PropertyToID("_PixelSize");
        public static readonly int PixelScale = Shader.PropertyToID("_PixelScale");
        public static readonly int AspectRatio = Shader.PropertyToID("_AspectRatio");
        public static readonly int Radius = Shader.PropertyToID("_Radius");
        public static readonly int Background = Shader.PropertyToID("_Background");
        public static readonly int Threads = Shader.PropertyToID("_Threads");

        public static readonly int GradientIntensity = Shader.PropertyToID("_GradientIntensity");
        public static readonly int LuminanceMin = Shader.PropertyToID("_LuminanceMin");
        public static readonly int LuminanceMax = Shader.PropertyToID("_LuminanceMax");
        public static readonly int GradientCIELabSamples = Shader.PropertyToID("_GradientCIELabSamples");

        public static readonly int Bevel = Shader.PropertyToID("_Bevel");
        public static readonly int BevelLightColor = Shader.PropertyToID("_BevelLightColor");
        public static readonly int BevelShadowColor = Shader.PropertyToID("_BevelShadowColor");
        public static readonly int BevelDepthSensitivity = Shader.PropertyToID("_BevelDepthSensitivity");
        public static readonly int BevelLightDirection = Shader.PropertyToID("_BevelLightDirection");

        public static readonly int ChromaticAberrationIntensity = Shader.PropertyToID("_ChromaticAberrationIntensity");
        public static readonly int ChromaticAberrationOffset = Shader.PropertyToID("_ChromaticAberrationOffset");

        public static readonly int DitherIntensity = Shader.PropertyToID("_DitherIntensity");
        public static readonly int DitherPatternScale = Shader.PropertyToID("_DitherPatternScale");
        public static readonly int DitherThresholdScale = Shader.PropertyToID("_DitherThresholdScale");
        public static readonly int DitherColorSteps = Shader.PropertyToID("_DitherColorSteps");

        public static readonly int PosterizeIntensity = Shader.PropertyToID("_PosterizeIntensity");
        public static readonly int PosterizeStepsRGB = Shader.PropertyToID("_PosterizeStepsRGB");
        public static readonly int PosterizeLuminanceSteps = Shader.PropertyToID("_PosterizeLuminanceSteps");
        public static readonly int PosterizeStepsHSV = Shader.PropertyToID("_PosterizeStepsHSV");
        public static readonly int PosterizeGamma = Shader.PropertyToID("_PosterizeGamma");

        public static readonly int FiltersIntensity = Shader.PropertyToID("_FiltersIntensity");
        public static readonly int SepiaIntensity = Shader.PropertyToID("_SepiaIntensity");
        public static readonly int CoolBlueIntensity = Shader.PropertyToID("_CoolBlueIntensity");
        public static readonly int WarmFilterIntensity = Shader.PropertyToID("_WarmFilterIntensity");
        public static readonly int InvertColorIntensity = Shader.PropertyToID("_InvertColorIntensity");
        public static readonly int HudsonIntensity = Shader.PropertyToID("_HudsonIntensity");
        public static readonly int HefeIntensity = Shader.PropertyToID("_HefeIntensity");
        public static readonly int XProIntensity = Shader.PropertyToID("_XProIntensity");
        public static readonly int RiseIntensity = Shader.PropertyToID("_RiseIntensity");
        public static readonly int ToasterIntensity = Shader.PropertyToID("_ToasterIntensity");
        public static readonly int IRFilterIntensity = Shader.PropertyToID("_IRFilterIntensity");
        public static readonly int ThermalFilterIntensity = Shader.PropertyToID("_ThermalFilterIntensity");
        public static readonly int DuotoneIntensity = Shader.PropertyToID("_DuotoneIntensity");
        public static readonly int DuotoneColorA = Shader.PropertyToID("_DuotoneColorA");
        public static readonly int DuotoneColorB = Shader.PropertyToID("_DuotoneColorB");
        public static readonly int NightVisionIntensity = Shader.PropertyToID("_NightVisionIntensity");
        public static readonly int PopArtIntensity = Shader.PropertyToID("_PopArtIntensity");
        public static readonly int BlueprintIntensity = Shader.PropertyToID("_BlueprintIntensity");
        public static readonly int BlueprintEdgeColor = Shader.PropertyToID("_BlueprintEdgeColor");
        public static readonly int BlueprintBackgroundColor = Shader.PropertyToID("_BlueprintBackgroundColor");
        public static readonly int BlueprintEdgeThreshold = Shader.PropertyToID("_BlueprintEdgeThreshold");

        public static readonly int Brightness = Shader.PropertyToID("_Brightness");
        public static readonly int Contrast = Shader.PropertyToID("_Contrast");
        public static readonly int Gamma = Shader.PropertyToID("_Gamma");
        public static readonly int Hue = Shader.PropertyToID("_Hue");
        public static readonly int Saturation = Shader.PropertyToID("_Saturation");
      }

      private static class Keywords
      {
        public static readonly string PixelationRectangle = "_PIXELATION_MODE_RECTANGLE";
        public static readonly string PixelationHexagon   = "_PIXELATION_MODE_HEXAGON";
        public static readonly string PixelationDiamond   = "_PIXELATION_MODE_DIAMOND";
        public static readonly string PixelationCircle    = "_PIXELATION_MODE_CIRCLE";
        public static readonly string PixelationTriangle  = "_PIXELATION_MODE_TRIANGLE";
        public static readonly string PixelationSquare    = "_PIXELATION_MODE_SQUARE";
        public static readonly string PixelationLeaf      = "_PIXELATION_MODE_LEAF";
        public static readonly string PixelationLed       = "_PIXELATION_MODE_LED";
        public static readonly string PixelationKnitted   = "_PIXELATION_MODE_KNITTED";

        public static readonly string Gradient            = "_GRADIENT";
        public static readonly string GradientCIELab      = "_GRADIENT_CIELAB";
        public static readonly string GradientLuminance   = "_GRADIENT_APPLY_LUMINANCE";

        public static readonly string Bevel               = "_BEVEL";

        public static readonly string Dither              = "_DITHER";

        public static readonly string ChromaticAberration = "_CHROMATIC_ABERRATION";

        public static readonly string Posterize           = "_POSTERIZE";

        public static readonly string Filters             = "_FILTERS";
      }

      private static class Textures
      {
        internal static readonly int GradientTexture = Shader.PropertyToID("_GradientTex");
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
      ~RenderPass()
      {
        gradient = null;
        gradientTexture = null;
      }

      /// <summary> Update gradient texture. </summary>
      public void UpdateGradientTexture()
      {
        gradient = settings.gradient;

        const int width = 256;
        const int height = 4;
        gradientTexture = new Texture2D(width, height, TextureFormat.RGB24, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };

        const float inv = 1.0f / (width - 1);
        for (int y = 0; y < height; ++y)
        {
          for (int x = 0; x < width; ++x)
            gradientTexture.SetPixel(x, y, gradient.Evaluate(x * inv));
        }

        gradientTexture.Apply();

        settings.forceGradientTextureUpdate = false;
      }

      private void UpdateMaterial()
      {
        material.shaderKeywords = null;
        material.SetFloat(ShaderIDs.Intensity, settings.intensity);

        float aspectRatio = settings.screenAspectRatio == true ? Screen.width / Screen.height : settings.aspectRatio;

        switch (settings.pixelationMode)
        {
          case PixelationModes.Rectangle:
            material.EnableKeyword(Keywords.PixelationRectangle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - settings.pixelSize) * 500.0f);
            material.SetVector(ShaderIDs.PixelScale, settings.pixelScale);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Circle:
            material.EnableKeyword(Keywords.PixelationCircle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - settings.pixelSize) * 200.0f);
            material.SetFloat(ShaderIDs.Radius, settings.radius);
            material.SetColor(ShaderIDs.Background, settings.background);
            break;
          case PixelationModes.Triangle:
            material.EnableKeyword(Keywords.PixelationTriangle);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - settings.pixelSize) * 500.0f);
            material.SetVector(ShaderIDs.PixelScale, settings.pixelScale);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Diamond:
            material.EnableKeyword(Keywords.PixelationDiamond);
            material.SetFloat(ShaderIDs.PixelSize, settings.pixelSize * 0.2f);
            break;
          case PixelationModes.Hexagon:
            material.EnableKeyword(Keywords.PixelationHexagon);
            material.SetFloat(ShaderIDs.PixelSize, settings.pixelSize * 0.02f);
            material.SetVector(ShaderIDs.PixelScale, settings.pixelScale);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Leaf:
            material.EnableKeyword(Keywords.PixelationLeaf);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - settings.pixelSize) * 10.0f);
            material.SetVector(ShaderIDs.PixelScale, settings.pixelScale * 20.0f);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            break;
          case PixelationModes.Led:
            material.EnableKeyword(Keywords.PixelationLed);
            material.SetFloat(ShaderIDs.PixelSize, (1.01f - settings.pixelSize) * 300.0f);
            material.SetFloat(ShaderIDs.AspectRatio, aspectRatio);
            material.SetFloat(ShaderIDs.Radius, settings.radius);
            material.SetColor(ShaderIDs.Background, settings.background);
            break;
          case PixelationModes.Knitted:
            material.EnableKeyword(Keywords.PixelationKnitted);
            material.SetFloat(ShaderIDs.PixelSize, Mathf.Max(settings.pixelSize * 32.0f, 0.05f));
            material.SetVector(ShaderIDs.PixelScale, settings.pixelScale);
            material.SetInt(ShaderIDs.Threads, settings.threads);
            break;
        }

        if (settings.gradientIntensity > 0.0f)
        {
          material.EnableKeyword(Keywords.Gradient);

          material.SetFloat(ShaderIDs.GradientIntensity, settings.gradientIntensity);
          if (settings.forceGradientTextureUpdate == true || gradient == null || gradient != settings.gradient)
            UpdateGradientTexture();
          material.SetTexture(Textures.GradientTexture, gradientTexture);

          if (settings.gradientApplyLuminance == true)
            material.EnableKeyword(Keywords.GradientLuminance);

          if (settings.gradientMappingMode == GradientMappingMode.Luminance)
          {
            material.SetFloat(ShaderIDs.LuminanceMin, settings.luminanceMin);
            material.SetFloat(ShaderIDs.LuminanceMax, settings.luminanceMax);
          }
          else
          {
            material.EnableKeyword(Keywords.GradientCIELab);
            material.SetInt(ShaderIDs.GradientCIELabSamples, settings.gradientCIELabSamples);
          }
        }

        if (settings.chromaticAberrationIntensity > 0.0f)
        {
          material.EnableKeyword(Keywords.ChromaticAberration);
          material.SetFloat(ShaderIDs.ChromaticAberrationIntensity, settings.chromaticAberrationIntensity);
          material.SetVector(ShaderIDs.ChromaticAberrationOffset, settings.chromaticAberrationOffset);
        }

        material.SetFloat(ShaderIDs.Brightness, settings.brightness);
        material.SetFloat(ShaderIDs.Contrast, settings.contrast);
        material.SetFloat(ShaderIDs.Gamma, 1.0f / settings.gamma);
        material.SetFloat(ShaderIDs.Hue, settings.hue);
        material.SetFloat(ShaderIDs.Saturation, settings.saturation);        

        if (settings.bevel > 0.0f)
        {
          material.EnableKeyword(Keywords.Bevel);
          material.SetFloat(ShaderIDs.Bevel, settings.bevel);
        }

        if (settings.ditherIntensity > 0.0f)
        {
          material.EnableKeyword(Keywords.Dither);
          material.SetFloat(ShaderIDs.DitherIntensity, settings.ditherIntensity);
          material.SetInt(ShaderIDs.DitherPatternScale, settings.ditherPatternScale);
          material.SetFloat(ShaderIDs.DitherThresholdScale, settings.ditherThresholdScale);
          material.SetInt(ShaderIDs.DitherColorSteps, settings.ditherColorSteps);
        }

        if (settings.posterizeIntensity > 0.0f)
        {
          material.EnableKeyword(Keywords.Posterize);
          material.SetFloat(ShaderIDs.PosterizeIntensity, settings.posterizeIntensity);
          material.SetVector(ShaderIDs.PosterizeStepsRGB, (Vector3)settings.posterizeRGBSteps);
          material.SetFloat(ShaderIDs.PosterizeLuminanceSteps, settings.posterizeLuminanceSteps);
          material.SetVector(ShaderIDs.PosterizeStepsHSV, (Vector3)settings.posterizeHSVSteps);
          material.SetFloat(ShaderIDs.PosterizeGamma, settings.posterizeGamma);
        }

        if (settings.filtersIntensity > 0.0f)
        {
          material.EnableKeyword(Keywords.Filters);
          material.SetFloat(ShaderIDs.FiltersIntensity, settings.filtersIntensity);
          material.SetFloat(ShaderIDs.SepiaIntensity, settings.sepiaIntensity);
          material.SetFloat(ShaderIDs.CoolBlueIntensity, settings.coolBlueIntensity);
          material.SetFloat(ShaderIDs.WarmFilterIntensity, settings.warmFilterIntensity);
          material.SetFloat(ShaderIDs.InvertColorIntensity, settings.invertColorIntensity);
          material.SetFloat(ShaderIDs.HudsonIntensity, settings.hudsonIntensity);
          material.SetFloat(ShaderIDs.HefeIntensity, settings.hefeIntensity);
          material.SetFloat(ShaderIDs.XProIntensity, settings.xproIntensity);
          material.SetFloat(ShaderIDs.RiseIntensity, settings.riseIntensity);
          material.SetFloat(ShaderIDs.ToasterIntensity, settings.toasterIntensity);
          material.SetFloat(ShaderIDs.IRFilterIntensity, settings.irFilterIntensity);
          material.SetFloat(ShaderIDs.ThermalFilterIntensity, settings.thermalFilterIntensity);
          material.SetFloat(ShaderIDs.DuotoneIntensity, settings.duotoneIntensity);
          material.SetColor(ShaderIDs.DuotoneColorA, settings.duotoneColorA);
          material.SetColor(ShaderIDs.DuotoneColorB, settings.duotoneColorB);
          material.SetFloat(ShaderIDs.NightVisionIntensity, settings.nightVisionIntensity);
          material.SetFloat(ShaderIDs.PopArtIntensity, settings.popArtIntensity);
          material.SetFloat(ShaderIDs.BlueprintIntensity, settings.blueprintIntensity);
          material.SetColor(ShaderIDs.BlueprintEdgeColor, settings.blueprintEdgeColor);
          material.SetColor(ShaderIDs.BlueprintBackgroundColor, settings.blueprintBackgroundColor);
          material.SetFloat(ShaderIDs.BlueprintEdgeThreshold, settings.blueprintEdgeThreshold);
        }
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

        colorBuffer = renderingData.cameraData.renderer.cameraColorTargetHandle;
        cmd.GetTemporaryRT(renderTextureHandle0, renderTextureDescriptor, settings.filterMode);
      }

      /// <inheritdoc/>
      public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
      {
        base.Configure(cmd, cameraTextureDescriptor);
        ConfigureInput(ScriptableRenderPassInput.None);
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

        cmd.Blit(colorBuffer, renderTextureHandle0, material, 0);
        cmd.Blit(renderTextureHandle0, colorBuffer);

        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        if (settings.enableProfiling == true)
          profilingScope.Dispose();

        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
      }

      public override void OnCameraCleanup(CommandBuffer cmd)
      {
        cmd.ReleaseTemporaryRT(renderTextureHandle0);

        settings.forceGradientTextureUpdate = true;
      }
#else
      #error Unsupported Unity version. Please update to a newer version of Unity.
#endif
    }
  }
}