using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkPerformance.BenchmarkPerformance;

public class Program
{
    public static void Main(string[] args)
    {
        var config = DefaultConfig.Instance
        .WithArtifactsPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BenchmarkDotNet"));

        var summary = BenchmarkRunner.Run<BenchmarkPerformanceService>();
    }
}