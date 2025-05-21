# Retro: Pixelator

Transform your game's visuals with Pixelator, a powerful post-processing effect that brings retro-style pixelation and stunning visual filters to your Unity projects.

From classic pixel art aesthetics to modern stylized looks, Pixelator offers extensive customization options including multiple pixelation patterns, dithering, color grading, and special effects - all optimized for the Universal Render Pipeline.

With access to over 6000 carefully curated color palettes, you can easily find and apply authentic retro color schemes to your game. The built-in palette browser lets you search, preview and test different palettes to achieve the perfect look for your project.

Whether you're creating a retro-inspired game or adding unique visual flair to your modern title, Pixelator delivers professional quality results with minimal performance impact.

> Come with me if you want to pixelate.

## Key Features

- Multiple pixelation modes: Rectangle, Circle, Triangle, Diamond, Hexagon, Leaf, LED, and Knitted patterns
- Customizable pixel size and aspect ratio
- Gradient overlay effects with adjustable intensity and luminance range
- Bevel effects for depth and dimension
- Dithering with adjustable pattern scale, threshold, and color steps
- Chromatic aberration effects
- Posterization with RGB, Luminance, and HSV modes
- Wide range of color filters including:
  - Sepia
  - Cool Blue
  - Warm Filter
  - Invert Colors
  - Hudson
  - Hefe
  - X-Pro
  - Rise
  - Toaster
  - IR Filter
  - Thermal Vision
  - Duotone
  - Night Vision
  - Pop Art
  - Blueprint
- Full control over brightness, contrast, gamma, hue and saturation
- Scene view support
- Profiling capabilities
- Flexible render pass event timing


## Requirements

- Unity 6000.0 or newer (Render Graph support)
- Unity 2022.3 or newer
- Universal Render Pipeline (URP)


## How to Use

Add the Pixelator Renderer Feature to your Universal Render Pipeline Asset:
 - Select your URP Asset in the Project window
 - Click "Add Renderer Feature" and select "Pixelator"

## Code Example

```chsarp
// Get the Pixelator renderer feature
var pixelator = Pixelator.Instance;

if (pixelator != null)
{
  // Basic setup
  pixelator.settings.intensity = 1.0f;
  pixelator.settings.pixelationMode = PixelationModes.Rectangle;
  pixelator.settings.pixelSize = 0.75f;
  
  // Add some effects
  pixelator.settings.bevel = 0.5f;
  pixelator.settings.ditherIntensity = 0.25f;
  pixelator.settings.posterizeIntensity = 0.5f;
  
  // Apply color adjustments
  pixelator.settings.brightness = 0.1f;
  pixelator.settings.contrast = 1.2f;
  pixelator.settings.saturation = 1.1f;
  
  // Enable a filter
  pixelator.settings.filtersIntensity = 1.0f;
  pixelator.settings.sepiaIntensity = 0.75f;
}
```


## Demo Scene

The included demo scene (`Assets/FronkonGames/Retro/Pixelator/Demo/Pixelator_Demo.scene`) provides an interactive environment to test all the features of Retro: Pixelator. Use the on-screen UI to switch between presets and adjust individual parameters to see their effects in real-time.


## Support

If you have any questions, issues, or feature requests, please contact: [fronkongames@gmail.com](mailto:fronkongames@gmail.com)

If you find this asset useful, please consider leaving a review on the Unity Asset Store.