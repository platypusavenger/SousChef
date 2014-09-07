using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SousChef.Classes;

namespace SousChef.Controllers
{
    public class TextToSpeechController : ApiController
    {
        private TextToSpeech _tts = new TextToSpeech();
        
        // GET api/TextToSpeech?text="This is my text"
        /// <summary>
        /// A pass-through for Text To Speech
        /// </summary>
        /// <returns>
        /// 200 - Success + An HTML fragment to test
        /// 500 - Internal Server Error + Exception
        /// </returns>
        public IHttpActionResult Get(string text)
        {
            string defaultText = "Text to speech is pretty neat";
            if (text != null)
                defaultText = text;
            defaultText = text.Replace("\"", "");
            
            string value = _tts.ProcessText(defaultText);
            string result = "<audio controls='controls' autobuffer='autobuffer' autoplay='autoplay' id='audioPlay' src='data:audio/wav;base64," + value + "'></audio>";
            return Ok(result);
        }
    }
}
