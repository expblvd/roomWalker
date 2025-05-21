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
using System.Collections.Generic;
using UnityEngine;

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Lo-Fi palette profile and color tools. </summary>
  [CreateAssetMenu(fileName = "lofiprofile", menuName = "Fronkon Games/Retro/Lo-Fi/Create profile")]
  public sealed class LoFiProfile : ScriptableObject
  {
    public string title;

    public Color[] colors;

    public List<string> tags;

    public int likes;

    internal const int Height = 8;

    public Texture2D ToTexture(BlendModes mode, bool invert, int width)
    {
      width = Math.Clamp(width, (int)PaletteResolutions._8, (int)PaletteResolutions._2048);
      Texture2D texture = new(width, Height, TextureFormat.RGB24, false)
      {
        wrapMode = TextureWrapMode.MirrorOnce,
        filterMode = FilterMode.Point
      };

      Color32[] pixels = new Color32[width * Height];
      for (int x = 0; x < width; ++x)
      {
        float t = (float)x / (width - 1);
        if (invert == true)
          t = 1.0f - t;

        for (int y = 0; y < Height; ++y)
          pixels[x + y * width] = Evaluate(t, mode);
      }

      texture.SetPixels32(pixels);
      texture.Apply();

      return texture;
    }

    private Color Evaluate(float time, BlendModes mode = BlendModes.Blend)
    {
      if (colors == null || colors.Length == 0)
        return Color.black;

      if (colors.Length == 1)
        return colors[0];

      time = Mathf.Clamp01(time);
      float index = time * colors.Length;
      int lowerIndex = Mathf.FloorToInt(index);
      float lerpFactor = index - lowerIndex;

      if (lowerIndex >= colors.Length - 1)
        return colors[^1];

      Color lowerColor = colors[lowerIndex];
      Color upperColor = colors[lowerIndex + 1];

      return mode switch
      {
        BlendModes.Fixed => lowerColor,
        BlendModes.Blend => Color.Lerp(lowerColor, upperColor, lerpFactor),
        _ => Color.magenta // Should never happen!
      };
    }
  }
}
