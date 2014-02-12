using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace StoreApp.Framework
{
    public class HttpHelper
    {
        private readonly Dictionary<string, string> _preDic;

        /// <summary>
        /// 获取或设置请求超时前等待的毫秒数，默认20秒
        /// </summary>
        public TimeSpan Timeout { get; set; }

        public HttpHelper()
        {
            _preDic = new Dictionary<string, string>();
            Timeout = TimeSpan.FromMilliseconds(20000);
        }

        /// <summary>
        /// 追加参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpHelper AppendParameter(string key, string value)
        {
            if (_preDic.ContainsKey(key))
                throw new ArgumentException("key is existed!");

            _preDic.Add(key, value);
            return this;
        }

        public string GetParam()
        {
            if (!_preDic.Any())
                return string.Empty;

            var sb = new StringBuilder("?");
            foreach (var item in _preDic)
            {
                sb.Append(string.Format("{0}={1}&", item.Key, item.Value));
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        public async Task<String> GetStringAsync(string url)
        {
            string result;
            using (var httpClient = new HttpClient { Timeout = Timeout })
            {
                httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));
                
                try
                {
                    var response = await httpClient.GetByteArrayAsync(String.Concat(url, GetParam()));
                    var responseString = Encoding.UTF8.GetString(response, 0, response.Length);
                    return responseString;

                    //result = await httpClient.GetStringAsync(String.Concat(url, GetParam()));
                }
                catch (HttpRequestException ex)
                {
                    result = ex.Message;
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
            }
            _preDic.Clear();
            return result;
        }

        /// <summary>
        /// Post请求（FormUrlEncoded）
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<String> PostAsync(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));

            HttpContent content = new FormUrlEncodedContent(_preDic);
            string result;
            try
            {
                var httpMessage = await httpClient.PostAsync(url, content);
                result = await httpMessage.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            _preDic.Clear();
            return result;
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileStream">上传文件的字节数组</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public async Task<String> PostAsync(string url, byte[] fileStream, string fileName = "image.jpg")
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));

            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");// 表单数据使用
            var content = new MultipartFormDataContent(boundary);

            // 表单参数
            var formContent = new FormUrlEncodedContent(_preDic);
            content.Add(formContent);

            // 文件参数
            var byteContent = new ByteArrayContent(fileStream);
            content.Add(byteContent, "file", fileName);

            var httpMessage = await httpClient.PostAsync(url, content);
            var result = await httpMessage.Content.ReadAsStringAsync();
            _preDic.Clear();
            return result;
        }
    }
}
