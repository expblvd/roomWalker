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

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Operation used to obtain the color of the palette. </summary>
  public enum BlendModes
  {
    Fixed,
    Blend,
  }

  /// <summary> Method used to sample color from the palette. </summary>
  public enum SampleMethod
  {
    Luminance,
    Distance,
    HSV,
    Similarity,
    Dominant,
  }

  /// <summary> Possible resolutions for the texture used in the color palette. </summary>
  public enum PaletteResolutions
  {
    _8    = 8,
    _16   = 16,
    _32   = 32,
    _64   = 64,
    _128  = 128,
    _256  = 256,
    _512  = 512,
    _1024 = 1024,
    _2048 = 2048,
  }
}
