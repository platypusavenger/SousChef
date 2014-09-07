using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SousChef.Classes;

namespace SousChef.Controllers
{
    public class TestController : ApiController
    {
        private TextToSpeech _tts = new TextToSpeech();
        
        // GET api/Test/
        /// <summary>
        /// A test stub for Text To Speech
        /// </summary>
        /// <returns>
        /// 200 - Success + An HTML fragment to test
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public string Get(string test)
        {
            string text = "I am the model of a major modern general";
            if (test != null)
                text = test;
            
            string value = _tts.ProcessText(text);
            string result = "<audio controls=\"controls\" autobuffer=\"autobuffer\" autoplay=\"autoplay\" runat=\"server\" id=\"audioPlay\" src=\"data:audio/wav;base64," + value + "\"></audio>";
            return result;
        }
    }
}
