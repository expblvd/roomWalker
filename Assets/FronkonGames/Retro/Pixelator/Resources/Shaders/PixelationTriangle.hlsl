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

#if _PIXELATION_MODE_TRIANGLE

float2 _PixelScale;
float _AspectRatio;

float3 PixelationTriangle(float3 pixel, float2 uv)
{
  float2 tri = float2(1.0, 0.5 * _AspectRatio) * _PixelSize;
  tri *= _PixelScale;

  float2 uv2 = floor(uv * tri) / tri;
  uv -= uv2;
  uv *= tri;

  return SampleMain(uv2 + float2(step(1.0 - uv.y, uv.x) / (2.0 * tri.x),
        							           step(uv.x, uv.y) / (2.0 * tri.y)));
}

#endif