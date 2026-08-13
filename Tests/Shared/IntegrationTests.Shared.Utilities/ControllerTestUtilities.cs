using Shared.Models;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using Dto.Security.Authentication;
using IntegrationTests.Shared.Utilities;
using IntegrationTests.Shared.Models;

namespace IntegrationTests.Shared
{
    public static class ControllerTestUtilities
    {

        #region Authenticate

        public static async Task<AuthenticationResponse> AuthenticateTestUserAndReturnAuthToken(HttpClient client, string email = TestConstants.DefaultTestUserEmail, string password = TestConstants.DefaultTestUserPassword, string applicationName = TestConstants.DefaultTestUserApplicationName)
        {
            var postReq = FormatPostRequest(new {
                Email = email,
                Password = password,
                ApplicationName = applicationName
            });

            var response = await client.PostAsync(ApiEndPoints.Security.Authentication.Base + "/Authenticate", postReq);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<AuthenticationResponse>>(response);

            ret.Errors.Count.Should().Be(0);

            Assert.IsType<ErrorValidationResult<AuthenticationResponse>>(ret);

            return ret.Response;
        }

        #endregion

        #region Get

        public static async Task<HttpResponseMessage> GetAllRecords(HttpClient client, string apiEndPoint, string token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, apiEndPoint);

            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);

            return response;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetAllRecordsWithValidationResult<TResponse>(HttpGetRequestParms req)
        {
            var response = await ExecuteDefaultGetRequest(req);

            response.StatusCode.Should().Be(req.ExpectedStatusCode);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetFilteredRecordsWithValidationResult<TResponse>(HttpPostRequestParms req)
        {
            req.ApiEndPoint += "/Filter";
            
            var response = await ExecuteDefaultPostRequest(req);

            response.StatusCode.Should().Be(req.ExpectedStatusCode);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new ErrorValidationResult<TResponse>();
            }

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetRecordByIdWithValidationResult<TResponse>(HttpGetRequestParms req)
        {
            req.ApiEndPoint = req.ApiEndPoint + "/" + req.RecordId;
            
            var response = await ExecuteDefaultGetRequest(req);
            response.StatusCode.Should().Be(req.ExpectedStatusCode);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

       public static async Task<HttpResponseMessage> GetRecordById(HttpClient client, string apiEndPoint, int id, string token)
       {
            using var request = new HttpRequestMessage(HttpMethod.Get, apiEndPoint + "/" + id);

            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);

            return response;
        }

        public static async Task<HttpResponseMessage> GetFilteredRecords(HttpClient client, string apiEndPoint, object req, string token)
        {
            var postReq = FormatPostRequest(req);

            using var request = new HttpRequestMessage(HttpMethod.Post, apiEndPoint + "/Filter");
            
            if (req != null)
            {
                request.Content = postReq;
            }
            
            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);

            return response;
        }

        #endregion

        #region Post

        public static async Task<HttpResponseMessage> CreateRecord(HttpClient client, string apiEndPoint, object req, string token)
        {
            var postReq = FormatPostRequest(req);

            using var request = new HttpRequestMessage(HttpMethod.Post, apiEndPoint);

            if (req != null)
            {
                request.Content = postReq;
            }

            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);
            
            return response;
        }

        public static async Task<ErrorValidationResult<TResponse>> CreateRecordWithValidationResult<TResponse>(HttpPostRequestParms req)
        {
            var response = await ExecuteDefaultPostRequest(req);
            response.StatusCode.Should().Be(req.ExpectedStatusCode);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ErrorValidationResult<TResponse>();
            }

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        #endregion

        #region Put

        public static async Task<ErrorValidationResult<TResponse>> UpdateRecordWithValidationResult<TResponse>(HttpPutRequestParms req)
        {
            req.ApiEndPoint += "/" + req.RecordId;

            var response = await ExecuteDefaultPutRequest(req);
            response.StatusCode.Should().Be(req.ExpectedStatusCode);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new ErrorValidationResult<TResponse>();
            }

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<HttpResponseMessage> UpdateRecord(HttpClient client, string apiEndPoint, object req, int id, string token)
        {
            var postReq = FormatPostRequest(req);

            using var request = new HttpRequestMessage(HttpMethod.Put, apiEndPoint + "/" + id);

            if (req != null)
            {
                request.Content = postReq;
            }

            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);
            
            return response;
        }

        #endregion

        #region Delete

        public static async Task<HttpResponseMessage> DeleteRecord(HttpClient client, string apiEndPoint, int id, string token)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, apiEndPoint + "/" + id);

            AddAuthorizationHeaderIfApplicable(request, token);

            var response = await client.SendAsync(request);

            return response;
        }

        #endregion

        #region Helpers

        public static StringContent FormatPostRequest(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        public static async Task<T> GetResponseContent<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<T>(stringResponse);

            return result;
        }

        public static string CreateIncludeInactiveQueryStringParm(bool includeInactive)
        {
            return $"IncludeInactive={includeInactive}";
        }

        public static string CreateDeleteCacheQueryStringParm(bool deleteCache)
        {
            return $"DeleteCache={deleteCache}";
        }

        public static string CreateIncludeRelatedQueryStringParm(bool includeRelated)
        {
            return $"IncludeRelated={includeRelated}";
        }
        
        public static string CreateIncludeReadOnlyQueryStringParm(bool includeReadOnly)
        {
            return $"IncludeReadOnly={includeReadOnly}";
        }

        public static string AddQueryStringParmToApiEndPointUrl(string apiEndPoint, string queryStringParm)
        {
            if (apiEndPoint.Contains("?"))
            {
                //verify url has query string and not just a question mark (Get ct of characters after ?)
                var count = 0;
                int index = apiEndPoint.IndexOf("?");
                count = apiEndPoint.Length - index - 1;

                if (count == 0)
                {
                    //url has ? but nothing after
                    apiEndPoint += queryStringParm;
                }
                else 
                {
                    //url has ? along with query string parm
                    apiEndPoint += "&" + queryStringParm;
                }
            }
            else
            {
                //url does not have any ? / query string parms
                apiEndPoint += "?" + queryStringParm;
            }

            return apiEndPoint;
        }

        public static HttpRequestMessage AddAuthorizationHeaderIfApplicable(HttpRequestMessage req, string jwtToken)
        {
            if (!string.IsNullOrWhiteSpace(jwtToken))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            }

            return req;
        }


        private static async Task<HttpResponseMessage> ExecuteDefaultGetRequest(HttpGetRequestParms req)
        {
            req.ApiEndPoint  = AddQueryStringParmsToApiEndPointUrl(req.ApiEndPoint, req.QueryStringParms);
            
            using var request = new HttpRequestMessage(HttpMethod.Get, req.ApiEndPoint);

            AddAuthorizationHeaderIfApplicable(request, req.Token);

            var response = await req.Client.SendAsync(request);
            return response;
        }

        private static async Task<HttpResponseMessage> ExecuteDefaultPostRequest(HttpPostRequestParms req, HttpMethod method = null)
        {
            method ??= HttpMethod.Post;

            var postReq = FormatPostRequest(req.RequestObject);

            req.ApiEndPoint  = AddQueryStringParmsToApiEndPointUrl(req.ApiEndPoint, req.QueryStringParms);
            
            using var request = new HttpRequestMessage(method, req.ApiEndPoint);

            AddAuthorizationHeaderIfApplicable(request, req.Token);
            
            if (req.RequestObject != null)
            {
                request.Content = postReq;
            }

            var response = await req.Client.SendAsync(request);
            return response;
        }

        private static async Task<HttpResponseMessage> ExecuteDefaultPutRequest(HttpPutRequestParms req)
        {
            return await ExecuteDefaultPostRequest(new HttpPostRequestParms
            {
                Client = req.Client,
                ApiEndPoint = req.ApiEndPoint,
                RequestObject = req.RequestObject,
                Token = req.Token,
                QueryStringParms = req.QueryStringParms,
                ExpectedStatusCode = req.ExpectedStatusCode
            }, HttpMethod.Put);
        }

        private static string AddQueryStringParmsToApiEndPointUrl(string apiEndPoint, BaseServiceGet queryStringParms)
        {
            if (queryStringParms.DeleteCache) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(queryStringParms.DeleteCache));
            }
            
            if (queryStringParms.IncludeInactive) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateIncludeInactiveQueryStringParm(queryStringParms.IncludeInactive));
            }

            if (queryStringParms.IncludeRelated) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateIncludeRelatedQueryStringParm(queryStringParms.IncludeRelated));
            }

            if (queryStringParms.IncludeReadOnly) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateIncludeReadOnlyQueryStringParm(queryStringParms.IncludeReadOnly));
            }

            return apiEndPoint;
        }

        #endregion
    }
}
