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

inline float Luminance601(float3 pixel)
{
  return dot(float3(0.299, 0.587, 0.114), pixel);
}

// RGB -> HSV http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
inline float3 RGB2HSV(float3 c)
{
  const float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  const float Epsilon = 1.0e-10;

  float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
  float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

  const float d = q.x - min(q.w, q.y);

  return float3(abs(q.z + (q.w - q.y) / (6.0 * d + Epsilon)), d / (q.x + Epsilon), q.x);
}

// HSV -> RGB http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
inline float3 HSV2RGB(float3 c)
{
  const float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  const float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);

  return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// Additive.
inline float3 BlendAdditive(const float3 s, const float3 d)
{
  return s + d;
}

// Color burn.
inline float3 BlendBurn(const float3 s, const float3 d)
{
  return 1.0 - (1.0 - d) / s;
}

// Color.
inline float3 BlendColor(float3 s, const float3 d)
{
  s = RGB2HSV(s);
  s.z = RGB2HSV(d).z;

  return HSV2RGB(s);
}

// Darken.
inline float3 BlendDarken(const float3 s, const float3 d)
{
  return min(s, d);
}

// Darker color.
inline float3 BlendDarker(float3 s, float3 d)
{
  return Luminance601(s) < Luminance601(d) ? s : d;
}

// Difference.
inline float3 BlendDifference(const float3 s, const float3 d)
{
  return abs(d - s);
}

// Divide.
inline float3 BlendDivide(float3 s, const float3 d)
{
  return (d > 0.0) ? s / d : s;
}

// Color dodge.
inline float3 BlendDodge(float3 s, const float3 d)
{
  return (s < 1.0) ? d / (1.0 - s) : s;
}

// HardMix.
inline float3 BlendHardMix(const float3 s, const float3 d)
{
  return floor(s + d);
}

// Hue.
inline float3 BlendHue(const float3 s, float3 d)
{
  d = RGB2HSV(d);
  d.x = RGB2HSV(s).x;

  return HSV2RGB(d);
}

// HardLight.
inline float3 BlendHardLight(const float3 s, const float3 d)
{
  return (s < 0.5) ? 2.0 * s * d : 1.0 - 2.0 * (1.0 - s) * (1.0 - d);
}

// Lighten.
inline float3 BlendLighten(const float3 s, const float3 d)
{
  return max(s, d);
}

// Lighter color.
inline float3 BlendLighter(float3 s, float3 d)
{
  return Luminance601(s) > Luminance601(d) ? s : d;
}

// Multiply.
inline float3 BlendMultiply(float3 s, const float3 d)
{
  return s * d;
}

// Overlay.
inline float3 BlendOverlay(const float3 s, const float3 d)
{
  return (s > 0.5) ? 1.0 - 2.0 * (1.0 - s) * (1.0 - d) : 2.0 * s * d;
}

// Screen.
inline float3 BlendScreen(const float3 s, const float3 d)
{
  return s + d - s * d;
}

// Solid.
inline float3 BlendSolid(const float3 s, const float3 d)
{
  return d;
}

// Soft light.
inline float3 BlendSoftLight(const float3 s, const float3 d)
{
  return (1.0 - s) * s * d + s * (1.0 - (1.0 - s) * (1.0 - d));
}

// Pin light.
inline float3 BlendPinLight(const float3 s, float3 d)
{
  return (2.0 * s - 1.0 > d) ? 2.0 * s - 1.0 : (s < 0.5 * d) ? 2.0 * s : d;
}

// Saturation.
inline float3 BlendSaturation(const float3 s, float3 d)
{
  d = RGB2HSV(d);
  d.y = RGB2HSV(s).y;

  return HSV2RGB(d);
}

// Subtract.
inline float3 BlendSubtract(const float3 s, const float3 d)
{
  return s - d;
}

// VividLight.
inline float3 BlendVividLight(float3 s, const float3 d)
{
  return (s < 0.5) ? (s > 0.0 ? 1.0 - (1.0 - d) / (2.0 * s) : s) : (s < 1.0 ? d / (2.0 * (1.0 - s)) : s);
}

// Luminosity.
inline float3 BlendLuminosity(const float3 s, const float3 d)
{
  const float dLum = Luminance601(s);
  const float sLum = Luminance601(d);

  const float lum = sLum - dLum;

  float3 c = d + lum;
  const float minC = min(min(c.r, c.g), c.b);
  const float maxC = max(max(c.r, c.b), c.b);

  if (minC < 0.0)
    return sLum + ((c - sLum) * sLum) / (sLum - minC);
  else if (maxC > 1.0)
    return sLum + ((c - sLum) * (1.0 - sLum)) / (maxC - sLum);

  return c;
}

inline float3 ColorBlend(const int blendOp, const float3 s, float3 d)
{
  switch (blendOp)
  {
    case 0:  return BlendAdditive(s, d);
    case 1:  return BlendBurn(s, d);
    case 2:  return BlendColor(s, d);
    case 3:  return BlendDarken(s, d);
    case 4:  return BlendDarker(s, d);  
    case 5:  return BlendDifference(s, d);
    case 6:  return BlendDivide(s, d);
    case 7:  return BlendDodge(s, d);
    case 8:  return BlendHardMix(s, d);
    case 9:  return BlendHue(s, d);
    case 10: return BlendHardLight(s, d);
    case 11: return BlendLighten(s, d);
    case 12: return BlendLighter(s, d);
    case 13: return BlendMultiply(s, d);
    case 14: return BlendOverlay(s, d);
    case 15: return BlendScreen(s, d);
    case 16: return d;
    case 17: return s;
    case 18: return BlendSoftLight(s, d);
    case 19: return BlendPinLight(s, d);
    case 20: return BlendSaturation(s, d);
    case 21: return BlendSubtract(s, d);
    case 22: return BlendVividLight(s, d);
    case 23: return BlendLuminosity(s, d);
    default: return d;
  }
}
