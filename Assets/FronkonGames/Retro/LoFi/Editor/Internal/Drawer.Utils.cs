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
using UnityEditor;

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Drawer base. </summary>
  public abstract partial class Drawer : PropertyDrawer
  {
    protected Vector2 Clamp(Vector2 value, float min, float max)
    {
      return new Vector2(Mathf.Clamp(value.x, min, max),
                         Mathf.Clamp(value.y, min, max));
    }

    protected Vector3 Clamp(Vector3 value, float min, float max)
    {
      return new Vector3(Mathf.Clamp(value.x, min, max),
                         Mathf.Clamp(value.y, min, max),
                         Mathf.Clamp(value.z, min, max));
    }

    protected Vector4 Clamp(Vector4 value, float min, float max)
    {
      return new Vector4(Mathf.Clamp(value.x, min, max),
                         Mathf.Clamp(value.y, min, max),
                         Mathf.Clamp(value.z, min, max),
                         Mathf.Clamp(value.w, min, max));
    }
  }
}
