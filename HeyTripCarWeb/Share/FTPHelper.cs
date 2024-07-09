using Microsoft.Extensions.Hosting;
using Renci.SshNet;
using Serilog;
using System.Net;

namespace HeyTripCarWeb.Share
{
    public class FTPHelper
    {
        public static void DownloadFile(string ftpFilePath, string localFilePath, string ftpUsername, string ftpPassword)
        {
            try
            {
                // 创建 ConnectionInfo 对象，并设置连接超时时间
                var connectionInfo = new Renci.SshNet.ConnectionInfo("securetransfer.avis-europe.com", 22, ftpUsername,
                    new PasswordAuthenticationMethod(ftpUsername, ftpPassword))
                {
                    Timeout = TimeSpan.FromMinutes(10) // 设置连接超时时间为3分钟
                };
                using (var client = new SftpClient(connectionInfo))
                {
                    client.Connect();

                    if (client.IsConnected)
                    {
                        using (Stream fileStream = File.Create(localFilePath))
                        {
                            client.DownloadFile(ftpFilePath, fileStream);
                        }

                        Log.Information($"Downloaded file '{ftpFilePath}' to '{localFilePath}' successfully.");
                    }
                    else
                    {
                        Log.Error("Failed to connect to the SFTP server.");
                    }

                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
            }
        }
    }
}