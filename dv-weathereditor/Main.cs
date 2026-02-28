using System;
using DV.Utils;
using DV.WeatherSystem;
using UnityEngine;
using UnityModManagerNet;

namespace dv_weathereditor;

public class Settings : UnityModManager.ModSettings, IDrawable
{
	[Draw("Toggle Key")]
	public KeyBinding ToggleKey = new KeyBinding { keyCode = KeyCode.F7 };

	public void OnChange() { }

	public override void Save(UnityModManager.ModEntry modEntry)
	{
		Save(this, modEntry);
	}
}

public static class Main
{
	private static UnityModManager.ModEntry? mod;
	private static Settings settings = new();
	private static bool enabled;
	private static WeatherEditorGUI? cachedEditorGUI;

	private static bool Load(UnityModManager.ModEntry modEntry)
	{
		try
		{
			mod = modEntry;
			settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

			modEntry.OnToggle = OnToggle;
			modEntry.OnGUI = OnGUI;
			modEntry.OnSaveGUI = OnSaveGUI;
			modEntry.OnUpdate = OnUpdate;
		}
		catch (Exception ex)
		{
			modEntry.Logger.LogException($"Failed to load {modEntry.Info.DisplayName}:", ex);
			return false;
		}

		return true;
	}

	private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
	{
		enabled = value;
		return true;
	}

	private static void OnGUI(UnityModManager.ModEntry entry)
	{
		settings.Draw(entry);
	}

	private static void OnSaveGUI(UnityModManager.ModEntry entry)
	{
		settings.Save(entry);
	}

	private static void OnUpdate(UnityModManager.ModEntry entry, float delta)
	{
		if (!enabled)
			return;

		if (settings.ToggleKey.Down())
		{
			if (cachedEditorGUI == null)
			{
				var weatherDriver = SingletonBehaviour<WeatherDriver>.Instance;
				if (weatherDriver != null)
				{
					cachedEditorGUI = weatherDriver.GetComponent<WeatherEditorGUI>();
				}
			}

			if (cachedEditorGUI != null)
			{
				cachedEditorGUI.enabled = !cachedEditorGUI.enabled;
			}
			else
			{
				mod?.Logger.Warning("WeatherEditorGUI component not found.");
			}
		}
	}
}
