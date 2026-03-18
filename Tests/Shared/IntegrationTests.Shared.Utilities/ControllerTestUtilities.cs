using Shared.Models;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace IntegrationTests.Shared
{
    public static class ControllerTestUtilities
    {

        #region Get

        public static async Task<ErrorValidationResult<TResponse>> GetAllRecordsWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, bool deleteCache = true)
        {
            apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(deleteCache));

            var response = await client.GetAsync(apiEndPoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetFilteredRecordsWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, object req)
        {
            var postReq = FormatPostRequest(req);

            var response = await client.PostAsync(apiEndPoint + "/Filter", postReq);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetRecordByIdWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, int id, bool deleteCache = true)
        {
            apiEndPoint = apiEndPoint + "/" + id;
            apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(deleteCache));

            var response = await client.GetAsync(apiEndPoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> GetRecordByIdWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, int id, BaseServiceGet req)
        {
            apiEndPoint = apiEndPoint + "/" + id;

            if (req.DeleteCache) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(req.DeleteCache));
            }
            
            if (req.IncludeInactive) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateIncludeInactiveQueryStringParm(req.IncludeInactive));
            }

            if (req.IncludeRelated) 
            {
                apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateIncludeRelatedQueryStringParm(req.IncludeRelated));
            }

            var response = await client.GetAsync(apiEndPoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        public static async Task<T[]> GetAllRecords<T>(HttpClient client, string apiEndPoint, bool deleteCache = true)
        {
            apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(deleteCache));

            var response = await client.GetAsync(apiEndPoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<T[]>(response);

            Assert.IsType<T[]>(ret);

            return ret;
        }

        public static async Task<T[]> GetFilteredRecords<T>(HttpClient client, string apiEndPoint, object req)
        {
            var postReq = FormatPostRequest(req);

            var response = await client.PostAsync(apiEndPoint + "/Filter", postReq);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<T[]>(response);

            Assert.IsType<T[]>(ret);

            return ret;
        }

        public static async Task<T> GetRecordById<T>(HttpClient client, string apiEndPoint, int id, bool deleteCache = true)
        {
            apiEndPoint = apiEndPoint + "/" + id;
            apiEndPoint = AddQueryStringParmToApiEndPointUrl(apiEndPoint, CreateDeleteCacheQueryStringParm(deleteCache));

            var response = await client.GetAsync(apiEndPoint);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<T>(response);

            Assert.IsType<T>(ret);

            return ret;
        }

        #endregion

        #region Post

        public static async Task<T> CreateRecord<T>(HttpClient client, string apiEndPoint, object req)
        {
            var postReq = FormatPostRequest(req);

            var response = await client.PostAsync(apiEndPoint, postReq);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var ret = await GetResponseContent<T>(response);

            Assert.IsType<T>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> CreateRecordWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, object req)
        {
            var postReq = FormatPostRequest(req);

            var response = await client.PostAsync(apiEndPoint, postReq);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        #endregion

        #region Put

        public static async Task<T> UpdateRecord<T>(HttpClient client, string apiEndPoint, object req, int id)
        {
            var putReq = FormatPostRequest(req);

            var response = await client.PutAsync(apiEndPoint + "/" + id, putReq);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<T>(response);

            Assert.IsType<T>(ret);

            return ret;
        }

        public static async Task<ErrorValidationResult<TResponse>> UpdateRecordWithValidationResult<TResponse>(HttpClient client, string apiEndPoint, object req, int id)
        {
            var putReq = FormatPostRequest(req);

            var response = await client.PutAsync(apiEndPoint + "/" + id, putReq);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var ret = await GetResponseContent<ErrorValidationResult<TResponse>>(response);

            Assert.IsType<ErrorValidationResult<TResponse>>(ret);

            return ret;
        }

        #endregion

        #region Delete

        public static async Task<HttpResponseMessage> DeleteRecord(HttpClient client, string apiEndPoint, int id)
        {
            var response = await client.DeleteAsync(apiEndPoint + "/" + id);

            //assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

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

        #endregion
    }
}
