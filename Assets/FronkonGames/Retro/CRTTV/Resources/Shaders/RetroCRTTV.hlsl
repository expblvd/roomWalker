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

#include "Retro.hlsl"

int _Frame;
float _ShadowmaskStrength;
float _ShadowmaskLuminosity;
float _ShadowmaskScale;
float3 _ShadowmaskColorOffset;
float _ShadowmaskVerticalGapHardness;
float _ShadowmaskHorizontalGapHardness;
float _FishEyeStrength;
float2 _FishEyeZoom;
float _Distortion;
float _DistortionSpeed;
float _DistortionAmplitude;
float _VignetteSmoothness;
float _VignetteRounding;
float2 _VignetteBorders;
float2 _VignetteSize;
float _ShineStrength;
float2 _ShinePosition;
float4 _ShineColor;
float _RGBOffsetStrength;
float _ColorBleedingStrength;
float _ColorBleedingDistance;
float4 _ColorCurves;
float4 _GammaColor;
float _Scanlines;
float _ScanlinesCount;
float _ScanlinesVelocity;
float _InterferenceStrength;
float _InterferencePeakStrength;
float _InterferencePeakPosition;
float _ShakeStrength;
float _ShakeRate;
float _MovementStrength;
float _MovementRate;
float _MovementSpeed;
float _Grain;
float _StaticNoise;
float _BarStrength;
float _BarHeight;
float _BarSpeed;
float _BarOverflow;
float _FlickerStrength;
float _FlickerSpeed;

float2 OnOff(float2 uv)
{
  uv -= (float2)0.5;

  float heightMul = max(1.0, 200.0 - pow(_Time.y, 1.5) * 200.0);
  heightMul += sin(_Time.y * 10.0) * max(0.0, 0.5 - pow(_Time.y, 4.0) * 0.2);
  uv.y *= heightMul;

  float widthMul = max(1.0, 200.0 - pow(_Time.y, 1.5) * 500.0);
  widthMul += sin(_Time.y * 10.0) * max(0.0, 0.5 - pow(_Time.y, 4.0) * 0.9);
  uv.x *= widthMul;

  uv += (float2)0.5;

  return uv;
}

float2 FishEye(const float2 uv)
{
  const float2 center = uv.xy - float2(0.5, 0.5);
  const float CdotC = dot(center, center);
  const float f = 1.0 + CdotC * (_FishEyeStrength * sqrt(CdotC));

  return f * _FishEyeZoom * center + 0.5;
}

float3 Shine(const float2 uv)
{
  return max(0.0, _ShineStrength - distance(uv, _ShinePosition)) * _ShineColor.rgb;
}

float Peak(const float x, const float xPos, const float scale)
{
  return clamp((1.0 - x) * scale * log(1.0 / abs(x - xPos)), 0.0, 1.0);
}

float2 Interference(float2 uv)
{
  const float r = Rand(float2(_Time.y, round(uv.y * _ScreenParams.y)));
  const float i1 = (16.0 * _InterferenceStrength) / _ScreenParams.x * r;
  const float i2 = (0.04 * _InterferenceStrength) * (r * Peak(uv.y, _InterferencePeakPosition, _InterferencePeakStrength));
  uv.x += i1 + -i2;

  return uv;
}

float2 FrameMovement(float2 uv)
{
  // Frame shaking.
  const float shakeRate = 1.0f - _ShakeRate; 
  const float shake1 = (1.0 - step(snoise(float2(_Time.y * 1.5, 5.0)), shakeRate)) * _ShakeStrength;
  const float shake2 = (1.0 - step(snoise(float2(_Time.y * 5.5, 5.0)), shakeRate * 0.5)) * _ShakeStrength;

  // Frame movement.
  const float movementRate = 1.0 - _MovementRate;
  const float movement = (1.0 - step(snoise(float2(_Time.y * _MovementSpeed, 8.0)), movementRate)) * _MovementStrength;
  const float offset = abs(sin(_Time.y) * 4.0) * movement + shake1 * shake2 * 0.3;

  uv.y = mod(uv.y + offset, 1.0);

  return uv;
}

float3 RGBOffset(const float2 uv, const float x)
{
  float3 pixel;
  pixel.r = SAMPLE_MAIN(float2(x + uv.x + 0.001 + (_RGBOffsetStrength * -0.01), uv.y + 0.001)).x + 0.05;
  pixel.g = SAMPLE_MAIN(float2(x + uv.x + 0.000, uv.y - 0.002)).y + 0.05;
  pixel.b = SAMPLE_MAIN(float2(x + uv.x - 0.002 + (_RGBOffsetStrength * 0.01), uv.y + 0.000)).z + 0.05;

  return pixel;
}

float3 ColorBleeding(const float2 uv, const float x)
{
  float3 pixel;
  pixel.r = _ColorBleedingStrength * 0.16 * SAMPLE_MAIN(_ColorBleedingDistance * float2(x + 0.025, -0.027) + float2(uv.x + 0.001, uv.y + 0.001)).x;
  pixel.g = _ColorBleedingStrength * 0.1 * SAMPLE_MAIN(_ColorBleedingDistance * float2(x + -0.022, -0.02) + float2(uv.x + 0.000, uv.y - 0.002)).y;
  pixel.b = _ColorBleedingStrength * 0.16 * SAMPLE_MAIN(_ColorBleedingDistance * float2(x + -0.02, -0.018) + float2(uv.x - 0.002, uv.y + 0.000)).z;

  return pixel;
}

float Vignette(float2 uv)
{
  uv = (uv - 0.5) * (2.0 + _VignetteBorders);
  const float amount = 1.0 - sqrt(pow(abs(uv.x), _VignetteRounding * 50.0) + pow(abs(uv.y), _VignetteRounding * 50.0));

  return smoothstep(0.0, _VignetteSmoothness, amount * 1.0);
}

float3 GammaTint(const float3 color, const float3 tint, const float q)
{
  return pow(color, 1.0 / tint) * q + (1.0 - q) * color;
}

float StaticNoise(float2 uv)
{
  const float staticHeight = snoise(float2(9.0, _Time.y * 1.2 + 3.0)) * 0.3 + 5.0;
  const float staticAmount = snoise(float2(1.0, _Time.y * 1.2 - 6.0)) * 0.1 + 0.3;
  const float staticStrength = snoise(float2(-9.75, _Time.y * 0.6 - 3.0)) * 2.0 + 2.0;

  return (1.0 - step(snoise(float2(5.0 * pow(abs(_Time).y, 2.0) + pow(abs(uv.x * 7.0), 1.2),
                                         pow(abs((mod(_Time.y, 100.0) + 100.0) * uv.y * 0.2 + 3.0), staticHeight))),
                            staticAmount)) * staticStrength;
}

float Grille(float x, float offset, float multiplier)
{
  return smoothstep(0.0, 1.0, sin(x * TWO_PI) * multiplier + offset);    
}

float3 ShadowMask(float2 uv)
{
  const float3 cols = float3(Grille(uv.x + _ShadowmaskColorOffset.x, -0.02, _ShadowmaskVerticalGapHardness),
                             Grille(uv.x + _ShadowmaskColorOffset.y, -0.02, _ShadowmaskVerticalGapHardness),
                             Grille(uv.x + _ShadowmaskColorOffset.z, -0.02, _ShadowmaskVerticalGapHardness));    

  uv.x *= 0.5;
  uv.x -= round(uv.x);
  if (uv.x < 0.0)
    uv.y += 0.5;

  const float rows = Grille(uv.y, 1.0, _ShadowmaskHorizontalGapHardness);
  
  return cols * rows;
}

half4 CRTTV(half4 color, float2 uv)
{
  float4 pixel = color;

  uv -= 0.5;
  uv *= 1.0;
  uv += 0.5;

  // Fisheye.
  const float2 fishEyeUV = FishEye(uv);

  // Interference.
  float2 curvedUV = Interference(fishEyeUV);

  // Frame movement.
  curvedUV = FrameMovement(curvedUV);

  // Distortion.
  const float distortionTime = _Time.y * _DistortionSpeed;
  const float x = sin(0.3 *        distortionTime + uv.y * (_DistortionAmplitude + 1.0)) *
                  sin(0.7 *        distortionTime + uv.y * (_DistortionAmplitude + 9.0)) *
                  sin(0.3 + 0.33 * distortionTime + uv.y * (_DistortionAmplitude + 11.0)) * _Distortion;

  // RGB offset.
  pixel.rgb = lerp(SAMPLE_MAIN(curvedUV).rgb, RGBOffset(curvedUV, x), _RGBOffsetStrength);

  // Color bleeding.
  pixel.rgb += ColorBleeding(curvedUV, x);

  // Level adjustment (curves).
  pixel.rgb *= lerp((float3)1.0, _ColorCurves.rgb, _ColorCurves.a);
  pixel.rgb = clamp(pixel.rgb * 1.3 + 0.75 * pixel.rgb * pixel.rgb + 1.25 * pixel.rgb * pixel.rgb * pixel.rgb * pixel.rgb * pixel.rgb, 0.0, 1.0);

  // Scanlines.
  float scanlines = clamp(0.35 + 0.35 * sin(5.0 * _ScanlinesVelocity * _Time.y + curvedUV.y * _ScreenParams.y * _ScanlinesCount), 0.0, 1.0) * _Scanlines;
  scanlines = pow(abs(scanlines), 1.7);
  pixel.rgb *= (float3)(0.4 + 0.7 * scanlines);

  // Gamma tint.
  pixel.rgb = lerp(pixel.rgb, GammaTint(pixel.rgb, _GammaColor.rgb, 1.0), _GammaColor.a);

  // Grain.
  const float2 seed = curvedUV * _ScreenParams.xy;
  const float3 grain = pow(float3(Rand(seed + _Time.y),
                                  Rand(seed + _Time.y * 2.0),
                                  Rand(seed + _Time.y * 3.0)), (float3)1.5);
  pixel.rgb -= grain * _Grain;

  // Static noise.
  pixel.rgb += (float3)StaticNoise(float2(curvedUV.x, curvedUV.y + -0.01)) * _StaticNoise * 0.1;

  // Bar effect
  const float bar = clamp(floor(sin(curvedUV.y * (10.0 - _BarHeight) + _Time.y * _BarSpeed) + 1.95), 0.0, _BarOverflow);
  pixel.rgb = lerp(pixel.rgb - _BarStrength * (float3)0.08, pixel.rgb, bar);

  // Flicker.
  pixel.rgb *= 1.0 - _FlickerStrength * (sin(_FlickerSpeed * _Time.y + curvedUV.y * 2.0) * 0.5 + 0.5);

  // Shadow mask.
  _ShadowmaskScale = 0.25 - (_ShadowmaskScale * 0.25f) + 0.01;
  pixel.rgb *= lerp((float3)1.0, ShadowMask(uv * _ScreenParams.xy * _ShadowmaskScale) * _ShadowmaskLuminosity, _ShadowmaskStrength);

  // Shine.
  pixel.rgb += lerp((float3)0.0, Shine(fishEyeUV), _ShineColor.a);

  // Vignette.
  pixel.rgb *= Vignette(fishEyeUV);

  return pixel;
}
