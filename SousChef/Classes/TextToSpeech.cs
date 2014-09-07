using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using RestSharp;
namespace SousChef.Classes
{
    public class TextToSpeech
    {
        #region Class variables and Data structures

        /// <summary>
        /// Temporary variables for processing
        /// </summary>
        private string fqdn;
        private string x_arg = "ClientApp=C#ClientApp,ClientVersion=2_2,ClientScreen=Browser,ClientSdk=C#Restful,DeviceType=WebServer,DeviceOs=Windows";
        
        /// <summary>
        /// Temporary variables for processing
        /// </summary>
        private string apiKey, secretKey, accessToken, scope, refreshToken, refreshTokenExpiryTime, accessTokenExpiryTime, bypassSSL;

        /// <summary>
        /// variable for having the posted file.
        /// </summary>
        private string TTSPlainText = string.Empty;
        private string TTSSSML = string.Empty;

        /// <summary>
        /// Gets or sets the value of refreshTokenExpiresIn
        /// </summary>
        private int refreshTokenExpiresIn;

        string xArgsData = string.Empty;

        public string TTSErrorMessage = string.Empty;
        public string TTSSuccessMessage = string.Empty;
        public byte[] receivedBytes = null;

        /// <summary>
        /// Access Token Types
        /// </summary>
        public enum AccessType
        {
            /// <summary>
            /// Access Token Type is based on Client Credential Mode
            /// </summary>
            ClientCredential,

            /// <summary>
            /// Access Token Type is based on Refresh Token
            /// </summary>
            RefreshToken
        }

        #endregion

        public TextToSpeech()
        {
            this.fqdn = "https://api.att.com";
            this.apiKey = ConfigurationManager.AppSettings.Get("ATTapiKey");
            this.secretKey = ConfigurationManager.AppSettings.Get("ATTsecretKey");
            this.scope = "TTS";
            this.refreshTokenExpiresIn = 24;


            //if (!IsPostBack)
            //{
            //    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["X-Arg"]))
            //    {
            //        xArgsData = HttpUtility.UrlEncode(ConfigurationManager.AppSettings["X-Arg"]);
            //        x_arg.Text = ConfigurationManager.AppSettings["X-Arg"];
            //    }
            //}

        }


        public string ProcessText(string text) {
            try
            {
                bool IsValid = true;

                IsValid = this.ReadAndGetAccessToken(ref TTSErrorMessage);
                if (IsValid == false)
                {
                    TTSErrorMessage = "Unable to get access token";
                    return null;
                }

                return this.ConvertTextToSpeech(this.fqdn, "/speech/v3/textToSpeech", this.accessToken, x_arg, "text/plain", text);
            }
            catch (Exception ex)
            {
                TTSErrorMessage = ex.Message;
                return null;
            }
        }


        #region Access Token Related Functions


        /// <summary>
        /// This function reads the Access Token File and stores the values of access token, expiry seconds
        /// refresh token, last access token time and refresh token expiry time
        /// This funciton returns true, if access token file and all others attributes read successfully otherwise returns false
        /// </summary>
        /// <param name="panelParam">Panel Details</param>
        /// <returns>Returns boolean</returns>    
        private bool ReadAccessTokenFile(ref string message)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream("C:\\sous\\SousChefAccessToken.txt", FileMode.OpenOrCreate, FileAccess.Read);
                streamReader = new StreamReader(fileStream);
                this.accessToken = streamReader.ReadLine();
                this.accessTokenExpiryTime = streamReader.ReadLine();
                this.refreshToken = streamReader.ReadLine();
                this.refreshTokenExpiryTime = streamReader.ReadLine();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
            finally
            {
                if (null != streamReader)
                {
                    streamReader.Close();
                }

                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }

            if ((this.accessToken == null) || (this.accessTokenExpiryTime == null) || (this.refreshToken == null) || (this.refreshTokenExpiryTime == null))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This function validates the expiry of the access token and refresh token.
        /// function compares the current time with the refresh token taken time, if current time is greater then returns INVALID_REFRESH_TOKEN
        /// function compares the difference of last access token taken time and the current time with the expiry seconds, if its more, returns INVALID_ACCESS_TOKEN    
        /// otherwise returns VALID_ACCESS_TOKEN
        /// </summary>
        /// <returns>string, which specifies the token validity</returns>
        private string IsTokenValid()
        {
            try
            {
                DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();
                if (currentServerTime >= DateTime.Parse(this.accessTokenExpiryTime))
                {
                    if (currentServerTime >= DateTime.Parse(this.refreshTokenExpiryTime))
                    {
                        return "INVALID_ACCESS_TOKEN";
                    }
                    else
                    {
                        return "REFRESH_TOKEN";
                    }
                }
                else
                {
                    return "VALID_ACCESS_TOKEN";
                }
            }
            catch
            {
                return "INVALID_ACCESS_TOKEN";
            }
        }

        /// <summary>
        /// This function get the access token based on the type parameter type values.
        /// If type value is 1, access token is fetch for client credential flow
        /// If type value is 2, access token is fetch for client credential flow based on the exisiting refresh token
        /// </summary>
        /// <param name="type">Type as integer</param>
        /// <param name="panelParam">Panel details</param>
        /// <returns>Return boolean</returns>
        private bool GetAccessToken(AccessType type, ref string message)
        {
            FileStream fileStream = null;
            Stream postStream = null;
            StreamWriter streamWriter = null;

            // This is client credential flow
            if (type == AccessType.ClientCredential)
            {
                try
                {
                    DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

                    WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.fqdn + "/oauth/v4/token");
                    accessTokenRequest.Method = "POST";
                    string oauthParameters = string.Empty;
                    if (type == AccessType.ClientCredential)
                    {
                        oauthParameters = "client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&grant_type=client_credentials&scope=" + this.scope;
                    }
                    else
                    {
                        oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
                    }

                    accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] postBytes = encoding.GetBytes(oauthParameters);
                    accessTokenRequest.ContentLength = postBytes.Length;

                    postStream = accessTokenRequest.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);

                    WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
                    using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
                    {
                        string jsonAccessToken = accessTokenResponseStream.ReadToEnd().ToString();
                        JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                        AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(jsonAccessToken, typeof(AccessTokenResponse));

                        this.accessToken = deserializedJsonObj.access_token;
                        this.accessTokenExpiryTime = currentServerTime.AddSeconds(Convert.ToDouble(deserializedJsonObj.expires_in)).ToString();
                        this.refreshToken = deserializedJsonObj.refresh_token;

                        DateTime refreshExpiry = currentServerTime.AddHours(this.refreshTokenExpiresIn);

                        if (deserializedJsonObj.expires_in.Equals("0"))
                        {
                            int defaultAccessTokenExpiresIn = 100; // In Yearsint yearsToAdd = 100;
                            this.accessTokenExpiryTime = currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongDateString() + " " + currentServerTime.AddYears(defaultAccessTokenExpiresIn).ToLongTimeString();
                        }

                        this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();

                        fileStream = new FileStream("C:\\sous\\SousChefAccessToken.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        streamWriter = new StreamWriter(fileStream);
                        streamWriter.WriteLine(this.accessToken);
                        streamWriter.WriteLine(this.accessTokenExpiryTime);
                        streamWriter.WriteLine(this.refreshToken);
                        streamWriter.WriteLine(this.refreshTokenExpiryTime);

                        // Close and clean up the StreamReader
                        accessTokenResponseStream.Close();
                        return true;
                    }
                }
                catch (WebException we)
                {
                    string errorResponse = string.Empty;

                    try
                    {
                        using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                        {
                            errorResponse = sr2.ReadToEnd();
                            sr2.Close();
                        }
                    }
                    catch
                    {
                        errorResponse = "Unable to get response";
                    }

                    message = errorResponse + Environment.NewLine + we.ToString();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
                finally
                {
                    if (null != postStream)
                    {
                        postStream.Close();
                    }

                    if (null != streamWriter)
                    {
                        streamWriter.Close();
                    }

                    if (null != fileStream)
                    {
                        fileStream.Close();
                    }
                }
            }
            else if (type == AccessType.RefreshToken)
            {
                try
                {
                    DateTime currentServerTime = DateTime.UtcNow.ToLocalTime();

                    WebRequest accessTokenRequest = System.Net.HttpWebRequest.Create(string.Empty + this.fqdn + "/oauth/v4/token");
                    accessTokenRequest.Method = "POST";

                    string oauthParameters = "grant_type=refresh_token&client_id=" + this.apiKey + "&client_secret=" + this.secretKey + "&refresh_token=" + this.refreshToken;
                    accessTokenRequest.ContentType = "application/x-www-form-urlencoded";

                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] postBytes = encoding.GetBytes(oauthParameters);
                    accessTokenRequest.ContentLength = postBytes.Length;

                    postStream = accessTokenRequest.GetRequestStream();
                    postStream.Write(postBytes, 0, postBytes.Length);

                    WebResponse accessTokenResponse = accessTokenRequest.GetResponse();
                    using (StreamReader accessTokenResponseStream = new StreamReader(accessTokenResponse.GetResponseStream()))
                    {
                        string accessTokenJSon = accessTokenResponseStream.ReadToEnd().ToString();
                        JavaScriptSerializer deserializeJsonObject = new JavaScriptSerializer();

                        AccessTokenResponse deserializedJsonObj = (AccessTokenResponse)deserializeJsonObject.Deserialize(accessTokenJSon, typeof(AccessTokenResponse));
                        this.accessToken = deserializedJsonObj.access_token.ToString();
                        DateTime accessTokenExpiryTime = currentServerTime.AddMilliseconds(Convert.ToDouble(deserializedJsonObj.expires_in.ToString()));
                        this.refreshToken = deserializedJsonObj.refresh_token.ToString();

                        fileStream = new FileStream("C:\\sous\\SousChefAccessToken.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        streamWriter = new StreamWriter(fileStream);
                        streamWriter.WriteLine(this.accessToken);
                        streamWriter.WriteLine(this.accessTokenExpiryTime);
                        streamWriter.WriteLine(this.refreshToken);

                        // Refresh token valids for 24 hours
                        DateTime refreshExpiry = currentServerTime.AddHours(24);
                        this.refreshTokenExpiryTime = refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString();
                        streamWriter.WriteLine(refreshExpiry.ToLongDateString() + " " + refreshExpiry.ToLongTimeString());

                        accessTokenResponseStream.Close();
                        return true;
                    }
                }
                catch (WebException we)
                {
                    string errorResponse = string.Empty;

                    try
                    {
                        using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                        {
                            errorResponse = sr2.ReadToEnd();
                            sr2.Close();
                        }
                    }
                    catch
                    {
                        errorResponse = "Unable to get response";
                    }

                    message = errorResponse + Environment.NewLine + we.ToString();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
                finally
                {
                    if (null != postStream)
                    {
                        postStream.Close();
                    }

                    if (null != streamWriter)
                    {
                        streamWriter.Close();
                    }

                    if (null != fileStream)
                    {
                        fileStream.Close();
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Neglect the ssl handshake error with authentication server 
        /// </summary>
        private static void BypassCertificateError()
        {
            string bypassSSL = ConfigurationManager.AppSettings["IgnoreSSL"];

            if ((!string.IsNullOrEmpty(bypassSSL))
                && (string.Equals(bypassSSL, "true", StringComparison.OrdinalIgnoreCase)))
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                    delegate(Object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                    {
                        return true;
                    };
            }
        }


        /// <summary>
        /// This function is used to read access token file and validate the access token
        /// this function returns true if access token is valid, or else false is returned
        /// </summary>
        /// <param name="panelParam">Panel Details</param>
        /// <returns>Returns Boolean</returns>
        private bool ReadAndGetAccessToken(ref string responseString)
        {
            bool result = true;
            if (this.ReadAccessTokenFile(ref responseString) == false)
            {
                result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
            }
            else
            {
                string tokenValidity = this.IsTokenValid();
                if (tokenValidity == "REFRESH_TOKEN")
                {
                    result = this.GetAccessToken(AccessType.RefreshToken, ref responseString);
                }
                else if (string.Compare(tokenValidity, "INVALID_ACCESS_TOKEN") == 0)
                {
                    result = this.GetAccessToken(AccessType.ClientCredential, ref responseString);
                }
            }

            if (this.accessToken == null || this.accessToken.Length <= 0)
            {
                return false;
            }
            else
            {
                return result;
            }
        }
        #endregion


        #region Speech Service Functions


        /// <summary>
        /// This function invokes api SpeechToText to convert the given wav amr file and displays the result.
        /// </summary>
        private string ConvertTextToSpeech(string parEndPoint, string parURI, string parAccessToken, string parXarg, string parContentType, string parContent)
        {
            try
            {

                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(string.Empty + parEndPoint + parURI);
                httpRequest.Headers.Add("Authorization", "Bearer " + parAccessToken);
                httpRequest.Headers.Add("X-SpeechContext", parXarg);
                httpRequest.ContentLength = parContent.Length;
                httpRequest.ContentType = parContentType;
                httpRequest.Accept = "audio/x-wav";
                httpRequest.Method = "POST";
                httpRequest.KeepAlive = true;

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postBytes = encoding.GetBytes(parContent);
                httpRequest.ContentLength = postBytes.Length;

                using (Stream writeStream = httpRequest.GetRequestStream())
                {
                    writeStream.Write(postBytes, 0, postBytes.Length);
                    writeStream.Close();
                }
                HttpWebResponse speechResponse = (HttpWebResponse)httpRequest.GetResponse();
                
                int offset = 0;
                int remaining = Convert.ToInt32(speechResponse.ContentLength);
                using (var stream = speechResponse.GetResponseStream())
                {
                    receivedBytes = new byte[speechResponse.ContentLength];
                    while (remaining > 0)
                    {
                        int read = stream.Read(receivedBytes, offset, remaining);
                        if (read <= 0)
                        {
                            TTSErrorMessage = String.Format("End of stream reached with {0} bytes left to read", remaining);
                            return null;
                        }

                        remaining -= read;
                        offset += read;
                    }
                    string result = Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None);
                    
                    //audioPlay.Attributes.Add("src", "data:audio/wav;base64," + Convert.ToBase64String(receivedBytes, Base64FormattingOptions.None));
                    TTSSuccessMessage = "Success";
                    return result;
                }
            }
            catch (WebException we)
            {
                string errorResponse = string.Empty;
                try
                {
                    using (StreamReader sr2 = new StreamReader(we.Response.GetResponseStream()))
                    {
                        errorResponse = sr2.ReadToEnd();
                        sr2.Close();
                    }
                    TTSErrorMessage = errorResponse;
                }
                catch
                {
                    errorResponse = "Unable to get response";
                    TTSErrorMessage = errorResponse;
                }
            }
            catch (Exception ex)
            {
                TTSErrorMessage = ex.Message;
                return null;
            }

            return null;
        }


        #endregion
    
    
    }


    #region Access Token and Speech Response Data Structures

    /// <summary>
    /// Access Token Data Structure
    /// </summary>
    public class AccessTokenResponse
    {
        /// <summary>
        /// Gets or sets Access Token ID
        /// </summary>
        public string access_token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Refresh Token ID
        /// </summary>
        public string refresh_token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets Expires in milli seconds
        /// </summary>
        public string expires_in
        {
            get;
            set;
        }
    }
    #endregion
}