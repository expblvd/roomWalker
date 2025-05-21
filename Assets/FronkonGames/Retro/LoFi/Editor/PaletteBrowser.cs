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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace FronkonGames.Retro.LoFi
{
  /// <summary> Palette browser window. </summary>
  public class PaletteBrowser : EditorWindow
  {
    public static void ShowWindow()
    {
      PaletteBrowser window = GetWindow<PaletteBrowser>("Palette Browser");
      window.minSize = new Vector2(600, 400);
      window.Show();
    }

    private Dictionary<string, int> tagCounts = new();
    private readonly List<string> selectedTags = new();
    private Vector2 tagScrollPosition;
    private bool showTagFilters = false;
    private GUIStyle tagButtonStyle;
    private GUIStyle tagButtonSelectedStyle;

    private List<LoFiProfile> allPalettes;
    private List<LoFiProfile> filteredPalettes;
    private int currentPage;
    private readonly int itemsPerPage = 5;
    private string searchQuery = string.Empty;
    private SortOption sortOption = SortOption.LikesDescending;

    private GUIStyle searchField;
    private GUIStyle paletteNameStyle;
    private GUIStyle likesStyle;

    private void OnEnable()
    {
      LoadAllPalettes();

      searchField = new GUIStyle(EditorStyles.toolbarSearchField);

      paletteNameStyle = new GUIStyle(EditorStyles.boldLabel)
      {
        fontSize = 12,
        alignment = TextAnchor.MiddleLeft
      };

      likesStyle = new GUIStyle(EditorStyles.miniLabel)
      {
        alignment = TextAnchor.MiddleRight
      };

      tagButtonStyle = new GUIStyle(EditorStyles.miniButton)
      {
        normal = { textColor = new Color(0.5f, 0.5f, 0.5f) },
        margin = new RectOffset(2, 2, 2, 2),
        padding = new RectOffset(4, 4, 2, 2),
        fontSize = 10
      };

      tagButtonSelectedStyle = new GUIStyle(tagButtonStyle)
      {
        normal = {
          textColor = Color.white,
          background = EditorGUIUtility.Load("IN BigTitle") as Texture2D
        }
      };
    }

    private void OnGUI()
    {
      DrawToolbar();
      
      EditorGUILayout.Space(5);
      
      if (allPalettes == null || allPalettes.Count == 0)
      {
        EditorGUILayout.HelpBox("No palettes found. Make sure you have palette assets in your project.", MessageType.Info);
        return;
      }
      
      DrawPalettes();
      
      DrawPagination();

      EditorGUILayout.Space(5);
    }

    private void DrawPalettes()
    {
      int startIndex = currentPage * itemsPerPage;
      int endIndex = Mathf.Min(startIndex + itemsPerPage, filteredPalettes.Count);

      float availableHeight = position.height - (showTagFilters ? 170.0f : 60.0f);
      float paletteHeight = availableHeight / itemsPerPage;
      
      for (int i = startIndex; i < endIndex; i++)
      {
        LoFiProfile palette = filteredPalettes[i];
        DrawPaletteItem(palette, paletteHeight);
      }
    }

    private void DrawPaletteItem(LoFiProfile palette, float height)
    {
      EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(height));

      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField(palette.title, paletteNameStyle);
      GUILayout.FlexibleSpace();
      EditorGUILayout.LabelField($"♥ {palette.likes}", likesStyle, GUILayout.Width(50));
      EditorGUILayout.EndHorizontal();

      // Calculate color display height (subtract header and button heights)
      float colorDisplayHeight = height - 50;
      Rect colorsRect = EditorGUILayout.GetControlRect(false, colorDisplayHeight);
      DrawPaletteColors(colorsRect, palette);

      EditorGUILayout.BeginHorizontal();
      GUILayout.FlexibleSpace();

      if (GUILayout.Button("Use", GUILayout.Width(80)) == true)
        ApplyPalette(palette);

      EditorGUILayout.EndHorizontal();

      EditorGUILayout.EndVertical();
    }

    private void DrawPaletteColors(Rect rect, LoFiProfile palette)
    {
      if (palette.colors == null || palette.colors.Length == 0)
        return;

      float colorWidth = rect.width / Mathf.Min(palette.colors.Length, 16);

      for (int i = 0; i < palette.colors.Length && i < 16; i++)
      {
        Rect colorRect = new(rect.x + (i * colorWidth), rect.y, colorWidth, rect.height);
        EditorGUI.DrawRect(colorRect, palette.colors[i]);

        Rect borderRect = new(colorRect.x, colorRect.y, colorRect.width, colorRect.height);
        Color borderColor = Color.black;
        borderColor.a = i % 2 == 0 ? 0.5f : 0.75f;

        if (i > 0)
        {
          Rect leftBorder = new(borderRect.x, borderRect.y, 1, borderRect.height);
          EditorGUI.DrawRect(leftBorder, borderColor);
        }
      }
    }

    private void DrawPagination()
    {
      if (filteredPalettes.Count <= itemsPerPage)
        return;

      int totalPages = Mathf.CeilToInt((float)filteredPalettes.Count / itemsPerPage);

      EditorGUILayout.BeginHorizontal();

      GUILayout.FlexibleSpace();

      GUI.enabled = currentPage > 0;
      if (GUILayout.Button("◄ Previous", GUILayout.Width(100)))
        currentPage--;

      GUILayout.Label($"Page {currentPage + 1} of {totalPages}", EditorStyles.centeredGreyMiniLabel,
        GUILayout.Width(100));

      GUI.enabled = currentPage < totalPages - 1;
      if (GUILayout.Button("Next ►", GUILayout.Width(100)))
        currentPage++;

      GUI.enabled = true;

      GUILayout.FlexibleSpace();

      EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
      EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
      
      GUILayout.FlexibleSpace();
      
      sortOption = (SortOption)EditorGUILayout.EnumPopup("Sort by:", sortOption);
      if (GUILayout.Button("Apply Sort", EditorStyles.toolbarButton, GUILayout.Width(80)))
        SortPalettes();
      
      GUILayout.FlexibleSpace();

      EditorGUI.BeginChangeCheck();
      searchQuery = GUILayout.TextField(searchQuery, searchField, GUILayout.Width(200));
      if (EditorGUI.EndChangeCheck())
      {
        currentPage = 0;
        FilterPalettes();
      }

      if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
      {
        searchQuery = string.Empty;
        currentPage = 0;
        FilterPalettes();
      }

      GUILayout.FlexibleSpace();

      showTagFilters = GUILayout.Toggle(showTagFilters, "Show Tag Filters", EditorStyles.toolbarButton, GUILayout.Width(120));
      
      EditorGUILayout.EndHorizontal();
      
      if (showTagFilters == true)
        DrawTagFilters();
    }
    
    private void DrawTagFilters()
    {
      EditorGUILayout.BeginVertical(EditorStyles.helpBox);
      
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Filter by Tags:", EditorStyles.boldLabel);
      
      GUILayout.FlexibleSpace();
      
      if (GUILayout.Button("Clear All", EditorStyles.miniButton, GUILayout.Width(70)))
      {
        selectedTags.Clear();
        FilterPalettes();
      }
      EditorGUILayout.EndHorizontal();
      
      tagScrollPosition = EditorGUILayout.BeginScrollView(tagScrollPosition, GUILayout.Height(80));
      
      EditorGUILayout.BeginHorizontal();
      float rowWidth = 0;
      float maxWidth = EditorGUIUtility.currentViewWidth - 40;
      
      var sortedTags = tagCounts.OrderByDescending(t => t.Value).ToList();
      
      foreach (var tagPair in sortedTags)
      {
        string tag = tagPair.Key;
        int count = tagPair.Value;
        
        float buttonWidth = tagButtonStyle.CalcSize(new GUIContent($"{tag} ({count})")).x + 10;
        
        if (rowWidth + buttonWidth > maxWidth)
        {
          EditorGUILayout.EndHorizontal();
          EditorGUILayout.BeginHorizontal();
          rowWidth = 0;
        }
        
        bool isSelected = selectedTags.Contains(tag);
        GUIStyle style = isSelected ? tagButtonSelectedStyle : tagButtonStyle;
        
        if (GUILayout.Button($"{tag} ({count})", style, GUILayout.Width(buttonWidth)))
        {
          if (isSelected)
            selectedTags.Remove(tag);
          else
            selectedTags.Add(tag);
            
          FilterPalettes();
        }
        
        rowWidth += buttonWidth + 5;
      }
      
      EditorGUILayout.EndHorizontal();
      EditorGUILayout.EndScrollView();
      EditorGUILayout.EndVertical();
    }

    private void LoadAllPalettes()
    {
      string[] guids = AssetDatabase.FindAssets("t:LoFiProfile");
      allPalettes = new List<LoFiProfile>();
      tagCounts = new Dictionary<string, int>();
      
      foreach (string guid in guids)
      {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        LoFiProfile palette = AssetDatabase.LoadAssetAtPath<LoFiProfile>(path);
        
        if (palette != null)
        {
          allPalettes.Add(palette);
          
          if (palette.tags != null)
          {
            foreach (string tag in palette.tags)
            {
              if (tagCounts.ContainsKey(tag))
                tagCounts[tag]++;
              else
                tagCounts[tag] = 1;
            }
          }
        }
      }
      
      FilterPalettes();
      SortPalettes();
    }

    private void FilterPalettes()
    {
      filteredPalettes = new List<LoFiProfile>(allPalettes);
      
      if (!string.IsNullOrEmpty(searchQuery))
      {
        string query = searchQuery.ToLowerInvariant();
        filteredPalettes = filteredPalettes.Where(p => 
          p.title.ToLowerInvariant().Contains(query) || 
          (p.tags != null && p.tags.Any(t => t.ToLowerInvariant().Contains(query)))
        ).ToList();
      }
      
      if (selectedTags.Count > 0)
      {
        filteredPalettes = filteredPalettes.Where(p => 
          p.tags != null && selectedTags.All(tag => p.tags.Contains(tag))
        ).ToList();
      }
      
      SortPalettes();
    }

    private void SortPalettes()
    {
      switch (sortOption)
      {
        case SortOption.NameAscending:
          filteredPalettes = filteredPalettes.OrderBy(p => p.title).ToList();
          break;
        case SortOption.NameDescending:
          filteredPalettes = filteredPalettes.OrderByDescending(p => p.title).ToList();
          break;
        case SortOption.LikesAscending:
          filteredPalettes = filteredPalettes.OrderBy(p => p.likes).ToList();
          break;
        case SortOption.LikesDescending:
          filteredPalettes = filteredPalettes.OrderByDescending(p => p.likes).ToList();
          break;
        case SortOption.ColorCountAscending:
          filteredPalettes = filteredPalettes.OrderBy(p => p.colors != null ? p.colors.Length : 0).ToList();
          break;
        case SortOption.ColorCountDescending:
          filteredPalettes = filteredPalettes.OrderByDescending(p => p.colors != null ? p.colors.Length : 0).ToList();
          break;
      }
    }

    private void ApplyPalette(LoFiProfile palette)
    {
      UniversalRenderPipelineAsset pipeline = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
      FieldInfo propertyInfo = pipeline.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
      bool applied = false;

      if (propertyInfo != null)
      {
        ScriptableRendererData[] scriptableRendererDatas = (ScriptableRendererData[])propertyInfo?.GetValue(pipeline);
        for (int i = 0; i < scriptableRendererDatas.Length; ++i)
        {
          for (int j = 0; j < scriptableRendererDatas[i].rendererFeatures.Count; ++j)
          {
            if (scriptableRendererDatas[i].rendererFeatures[j] is LoFi loFi)
            {
              LoFi.Settings settings = loFi.settings;
              settings.profile = palette;
              settings.mustUpdateTexture = true;
              applied = true;

              EditorUtility.SetDirty(loFi);
            }
          }
        }
      }

      if (applied == false)
        EditorUtility.DisplayDialog("No LoFi Effect Found", "Could not find an active LoFi effect in the scene. Add a Renderer Feature with LoFi effect first.", "OK");
    }

    private enum SortOption
    {
      NameAscending,
      NameDescending,
      LikesAscending,
      LikesDescending,
      ColorCountAscending,
      ColorCountDescending
    }
  }
}