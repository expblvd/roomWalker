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

#if _PIXELATION_MODE_LEAF

float2 _PixelScale;
float _AspectRatio;

float3 PixelationLeaf(float3 pixel, float2 uv)
{
  float2 pixelScale = _PixelSize * float2(_PixelScale.x, _PixelScale.y / _AspectRatio);
  float2 coord = floor(uv * pixelScale) / pixelScale;
  uv -= coord;
  uv *= pixelScale;

  coord += float2(step(1.0 - uv.y, uv.x) / (pixelScale.x),
                  step(uv.x, uv.y) / (pixelScale.y));

  return SampleMain(coord);
}

#endif