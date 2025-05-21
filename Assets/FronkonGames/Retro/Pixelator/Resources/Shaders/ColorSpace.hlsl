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

// sRGB to Linear
// Assumes input is in [0, 1] range
inline float SrgbToLinear(float c)
{
  if (c <= 0.04045)
    return c / 12.92;
  return pow(abs((c + 0.055) / 1.055), 2.4);
}

inline float3 SrgbToLinear(float3 c)
{
  return float3(SrgbToLinear(c.r), SrgbToLinear(c.g), SrgbToLinear(c.b));
}

// Linear to sRGB
// Assumes input is in [0, 1] range
inline float LinearToSrgb(float c)
{
  if (c <= 0.0031308)
    return c * 12.92;
  return 1.055 * pow(c, 1.0 / 2.4) - 0.055;
}

inline float3 LinearToSrgb(float3 c)
{
  return float3(LinearToSrgb(c.r), LinearToSrgb(c.g), LinearToSrgb(c.b));
}

// Standard D65 white point
static const float3 D65_XYZ = float3(0.95047, 1.00000, 1.08883);

// RGB (linear) to XYZ
// Assumes input RGB is linear and in [0,1] range.
// Uses sRGB primaries and D65 white point.
inline float3 RgbLinearToXyz(float3 rgbLinear)
{
  // sRGB to XYZ matrix (D65)
  // Transposed for HLSL float3x3 constructor
  float3x3 rgbToXyzMat = float3x3(
    0.4124564, 0.3575761, 0.1804375,
    0.2126729, 0.7151522, 0.0721750,
    0.0193339, 0.1191920, 0.9503041
);

return mul(rgbToXyzMat, rgbLinear);
}

// XYZ to CIELab
inline float3 XyzToCIELab(float3 xyz)
{
  float3 lab;
  float3 xyz_ref = xyz / D65_XYZ;
  
  float fx, fy, fz;
  
  if (xyz_ref.x > 0.008856) fx = pow(xyz_ref.x, 1.0 / 3.0);
  else fx = (7.787 * xyz_ref.x) + (16.0 / 116.0);
  
  if (xyz_ref.y > 0.008856) fy = pow(xyz_ref.y, 1.0 / 3.0);
  else fy = (7.787 * xyz_ref.y) + (16.0 / 116.0);
  
  if (xyz_ref.z > 0.008856) fz = pow(xyz_ref.z, 1.0 / 3.0);
  else fz = (7.787 * xyz_ref.z) + (16.0 / 116.0);
  
  lab.x = (116.0 * fy) - 16.0; // L*
  lab.y = 500.0 * (fx - fy);   // a*
  lab.z = 200.0 * (fy - fz);   // b*
  
  return lab; // L* in [0, 100], a* and b* in approx [-100, 100]
}

// Helper to convert sRGB directly to CIELab
// Input: sRGB color [0,1]
// Output: CIELab (L* [0,100], a* [-100,100], b* [-100,100] typically)
inline float3 SrgbToCIELab(float3 sRgbColor)
{
  float3 linearRgb = SrgbToLinear(sRgbColor);
  float3 xyz = RgbLinearToXyz(linearRgb);
  return XyzToCIELab(xyz);
}

// Helper to convert Linear RGB directly to CIELab
// Input: Linear RGB color [0,1]
// Output: CIELab (L* [0,100], a* [-100,100], b* [-100,100] typically)
inline float3 RgbLinearToCIELab(float3 linearRgbColor)
{
  float3 xyz = RgbLinearToXyz(linearRgbColor);
  return XyzToCIELab(xyz);
}


// CIELab Delta E (1976) - Euclidean distance
// Input: two CIELab colors
inline float CIELabDistance(float3 lab1, float3 lab2)
{
  float dL = lab1.x - lab2.x;
  float da = lab1.y - lab2.y;
  float db = lab1.z - lab2.z;
  return sqrt(dL * dL + da * da + db * db);
}

// TODO: Consider CIELab to RGB if needed for debugging or other effects.
// For this specific task, we only need RGB -> CIELab and DeltaE. 