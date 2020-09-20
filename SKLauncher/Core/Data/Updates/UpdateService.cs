using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Launcher.Core.Services;
using Launcher.Core.Services.Updates;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Newtonsoft.Json;

namespace Launcher.Core.Data.Updates
{
    public class UpdateService : IUpdateService
    { 
        private readonly Uri _updateInfoUri = new Uri("https://www.dropbox.com/s/dxow0tqx0v30ich/updateInfo.json?dl=1");
        private readonly string _updatePath = Path.Combine(FolderConstant.UpdateFolder, "34H Update.exe");

        private readonly IVersionService _versionService;
        private readonly IUpdateControl _updateControl;

        private WebClientWithTimeout _webClient;

        public UpdateService(IVersionService versionService)
        {
            _versionService = versionService;

            //_updateControl = viewModelLocator.UpdateControlViewModel;
            //_updateControl.CancelRequested += _UpdateDownloadCancelRequestedHandler;
        }

        public void BeginUpdate()
        {
            _webClient = new WebClientWithTimeout();
            _webClient.DownloadStringCompleted += _DownloadStringCompletedHandler;
            _webClient.DownloadStringAsync(_updateInfoUri);
        }

        private void _DownloadStringCompletedHandler(object sender, DownloadStringCompletedEventArgs e)
        {
            _webClient.DownloadStringCompleted -= _DownloadStringCompletedHandler;
            _CloseWebClient();

            if (e.Error == null)
            {
                try
                {
                    var updateInfoObject = JsonConvert.DeserializeObject<UpdateInfoObject>(e.Result);
                    var version = LauncherVersion.Parse(updateInfoObject.Version);
                    var currentVersion = _versionService.GetAssemblyVersion();

                    if (version.AssemblyVersion <= currentVersion) return;

                    var acceptUpdate = UpdateAvailableResolver.Invoke(version);
                    if (!acceptUpdate) return;

                    InDownloadStage = true;
                    _BeginDownload(updateInfoObject);
                }
                catch (Exception ex)
                {
                    _ReportError(ex.Message);
                }
            }
            else
            {
                _ReportError(e.Error.Message);
            }
            
        }

        private void _CloseWebClient()
        {
            _webClient.CancelAsync();
            _webClient.Dispose();
        }

        private void _BeginDownload(UpdateInfoObject updateInfoObject)
        {
            _webClient = new WebClientWithTimeout();
            _webClient.DownloadFileCompleted += _DownloadFileCompletedHandler;
            _webClient.DownloadProgressChanged += _DownloadFileProgressChangedHandler;
            _webClient.DownloadFileAsync(new Uri(updateInfoObject.Link), _updatePath, updateInfoObject);

            _updateControl.Show();
        }

        private void _DownloadFileProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
            => _updateControl.ReportProgress(e.ProgressPercentage);

        private void _DownloadFileCompletedHandler(object sender, AsyncCompletedEventArgs e)
        {
            InDownloadStage = false;

            _updateControl.Hide();
            _CloseWebClient();

            if (e.Cancelled) return;
            try
            {
                if (e.Error == null)
                {
                    var state = e.UserState as UpdateInfoObject;
                    var hash = FileHelper.ComputeHash(_updatePath);

                    if (hash.Equals(state.Hash)) _BeginInstall();
                    else throw new Exception("Cannot install update for reason: cache damaged");
                }
                else throw new Exception($"Cannot download update for reason: {e.Error.Message}");
            }
            catch (Exception ex)
            {
                _ReportError(ex.Message);
            }
        }

        private void _BeginInstall()
        {
            Process.Start(_updatePath, $"APPDIR=\"{FolderConstant.BaseDir}\"");
            Application.Current.Shutdown(0);
        }

        public bool InDownloadStage { get; private set; }
        public Func<Task<bool>> CancelDownloadResolver { get; set; }
        public Func<LauncherVersion, bool> UpdateAvailableResolver { get; set; }

        public event EventHandler<UpdateErrorEventArgs> Error;

        private void _ReportError(string errorMessage) => Error?.Invoke(this, new UpdateErrorEventArgs(errorMessage));

        private async void _UpdateDownloadCancelRequestedHandler(object sender, EventArgs e)
        {
            var resolvedValue = await CancelDownloadResolver.Invoke();
            if (resolvedValue) _CloseWebClient();
        }
    }
}