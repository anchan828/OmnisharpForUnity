using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using System.IO;
using UnityEditor.Callbacks;
using UnityEditorInternal;


namespace OmnisharpForUnity
{
    internal class SublimeTextUtility
    {
        private static string sublimeProjectName
        {
            get
            {
                return Path.GetFileName((Directory.GetParent(Application.dataPath).FullName)) + ".sublime-project";
            }
        }

        private static string sublPath
        {
            get
            {
                var _sublPath = InternalEditorUtility.GetExternalScriptEditor() + "/Contents/SharedSupport/bin/subl";

                if (!File.Exists(_sublPath))
                    throw new FileNotFoundException("subl");

                return _sublPath;
            }
        }


        private static bool isGeneratedProject
        {
            get
            {
                return File.Exists(sublimeProjectName);
            }
        }


        [MenuItem("Omnisharp/Sublime Text/Generate Sublime Project")]
        static void GenerateSublimeProject()
        {
            var projectName = Path.GetFileName((Directory.GetParent(Application.dataPath).FullName));

            var text = "{\"folders\":[{\"follow_symlinks\":true,\"path\":\".\",\"file_exclude_patterns\":[\"*.sln\",\"*.csproj\",\"*.meta\",\"*.unityproj\",\"*.unitypackage\",],\"folder_exclude_patterns\":[\"Temp\",\"Library\",\"obj\",]}],\"solution_file\":\"#SOLUTION_FILE#\",\"settings\":{\"auto_complete_triggers\":[{\"characters\":\".\",\"selector\":\"source.cs\"}]}}";

            var solusionFileName = projectName + ".sln";

            if (!File.Exists(solusionFileName))
                EditorApplication.ExecuteMenuItem("Assets/Sync MonoDevelop Project");

            text = Regex.Replace(text, "#SOLUTION_FILE#", solusionFileName);

            File.WriteAllText(projectName + ".sublime-project", text);
        }


        [OnOpenAsset]
        static bool OnOpenAsset(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is TextAsset || obj is MonoScript)
            {
                if (!isGeneratedProject)
                    GenerateSublimeProject();

                var args = GetSublArgs(AssetDatabase.GetAssetPath(instanceID), Mathf.Clamp(line, 0, int.MaxValue));
                System.Diagnostics.Process.Start(sublPath, string.Join(" ", args));
                return true;
            }

            return false;
        }

        private static string[] GetSublArgs(string filePath, int line)
        {
            if (filePath == null)
                throw new System.ArgumentNullException("filePath");

            return new []
            {
                "-a",
                "--project",
                sublimeProjectName,
                string.Format("{0}:{1}", filePath, line),
            };
        }
    }
}