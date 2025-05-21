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
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.Pixelator
{
  ///------------------------------------------------------------------------------------------------------------------
  /// <summary> Pixelator effect tools. </summary>
  /// <remarks> Only available for Universal Render Pipeline. </remarks>
  ///------------------------------------------------------------------------------------------------------------------
  public sealed partial class Pixelator // Partial class to match Pixelator.cs
  {
    private const string RenderListFieldName = "m_RendererDataList";
    private const BindingFlags BindingFlagsInternal = BindingFlags.Instance | BindingFlags.NonPublic;

    private static readonly Pixelator[] NoEffects = new Pixelator[0];

    /// <summary> Is the Pixelator effect in the default render pipeline? </summary>
    /// <returns> True if present, false otherwise. </returns>
    /// <remarks> This function uses Reflection and can be slow. Cache results if called frequently. </remarks>
    public static bool IsInRenderFeatures() => Instance != null;

    /// <summary> Is the Pixelator effect in any of the renderer data lists of the current URP asset? </summary>
    /// <returns> True if present in any, false otherwise. </returns>
    /// <remarks> This function uses Reflection and can be slow. Cache results if called frequently. </remarks>
    public static bool IsInAnyRenderFeatures() => Instances.Length > 0;

    /// <summary> Returns the Pixelator effect instance from the default render pipeline or null if not found. </summary>
    /// <returns> The Pixelator ScriptableRendererFeature instance or null. </returns>
    /// <remarks> This function uses Reflection and can be slow. Cache results if called frequently. </remarks>
    public static Pixelator Instance
    {
      get
      {
        if (GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset pipelineAsset)
        {
          FieldInfo propertyInfo = pipelineAsset.GetType().GetField(RenderListFieldName, BindingFlagsInternal);
          if (propertyInfo?.GetValue(pipelineAsset) is ScriptableRendererData[] rendererDataList && rendererDataList.Length > 0)
          {
            ScriptableRendererData scriptableRendererData = rendererDataList[0]; // Check the first/default renderer data.
            if (scriptableRendererData != null)
            {
              foreach (var feature in scriptableRendererData.rendererFeatures)
              {
                if (feature is Pixelator ntscFeature)
                  return ntscFeature;
              }
            }
          }
        }
        return null;
      }
    }

    /// <summary> Returns an array with all Pixelator effect instances found across all renderer data in the current URP asset. </summary>
    /// <returns> An array of Pixelator ScriptableRendererFeature instances. Empty if none found. </returns>
    /// <remarks> This function uses Reflection and can be slow. Cache results if called frequently. </remarks>
    public static Pixelator[] Instances
    {
      get
      {
        if (UniversalRenderPipeline.asset != null)
        {
          if (typeof(UniversalRenderPipelineAsset).GetField(RenderListFieldName, BindingFlagsInternal)?.GetValue(UniversalRenderPipeline.asset) is ScriptableRendererData[] rendererDataList)
          {
            List<Pixelator> effects = new List<Pixelator>();
            foreach (var rendererData in rendererDataList)
            {
              if (rendererData != null)
              {
                foreach (var feature in rendererData.rendererFeatures)
                {
                  if (feature is Pixelator ntscFeature)
                    effects.Add(ntscFeature);
                }
              }
            }
            return effects.ToArray();
          }
        }
        return NoEffects;
      }
    }
    // Add any Pixelator-specific tools or helper methods here if needed in the future.
  }
} 