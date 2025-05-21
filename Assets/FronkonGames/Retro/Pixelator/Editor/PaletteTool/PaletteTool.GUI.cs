////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Martin Bustos @FronkonGames <fronkongames@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of
// the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.Pixelator
{
  /// <summary>
  /// Color palette search tool for Unity.
  /// Allows users to browse, search, and apply color palettes in their projects.
  /// </summary>
  public partial class PaletteTool : EditorWindow
  {
    /// <summary>
    /// Renders the search interface elements.
    /// Includes a text field for search input and a search button.
    /// </summary>
    private void SearchGUI()
    {
      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal(GUILayout.Height(20));
      {
        GUILayout.Space(10);

        // Search text field
        searchText = GUILayout.TextField(searchText, GUILayout.ExpandWidth(true), GUILayout.Height(18));

        // Trigger search on Enter key press
        if (Event.current.isKey == true && Event.current.keyCode == KeyCode.Return && string.IsNullOrEmpty(searchText) == false)
        {
          SearchPalettes();
          this.Repaint();
        }

        GUILayout.Space(5);

        // Search button with magnifying glass icon
        if (string.IsNullOrEmpty(searchText) == false && GUILayout.Button(EditorGUIUtility.IconContent("d_Search Icon"), GUILayout.Width(55), GUILayout.Height(18)) == true)
          SearchPalettes();

        GUILayout.Space(10);
      }
      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Renders the palette list container.
    /// Handles pagination and displays filtered palettes.
    /// </summary>
    private void PalettesGUI()
    {
      GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
      {
        if (filtered.Count == 0)
          GUILayout.FlexibleSpace(); // Center empty state vertically
        else
        {
          // Calculate total pages based on palette count and palettes per page
          pageCount = filtered.Count / PalettesPerPage;

          // Display palettes for the current page
          for (int i = pageCurrent * PalettesPerPage; i < filtered.Count && i - (pageCurrent * PalettesPerPage) < PalettesPerPage; ++i)
            PaletteGUI(filtered[i]);
        }
      }
      GUILayout.EndVertical();
    }

    /// <summary>
    /// Renders a single palette entry with its colors.
    /// Displays the palette name, favorite stars, and color swatches.
    /// </summary>
    /// <param name="palette">The palette to display</param>
    private void PaletteGUI(Palette palette)
    {
      GUILayout.BeginHorizontal("box", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true), GUILayout.MaxHeight(200));
      {
        GUILayout.BeginVertical();
        {
          // Palette header with name and favorite stars
          GUILayout.BeginHorizontal();
          {
            if (GUILayout.Button(palette.name) == true)
            {
              UniversalRenderPipelineAsset pipeline = ((UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline);
              FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
              if (propertyInfo != null)
              {
                ScriptableRendererData[] scriptableRendererDatas = (ScriptableRendererData[])propertyInfo?.GetValue(pipeline);
                for (int i = 0; i < scriptableRendererDatas.Length; ++i)
                {
                  for (int j = 0; j < scriptableRendererDatas[i].rendererFeatures.Count; ++j)
                  {
                    if (scriptableRendererDatas[i].rendererFeatures[j] is Pixelator pixelator)
                    {
                      Pixelator.Settings settings = pixelator.settings;

                      Color[] colors = new Color[5];
                      for (int k = 0; k < 5; ++k)
                        colors[k] = ParseHtmlColor(palette.colors[k]);

                      System.Array.Sort(colors, Luminance.Compare);

                      settings.gradient = new Gradient()
                      {
                        colorKeys = new GradientColorKey[]
                        {
                          new(colors[0], 0.0f),
                          new(colors[1], 0.25f),
                          new(colors[2], 0.5f),
                          new(colors[3], 0.75f),
                          new(colors[4], 1.0f),
                        }
                      };

                      RepaintInspectors();
                    }
                  }
                }
              }
            }
#if false
            GUILayout.Space(5);

            if (GUILayout.Button("copy", EditorStyles.miniButton) == true)
            {
              string[] colors = new string[5];
              for (int i = 0; i < 5; ++i)
              {
                ColorUtility.TryParseHtmlString(palette.colors[i], out Color color);
                colors[i] = $"new({color.r}, {color.g}, {color.b})";
              }

              EditorGUIUtility.systemCopyBuffer = string.Join(",", colors);
            }
#endif
            GUILayout.FlexibleSpace();

            // Display favorite stars (filled or empty)
            for (int i = 0; i < 5; ++i)
              GUILayout.Label(i < palette.favorites ? "★" : "☆");
          }
          GUILayout.EndHorizontal();

          // Color swatches row
          GUILayout.BeginHorizontal();
          {
            // Display up to 5 colors from the palette
            for (int i = 0; i < 5 && i < palette.colors.Length; ++i)
            {
              Color color = ParseHtmlColor(palette.colors[i]);

              GUILayout.BeginVertical();
              {
                // Create an expandable area for the color swatch
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                }
                GUILayout.EndHorizontal();

                // Get the rectangle for the color swatch
                Rect rect = GUILayoutUtility.GetLastRect();

                // Draw the color swatch
                GUI.DrawTexture(rect, MakeTexture((int)rect.width, (int)rect.height, color));

                // Display the color hex value as a button
                ColorLabelGUI(palette.colors[i].ToUpper());
              }
              GUILayout.EndVertical();
            }
          }
          GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
      }
      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Renders a clickable label for a color.
    /// When clicked, copies the color hex value to the clipboard.
    /// </summary>
    /// <param name="color">The hex color code to display</param>
    private void ColorLabelGUI(string color)
    {
      GUILayout.BeginHorizontal();
      {
        GUILayout.FlexibleSpace();

        // Button that copies the color hex to clipboard when clicked
        if (GUILayout.Button(color, EditorStyles.miniButton) == true)
          EditorGUIUtility.systemCopyBuffer = color;

        GUILayout.FlexibleSpace();
      }
      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Renders the footer with pagination controls and attribution.
    /// Only displays pagination if there are multiple pages.
    /// </summary>
    private void FooterGUI()
    {
      GUILayout.BeginHorizontal();
      {
        if (filtered.Count > 1)
        {
          GUILayout.FlexibleSpace();

          // First page button
          if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.FirstKey"), GUILayout.Height(16)) == true)
            pageCurrent = 0;

          // Previous page button
          if (GUILayout.Button(EditorGUIUtility.IconContent("Profiler.PrevFrame"), GUILayout.Height(16)) == true && pageCurrent > 0)
            pageCurrent--;

          // Page indicator
          GUILayout.Label($"{pageCurrent + 1} / {pageCount}", EditorStyles.boldLabel);

          // Next page button
          if (GUILayout.Button(EditorGUIUtility.IconContent("Profiler.NextFrame"), GUILayout.Height(16)) == true && pageCurrent < pageCount)
            pageCurrent++;

          // Last page button
          if (GUILayout.Button(EditorGUIUtility.IconContent("Animation.LastKey"), GUILayout.Height(16)) == true)
            pageCurrent = pageCount;

          GUILayout.FlexibleSpace();

          // Attribution link
          if (GUILayout.Button("maintained by Fronkon Games", EditorStyles.linkLabel) == true)
            Application.OpenURL("https://fronkongames.github.io");
        }
      }
      GUILayout.EndHorizontal();

      EditorGUILayout.Separator();
    }

    /// <summary>
    /// Main GUI rendering method called by Unity.
    /// Organizes the layout of all UI components.
    /// </summary>
    private void OnGUI()
    {
      GUILayout.BeginVertical();
      {
        SearchGUI();    // Search bar at the top
        PalettesGUI();  // Palette list in the middle
        FooterGUI();    // Pagination and attribution at the bottom
      }
      GUILayout.EndVertical();
    }
  }
}
