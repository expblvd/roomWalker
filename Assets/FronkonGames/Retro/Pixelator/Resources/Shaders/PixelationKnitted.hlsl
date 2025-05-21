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

#if _PIXELATION_MODE_KNITTED

float2 _PixelScale;
uint _Threads;

float3 PixelationKnitted(float3 pixel, float2 uv)
{
  float2 tileSize = _PixelSize * _PixelScale;
  float2 coord = uv * _ScreenParams.xy;

  float2 posInTile = mod(coord, tileSize);
  float2 tileNum = floor(coord / tileSize);  

  float2 nrmPosInTile = posInTile / tileSize;
  tileNum.y += floor(abs(nrmPosInTile.x - 0.5) + nrmPosInTile.y);

  float2 tileUV = tileNum * tileSize / _ScreenParams.xy;

  pixel = SampleMain(tileUV).rgb;
  pixel *= frac((nrmPosInTile.y + abs(nrmPosInTile.x - 0.5)) * floor(_Threads));

  return pixel;
}

#endif