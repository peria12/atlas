// <copyright file="RestClientService.cs" company="Atlas Air">
// Copyright (c) 2016 All Rights Reserved
// <author>Anil Gopalan</author>
// </copyright>
// <created Date> 04/20/2020 </created Date>
// <summary>Flight services  </summary>
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GlobalNetApps.AtlasFlightSchedule.RestClient
{
    public class RestClientService : HttpClient
    {
        private static readonly RestClientService instance = new RestClientService();
        public HttpCookie authCookie = System.Web.HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];

        static RestClientService() { }

        public static RestClientService Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Get API URL from configuration file.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public string GetApiUrl(string relativePath)
        {
            var baseUrl = System.Configuration.ConfigurationManager.AppSettings["ApiUrl"];
            return baseUrl + relativePath;
        }

        /// <summary>
        /// Build URL with parameters.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        protected string BuildCompleteUri(string relativePath, Dictionary<string, string> param = null)
        {
            string apiurl = GetApiUrl(relativePath);

            if (param != null)
            {
                apiurl += "?";
                foreach (var item in param) { apiurl += item.Key + "=" + item.Value + "&"; }
                apiurl = apiurl.Remove(apiurl.LastIndexOf("&"));
            }

            return apiurl;
        }

        /// <summary>
        /// To get the API Service data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="relativePath"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> GetListItems<T>(string relativePath, Dictionary<string, string> param)
        {
            var url = new Uri(BuildCompleteUri(relativePath, param));

            var authCookie = System.Web.HttpContext.Current.Request.Cookies[System.Web.Security.FormsAuthentication.FormsCookieName];

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer", parameter: authCookie.Value.ToString());
            var responseMessage = SendAsync(requestMessage);

            responseMessage.Wait();
            if (responseMessage.Result.IsSuccessStatusCode)
            {
                var content = responseMessage.Result.Content.ReadAsStringAsync();
                content.Wait();
                var Items = JsonConvert.DeserializeObject<List<T>>(content.Result);
                return Items;
            }
            throw new Exception(responseMessage.Result.ReasonPhrase);
        }

        /// <summary>
        /// Post data to the API service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public string PostItem<T>(T item, string relativePath)
        {
            var uri = new Uri(BuildCompleteUri(relativePath));
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            Task<HttpResponseMessage> response = null;
            response = PostAsync(uri, content);
            response.Wait();
            if (response.Result.IsSuccessStatusCode)
            {
                return response.Result.IsSuccessStatusCode.ToString();
            }

            throw new Exception(response.Result.ReasonPhrase);
        }
    }

}