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

float _PosterizeIntensity;
float3 _PosterizeStepsRGB;
float _PosterizeLuminanceSteps;
float3 _PosterizeStepsHSV;
float _PosterizeGamma;

// Converts an RGB color value to HSV.
// Conversion formula adapted from http://en.wikipedia.org/wiki/HSV_color_space.
// Assumes r, g, and b are contained in the set [0, 1] and
// returns h, s, and v in the set [0, 1].
float3 RGBToHSV(float3 c)
{
  float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
  float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
  float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

  float d = q.x - min(q.w, q.y);
  float e = 1.0e-10; // Epsilon
  return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

// Converts an HSV color value to RGB.
// Conversion formula adapted from http://en.wikipedia.org/wiki/HSV_color_space.
// Assumes h, s, and v are contained in the set [0, 1] and
// returns r, g, and b in the set [0, 1].
float3 HSVToRGB(float3 c)
{
  float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
  float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
  return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

// Posterizes a single float value.
float PosterizeValue(float value, float steps)
{
  if (steps <= 1.0) return value; // Avoid division by zero or no change
  return floor(value * steps) / steps;
}

// Applies all posterization passes sequentially.
float3 ApplyPosterizationPasses(float3 color, // uv is kept for potential future use or consistency
                                float3 stepsRGBToUse,
                                float stepsLuminanceToUse,
                                float3 stepsHSVToUse)
{
  float3 processedColor = color;

  // RGB Posterization
  processedColor.r = PosterizeValue(processedColor.r, stepsRGBToUse.r);
  processedColor.g = PosterizeValue(processedColor.g, stepsRGBToUse.g);
  processedColor.b = PosterizeValue(processedColor.b, stepsRGBToUse.b);

  // Luminance Posterization
  float luma = dot(processedColor.rgb, float3(0.2126, 0.7152, 0.0722));
  float posterizedLuma = PosterizeValue(luma, stepsLuminanceToUse);
  if (luma > 1.0e-5) // Avoid division by zero
    processedColor = processedColor * (posterizedLuma / luma);
  else
    processedColor = float3(posterizedLuma, posterizedLuma, posterizedLuma); // Fallback to grayscale

  // HSV Posterization
  float3 hsv = RGBToHSV(processedColor);
  hsv.x = PosterizeValue(hsv.x, stepsHSVToUse.x); // Hue
  hsv.y = PosterizeValue(hsv.y, stepsHSVToUse.y); // Saturation
  hsv.z = PosterizeValue(hsv.z, stepsHSVToUse.z); // Value
  hsv.x = frac(hsv.x); // Ensure Hue wraps around
  processedColor = HSVToRGB(hsv);

  return saturate(processedColor);
}

float3 Posterize(float3 pixel, float2 uv)
{
  float3 originalColor = pixel;
  pixel = pow(abs(pixel), _PosterizeGamma);

  float3 calculatedColor = ApplyPosterizationPasses(pixel,
                                                    _PosterizeStepsRGB,
                                                    _PosterizeLuminanceSteps,
                                                    _PosterizeStepsHSV);

  calculatedColor = pow(calculatedColor, 1.0 / _PosterizeGamma);

  return lerp(originalColor, calculatedColor, _PosterizeIntensity);
}

