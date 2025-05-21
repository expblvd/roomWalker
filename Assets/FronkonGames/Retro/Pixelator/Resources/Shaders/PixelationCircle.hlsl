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

#if _PIXELATION_MODE_CIRCLE

float _Radius;
float4 _Background;

float3 PixelationCircle(float3 pixel, float2 uv)
{
  const float pixelScale = 1.0 / _PixelSize;
  const float ratio = _ScreenParams.y / _ScreenParams.x;

  uv.x = uv.x / ratio;
  
  const float2 coord = float2(floor(uv.x / pixelScale), floor(uv.y / pixelScale));
  float2 center = coord * pixelScale + pixelScale * 0.5;
  const float dist = length(uv - center) * _PixelSize;
  center.x *= ratio;

  pixel = SampleMain(center).rgb;

  UNITY_BRANCH
  if (dist > _Radius)
    pixel = lerp(pixel, _Background.rgb, _Background.a);

  return pixel;
}

#endif