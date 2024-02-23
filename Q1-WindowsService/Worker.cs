using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Q1_WindowsService;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly EmailService _emailService;
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(5)); // Change to 12 Hours

    public Worker(ILogger<Worker> logger, EmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
        {
            var currentWorkload = GetMachineCurrentWorkload();

            _emailService.SendEmail();
        }
    }

    private Workload GetMachineCurrentWorkload()
    {
        PerformanceCounter cpuCounter = new("Processor Information", "% Processor Utility", "_Total");
        PerformanceCounter ramCounter = new("Memory", "Available MBytes");

        float cpuCount = cpuCounter.NextValue();
        float ramCount = ramCounter.NextValue();

        // Needed for correct count
        if (Math.Abs(cpuCount) <= 0.00)
        {
            cpuCount = cpuCounter.NextValue();
        }

        long totalFreeDriveSize = 0;
        long totalNetworkSentBytes = 0;
        long totalNetworkReceivedBytes = 0;

        foreach (var drive in DriveInfo.GetDrives())
        {
            long availableFreeSpace = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
            string driveFormat = drive.DriveFormat;
            string name = drive.Name;
            long totalSize = drive.TotalSize / 1024 / 1024 / 1024;

            totalFreeDriveSize += availableFreeSpace;

            //await Console.Out.WriteLineAsync($"{name} ({driveFormat}): {availableFreeSpace} GB Free out of {totalSize} GB");
        }

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            totalNetworkSentBytes += (ni.GetIPv4Statistics().BytesSent / 1024);
            totalNetworkReceivedBytes += (ni.GetIPv4Statistics().BytesReceived / 1024);
        }

        _logger.LogInformation("CPU: {cpuCount}% - RAM: {ramCount} MB - HDD: {hddCount} GB Free - Network Sent: {netSentCount} KB Received: {netReceivedCount} KB", cpuCount, ramCount, totalFreeDriveSize, totalNetworkSentBytes, totalNetworkReceivedBytes);

        return new()
        {
            CpuUtilizationPercentage = cpuCount,
            MemoryUtilization = ramCount,
            TotalFreeDriveSize = totalFreeDriveSize,
            TotalNetworkSentSize = totalNetworkSentBytes,
            TotalNetworkReceivedSize = totalNetworkReceivedBytes
        };
    }
}
