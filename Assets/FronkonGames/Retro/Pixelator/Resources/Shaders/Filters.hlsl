#pragma once

#if _FILTERS

float _FiltersIntensity;

float _SepiaIntensity;
float _CoolBlueIntensity;
float _WarmFilterIntensity;
float _InvertColorIntensity;
float _HudsonIntensity;
float _HefeIntensity;
float _XProIntensity;
float _RiseIntensity;
float _ToasterIntensity;
float _IRFilterIntensity;
float _ThermalFilterIntensity;
float _DuotoneIntensity;
float3 _DuotoneColorA;
float3 _DuotoneColorB;
float _NightVisionIntensity;
float _PopArtIntensity;
float _PsychedelicIntensity;
float _PsychedelicHueShiftSpeed;
float _PsychedelicWarpAmount;
float _BlueprintIntensity;
float3 _BlueprintEdgeColor;
float3 _BlueprintBackgroundColor;
float _BlueprintEdgeThreshold;

float3 ApplySepia(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;
  float gray = dot(pixel, float3(0.299, 0.587, 0.114));
  float3 sepiaColor = float3(gray * 1.2, gray * 1.0, gray * 0.8);
  return lerp(pixel, saturate(sepiaColor), intensity);
}

float3 ApplyCoolBlue(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;
  float3 coolTint = float3(0.8, 0.9, 1.15); // Emphasize blue, slightly reduce red
  return lerp(pixel, saturate(pixel * coolTint), intensity);
}

float3 ApplyWarmFilter(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;
  float3 warmTint = float3(1.15, 0.9, 0.8); // Emphasize red, slightly reduce blue
  return lerp(pixel, saturate(pixel * warmTint), intensity);
}

float3 ApplyInvertColor(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;
  float3 invertedColor = 1.0 - pixel.rgb;
  return lerp(pixel, invertedColor, intensity);
}

// Overlay blend mode
// Base: The original color
// Blend: The color to blend with (e.g., our gradient mask)
float3 OverlayBlend(float3 base, float3 blend)
{
  float3 result;
  // Simplified check: if blend is < 0.5 (darker range)
  // HLSL does not have component-wise conditional, so we do it for luminance or average
  // Using average for simplicity here as true luminance needs specific weights.
  float blendLuminance = (blend.r + blend.g + blend.b) / 3.0;

  if (blendLuminance < 0.5)
      result = 2.0 * base * blend;
  else
      result = 1.0 - 2.0 * (1.0 - base) * (1.0 - blend);

  return saturate(result);
}

float3 ApplyHudson(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  // 1. Radial gradient for vignette (center brighter, edges darker/bluer)
  float2 center = float2(0.5, 0.5);
  float dist = distance(uv, center);

  // vignetteFactor: 0 at center (dist < 0.2), 1 at edges (dist > 0.7)
  float vignetteAmount = smoothstep(0.2, 0.7, dist);
  
  // Color for the vignette overlay: a cool, somewhat desaturated blue
  float3 vignetteColor = float3(0.4, 0.5, 0.7); // A bit like a light blue filter

  // The layer to blend: for overlay, 0.5 is neutral. 
  // We want the center to be mostly unaffected (close to 0.5) and edges to be vignetteColor.
  // So, we lerp from a neutral gray (0.5) to the vignetteColor based on vignetteAmount.
  float3 blendLayer = lerp(float3(0.5, 0.5, 0.5), vignetteColor, vignetteAmount);

  // 2. Apply some general color adjustments for Hudson (cooler, slightly desaturated)
  float3 processedPixel = pixel;
  processedPixel.b *= 1.1; // Boost blue
  processedPixel.r *= 0.9; // Reduce red
  processedPixel.g *= 0.95; // Slightly reduce green
  
  // Desaturate slightly
  float luma = dot(processedPixel, float3(0.299, 0.587, 0.114));
  processedPixel = lerp(processedPixel, float3(luma, luma, luma), 0.15); // 15% desaturation

  // 3. Apply overlay blend with the vignette
  float3 hudsonEffect = OverlayBlend(processedPixel, blendLayer);
  
  return lerp(pixel, saturate(hudsonEffect), intensity);
}

// Random function (simple version for static noise)
float rand(float2 co)
{
  return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
}

float3 ApplyHefe(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  float3 processedPixel = pixel;

  // Vignette
  float2 center = float2(0.5, 0.5);
  float dist = distance(uv, center);
  float vignette = smoothstep(0.8, 0.3, dist); // Stronger vignette, darkens edges
  processedPixel *= vignette;

  // Over-exposed feel: slightly increase brightness and contrast
  processedPixel = (processedPixel - 0.5) * 1.1 + 0.5; // Contrast
  processedPixel += 0.05; // Brightness

  // Noise - more at edges (less where vignette is strong)
  float noiseAmount = rand(uv * _ScreenParams.xy * 0.01) * 0.1 * (1.0 - vignette * 0.5); // Modulate noise by inverted vignette
  processedPixel += noiseAmount;

  return lerp(pixel, saturate(processedPixel), intensity);
}

float3 ApplyXPro(float3 pixel, float2 uv, float intensity)
{
    if (intensity == 0.0) return pixel;

    float3 processedPixel = pixel;

    // High saturation
    float luma = dot(processedPixel, float3(0.299, 0.587, 0.114));
    processedPixel = lerp(float3(luma, luma, luma), processedPixel, 1.8); // Saturate by 1.8

    // Vignette (darken corners)
    float2 center = float2(0.5, 0.5);
    float dist = distance(uv, center);
    float vignette = smoothstep(0.7, 0.2, dist);
    processedPixel *= lerp(0.6, 1.0, vignette); // Darken edges to 60%

    // Slight contrast boost
    processedPixel = (processedPixel - 0.5) * 1.15 + 0.5;

    return lerp(pixel, saturate(processedPixel), intensity);
}

float3 ApplyRise(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  float3 processedPixel = pixel;

  // Reduce saturation
  float luma = dot(processedPixel, float3(0.299, 0.587, 0.114));
  processedPixel = lerp(processedPixel, float3(luma, luma, luma), 0.4); // Desaturate, keep 40% color

  // Add red and yellow tint (warmth)
  processedPixel.r *= 1.15;
  processedPixel.g *= 1.05;
  // processedPixel.b *= 0.9; // Optionally reduce blue a bit

  // Slight brightness increase
  processedPixel += 0.05;

  return lerp(pixel, saturate(processedPixel), intensity);
}

float3 ApplyToaster(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  float3 processedPixel = pixel;

  // Red to purple tint overlay
  // Center of screen gets a warmer (orangey-red) tint, edges more magenta/purple
  float2 center = float2(0.5, 0.5);
  float dist = distance(uv, center);
  
  float3 centerColor = float3(0.9, 0.5, 0.3); // Orangey-Red
  float3 edgeColor = float3(0.7, 0.3, 0.8);   // Magenta/Purple

  float3 tint = lerp(centerColor, edgeColor, smoothstep(0.1, 0.6, dist));    
  processedPixel *= tint; // Apply as a multiply blend for a strong effect

  // Vignette - more pronounced
  float vignette = smoothstep(0.6, 0.1, dist);
  processedPixel = lerp(processedPixel * 0.5, processedPixel, vignette);

  return lerp(pixel, saturate(processedPixel), intensity);
}

float3 ApplyIRFilter(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;

  // Emphasize red, desaturate others, darken blue for sky effect
  float r = pixel.r * 1.5; // Boost red for foliage
  float g = pixel.g * 0.7;
  float b = pixel.b * 0.3; // Darken blue for sky & water

  // A common IR simulation technique: make it mostly monochrome based on a biased mix
  // then reintroduce strong red influences.
  float gray = (r * 0.6 + g * 0.3 + b * 0.1); // Biased luminance, favors boosted red
  float3 irPixel = float3(gray + r * 0.4, gray, gray - b * 0.2); // Re-introduce strong red, pull back blue influence

  // Increase contrast
  irPixel = (irPixel - 0.5) * 1.5 + 0.5; // Contrast boost

  // Optional: slight overall desaturation to lean towards B&W IR
  float finalLuma = dot(irPixel, float3(0.299, 0.587, 0.114));
  irPixel = lerp(irPixel, float3(finalLuma, finalLuma, finalLuma), 0.2);

  return lerp(pixel, saturate(irPixel), intensity);
}

float3 ApplyThermalFilter(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;

  float luma = dot(pixel, float3(0.299, 0.587, 0.114));

  // Optional: Posterize luminance for more distinct bands
  float posterizedLuma = floor(luma * 10.0) / 10.0; // 10 bands

  float3 thermalColor;
  // Gradient: Black -> Dark Blue -> Blue -> Green -> Yellow -> Red -> White
  // These are example colors and thresholds, can be tweaked.
  float3 c0 = float3(0.0, 0.0, 0.0);    // Black
  float3 c1 = float3(0.0, 0.0, 0.5);    // Dark Blue
  float3 c2 = float3(0.0, 0.2, 1.0);    // Blue
  float3 c3 = float3(0.0, 0.8, 0.2);    // Green
  float3 c4 = float3(1.0, 1.0, 0.0);    // Yellow
  float3 c5 = float3(1.0, 0.2, 0.0);    // Red
  float3 c6 = float3(1.0, 1.0, 1.0);    // White

  if (posterizedLuma < 0.15)
      thermalColor = lerp(c0, c1, posterizedLuma / 0.15);
  else if (posterizedLuma < 0.3)
      thermalColor = lerp(c1, c2, (posterizedLuma - 0.15) / 0.15);
  else if (posterizedLuma < 0.45)
      thermalColor = lerp(c2, c3, (posterizedLuma - 0.3) / 0.15);
  else if (posterizedLuma < 0.6)
      thermalColor = lerp(c3, c4, (posterizedLuma - 0.45) / 0.15);
  else if (posterizedLuma < 0.8)
      thermalColor = lerp(c4, c5, (posterizedLuma - 0.6) / 0.2);
  else
      thermalColor = lerp(c5, c6, (posterizedLuma - 0.8) / 0.2);

  return lerp(pixel, thermalColor, intensity);
}

float3 ApplyDuotoneFilter(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;

  float luma = dot(pixel, float3(0.299, 0.587, 0.114));
  float3 duotoneColor = lerp(_DuotoneColorA, _DuotoneColorB, luma);
  
  return lerp(pixel, duotoneColor, intensity);
}

float3 ApplyNightVisionFilter(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  float luma = dot(pixel, float3(0.299, 0.587, 0.114));
  float3 nightVisionColor = float3(luma * 0.4, luma * 1.1, luma * 0.4); // Green tint

  // Add some noise
  float noise = (rand(uv * _Time.y) - 0.5) * 0.15; // Time-based random noise
  nightVisionColor += noise;
  
  // Optional: Scanlines
  // float scanline = sin(uv.y * 300.0) * 0.05;
  // nightVisionColor -= scanline;

  // Optional: Vignette for goggle effect
  float2 center = float2(0.5, 0.5);
  float dist = distance(uv, center);
  float vignette = smoothstep(0.7, 0.4, dist);
  nightVisionColor *= vignette;
  
  // Boost brightness slightly
  nightVisionColor = saturate(nightVisionColor * 1.2);

  return lerp(pixel, nightVisionColor, intensity);
}

float3 ApplyPopArtFilter(float3 pixel, float intensity)
{
  if (intensity == 0.0) return pixel;

  float luma = dot(pixel, float3(0.299, 0.587, 0.114));

  // Define Pop Art palette (example)
  float3 color1 = float3(0.9, 0.9, 0.1); // Bright Yellow
  float3 color2 = float3(0.9, 0.1, 0.5); // Magenta/Pink
  float3 color3 = float3(0.1, 0.9, 0.9); // Cyan
  float3 color4 = float3(0.1, 0.1, 0.3); // Dark Blue/Purple (for shadows)

  float3 popArtColor;
  // Posterize into 4 levels and map to colors
  if (luma > 0.75)
      popArtColor = color1;
  else if (luma > 0.5)
      popArtColor = color2;
  else if (luma > 0.25)
      popArtColor = color3;
  else
      popArtColor = color4;

  return lerp(pixel, popArtColor, intensity);
}

float3 ApplyBlueprintFilter(float3 pixel, float2 uv, float intensity)
{
  if (intensity == 0.0) return pixel;

  float2 texelSize = _MainTex_TexelSize.xy;

  // Sobel operator kernels
  float Gx[9] = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
  float Gy[9] = {-1, -2, -1, 0, 0, 0, 1, 2, 1};

  float sumGx = 0.0;
  float sumGy = 0.0;

  // Sample 3x3 neighborhood
  for (int i = -1; i <= 1; i++)
  {
    for (int j = -1; j <= 1; j++)
    {
      float3 neighborPixel = SAMPLE_MAIN_LOD(uv + float2(i, j) * texelSize).rgb;
      float neighborLuma = Luminance(neighborPixel);
      int kernelIndex = (i + 1) * 3 + (j + 1);
      sumGx += Gx[kernelIndex] * neighborLuma;
      sumGy += Gy[kernelIndex] * neighborLuma;
    }
  }

  float edgeMagnitude = sqrt(sumGx * sumGx + sumGy * sumGy);

  float3 blueprintColor = lerp(_BlueprintBackgroundColor, _BlueprintEdgeColor, step(_BlueprintEdgeThreshold, edgeMagnitude));
  
  return lerp(pixel, blueprintColor, intensity);
}

float3 Filters(float3 pixel, float2 uv)
{
  float3 originalPixel = pixel;
  float3 processedPixel = pixel;

  processedPixel = ApplySepia(processedPixel, _SepiaIntensity);
  processedPixel = ApplyCoolBlue(processedPixel, _CoolBlueIntensity);
  processedPixel = ApplyWarmFilter(processedPixel, _WarmFilterIntensity);
  processedPixel = ApplyInvertColor(processedPixel, _InvertColorIntensity);
  processedPixel = ApplyHudson(processedPixel, uv, _HudsonIntensity);
  processedPixel = ApplyHefe(processedPixel, uv, _HefeIntensity);
  processedPixel = ApplyXPro(processedPixel, uv, _XProIntensity);
  processedPixel = ApplyRise(processedPixel, uv, _RiseIntensity);
  processedPixel = ApplyToaster(processedPixel, uv, _ToasterIntensity);
  processedPixel = ApplyIRFilter(processedPixel, _IRFilterIntensity);
  processedPixel = ApplyThermalFilter(processedPixel, _ThermalFilterIntensity);
  processedPixel = ApplyDuotoneFilter(processedPixel, _DuotoneIntensity);
  processedPixel = ApplyNightVisionFilter(processedPixel, uv, _NightVisionIntensity);
  processedPixel = ApplyPopArtFilter(processedPixel, _PopArtIntensity);
  processedPixel = ApplyBlueprintFilter(processedPixel, uv, _BlueprintIntensity);

  return lerp(originalPixel, processedPixel, _FiltersIntensity);
}

#else

inline float3 Filters(float3 pixel, float2 uv)
{
  return pixel;
}

#endif

