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
#pragma once

#if _BEVEL

float _Bevel;

inline half3 Bevel(float3 pixel, float2 uv)
{
  const float lum = Luminance(pixel);

  float dp = clamp(ddx(lum) * _Bevel, 0.0, 1.0);
  dp += clamp(ddy(-lum) * _Bevel, 0.0, 1.0);
  pixel += pixel * dp * 2.0;
  
  dp = clamp(ddx(-lum) * _Bevel, 0.0, 1.0);
  dp += clamp(ddy(lum) * _Bevel, 0.0, 1.0);
  pixel -= pixel * dp * 0.5;
  
  return pixel;
}

#else

inline half3 Bevel(float3 pixel, float2 uv)
{
  return pixel;
}

#endif
