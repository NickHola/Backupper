using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Main.Logs;
using Main.Serializes;

namespace Backupper
{
    class RESTBackups
    {
        HttpClient httpClient;
        public event EventHandler<RESTBackupsEventArgs> GetBackupsComplete;

        async public Task<string> GetBackupsAsync(int timeoutRequestMs = 9000)
        {
            if (httpClient == null) httpClient = new HttpClient();
            string backupsSerialization = "";

            if (SettingsM.Instance.RestSetting == null)
                throw new Exception("SettingsM.Instance.RestSetting is null"); //With TaskCanceledException I can not read the exception message in the parent catch

            SettingsM.Instance.RestSetting.ValidMySelf();
            if (SettingsM.Instance.RestSetting.IsValid == false)
                throw new Exception("SettingsM.Instance.RestSetting.IsValid is false"); //With TaskCanceledException I can not read the exception message in the parent catch

            var timeoutCancellationTokenSource = new CancellationTokenSource();

            string url = SettingsM.Instance.RestSetting.RootAddress + SettingsM.Instance.RestSetting.RoutePrefix + SettingsM.Instance.RestSetting.RouteOfGetBackups;

            var task = httpClient.GetAsync(url, timeoutCancellationTokenSource.Token);

            int taskIndex = Task.WaitAny(task, Task.Delay(timeoutRequestMs, timeoutCancellationTokenSource.Token));

            if (taskIndex == 0)
            {
                //timeoutCancellationTokenSource.Cancel();
                if (task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    backupsSerialization = await task.Result.Content.ReadAsStringAsync();
                    GetBackupsComplete?.Invoke(this, new RESTBackupsEventArgs(backupsSerialization));
                }
                return backupsSerialization;
            }
            else
            { throw new Exception("GetAsync has gone in timeout"); } //With TaskCanceledException I can not read the exception message in the parent catch

        }

        async public Task PutBackupsAsync(string backupsSerialization, int timeoutRequestMs = 9000)
        {
            if (httpClient == null) httpClient = new HttpClient();

            if (SettingsM.Instance.RestSetting == null)
                throw new Exception("ricevuto SettingsM.Instance.RestSetting a null");

            SettingsM.Instance.RestSetting.ValidMySelf();
            if (SettingsM.Instance.RestSetting.IsValid == false)
                throw new Exception("ricevuto SettingsM.Instance.RestSetting a null");

            var timeoutCancellationTokenSource = new CancellationTokenSource();

            string url = SettingsM.Instance.RestSetting.RootAddress + SettingsM.Instance.RestSetting.RoutePrefix + SettingsM.Instance.RestSetting.RouteOfPutBackups;

            var httpContent = new StringContent(backupsSerialization, Encoding.UTF8, "application/json");

            Task<HttpResponseMessage> task = httpClient.PutAsync(url, httpContent, timeoutCancellationTokenSource.Token);
         
            int taskIndex = Task.WaitAny(task, Task.Delay(timeoutRequestMs, timeoutCancellationTokenSource.Token));
            if (taskIndex == 0)
            {
                //timeoutCancellationTokenSource.Cancel();
                //await task;
                try
                {
                    if (task.Result.StatusCode == System.Net.HttpStatusCode.OK)
                    { string resultMessage = task.Result.Content.ReadAsStringAsync().Result; }
                    else
                    {
                        string content = await task.Result.Content.ReadAsStringAsync();
                        throw new Exception("PutAsync return error, task.Result.StatusCode:<" + task.Result.StatusCode.ToString() + ">, task.Result.Content:<" + content + ">");
                    }
                    return;
                }
                catch (Exception ex)
                {
                    Log.main.Add(new Mess(Tipi.Warn, "", "ex.tostring:<" + ex.ToString() + ">"));
                    throw new Exception("Something went wrong in REST call, see log");
                }
            }
            else //Timeout
                throw new Exception("PutAsync has gone in timeout");
        }
    }

    public class RESTBackupsEventArgs : EventArgs
    {
        public string backupsSerialization;

        public RESTBackupsEventArgs(string backupsSerialization)
        {
            this.backupsSerialization = backupsSerialization;
        }
    }
}
