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

#if _PIXELATION_MODE_LED

float _Radius;
float4 _Background;
float _AspectRatio;

inline float2 LedUV(half2 uv)
{
  float pixelScale = 1.0 / _PixelSize;

  return float2(pixelScale * floor(uv.x / (pixelScale)),
                (pixelScale * _AspectRatio) * floor(uv.y / (pixelScale * _AspectRatio)));
}

float3 PixelationLed(float3 pixel, float2 uv)
{
  pixel = SampleMain(LedUV(uv)).rgb;
  float2 coord = uv * float2(_PixelSize, _PixelSize / _AspectRatio);

  float ledX = abs(sin(coord.x * PI)) * 1.5;
  float ledY = abs(sin(coord.y * PI)) * 1.5;

  float ledValue = ledX * ledY;
  float radius = step(ledValue, _Radius);

  return ((1.0 - radius) * pixel) + ((pixel * ledValue) * radius) + radius * (1.0 - ledValue) * _Background;
}

#endif