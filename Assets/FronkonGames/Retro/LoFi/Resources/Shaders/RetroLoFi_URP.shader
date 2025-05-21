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
Shader "Hidden/Fronkon Games/Retro/LoFi URP"
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
      Name "Fronkon Games Retro Lo-Fi"

      HLSLPROGRAM
      #pragma vertex RetroVert
      #pragma fragment RetroFrag
      #pragma fragmentoption ARB_precision_hint_fastest
      #pragma exclude_renderers d3d9 d3d11_9x ps3 flash
      #pragma multi_compile _ USE_PALETTE
      #pragma multi_compile _ USE_LUMINANCE_SAMPLE USE_DISTANCE_SAMPLE USE_HSV_SAMPLE USE_DOMINANT_SAMPLE
      #pragma multi_compile _ USE_PIXELATE
      #pragma multi_compile _ USE_SOBEL
      #pragma multi_compile _ USE_QUANTIZATION
      #pragma multi_compile _ USE_BORDER

      #include "Retro.hlsl"
      #include "ColorBlend.hlsl"

      float _Vignette;
      float _Shine;
      float _ShineSize;
      float _Curvature;
      float _Aperture;
      float _Scanline;
      float _ScanlineCount;
      float _ScanlineSpeed;

      float2 CurveUV(float2 uv)
      {
        uv = (uv - 0.5) * 2.0;
        uv *= _Aperture;
        uv.x *= 1.0 + pow((abs(uv.y) / 2.5 * _Curvature), 2.0);
        uv.y *= 1.0 + pow((abs(uv.x) / 2.0 * _Curvature), 2.0);
        uv = (uv / 2.0) + 0.5;
        uv = lerp(uv, uv * 0.92 + 0.04, _Curvature);

        return uv;
      }

#if USE_PALETTE
      TEXTURE2D_X(_GradientTex);
      SAMPLER(sampler_GradientTex);

      float _LuminancePow;
      float _RangeMin;
      float _RangeMax;
      float _ColorSamples;
      float _ColorThreshold;

      static const float3x3 RGB2XYZ_MAT = float3x3(
        0.4124564, 0.3575761, 0.1804375,
        0.2126729, 0.7151522, 0.0721750,
        0.0193339, 0.1191920, 0.9503041
      );
      
      float3 RGB2XYZ(float3 rgb)
      {
        float3 linearRGB;
        linearRGB.x = (rgb.x <= 0.04045) ? rgb.x / 12.92 : pow((rgb.x + 0.055) / 1.055, 2.4);
        linearRGB.y = (rgb.y <= 0.04045) ? rgb.y / 12.92 : pow((rgb.y + 0.055) / 1.055, 2.4);
        linearRGB.z = (rgb.z <= 0.04045) ? rgb.z / 12.92 : pow((rgb.z + 0.055) / 1.055, 2.4);
        
        return mul(RGB2XYZ_MAT, linearRGB);
      }
      
      float3 XYZ2Lab(float3 xyz)
      {
        const float3 REF_WHITE = float3(0.95047, 1.0, 1.08883);
        float3 normalized = xyz / REF_WHITE;
        
        float3 v;
        v.x = (normalized.x > 0.008856) ? pow(normalized.x, 1.0/3.0) : (7.787 * normalized.x) + (16.0/116.0);
        v.y = (normalized.y > 0.008856) ? pow(normalized.y, 1.0/3.0) : (7.787 * normalized.y) + (16.0/116.0);
        v.z = (normalized.z > 0.008856) ? pow(normalized.z, 1.0/3.0) : (7.787 * normalized.z) + (16.0/116.0);
        
        return float3(
          (116.0 * v.y) - 16.0,    // L*
          500.0 * (v.x - v.y),     // a*
          200.0 * (v.y - v.z)      // b*
        );
      }
#endif

      inline half3 SamplePalette(float2 uv)
      {
#if USE_PALETTE
#if USE_LUMINANCE_SAMPLE
        float luminance = RemapValue(Luminance601(SAMPLE_MAIN_LOD(uv).rgb), _RangeMin, _RangeMax);
        
        return SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_GradientTex, float2(pow(luminance, _LuminancePow), 0.5), 0).rgb;
#elif USE_DISTANCE_SAMPLE
        float bestDistance = 100000.0;
        float3 originalColor = SAMPLE_MAIN_LOD(uv).rgb;
        float3 bestColor = originalColor;

        UNITY_LOOP
        for (int i = 0; i < _ColorSamples; i++)
        {
          float3 paletteColor = SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_GradientTex, float2((i + 0.5) / _ColorSamples, 0.5), 0.0).rgb;
          
          float distance = length((originalColor - paletteColor) * float3(0.299, 0.587, 0.114));
          if (distance < bestDistance)
          {
            bestDistance = distance;
            bestColor = paletteColor;

            if (bestDistance < _ColorThreshold)
              break;
          }
        }

        return bestColor;
#elif USE_HSV_SAMPLE
        float bestDistance = 100000.0;
        float3 originalColor = SAMPLE_MAIN_LOD(uv).rgb;
        float3 bestColor = originalColor;
        
        float3 originalHSV = RGB2HSV(originalColor);

        UNITY_LOOP
        for (int i = 0; i < _ColorSamples; i++)
        {
          float3 paletteColor = SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_GradientTex, float2((i + 0.5) / _ColorSamples, 0.5), 0.0).rgb;
          float3 paletteHSV = RGB2HSV(paletteColor);
          
          float hueDist = min(abs(originalHSV.x - paletteHSV.x), 1.0 - abs(originalHSV.x - paletteHSV.x));
          float satDist = abs(originalHSV.y - paletteHSV.y);
          float valDist = abs(originalHSV.z - paletteHSV.z);
          
          float distance = hueDist * 0.6 + satDist * 0.2 + valDist * 0.2;
          if (distance < bestDistance)
          {
            bestDistance = distance;
            bestColor = paletteColor;

            if (bestDistance < _ColorThreshold)
              break;
          }
        }

        return bestColor;
#elif USE_DOMINANT_SAMPLE
        float3 dominantColor = float3(0, 0, 0);
        float maxWeight = 0.0;
        
        UNITY_LOOP
        for (int x = -1; x <= 1; x++)
        {
          for (int y = -1; y <= 1; y++)
          {
            float2 offsetUV = uv + float2(x, y) * TEXEL_SIZE.xy;
            float3 sampleColor = SAMPLE_MAIN_LOD(offsetUV).rgb;
            float luminance = Luminance601(sampleColor);
            
            float weight = luminance + 0.1;
            dominantColor += sampleColor * weight;
            maxWeight += weight;
          }
        }
        
        dominantColor = (maxWeight > 0.0) ? dominantColor / maxWeight : SAMPLE_MAIN_LOD(uv).rgb;
        
        float bestDistance = 100000.0;
        float3 bestColor = dominantColor;
        
        UNITY_LOOP
        for (int i = 0; i < _ColorSamples; i++)
        {
          float3 paletteColor = SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_GradientTex, float2((i + 0.5) / _ColorSamples, 0.5), 0.0).rgb;
          
          float distance = length((dominantColor - paletteColor) * float3(0.299, 0.587, 0.114));
          if (distance < bestDistance)
          {
            bestDistance = distance;
            bestColor = paletteColor;

            if (bestDistance < _ColorThreshold)
              break;
          }
        }
        
        return bestColor;
#else
        float bestDistance = 100000.0;
        float3 originalColor = SAMPLE_MAIN_LOD(uv).rgb;
        float3 bestColor = originalColor;
        
        float3 originalLab = XYZ2Lab(RGB2XYZ(originalColor));

        UNITY_LOOP
        for (int i = 0; i < _ColorSamples; i++)
        {
          float3 paletteColor = SAMPLE_TEXTURE2D_X_LOD(_GradientTex, sampler_GradientTex, float2((i + 0.5) / _ColorSamples, 0.5), 0.0).rgb;
          float3 paletteLab = XYZ2Lab(RGB2XYZ(paletteColor));

          float distance = length(originalLab - paletteLab);  // CIE76 formula
          if (distance < bestDistance)
          {
            bestDistance = distance;
            bestColor = paletteColor;

            if (bestDistance < _ColorThreshold)
              break;
          }
        }

        return bestColor;
#endif
#else
        return SAMPLE_MAIN_LOD(uv).rgb;
#endif
      }

      float _ChromaticAberration;

      inline half3 SampleMain(float2 uv)
      {
        float2 dir = uv - float2(0.5, 0.5);
        float angle = atan2(dir.y, dir.x);
        float2 offset = float2(cos(angle), sin(angle)) * _ChromaticAberration * TEXEL_SIZE.xy;
        float2 offsetUV = saturate(uv + offset * length(dir));

        return half3(SamplePalette(offsetUV - offset * 0.3).r,
                     SamplePalette(offsetUV).g,
                     SamplePalette(offsetUV + offset * 0.3).b);
      }

#if USE_PIXELATE
      float _Pixelate;
      float _Samples;
      float _PixelRound;
      int _PixelBlend;
      float4 _PixelTint;
      float _Bevel;
#endif
      float _PixelSize;

#if USE_QUANTIZATION
      float _Quantization;
#endif

#if USE_SOBEL
      float _PixelSobel;
      float _PixelSobelPower;
      float3 _PixelSobelLight;
      float _PixelSobelLightIntensity;
      float _PixelSobelAmbient;

      static const float SobelX[9] = {-1,  0,  1, -2, 0, 2, -1, 0, 1};
      static const float SobelY[9] = {-1, -2, -1,  0, 0, 0,  1, 2, 1};

      float3 CalculateNormal(float2 block)
      {
        float gradientX = 0;
        float gradientY = 0;
        int kernelIndex = 0;

        UNITY_LOOP
        for (int x = -1; x <= 1; x++)
        {
          UNITY_LOOP
          for (int y = -1; y <= 1; y++)
          {
            float2 neighborBlock = block + float2(x, y);
            float2 neighborUV = (neighborBlock * _PixelSize + _PixelSize * 0.5) / _ScreenParams.xy;
            float3 color = SampleMain(neighborUV);
            float luminance = dot(color, float3(0.2126, 0.7152, 0.0722));
            
            gradientX += luminance * SobelX[kernelIndex];
            gradientY += luminance * SobelY[kernelIndex];
            kernelIndex++;
          }
        }
        
        return normalize(float3(-gradientX, -gradientY, 1.0));
      }
#endif

#if USE_QUANTIZATION
      inline float Quantize(float inp, float period)
      {
        return floor((inp + period / 2.0) / period) * period;
      }
#endif

#if USE_BORDER
      float3 _BorderColor;
      float _BorderSmooth;
      float _BorderNoise;
      float2 _BorderMargins;

      float2 BorderCurve(float2 uv, float r) 
      {
        uv = (uv - 0.5) * 2.0;
        uv = r * uv / sqrt(r * r - dot(uv, uv));

        return (uv / 2.0) + 0.5;
      }

      inline float RoundSquare(float2 p, float2 b, float r)
      {
        return length(max(abs(p * _BorderMargins) - b, 0.0)) - r;
      }
#endif

      half4 RetroFrag(const RetroVaryings input) : SV_Target
      {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        const float2 uv = UnityStereoTransformScreenSpaceTex(input.texcoord).xy;
        const half4 color = SAMPLE_MAIN(uv);
        half4 pixel = color;
        float2 curvedUV = CurveUV(uv);
        const float2 coord = uv * _ScreenParams.xy;

#if !USE_BORDER
        UNITY_BRANCH
        if (curvedUV.x < 0.0 || curvedUV.x > 1.0 || curvedUV.y < 0.0 || curvedUV.y > 1.0)
          return half4(0.0, 0.0, 0.0, 1.0);
#endif
        pixel.rgb = SampleMain(curvedUV).rgb;

#if USE_PIXELATE
        pixel.rgb = 0.0;

        float tileSize = floor(_PixelSize);
        tileSize += mod(_PixelSize, 2.0);
        float2 tileNumber = floor(coord / tileSize);

        float2 pixelCenter = (tileNumber + 0.5) * tileSize;

        float2 distFromCenter = abs(coord - pixelCenter);
        float maxDist = tileSize * (1.0 - _PixelRound);

        float distToCorner = length(distFromCenter) / maxDist;
        float circleMask = smoothstep(1.0 - _PixelRound, 1.0, distToCorner);
        circleMask *= pow(circleMask, 5.0);

        UNITY_LOOP
        for (float y = 0.0; y < _Samples; ++y)
        {
          UNITY_LOOP
          for (float x = 0.0; x < _Samples; ++x)
            pixel.rgb += SampleMain((tileNumber + float2((x + 0.5) / _Samples, (y + 0.5) / _Samples)) * tileSize / _ScreenParams.xy).rgb;
        }

        pixel.rgb /= _Samples * _Samples;
        pixel.rgb *= 1.0 - circleMask;

        pixel.rgb = lerp(pixel.rgb, ColorBlend(_PixelBlend, color.rgb, pixel.rgb * _PixelTint.rgb), _PixelTint.a);

#if USE_SOBEL
        float3 normal = CalculateNormal(tileNumber);
        float diffuse = saturate(dot(normal, _PixelSobelLight));
        float lighting = _PixelSobelAmbient + (diffuse * _PixelSobelLightIntensity);
        pixel.rgb = lerp(pixel.rgb, pixel.rgb * pow(abs(lighting), _PixelSobelPower), _PixelSobel);
#endif

        const float lum = Luminance(pixel.rgb);
        float dp = clamp(ddx(lum) * _Bevel * 10.0, 0.0, 1.0);
        dp += clamp(ddy(-lum) * _Bevel * 10.0, 0.0, 1.0);
        pixel.rgb += pixel.rgb * dp * 2.0;

        dp = clamp(ddx(-lum) * _Bevel * 10.0, 0.0, 1.0);
        dp += clamp(ddy(lum) * _Bevel * 10.0, 0.0, 1.0);
        pixel.rgb -= dp * 1.0;
#endif

#if USE_QUANTIZATION
        const float3 quantizationPeriod = (float3)(1.0 / (_Quantization - 1.0));
        pixel.rgb = float3(Quantize(pixel.r, quantizationPeriod.r),
                           Quantize(pixel.g, quantizationPeriod.g),
                           Quantize(pixel.b, quantizationPeriod.b));        
#endif

        float scanLineFX = (sin(curvedUV.y * _ScanlineCount + _Time.y * _ScanlineSpeed) + 1.0) / 2.0;
        scanLineFX = clamp(pow(scanLineFX, 4.0), 0.3, 1.0);
        pixel.rgb *= lerp(1.0, 1.0 + scanLineFX, _Scanline);

        float vignette = curvedUV.x * curvedUV.y * (1.0 - curvedUV.x) * (1.0 - curvedUV.y);
        vignette = clamp(pow(abs(128.0 * vignette), _Vignette), 0.0, 1.0);
        pixel.rgb = lerp(pixel.rgb, pixel.rgb * vignette, 1.0);

        pixel.rgb += max(0.0, _ShineSize - distance(curvedUV, float2(0.5, 1.0))) * _Shine;

#if USE_BORDER
        float3 border = clamp(_BorderColor + Rand(curvedUV + 1.0) * _BorderNoise - 0.025, 0.0, 1.0);

        float frame = smoothstep(-_BorderSmooth, _BorderSmooth, RoundSquare(curvedUV - float2(0.5, 0.5), 0.4, 0.05)) *
                      smoothstep(-_BorderSmooth, _BorderSmooth, RoundSquare(curvedUV - float2(0.5, 0.5), 0.4 + 0.05, 0.05));
        pixel.rgb = lerp(pixel.rgb, border * frame, frame);

        pixel.rgb += (border - 0.4) * 
       	  smoothstep(-_BorderSmooth * 2.0, _BorderSmooth * 2.0, RoundSquare(curvedUV - float2(0.5, 0.495), 0.46, 0.05)) * 
          smoothstep(_BorderSmooth * 2.0, -_BorderSmooth * 2.0, RoundSquare(curvedUV - float2(0.5, 0.505), 0.46, 0.05)); 

        pixel.rgb += (border + 0.1) *
          smoothstep(-_BorderSmooth * 2.0, _BorderSmooth * 2.0, RoundSquare(curvedUV - float2(0.5, 0.505), 0.46, 0.05)) * 
          smoothstep(_BorderSmooth * 2.0, -_BorderSmooth * 2.0, RoundSquare(curvedUV - float2(0.5, 0.495), 0.46, 0.05));
#endif

        pixel.rgb = ColorAdjust(pixel.rgb, _Contrast, _Brightness, _Hue, _Gamma, _Saturation);

#if 0
        pixel.rgb = PixelDemo(color.rgb, pixel.rgb, uv);
#endif
        return lerp(color, pixel, _Intensity);
      }

      ENDHLSL
    }    
  }
  
  FallBack "Diffuse"
}
