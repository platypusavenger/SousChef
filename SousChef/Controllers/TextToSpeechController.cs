using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SousChef.Classes;
using RestSharp;

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
        
        [HttpGet]
        [Route("api/texttospeech/fakeSomeData")]
        public IHttpActionResult FakeSomeData()
        {
            List<string> result = new List<string>();
            string startTime = DateTime.UtcNow.ToString("o");
            DateTime newTime = DateTime.Parse(startTime);
            newTime = newTime.AddMinutes(1.00);
            startTime = newTime.ToString("o");
            string leaderEndLat = "26.1333";
            string leaderEndLong = "-80.15";
            // Start 33.755, 84.3900;  end 26.1333° N, 80.1500° 
            string leader = "e69d0f5b75ae4ed23dd0f7f8b91c12c2";
            string leaderStartLat = "33.7550";
            string leaderStartLong = "-84.3900";
            string leaderFuel = "73";
            string leaderSpeed = "55";
            string leaderSteering = "170";

            string follower1 = "c2f6362f23984502c750a5377e51fe01";
            string follower1StartLat = "33.990";
            string follower1StartLong = "-84.5600";
            string follower1Fuel = "61";

            string follower2 = "860ff4464436ba6c2cdfa3e93040dee7";
            string follower2StartLat = "33.690";
            string follower2StartLong = "-85.5600";
            string follower2Fuel = "94";

            for (int i = 0; i < 5; i++)
            {
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/latitude/value { 'at': '" + startTime + "', 'value': '" + leaderStartLat + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower1 + @"/streams/latitude/value { 'at': '" + startTime + "', 'value': '" + follower1StartLat + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower2 + @"/streams/latitude/value { 'at': '" + startTime + "', 'value': '" + follower2StartLat + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/steering_wheel_angle/value { 'at': '" + startTime + "', 'value': '" + leaderSteering + "'}");
                result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/speed/value { 'at': '" + startTime + "', 'value': '" + leaderSpeed + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/longitude/value { 'at': '" + startTime + "', 'value': '" + leaderStartLong + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower1 + @"/streams/longitude/value { 'at': '" + startTime + "', 'value': '" + follower1StartLong + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower2 + @"/streams/longitude/value { 'at': '" + startTime + "', 'value': '" + follower2StartLong + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + leaderFuel + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower1 + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + follower1Fuel + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower2 + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + follower2Fuel + "'}");

                newTime = DateTime.Parse(startTime);
                newTime = newTime.AddMinutes(1.00);
                startTime = newTime.ToString("o");
                leaderStartLat = (double.Parse(leaderStartLat) - 0.15).ToString();
                leaderStartLong = (double.Parse(leaderStartLong) - 0.15).ToString();
                follower1StartLat = (double.Parse(follower1StartLat) - Math.Abs((double.Parse(leaderStartLat) - double.Parse(follower1StartLat))/3)).ToString();
                follower1StartLong = (double.Parse(follower1StartLat) - Math.Abs((double.Parse(leaderStartLat) - double.Parse(follower1StartLat)) / 3)).ToString();
                follower2StartLat = (double.Parse(follower2StartLat) - Math.Abs((double.Parse(leaderStartLat) - double.Parse(follower2StartLat)) / 5)).ToString();
                follower2StartLong = (double.Parse(follower2StartLat) - Math.Abs((double.Parse(leaderStartLat) - double.Parse(follower2StartLat)) / 5)).ToString();
                Random random = new Random();
                leaderSpeed = (4400 + random.Next(-1000, 1200)).ToString();
                leaderSteering = (180 + random.Next(-5, 5)).ToString();
                System.Threading.Thread.Sleep(150);
            }

            for (int i = 0; i < 1000; i++)
            {
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/latitude/value { 'at': '" + startTime + "', 'value': '" + leaderStartLat + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/longitude/value { 'at': '" + startTime + "', 'value': '" + leaderStartLong + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/steering_wheel_angle/value { 'at': '" + startTime + "', 'value': '" + leaderSteering + "'}");
                result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/speed/value { 'at': '" + startTime + "', 'value': '" + leaderSpeed + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + leader + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + leaderFuel + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower1 + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + leaderFuel + "'}");
                //result.Add(@"PUT https://api-m2x.att.com/v1/feeds/" + follower2 + @"/streams/fuel_usage/value { 'at': '" + startTime + "', 'value': '" + leaderFuel + "'}");
                
                newTime = DateTime.Parse(startTime);
                newTime = newTime.AddMinutes(1.00);
                startTime = newTime.ToString("o");
                leaderStartLat = (double.Parse(leaderStartLat) - 0.05).ToString();
                leaderStartLong = (double.Parse(leaderStartLong) - 0.01).ToString();
                Random random = new Random();
                leaderSpeed = (4400 + random.Next(-1000, 1200)).ToString();
                leaderSteering = (180 + random.Next(-5, 5)).ToString();

                if (i % 5 == 0)
                {
                    leaderFuel = (int.Parse(leaderFuel) - 1).ToString();
                    follower1Fuel = (int.Parse(follower1Fuel) - 1).ToString();
                    follower2Fuel = (int.Parse(follower2Fuel) - 1).ToString();
                }
                System.Threading.Thread.Sleep(150);
            }

                return Ok(result);
        }
    }
}
