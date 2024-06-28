using System.Net;

namespace HeyTripCarWeb.Share
{
    public class FTPHelper
    {
        public static void DownloadFile(string ftpFilePath, string localFilePath, string ftpUsername, string ftpPassword)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpFilePath);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create))
                        {
                            responseStream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to download file {ftpFilePath}: {ex.Message}", ex);
            }
        }
    }
}