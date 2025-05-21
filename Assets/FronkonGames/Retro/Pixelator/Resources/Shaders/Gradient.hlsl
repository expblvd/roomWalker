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

#include "ColorSpace.hlsl"

#if _GRADIENT

float _GradientIntensity;
TEXTURE2D_X(_GradientTex);
float _LuminanceMin;
float _LuminanceMax;
uint _GradientCIELabSamples;

inline half3 Gradient(float3 pixel)
{
  float3 mappedColor;
  float luminance = Luminance(pixel);

#if _GRADIENT_CIELAB
  float3 pixel_lab = RgbLinearToCIELab(pixel);

  float minDeltaE = 100000.0;
  float3 bestGradientColorLinear = pixel;

  int numSamples = max(2, _GradientCIELabSamples);
  for (int i = 0; i < numSamples; ++i)
  {
    float u = (float)i / (float)(numSamples - 1);
    float3 gradSampleSrgb = SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_LinearClamp, float2(u, 0.5), 0).rgb;
    float3 gradSampleLinear = SrgbToLinear(gradSampleSrgb);
    float3 gradSampleLab = RgbLinearToCIELab(gradSampleLinear);
    
    float deltaE = CIELabDistance(pixel_lab, gradSampleLab);
    if (deltaE < minDeltaE)
    {
      minDeltaE = deltaE;
      bestGradientColorLinear = gradSampleLinear;
    }
  }
  mappedColor = bestGradientColorLinear;
#if _GRADIENT_APPLY_LUMINANCE
  mappedColor *= luminance;
#endif
#else
  float normLuminance = clamp((1.0 / max(0.0001, _LuminanceMax - _LuminanceMin)) * (luminance - _LuminanceMin), 0.0, 1.0);
  float3 gradientSampleLinear = SAMPLE_TEXTURE2D_X(_GradientTex, sampler_LinearClamp, float2(normLuminance, 0.5)).rgb;

  mappedColor = gradientSampleLinear;
#if _GRADIENT_APPLY_LUMINANCE
  mappedColor *= normLuminance;
#endif
#endif

  return lerp(pixel, mappedColor, _GradientIntensity);
}

#else

inline half3 Gradient(float3 pixel)
{
  return pixel;
}

#endif
