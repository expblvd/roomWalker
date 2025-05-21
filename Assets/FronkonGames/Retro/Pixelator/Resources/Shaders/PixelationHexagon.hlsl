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

#if _PIXELATION_MODE_HEXAGON

float2 _PixelScale;
float _AspectRatio;

float2 NearestHexagon(float s, float2 st)
{
  const float h = 0.5 * s;
  const float r = 0.8660254 * s;
  const float b = s + 2.0 * h;
  const float a = 2.0 * r;
  const float m = h / r;
  
  float2 sect = st / float2(2.0 * r, h + s);
  float2 sectPxl = fmod(st, float2(2.0 * r, h + s));
  
  float aSection = fmod(floor(sect.y), 2.0);
  
  float2 coord = floor(sect);
  if (aSection > 0.0)
  {
    if (sectPxl.y < (h - sectPxl.x * m))
      coord -= 1.0;
    else if (sectPxl.y < (-h + sectPxl.x * m))
      coord.y -= 1.0;
  }
  else
  {
    if (sectPxl.x > r)
    {
      if (sectPxl.y < (2.0 * h - sectPxl.x * m))
        coord.y -= 1.0;
    }
    else
    {
      if (sectPxl.y < (sectPxl.x * m))
        coord.y -= 1.0;
      else
        coord.x -= 1.0;
    }
  }
  
  float xoff = fmod(coord.y, 2.0) * r;

  return float2(coord.x * 2.0 * r - xoff, coord.y * (h + s)) + float2(r * 2.0, s);
}

float3 PixelationHexagon(float3 pixel, float2 uv)
{
  float2 ratio = float2(_AspectRatio * _PixelScale.x, _PixelScale.y);

  float2 nearest = NearestHexagon(_PixelSize, uv * ratio);

  return SampleMain(nearest / ratio);
}

#endif