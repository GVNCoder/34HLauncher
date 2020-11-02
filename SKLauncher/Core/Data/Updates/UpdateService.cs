using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

using Launcher.Core.Service;
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
        private WebClientWithTimeout _webClient;

        public UpdateService(IVersionService versionService, IViewModelSource viewModelLocator)
        {
            _versionService = versionService;
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
                    _RaiseError(ex.Message);
                }
            }
            else
            {
                _RaiseError(e.Error.Message);
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

            _RaiseBeginDownload();
        }

        private void _DownloadFileProgressChangedHandler(object sender, DownloadProgressChangedEventArgs e)
            => _RaiseReportProgress(e.ProgressPercentage);

        private void _DownloadFileCompletedHandler(object sender, AsyncCompletedEventArgs e)
        {
            InDownloadStage = false;

            _RaiseEndDownload(false);
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
                _RaiseError(ex.Message);
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
        public event EventHandler<UpdateProgressEventArgs> ReportProgress;
        public event EventHandler<UpdateDownloadEventArgs> EndDownload;
        public event EventHandler BeginDownload;

        private void _RaiseError(string errorMessage) => Error?.Invoke(this, new UpdateErrorEventArgs(errorMessage));
        private void _RaiseReportProgress(int value) => ReportProgress?.Invoke(this, new UpdateProgressEventArgs(value));
        private void _RaiseBeginDownload() => BeginDownload?.Invoke(this, EventArgs.Empty);
        private void _RaiseEndDownload(bool isCanceled) => EndDownload?.Invoke(this, new UpdateDownloadEventArgs(isCanceled));

        public async Task CancelDownload()
        {
            var resolvedValue = await CancelDownloadResolver.Invoke();
            if (resolvedValue) _CloseWebClient();
        }
    }
}