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

namespace FronkonGames.Retro.Pixelator
{
  /// <summary> Luminance utils. </summary>
  public static class Luminance
  {
    private static float sRGBToLin(float channel) => channel <= 0.4045f ? channel / 12.92f : Mathf.Pow(((channel + 0.055f) / 1.055f), 2.4f);

    private static float YToLstar(float y) => y <= 0.008856f ? y * 903.2962f : Mathf.Pow(y, (1.0f / 3.0f)) * 116.0f - 16.0f;

    /// <summary> Compare two colors according to their luminance. </summary>
    public static int Compare(Color color1, Color color2)
    {
      float Y1 = 0.2126f * sRGBToLin(color1.linear.r) + 0.7152f * sRGBToLin(color1.linear.g) + 0.0722f * sRGBToLin(color1.linear.b);
      Y1 = YToLstar(Y1);

      float Y2 = 0.2126f * sRGBToLin(color2.linear.r) + 0.7152f * sRGBToLin(color2.linear.g) + 0.0722f * sRGBToLin(color2.linear.b);
      Y2 = YToLstar(Y2);

      return Y1 > Y2 ? 1 : Y1 < Y2 ? -1 : 0;
    }
  }
}