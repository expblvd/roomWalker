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

namespace FronkonGames.Retro.CRTTV
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary>
  /// 
  /// 🕹️ Online documentation: https://fronkongames.github.io/store/retro/
  /// 📄 Online demo:          https://fronkongames.github.io/demos-retro/crttv/
  /// ❤️ More assets:          https://assetstore.unity.com/publishers/241924
  /// 
  /// 💡 Any questions or suggestions? Do not hesitate to send me an email to fronkongames@gmail.com
  /// 💡 Do you want to report an error? Please, send the log file along with the mail.
  /// 
  /// ❤️ Write a review if you found this asset useful, thanks! ❤️
  /// 
  /// </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  [DisallowMultipleRendererFeature(Constants.Asset.Name)]
  public sealed partial class CRTTV : ScriptableRendererFeature
  {
    // MUST be named "settings" (lowercase) to be shown in the Render Features inspector.
    public Settings settings = new();

    private RenderPass renderPass;

    private Material material;

    /// <summary> Initializes this feature's resources. </summary>
    public override void Create() => renderPass ??= new RenderPass(settings);

    /// <summary> Injects one or multiple ScriptableRenderPass in the renderer. Called every frame once per camera. </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
      if (renderingData.cameraData.cameraType == CameraType.Preview || renderingData.cameraData.cameraType == CameraType.Reflection)
        return;

      renderPass.renderPassEvent = settings.whenToInsert;
      if (material == null)
      {
        string shaderPath = $"Shaders/{Constants.Asset.ShaderName}_URP";
        Shader shader = Resources.Load<Shader>(shaderPath);
        if (shader != null)
        {
          if (shader.isSupported == true)
            material = CoreUtils.CreateEngineMaterial(shader);
          else
            Log.Warning($"'{shaderPath}.shader' not supported");
        }
      }

      renderPass.material = material;
      renderer.EnqueuePass(renderPass);
    }

    protected override void Dispose(bool disposing) => CoreUtils.Destroy(material);
  }
}
