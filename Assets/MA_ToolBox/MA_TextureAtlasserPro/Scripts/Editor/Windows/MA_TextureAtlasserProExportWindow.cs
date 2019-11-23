﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MA_Editor;
using MA_Texture;

namespace MA_TextureAtlasserPro
{
    public class MA_TextureAtlasserProExportWindow : EditorWindow
    {
        private const int windowHeight = 300;
        private const int windowWidth = 320;

        //Editor
        private static MA_TextureAtlasserProExportWindow thisWindow;
        public static MA_TextureAtlasserProWindow curWindow;

        //Data
        private static bool isLoaded = false;       //Make sure we wait a frame at the start to setup and don't draw.

        [MenuItem("MA_ToolKit/MA_TextureAtlasserPro/Export Atlas")]	
		private static void Init()
        {
            GetCurrentWindow();

            thisWindow.minSize = new Vector2(windowWidth, windowHeight);
            thisWindow.titleContent.text = "MA_ExportTextureAtlas";

            thisWindow.Show();
        }

		public static void InitEditorWindow(MA_TextureAtlasserProWindow currentEditorWindow)
		{
			curWindow = currentEditorWindow;

			GetCurrentWindow();

			thisWindow.minSize = new Vector2(windowWidth, windowHeight);
			thisWindow.titleContent.text = "MA_ExportTextureAtlas";

			thisWindow.Show();
		}

		private static void GetCurrentWindow()
		{
			thisWindow = (MA_TextureAtlasserProExportWindow)EditorWindow.GetWindow<MA_TextureAtlasserProExportWindow>();
		}

		private void CloseWindow()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
				thisWindow.Close();
			}
			else
			{
				thisWindow.Close();
			}
		}

		private Event ProcessEvents()
		{
			Event e = Event.current;

			return e;
		}

		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnReload()
		{
			//Make sure that when the compiler is finished and reloads the scripts, we are waiting for the next Event.
			isLoaded = false;
		}

		private void OnGUI()
		{
			if(thisWindow == null)
			{
				GetCurrentWindow();
				return;
			}

			//Get current event
            Event e = ProcessEvents();

			if(isLoaded)
			{
				GUILayout.BeginArea(new Rect(MA_TextureAtlasserProUtils.VIEW_OFFSET, MA_TextureAtlasserProUtils.VIEW_OFFSET, position.width - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2), position.height - (MA_TextureAtlasserProUtils.VIEW_OFFSET * 2)));
				GUILayout.BeginVertical();

				
				if(curWindow != null && curWindow.textureAtlas != null)
				{
					//Export
					GUILayout.BeginVertical();
                    DrawExportMenu();

                    curWindow.textureAtlas.exportSettings = (MA_TextureAtlasserProExportSettings)EditorGUILayout.ObjectField(curWindow.textureAtlas.exportSettings, typeof(MA_TextureAtlasserProExportSettings), false);

                    if(curWindow.textureAtlas.exportSettings != null)
                    {
                        DrawExportAdvancedOptions();
                    }

					GUILayout.EndVertical();
				}
				else if(curWindow == null)
				{
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: Link with the Texture Atlas Editor lost!", EditorStyles.helpBox);
					if(GUILayout.Button("Link Atlas Editor", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
					{
						curWindow = (MA_TextureAtlasserProWindow)EditorWindow.GetWindow<MA_TextureAtlasserProWindow>();
					}
					GUI.backgroundColor = Color.white;
				}
				else if(curWindow.textureAtlas == null)
				{
					GUI.backgroundColor = Color.red;
					GUILayout.Box("Error: No Texture Atlas found make sure to open one!", EditorStyles.helpBox);
					GUI.backgroundColor = Color.white;
				}

				GUILayout.EndVertical();
				GUILayout.EndArea();
			}

			if(e.type == EventType.Repaint)
				isLoaded = true;
		}

		private void DrawExportMenu()
		{
			GUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(44));

            if (GUILayout.Button(MA_TextureAtlasserProIcons.createAtlasIcon, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
            {
                MA_TextureAtlasserProCreateExportWindow.InitWindow(curWindow);
            }

            bool wasEnabled = GUI.enabled;

            if (curWindow.textureAtlas.exportSettings != null)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }

            if (GUILayout.Button("Export", GUILayout.ExpandWidth(true), GUILayout.Height(37)))
            {
                MA_TextureAtlasserProUtils.ExportAtlasModels(curWindow.textureAtlas, curWindow.textureAtlas.exportSettings.modelExportSettings);
                MA_TextureAtlasserProUtils.ExportAtlasTextures(curWindow.textureAtlas, curWindow.textureAtlas.exportSettings.textureExportSettings);
                //MA_TextureAtlasserProUtils.ExportAtlasMaterial(curWindow.textureAtlas, curWindow.textureAtlas.exportSettings.materialExportSettings);
            }

            GUI.enabled = wasEnabled;

            GUILayout.EndHorizontal();
		}

		private void DrawExportAdvancedOptions()
		{
			bool wasEnabled = GUI.enabled;

            if (curWindow.textureAtlas.exportSettings.canModify)
            {
                GUI.enabled = true;
            }
            else
            {
                GUI.enabled = false;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("Models:", EditorStyles.miniBoldLabel);
            curWindow.textureAtlas.exportSettings.modelExportSettings.modelFormat = (ModelFormat)EditorGUILayout.EnumPopup("ModelFormat:", curWindow.textureAtlas.exportSettings.modelExportSettings.modelFormat);
            curWindow.textureAtlas.exportSettings.modelExportSettings.replaceModel = EditorGUILayout.Toggle("ReplaceModels:", curWindow.textureAtlas.exportSettings.modelExportSettings.replaceModel);
            curWindow.textureAtlas.exportSettings.modelExportSettings.uvFlipY = EditorGUILayout.Toggle("UV FlipY:", curWindow.textureAtlas.exportSettings.modelExportSettings.uvFlipY);
            curWindow.textureAtlas.exportSettings.modelExportSettings.uvChannel = EditorGUILayout.IntField("UV Channel:", curWindow.textureAtlas.exportSettings.modelExportSettings.uvChannel);
            curWindow.textureAtlas.exportSettings.modelExportSettings.uvWrap = EditorGUILayout.Toggle("UV Wrap:", curWindow.textureAtlas.exportSettings.modelExportSettings.uvWrap);

            GUILayout.Label("Textures:", EditorStyles.miniBoldLabel);
            curWindow.textureAtlas.exportSettings.textureExportSettings.textureFormat = (TextureFormat)EditorGUILayout.EnumPopup("TextureFormat:", curWindow.textureAtlas.exportSettings.textureExportSettings.textureFormat);
            curWindow.textureAtlas.exportSettings.textureExportSettings.textureType = (TextureType)EditorGUILayout.EnumPopup("TextureType:", curWindow.textureAtlas.exportSettings.textureExportSettings.textureType);
            curWindow.textureAtlas.exportSettings.textureExportSettings.textureScaleMode = (MA_TextureUtils.TextureScaleMode)EditorGUILayout.EnumPopup("TextureScaleMode:", curWindow.textureAtlas.exportSettings.textureExportSettings.textureScaleMode);

            EditorGUILayout.EndVertical();

			GUI.enabled = wasEnabled;
		}
	}
}
#endif