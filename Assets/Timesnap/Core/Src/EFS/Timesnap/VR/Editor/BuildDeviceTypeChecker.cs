using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

    /*
namespace Timesnap.Src.EFS.Timesnap.VR.Editor
{
    public class MyScenePostprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        { 
            var foo = PlayerSettings.GetVirtualRealitySDKs(report.summary.platformGroup);
            var buildingForOculus = foo.Length == 1 && foo.First() == "Oculus";
            var adbPath = GetAdbPath();
            var oculusMarker = "device:pacific";

            Debug.Log(adbPath);
            if (!string.IsNullOrEmpty(adbPath))
            {
                var result = RunCommand(adbPath, "devices -l");
                var output = result.Output.Split('\n').Skip(1).JoinToString();
                if (!string.IsNullOrEmpty(result.Errors))
                {
                    Debug.LogError(
                        "Errors occurred while attempting to determine the type of attached android devices");
                }

                Debug.Log("output: " + output);
                Debug.Log("building for oculus?" + buildingForOculus);
                if (string.IsNullOrWhiteSpace(output))
                {
                    if (!EditorUtility.DisplayDialog("No android devices found",
                        "No android devices appear to be attached. Try to build anyway?",
                        "Yes", "No"))
                    {
                        throw new Exception("Cancelled");
                    }

                    return;
                }

                if (buildingForOculus)
                {
                    if (!output.Contains(oculusMarker))
                    {
                        Debug.Log("RunCommand result: " + result);
                        if (!EditorUtility.DisplayDialog("Device type mismatch",
                            "This build's settings target the Oculus, but no device labeled 'device:pacific' is attached. Build anyway?",
                            "Yes", "No"))
                        {
                            throw new Exception("Cancelled");
                        }
                    }
                }
                else
                {
                    if (output.Contains(oculusMarker))
                    {
                        if (!EditorUtility.DisplayDialog("Device type mismatch",
                            "This build's settings do not target the Oculus, but a device labeled 'device:pacific' is attached. Build anyway?",
                            "Yes", "No"))
                        {
                            throw new Exception("Cancelled");
                        }
                    }
                }
            }
        }

        private static RunCommandResult RunCommand(string command, string arguments)
        {
            using (var process = new System.Diagnostics.Process())
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo(command, arguments)
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                process.StartInfo = startInfo;

                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();
                process.OutputDataReceived += (o, ef) => outputBuilder.AppendLine(ef.Data);
                process.ErrorDataReceived += (o, ef) => errorBuilder.AppendLine(ef.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                process.Close();

                // Trims the output strings to make comparison easier.
                return new RunCommandResult
                {
                    Output = outputBuilder.ToString().Trim(),
                    Errors = errorBuilder.ToString().Trim(),
                    Command = command + " " + arguments
                };
            }
        }

        private class RunCommandResult
        {
            public string Output;
            public string Errors;
            public string Command;

            public override string ToString()
            {
                return $"command: {Command}, output: {Output}, errors: {Errors}";
            }
        }

        // returns null if no path found
        private static string GetAdbPath()
        {
           /* var sdkRoot = EditorPrefs.GetString("AndroidSdkRoot");
            var adbPath = Path.Combine(Path.GetFullPath(sdkRoot),
                "platform-tools" + Path.DirectorySeparatorChar + "adb");
#if UNITY_EDITOR_WIN
            adbPath = Path.ChangeExtension(adbPath, "exe");
#endif // UNITY_EDITOR_WIN

            if (File.Exists(adbPath))
            {
                return adbPath;
            }

            Debug.LogErrorFormat(
                "adb not found at \"{0}\". Please add adb to your SDK path and restart the Unity editor.", adbPath);
                */
                /*
            return null;
        }
    }
}
*/