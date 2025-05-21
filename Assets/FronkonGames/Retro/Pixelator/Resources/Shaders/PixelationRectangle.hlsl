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

#if _PIXELATION_MODE_RECTANGLE

float2 _PixelScale;
float _AspectRatio;

float3 PixelationRectangle(float3 pixel, float2 uv)
{
  const float pixelScale = 1.0 / _PixelSize;

  uv = float2(pixelScale * _PixelScale.x * floor(uv.x / (pixelScale * _PixelScale.x)),
             (pixelScale * _AspectRatio * _PixelScale.y) * floor(uv.y / (pixelScale * _AspectRatio * _PixelScale.y)));

  return SampleMain(uv).rgb;
}

#endif