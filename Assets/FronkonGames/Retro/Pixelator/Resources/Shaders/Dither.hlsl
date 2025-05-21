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

#if _DITHER

float _DitherIntensity;
float _DitherThresholdScale;
int _DitherPatternScale;
int _DitherColorSteps;

static const float Bayer2x2[4] =
{
  0.0/4.0, 2.0/4.0,
  3.0/4.0, 1.0/4.0
};

static const float Bayer4x4[16] =
{
   0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
  12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
   3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
  15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
};

static const float Bayer8x8[64] =
{
   0.0/64.0, 32.0/64.0,  8.0/64.0, 40.0/64.0,  2.0/64.0, 34.0/64.0, 10.0/64.0, 42.0/64.0,
  48.0/64.0, 16.0/64.0, 56.0/64.0, 24.0/64.0, 50.0/64.0, 18.0/64.0, 58.0/64.0, 26.0/64.0,
  12.0/64.0, 44.0/64.0,  4.0/64.0, 36.0/64.0, 14.0/64.0, 46.0/64.0,  6.0/64.0, 38.0/64.0,
  60.0/64.0, 28.0/64.0, 52.0/64.0, 20.0/64.0, 62.0/64.0, 30.0/64.0, 54.0/64.0, 22.0/64.0,
   3.0/64.0, 35.0/64.0, 11.0/64.0, 43.0/64.0,  1.0/64.0, 33.0/64.0,  9.0/64.0, 41.0/64.0,
  51.0/64.0, 19.0/64.0, 59.0/64.0, 27.0/64.0, 49.0/64.0, 17.0/64.0, 57.0/64.0, 25.0/64.0,
  15.0/64.0, 47.0/64.0,  7.0/64.0, 39.0/64.0, 13.0/64.0, 45.0/64.0,  5.0/64.0, 37.0/64.0,
  63.0/64.0, 31.0/64.0, 55.0/64.0, 23.0/64.0, 61.0/64.0, 29.0/64.0, 53.0/64.0, 21.0/64.0
};

float GetBayerValue(float2 screenUV, int matrixDimension)
{
  float scalingFactor = _ScreenParams.y / 1080.0;

  scalingFactor = clamp(scalingFactor, 0.1, 10.0);
  if (scalingFactor < 0.01)
      scalingFactor = 1.0;

  float currentPixelX = screenUV.x * _ScreenParams.x;
  float currentPixelY = screenUV.y * _ScreenParams.y;

  float scaledPixelX = currentPixelX / scalingFactor;
  float scaledPixelY = currentPixelY / scalingFactor;

  int ix = (int)fmod(scaledPixelX, (float)matrixDimension);
  int iy = (int)fmod(scaledPixelY, (float)matrixDimension);

  if (matrixDimension == 2)
    return Bayer2x2[iy * 2 + ix];
  else if (matrixDimension == 4)
    return Bayer4x4[iy * 4 + ix];
  else if (matrixDimension == 8)
    return Bayer8x8[iy * 8 + ix];
  else
    return 0.0;
}

float3 Dither(float3 pixel, float2 uv)
{
  float bayer = GetBayerValue(uv, _DitherPatternScale);

  float3 colorStepped = floor(pixel * (_DitherColorSteps - 1));
  float3 ditheredColor = floor(pixel * (_DitherColorSteps - 1) + bayer * _DitherThresholdScale);
  ditheredColor /= (_DitherColorSteps - 1);
  
  return lerp(pixel, saturate(ditheredColor), _DitherIntensity);
}

#else

inline float3 Dither(float3 pixel, float2 uv)
{
  return pixel;
}

#endif