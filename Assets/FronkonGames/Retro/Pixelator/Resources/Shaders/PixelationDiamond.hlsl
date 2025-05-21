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

#if _PIXELATION_MODE_DIAMOND

float3 PixelationDiamond(float3 pixel, float2 uv)
{
  const float2 pixelSize = 10.0 / _PixelSize;
  float2 coord = uv * pixelSize;
  
  const uint direction = int(dot(frac(coord), float2(1, 1)) >= 1.0) + 2 * int(dot(frac(coord), float2(1, -1)) >= 0.0);
  coord = floor(coord);
  
  if (direction == 0) coord += float2(0.0, 0.5);
  if (direction == 1) coord += float2(0.5, 1.0);
  if (direction == 2) coord += float2(0.5, 0.0);
  if (direction == 3) coord += float2(1.0, 0.5);

  return SampleMain(coord / pixelSize);
}

#endif