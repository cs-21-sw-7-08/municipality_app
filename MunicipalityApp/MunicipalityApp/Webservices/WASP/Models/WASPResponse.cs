using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalityApp.Webservices.WASP
{
    public class WASPResponse
    {
        public WASPResponse()
        {

        }

        public bool IsSuccessful { get; set; }
        public int ErrorNo { get; set; }
        public string ErrorMessage { get; set; }

        public string ResponseErrorMessage
        {
            get
            {
                if (!IsSuccessful && ErrorNo != 0)
                    return $"WASP error: {ErrorNo}";
                else if (ErrorMessage != null)
                    return ErrorMessage;
                return null;
            }
        }
    }

    public class WASPResponse<ResultType> : WASPResponse
    {
        public WASPResponse()
        {

        }

        public ResultType Result { get; set; }
    }
}
