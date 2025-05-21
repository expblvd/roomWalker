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
Shader "Hidden/Fronkon Games/Retro/Pixelator URP"
{
  Properties
  {
    _MainTex("Main Texture", 2D) = "white" {}
  }

  SubShader
  {
    Tags
    {
      "RenderType" = "Opaque"
      "RenderPipeline" = "UniversalPipeline"
    }
    LOD 100
    ZTest Always ZWrite Off Cull Off

    Pass
    {
      Name "Fronkon Games Retro Pixelator"

      HLSLPROGRAM
      #include "Retro.hlsl"
      
      #pragma vertex RetroVert
      #pragma fragment RetroFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ _USE_DRAW_PROCEDURAL
      #pragma multi_compile _ _GRADIENT
      #pragma multi_compile _ _GRADIENT_CIELAB
      #pragma multi_compile _ _GRADIENT_APPLY_LUMINANCE
      #pragma multi_compile _ _BEVEL
      #pragma multi_compile _ _CHROMATIC_ABERRATION
      #pragma multi_compile _ _DITHER
      #pragma multi_compile _ _POSTERIZE
      #pragma multi_compile _ _FILTERS
      #pragma multi_compile _ _PIXELATION_MODE_RECTANGLE \
                              _PIXELATION_MODE_HEXAGON \
                              _PIXELATION_MODE_DIAMOND \
                              _PIXELATION_MODE_CIRCLE \
                              _PIXELATION_MODE_TRIANGLE \
                              _PIXELATION_MODE_LEAF \
                              _PIXELATION_MODE_KNITTED \
                              _PIXELATION_MODE_LED

      float _PixelSize;

      #include "Gradient.hlsl"
      #include "ChromaticAberration.hlsl"
      #include "Dither.hlsl"
      #include "Bevel.hlsl"
      #include "Posterize.hlsl"
      #include "Filters.hlsl"

      #include "PixelationRectangle.hlsl"
      #include "PixelationCircle.hlsl"
      #include "PixelationTriangle.hlsl"
      #include "PixelationDiamond.hlsl"
      #include "PixelationHexagon.hlsl"
      #include "PixelationLeaf.hlsl"
      #include "PixelationLed.hlsl"
      #include "PixelationKnitted.hlsl"

      half4 RetroFrag(RetroVaryings input) : SV_Target
      {
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half3 color = SAMPLE_MAIN(uv).rgb;
        half3 pixel = color;

        float2 pixelatedUV = uv;
#if _PIXELATION_MODE_RECTANGLE
        pixel = PixelationRectangle(pixel, uv);
#elif _PIXELATION_MODE_CIRCLE
        pixel = PixelationCircle(pixel, uv);
#elif _PIXELATION_MODE_TRIANGLE
        pixel = PixelationTriangle(pixel, uv);
#elif _PIXELATION_MODE_DIAMOND
        pixel = PixelationDiamond(pixel, uv);
#elif _PIXELATION_MODE_HEXAGON
        pixel = PixelationHexagon(pixel, uv);
#elif _PIXELATION_MODE_LEAF
        pixel = PixelationLeaf(pixel, uv);
#elif _PIXELATION_MODE_LED
        pixel = PixelationLed(pixel, uv);
#elif _PIXELATION_MODE_KNITTED
        pixel = PixelationKnitted(pixel, uv);
#endif

        pixel.rgb = Dither(pixel.rgb, uv);

        pixel.rgb = Posterize(pixel.rgb, uv);

        pixel.rgb = Bevel(pixel.rgb, uv);

        pixel.rgb = Gradient(pixel.rgb);

        pixel.rgb = Filters(pixel.rgb, uv);

        // Color adjust.
        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif

        return float4(lerp(color.rgb, pixel.rgb, _Intensity), 1.0);
      }      
      ENDHLSL
    }
  }
  
  FallBack "Diffuse"
}