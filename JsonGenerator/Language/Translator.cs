using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace JsonGenerator.Language
{
    public class Translator
    {
        private Translator()
        {
            AdmAccessToken admToken;
            string headerValue;
            //Get Client Id and Client Secret from https://datamarket.azure.com/developer/applications/
            //Refer obtaining AccessToken (http://msdn.microsoft.com/en-us/library/hh454950.aspx) 
            AdmAuthentication admAuth = new AdmAuthentication("23a38075-2526-4ad7-9c3f-cc57d8bc95fd", "H8NJoRlvyHwANQoo/hyVKUULGy08XwIZ1cY0c5bSkrs=");

            admToken = admAuth.GetAccessToken();
            headerValue = "Bearer " + admToken.access_token;


            httpRequestProperty = new HttpRequestMessageProperty();
            httpRequestProperty.Headers.Add("Authorization", headerValue);
            languageServiceClient = new TranslationService.LanguageServiceClient();
        }

        TranslationService.LanguageServiceClient languageServiceClient = null;
        HttpRequestMessageProperty httpRequestProperty = null;
        static Translator instance = null;
        public static Translator Instance
        {
            get {
                if (instance == null)
                    instance = new Translator();
                return instance;
            }
        }

        public String Translate(String word)
        {
            var languageCode = System.Configuration.ConfigurationManager.AppSettings["languageCode"] as String;
            using (OperationContextScope scope = new OperationContextScope(languageServiceClient.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpRequestProperty;

                string translationResult;
                //Keep appId parameter blank as we are sending access token in authorization header.
                translationResult = languageServiceClient.Translate("", word, "en", languageCode, "text/plain", "", "");
                return translationResult;
            }
        }
    }
}
