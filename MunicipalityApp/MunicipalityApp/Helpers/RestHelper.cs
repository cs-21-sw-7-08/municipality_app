using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp
{
    public class RestResponse
    {
        public HttpResponseMessage Response { get; private set; }
        public string Body { get; private set; }
        public bool Timeout { get; private set; }
        public string Exception { get; private set; }
        public string ErrorMessage
        {
            get
            {
                if (Exception != null)
                    return Exception;
                else if (Timeout)
                    return "HTTP error: Timeout";
                else if (((int)Response.StatusCode) < 200 || ((int)Response.StatusCode) > 299)
                    return $"HTTP error: {(int)Response.StatusCode}";
                return null;
            }
        }
        public bool IsSuccess
        {
            get
            {
                if (Exception != null || Timeout)
                    return false;
                else if (((int)Response.StatusCode) < 200 || ((int)Response.StatusCode) > 299)
                    return false;
                return true;
            }
        }

        public RestResponse(HttpResponseMessage response, string body)
        {
            Response = response;
            Body = body;
        }

        public RestResponse(bool timeout = true)
        {
            Timeout = timeout;
        }

        public RestResponse(string exception)
        {
            Exception = exception;
        }
    }

    public class RestParameter
    {
        public RestParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }
        public string Value { get; private set; }
    }

    public class RestHelper
    {
        #region Variables



        #endregion

        #region Constructor

        public RestHelper(string url, int timeoutInSeconds = 30)
        {
            Url = url;
            TimeoutInSeconds = timeoutInSeconds;
            Client = new HttpClient();
        }

        #endregion

        #region Private methods

        private string GetParametersAsString(List<RestParameter> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return string.Empty;
            var tempString = "";
            foreach (var parameter in parameters)
            {
                tempString += string.IsNullOrEmpty(tempString) ? "?" : "&";
                tempString += $"{parameter.Name}={parameter.Value}";
            }
            return tempString;
        }

        private string GetCompleteUri(string controllerPath, string functionName)
        {
            return $"{Url}{controllerPath}{functionName}";
        }

        private async Task<RestResponse> MakeRequest(Func<Task<HttpResponseMessage>> requestFunction)
        {
            try
            {
                var response = await requestFunction();
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return new RestResponse(response, responseBody);
            }
            catch (Exception exc)
            {
                return new RestResponse(exc.Message);
            }
        }

        #endregion

        #region Public methods

        public async Task<RestResponse> SendGetRequest(string function, List<RestParameter> restParameters = null)
        {
            var urlComplete = Url + function + GetParametersAsString(restParameters);
            return await MakeRequest(async () => await Client.GetAsync(urlComplete));
        }

        public async Task<RestResponse> SendPostRequest(string function, string body = null, List<RestParameter> restParameters = null)
        {
            var urlComplete = Url + function + GetParametersAsString(restParameters);
            return await MakeRequest(async () =>
                await Client.PostAsync(
                        urlComplete,
                        new StringContent(body == null ? "" : body, Encoding.UTF8, "application/json")
                    )
                );
        }

        public async Task<RestResponse> SendPutRequest(string function, string body = null, List<RestParameter> restParameters = null)
        {
            var urlComplete = Url + function + GetParametersAsString(restParameters);
            return await MakeRequest(async () => await Client.PutAsync(
                        urlComplete,
                        new StringContent(body == null ? "" : body, Encoding.UTF8, "application/json")
                    )
                );
        }

        public async Task<RestResponse> SendDeleteRequest(string function, List<RestParameter> restParameters = null)
        {
            var urlComplete = Url + function + GetParametersAsString(restParameters);
            return await MakeRequest(async () =>
                await Client.DeleteAsync(urlComplete));
        }

        #endregion

        #region Properties

        private string Url { get; set; }
        private int TimeoutInSeconds { get; set; }
        private HttpClient Client { get; set; }

        #endregion
    }
}
