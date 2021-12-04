using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP.Interfaces
{
    public interface IWASPServiceFunctions
    {
        Task<WASPServiceResponse<WASPResponse<List<Category>>>> GetListOfCategories();
        Task<WASPServiceResponse<WASPResponse>> BlockCitizen(int citizenId);
        Task<WASPServiceResponse<WASPResponse>> UnblockCitizen(int citizenId);
        Task<WASPServiceResponse<WASPResponse<Issue>>> GetIssueDetails(int issueId);
        Task<WASPServiceResponse<WASPResponse<List<Issue>>>> GetListOfIssues(IssuesOverviewFilter filter);
        Task<WASPServiceResponse<WASPResponse>> UpdateIssueStatus(int issueId, int issueStateId);
        Task<WASPServiceResponse<WASPResponse<List<ReportCategory>>>> GetListOfReportCategories();
        Task<WASPServiceResponse<WASPResponse<MunicipalityUser>>> LogInMunicipality(MunicipalityUserLogin municipalityUser);
        Task<WASPServiceResponse<WASPResponse>> CreateMunicipalityResponse(MunicipalityResponseCreate municipalityResponse);
        Task<WASPServiceResponse<WASPResponse>> UpdateMunicipalityResponse(int municipalityResponseId, List<WASPUpdate> waspUpdates);
        Task<WASPServiceResponse<WASPResponse>> DeleteMunicipalityResponse(int municipalityResponseId);
        Task<WASPServiceResponse<WASPResponse<List<Issue>>>> GetListOfReports(int municipalityId);
        Task<WASPServiceResponse<WASPResponse<List<Citizen>>>> GetListOfCitizens(int municipalityId, bool isBlocked);
        Task<WASPServiceResponse<WASPResponse>> SignUpMunicipalityUser(MunicipalityUserSignUp municipalityUser);
    }
}
