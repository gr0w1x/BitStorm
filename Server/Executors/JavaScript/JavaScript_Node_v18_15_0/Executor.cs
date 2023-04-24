using System.Diagnostics;
using System.Text;
using System.Text.Json;
using CommonServer.Asp.HostedServices;
using ExecutorTemplate;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Types.Dtos;
using Types.Languages;

namespace JavaScript_Node_v18_15_0;

public class Executor: BaseExecutorService<Types.Languages.JavaScript_Node_v18_15_0>
{
    public Executor(
        IConnectionFactory factory,
        ILogger<Executor> logger,
        RabbitMqProvider provider
    ) : base(factory, logger, provider) { }

    protected static readonly VersionDto Version = CodeLanguages.VersionsByType[typeof(Types.Languages.JavaScript_Node_v18_15_0)];

    protected static string MessageToCode(ExecuteCodeDto execute) =>
$@"
// Preloaded
{execute.Preloaded}
// Solution
{execute.Solution}
// Tests
{execute.Tests}
";

    public static async Task<ExecuteCodeResultDto> ExecuteNodeCode(
        ExecuteCodeDto execute,
        ILogger<BaseExecutorService<Types.Languages.JavaScript_Node_v18_15_0>> ExecutorLogger
    )
    {
        var folder = $"./node/tests/{Path.GetRandomFileName()}";
        ExecuteCodeResultDto? message;
        var watcher = Stopwatch.StartNew();
        try
        {
            Directory.CreateDirectory(folder);

            var codeFile = $"{folder}/test.js";
            var resultsFile = $"{folder}/results.json";

            string code = MessageToCode(execute);
            await File.WriteAllTextAsync(codeFile, code);

            Task<(int exitCode, string output)> processTask = Task.Run(() =>
            {
                StringBuilder builder = new();
                void onRecieved(object _, DataReceivedEventArgs e) => builder.AppendLine(e.Data);
                using Process process = new();
                process.StartInfo.FileName = "bash";
                process.StartInfo.Arguments = $"-c \"ulimit -v {Version.RamSizeLimitMb}000 && node ./node/node_modules/mocha/bin/mocha.js node/test.js\"";
                process.StartInfo.EnvironmentVariables["TEST"] = codeFile;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.OutputDataReceived += onRecieved;
                process.ErrorDataReceived += onRecieved;
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
                return (process.ExitCode, builder.ToString());
            });

            if (processTask.Wait(Version.ExecutionTimeout))
            {
                watcher.Stop();
                ExecuteTests? executeTests = null;
                if (Path.Exists(resultsFile))
                {
                    executeTests = JsonSerializer.Deserialize<ExecuteTests>(await File.ReadAllTextAsync(resultsFile));
                }
                var output = await processTask;
                message = new ExecuteCodeResultDto()
                {
                    Details = output.output,
                    ExitStatus = output.exitCode,
                    Time = watcher.Elapsed,
                    Tests = executeTests
                };
            }
            else
            {
                message = new ExecuteCodeResultDto()
                {
                    Details = "Execution Timed Out",
                    Time = Version.ExecutionTimeout,
                    ExitStatus = -1,
                };
            }
        }
        catch (Exception e)
        {
            watcher.Stop();
            ExecutorLogger.LogError(e.Message);
            ExecutorLogger.LogError(e.StackTrace);
            message = new ExecuteCodeResultDto()
            {
                Details = "Something happenning wrong",
                Time = watcher.Elapsed,
                ExitStatus = -1,
            };
        }
        finally
        {
            Directory.Delete(folder, true);
        }
        return message;
    }

    protected override Task<ExecuteCodeResultDto> ExecuteCode(ExecuteCodeDto execute)
        => ExecuteNodeCode(execute, ExecutorLogger);
}
