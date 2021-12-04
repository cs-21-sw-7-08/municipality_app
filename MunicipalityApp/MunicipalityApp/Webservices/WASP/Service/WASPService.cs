using MunicipalityApp.Webservices.WASP.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class WASPServiceResponse<ResponseType>
        where ResponseType : WASPResponse
    {
        public string Exception { get; private set; }
        public ResponseType WASPResponse { get; private set; }
        public bool IsSuccess
        {
            get
            {
                if (Exception != null) return false;
                return WASPResponse.IsSuccessful;
            }
        }
        public string ErrorMessage
        {
            get
            {
                if (Exception != null)
                    return Exception;
                if (WASPResponse != null)
                    return WASPResponse.ResponseErrorMessage;

                return null;
            }
        }

        public WASPServiceResponse(string exception)
        {
            Exception = exception;
        }

        public WASPServiceResponse(ResponseType waspResponse)
        {
            WASPResponse = waspResponse;
            Exception = null;
        }
    }

    public class WASPService : IWASPServiceFunctions
    {
        #region Variables

        const string IssueControllerPath = "/WASP/Issues/";
        const string MunicipalityControllerPath = "/WASP/Municipality/";
        const string CitizenControllerPath = "/WASP/Citizen/";

        #endregion

        #region Properties

        private RestHelper RestHelper { get; set; }

        #endregion

        #region Constructor

        public WASPService(string url)
        {
            RestHelper = new RestHelper(url);
        }

        #endregion

        #region Private methods

        private async Task<WASPServiceResponse<T>> ActionCallAsync<T>(Func<Task<RestResponse>> getResponse) where T : WASPResponse
        {
            var response = await getResponse();
            // Check for HTTP errors
            if (!response.IsSuccess)
                return new WASPServiceResponse<T>(response.ErrorMessage);
            // Deserialize response
            var waspResponse = JsonConvert.DeserializeObject<T>(response.Body);
            // Return response
            return new WASPServiceResponse<T>(waspResponse);
        }

        #endregion

        #region Methods

        public async Task<WASPServiceResponse<WASPResponse<List<Category>>>> GetListOfCategories()
        {
            return await ActionCallAsync<WASPResponse<List<Category>>>(
                () => RestHelper.SendGetRequest($"{IssueControllerPath}GetListOfCategories"));
        }

        public async Task<WASPServiceResponse<WASPResponse>> BlockCitizen(int citizenId)
        {
            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPutRequest($"{CitizenControllerPath}BlockCitizen",
                restParameters: new List<RestParameter>() {
                    new RestParameter("citizenId", citizenId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse>> UnblockCitizen(int citizenId)
        {
            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPutRequest($"{CitizenControllerPath}UnblockCitizen",
                restParameters: new List<RestParameter>() {
                    new RestParameter("citizenId", citizenId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse<Issue>>> GetIssueDetails(int issueId)
        {
            return await ActionCallAsync<WASPResponse<Issue>>(
                () => RestHelper.SendGetRequest($"{IssueControllerPath}GetIssueDetails",
                restParameters: new List<RestParameter>() {
                    new RestParameter("issueId", issueId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse<List<Issue>>>> GetListOfIssues(IssuesOverviewFilter filter)
        {
            var jsonString = JsonConvert.SerializeObject(filter, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return await ActionCallAsync<WASPResponse<List<Issue>>>(
                () => RestHelper.SendPostRequest($"{IssueControllerPath}GetListOfIssues", body: jsonString));
        }

        public async Task<WASPServiceResponse<WASPResponse>> UpdateIssueStatus(int issueId, int issueStateId)
        {
            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPutRequest($"{IssueControllerPath}UpdateIssueStatus",
                restParameters: new List<RestParameter>() {
                    new RestParameter("issueId", issueId.ToString()),
                    new RestParameter("issueStateId", issueStateId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse<List<ReportCategory>>>> GetListOfReportCategories()
        {
            return await ActionCallAsync<WASPResponse<List<ReportCategory>>>(
                () => RestHelper.SendGetRequest($"{IssueControllerPath}GetListOfReportCategories"));
        }

        public async Task<WASPServiceResponse<WASPResponse<MunicipalityUser>>> LogInMunicipality(MunicipalityUserLogin municipalityUser)
        {
            var jsonString = JsonConvert.SerializeObject(municipalityUser, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return await ActionCallAsync<WASPResponse<MunicipalityUser>>(
                () => RestHelper.SendPostRequest($"{MunicipalityControllerPath}LogInMunicipality", body: jsonString));
        }

        public async Task<WASPServiceResponse<WASPResponse>> CreateMunicipalityResponse(MunicipalityResponseCreate municipalityResponse)
        {
            var jsonString = JsonConvert.SerializeObject(municipalityResponse, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPostRequest($"{MunicipalityControllerPath}CreateMunicipalityResponse", body: jsonString));
        }

        public async Task<WASPServiceResponse<WASPResponse>> UpdateMunicipalityResponse(int municipalityResponseId, List<WASPUpdate> waspUpdates)
        {
            var jsonString = JsonConvert.SerializeObject(waspUpdates, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPutRequest($"{MunicipalityControllerPath}UpdateMunicipalityResponse", body: jsonString, restParameters: new List<RestParameter>
                {
                    new RestParameter("responseId", municipalityResponseId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse>> DeleteMunicipalityResponse(int municipalityResponseId)
        {
            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendDeleteRequest($"{MunicipalityControllerPath}DeleteMunicipalityResponse",
                restParameters: new List<RestParameter>() {
                    new RestParameter("responseId", municipalityResponseId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse<List<Issue>>>> GetListOfReports(int municipalityId)
        {
            return await ActionCallAsync<WASPResponse<List<Issue>>>(
                () => RestHelper.SendGetRequest($"{IssueControllerPath}GetListOfReports", restParameters: new List<RestParameter>
                {
                    new RestParameter("municipalityId", municipalityId.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse<List<Citizen>>>> GetListOfCitizens(int municipalityId, bool isBlocked)
        {
            return await ActionCallAsync<WASPResponse<List<Citizen>>>(
                () => RestHelper.SendGetRequest($"{CitizenControllerPath}GetListOfCitizens", restParameters: new List<RestParameter>
                {
                    new RestParameter("municipalityId", municipalityId.ToString()),
                    new RestParameter("isBlocked", isBlocked.ToString())
                }));
        }

        public async Task<WASPServiceResponse<WASPResponse>> SignUpMunicipalityUser(MunicipalityUserSignUp municipalityUser)
        {
            var jsonString = JsonConvert.SerializeObject(municipalityUser, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            return await ActionCallAsync<WASPResponse>(
                () => RestHelper.SendPostRequest($"{MunicipalityControllerPath}SignUpMunicipality", body: jsonString));
        }

        #endregion

    }
}
