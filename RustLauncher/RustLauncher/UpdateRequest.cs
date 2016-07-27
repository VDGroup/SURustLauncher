using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Threading;
using System.ComponentModel;

namespace RustLauncher
{
    public class UpdateRequest
    {
        #region Declaration
        private bool _updated;
        private string _address;
        private string sVersion;
        private string lVersion;
        private bool _failed;
        private string _versionFile;
        private Thread _counterThread;
        private byte _percentage;
        private long _total;
        private long _completed;
        public EventHandler ProgressChanged;
        public EventHandler DownloadCompleted;
        public EventHandler InstallationCompleted;

        #endregion
        public UpdateRequest(string updateServerAddress, string versionFile)
        {
            _versionFile = versionFile;
            _updated = false;
            _address = updateServerAddress;
            UpdateCheck();

        }

        private void UpdateCheck()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    sVersion = client.DownloadString(_address);
                }
            }
            catch (Exception e)
            {
                _failed = true;
                throw e;
            }
            try
            {
                lVersion = File.ReadAllText(_versionFile);
            }
            catch (Exception e)
            {
                _failed = true;
                throw e;
            }
            if (!_failed) if (!(Convert.ToInt32(lVersion) < Convert.ToInt32(sVersion)))
                    _updated = true;
        }

        public bool IsUpToDate
        {
            get { return _updated; }
        }
        public bool Update()
        {
            if (_failed) return false;
            DownloadUpdatePackage();
            return true;
        }

        private void DownloadUpdatePackage()
        {
            _counterThread = new Thread(CalcPercentage);
            WebClient c = new WebClient();
            c.DownloadProgressChanged += C_DownloadProgressChanged;
            c.DownloadFileCompleted += C_DownloadFileCompleted;
            c.DownloadFileAsync(new Uri(_address), "Update.package");
            ResetCounter();
        }

        private void ResetCounter()
        {
            _percentage = 0;
            _completed = 0;
            _total = 0;
        }

        private void CalcPercentage()
        {
            while (true)
            {

                if (_completed != 0)
                    _percentage = (byte)(100 * _total / _completed);
                Thread.Sleep(100);
            }
        }

        private void C_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (DownloadCompleted != null)
                DownloadCompleted(this, null);
        }

        private void C_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            if (ProgressChanged != null)
            {
                ProgressEventArgs z = new ProgressEventArgs();
                _percentage = (byte)(100 * e.TotalBytesToReceive / e.BytesReceived);
                z.Percentage = _percentage;
                z.BytesTotal = e.TotalBytesToReceive;
                z.BytesDone = e.BytesReceived;
                ProgressChanged(this, z);
            }
        }



    }
    public class ProgressEventArgs : EventArgs
    {
        public int Percentage { get; set; }
        public long BytesDone { get; set; }
        public long BytesTotal { get; set; }
    }
}
