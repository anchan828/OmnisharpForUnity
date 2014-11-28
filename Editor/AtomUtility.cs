using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditorInternal;
using UnityEditor.Callbacks;

namespace OmnisharpForUnity
{
    internal class AtomUtility : BaseUtility
    {
        private static string atomPath
        {
            get
            {
                return InternalEditorUtility.GetExternalScriptEditor() + "/Contents/MacOS/Atom";
            }
        }
        private static bool usingAtom
        {
            get
            {
                return File.Exists(atomPath);
            }
        }

        [MenuItem("Omnisharp/Atom/Create UnityAssemblies")]
        static void CreateUnityAssemblies()
        {
            Directory.CreateDirectory(unityAssembliesDirectoryPath);

            foreach (var dllPath in loadedAssemblyPaths)
            {
                var dllName = Path.GetFileName(dllPath);

                if (scriptAssemblies.Contains(dllName))
                    continue;
                    
                File.Copy(dllPath, Path.Combine(unityAssembliesDirectoryPath, dllName), true);
            }
        }

        [MenuItem("Omnisharp/Atom/Open Atom")]
        static void OpenAtom()
        {
            System.Diagnostics.Process.Start(atomPath, projectPath);
        }

        [OnOpenAsset]
        static bool OnOpenAsset(int instanceID, int line)
        {
            if (!usingAtom)
                return false;
            
            var obj = EditorUtility.InstanceIDToObject(instanceID);

            if (obj is TextAsset || obj is MonoScript)
            {
                CreateUnityAssemblies();

                var args = GetSublArgs(AssetDatabase.GetAssetPath(instanceID));
                System.Diagnostics.Process.Start(atomPath, string.Join(" ", args));
                if (line != -1)
                    System.Diagnostics.Process.Start("osascript", "-e " + string.Join(" -e ", GetAppleScript(line)));

                return true;
            }

            return false;
        }

        private static string[] GetSublArgs(string filePath)
        {
            return new []
            {
                "\"" + projectPath + "\"",
                "\"" + filePath + "\""
            };
        }

        private static string[] GetAppleScript(int line)
        {

            return new []
            {
                "'tell application \"Atom\"'",
                "'activate'",
                "'delay 0.2'",
                "'tell application \"System Events\"'",
                "'delay 0.1'",
                "'keystroke \"g\" using control down'",
                string.Format("'keystroke \"{0}\"'", line),
                "'key code 76'",
                "'end tell'",
                "'end tell'",
            };

        }
    }
}
