using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyBuilder : MonoBehaviour
{
	public const string PRODUCT_NAME_ARG = "productName";
	public const string VERSION_ARG = "version";
	public const string VERSION_TYPE_ARG = "versionType";
	public const string COMPANY_NAME_ARG = "companyName";
	public const string SCRIPTING_BACKEND_ARG = "scriptingBackend";
	public const string BUILD_NAME_ARG = "buildName";

	[MenuItem("Build/Build Android")]
	public static void BuildAndroid()
	{
		var productName = GetArg(PRODUCT_NAME_ARG);
		var version = GetArg(VERSION_ARG);
		var versionType = GetArg(VERSION_TYPE_ARG);
		var companyName = GetArg(COMPANY_NAME_ARG);
		var scriptingBackend = GetArg(SCRIPTING_BACKEND_ARG);
		var buildName = GetArg(BUILD_NAME_ARG);

		if (!string.IsNullOrEmpty(productName))
			PlayerSettings.productName = productName;
		if (!string.IsNullOrEmpty(version) && !string.IsNullOrEmpty(versionType))
		{
			CheckVersion(version, int.Parse(versionType));
			PlayerSettings.bundleVersion = version;
		}
		if (!string.IsNullOrEmpty(companyName))
			PlayerSettings.companyName = companyName;
		if (!string.IsNullOrEmpty(scriptingBackend))
			PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, (ScriptingImplementation)int.Parse(scriptingBackend));

		BuildPlayerOptions options = new BuildPlayerOptions()
		{
			scenes = new string[] { "Assets/Scenes/SampleScene.unity" },
			targetGroup = BuildTargetGroup.Android,
			target = BuildTarget.Android,
			options = BuildOptions.Development,
			locationPathName = $"../Build/Test.apk"
		};

		if (!string.IsNullOrEmpty(buildName))
			options.locationPathName = $"../Build/{buildName}.apk";

		BuildPipeline.BuildPlayer(options);
	}

	private static void CheckVersion(string version, int buildType)
	{
		// should be validated in Unity if it fits
		// Major.Minor.Third and in addition
		// Third -d.BuildNumber for Develop Branch Builds,
		// -f.Buildnumber for Feature Branch and just
		// Major.Minor.Third for Master Builds <- this should also be automatically set
		// in the jenkins job, parameter should only be Major Minor and Third
		System.Console.Out.WriteLine($"Version: {version} - Type: {buildType}");
		var splitVer = version.Split('.');

		switch (buildType)
		{
			case 0:
			default:
				if (splitVer.Length != 3)
					throw new System.Exception("Wrong version layout.");
				break;
			case 1:
				if (splitVer.Length != 2 || !splitVer[0].EndsWith("-d"))
					throw new System.Exception("Wrong version layout.");
				break;
			case 2:
				if (splitVer.Length != 2 || !splitVer[0].EndsWith("-f"))
					throw new System.Exception("Wrong version layout.");
				break;
		}
	}

	private static string GetArg(string reqArgName)
	{
		var allArgs = System.Environment.GetCommandLineArgs();

		for (int i = 0; i < allArgs.Length; i++)
		{
			var argName = allArgs[i].Trim();

			if (argName.Equals($"-{reqArgName}"))
			{
				var argVal = "";

				argVal = allArgs[i + 1].Trim();
				if (argVal.StartsWith("-"))
					return null;

				return argVal;
			}
		}

		return null;
	}
}
