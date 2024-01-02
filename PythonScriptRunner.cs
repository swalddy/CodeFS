using UnityEngine;
using System.Diagnostics;
using System.Threading.Tasks;
using System.IO; 

public class PythonScriptRunner : MonoBehaviour
{
    void Start()
    {
        RunPythonScriptAsync();
    }

    async void RunPythonScriptAsync()
    {
        string pythonScriptPath = Path.Combine(Application.dataPath, "../", "main.py"); 

        await Task.Run(() => 
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("python")
            {
                Arguments = $"\"{pythonScriptPath}\"", 
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            Process process = new Process { StartInfo = startInfo };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                UnityEngine.Debug.LogError("Python error: " + err);
            }
            else
            {
                UnityEngine.Debug.Log("Python output: " + output);
            }

            process.Close();
        });
    }
}
