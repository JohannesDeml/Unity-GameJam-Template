// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuildScriptMenu.cs">
//   Copyright (c) 2022 Johannes Deml. All rights reserved.
// </copyright>
// <author>
//   Johannes Deml
//   public@deml.io
// </author>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace UnityBuilderAction
{
	/// <summary>
	/// Menu items for <see cref="BuildScript"> to build the project in the editor
	/// Helpful for testing the CI behavior and semi-automated builds
	/// </summary>
	public class BuildScriptMenu
	{
		private static readonly List<string> baseParameters = new List<string>()
		{
			"-projectPath", "",
			"-buildVersion", PlayerSettings.bundleVersion,
			"-androidVersionCode", PlayerSettings.Android.bundleVersionCode.ToString(CultureInfo.InvariantCulture)
		};
		
		[MenuItem("Tools/Build/Build current platform")]
		public static void BuildCurrentPlatform()
		{
			var parameters = new List<string>(baseParameters);
			string tag = $"{Application.version}";
			if (IsDevelopmentBuild)
			{
				tag += "-debug";
			}
			var buildTarget = EditorUserBuildSettings.activeBuildTarget;
			string extension = GetBuildExtension(buildTarget);
			SetBuildTarget(buildTarget, ref parameters);
			SetParameterValue("-autorunplayer", IsAutoRunEnabled.ToString(), ref parameters);
			SetParameterValue("-tag", tag, ref parameters);
			SetParameterValue("-customBuildPath", $"Builds/{buildTarget}/{tag}/{Application.productName}{extension}", ref parameters);
			SetParameterValue("-customBuildName", tag, ref parameters);
			BuildWithParameters(parameters);
		}

		private static void BuildWithParameters(List<string> parameters)
		{
			Debug.Log($"Build project with parameters [{string.Join(" ", parameters)}]");
			BuildScript.Build(parameters.ToArray());
		}

		private static void SetBuildTarget(BuildTarget target, ref List<string> parameters)
		{
			SetParameterValue("-buildTarget", target.ToString(), ref parameters);
		}

		private static void SetParameterValue(string parameter, string value, ref List<string> parameters)
		{
			parameters.Add(parameter);
			parameters.Add(value);
		}

		private static string GetBuildExtension(BuildTarget buildTarget)
		{
			switch (buildTarget)
			{
				case BuildTarget.Android:
					return ".apk";
				case BuildTarget.iOS:
				case BuildTarget.WebGL:
					return "/";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return ".exe";
				case BuildTarget.StandaloneOSX:
					return ".app";
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.LinuxHeadlessSimulation:
				case BuildTarget.EmbeddedLinux:
					return "/";
				
				default:
					throw new NotImplementedException($"Missing implementation for {buildTarget}");
			}
		}
		
		private const string AutoRunBuildsMenuName = "Tools/Build/AutoRunBuild";
		private const string DevelopmentBuildsMenuName = "Tools/Build/DevelopmentBuild";

		private static bool IsAutoRunEnabled
		{
			get { return EditorPrefs.GetBool(AutoRunBuildsMenuName, true); }
			set { EditorPrefs.SetBool(AutoRunBuildsMenuName, value); }
		}
		
		private static bool IsDevelopmentBuild
		{
			get { return EditorPrefs.GetBool(DevelopmentBuildsMenuName, true); }
			set { EditorPrefs.SetBool(DevelopmentBuildsMenuName, value); }
		}
         
		[MenuItem(AutoRunBuildsMenuName, false, 100)]
		private static void ToggleAutorun()
		{
			IsAutoRunEnabled = !IsAutoRunEnabled;
		}
 
		[MenuItem(AutoRunBuildsMenuName, true)]
		private static bool ToggleAutorunValidate()
		{
			Menu.SetChecked(AutoRunBuildsMenuName, IsAutoRunEnabled);
			return true;
		}
		
		[MenuItem(DevelopmentBuildsMenuName, false, 100)]
		private static void ToggleDevelopmentBuild()
		{
			IsDevelopmentBuild = !IsDevelopmentBuild;
		}
 
		[MenuItem(DevelopmentBuildsMenuName, true)]
		private static bool ToggleDevelopmentBuildValidate()
		{
			Menu.SetChecked(DevelopmentBuildsMenuName, IsDevelopmentBuild);
			return true;
		}
	}
}