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

#if _CHROMATIC_ABERRATION

float _ChromaticAberrationIntensity;
float3 _ChromaticAberrationOffset;

inline half3 SampleMain(float2 uv)
{
  _ChromaticAberrationOffset *= _ChromaticAberrationIntensity;

  return half3(SAMPLE_MAIN(uv + _ChromaticAberrationOffset.r * TEXEL_SIZE.xy).r,
    SAMPLE_MAIN(uv + _ChromaticAberrationOffset.g * TEXEL_SIZE.xy).g,
    SAMPLE_MAIN(uv + _ChromaticAberrationOffset.b * TEXEL_SIZE.xy).b);
}


#else

inline half3 SampleMain(float2 uv)
{
  return SAMPLE_MAIN(uv).rgb;
}

#endif
