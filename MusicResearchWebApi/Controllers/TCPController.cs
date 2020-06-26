using MusicResearchWebApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Sockets;
using System.Configuration;
using System.Globalization;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MusicResearchWebApi.DatabaseModels;

namespace MusicResearchWebApi.Controllers
{
    [RoutePrefix("tcp")]
    public class TCPController : ApiController
    {
        private readonly int port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private readonly string server = ConfigurationManager.AppSettings["host"];
        private readonly string azureMLApiKey = ConfigurationManager.AppSettings["azureMLApiKey"];
        private readonly string scoringUri = ConfigurationManager.AppSettings["scoringUri"];

        private static readonly HttpClient client = new HttpClient();

        public TcpClient Client { get; set; }
        public NetworkStream Stream { get; set; }

        [HttpGet]
        [Route("{id}")]
        public async Task<HttpResponseMessage> GetSongGenre(Guid id)
        {
            BlobPathModel blobModel = new BlobPathModel
            {
                blob_path = String.Format("{0}/music.mp3", id)
            };
            var json = new JavaScriptSerializer().Serialize(blobModel);

            var response = GetMLParameterFromBlobPath(json);
            if (!string.IsNullOrEmpty(response))
            {
                AzureSongModel model = MapResponseToAzureSongModel(response, id);

                var songGenre = await GetSongGenreFromAzureML(model);

                if (!string.IsNullOrEmpty(songGenre))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, songGenre);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
        }

        private string GetMLParameterFromBlobPath(string blobPathJson)
        {
            try
            {
                Regex regex = new Regex(@"(?<=\[).+?(?=\])");

                // Create a TcpClient.
                Client = new TcpClient(server, port);

                // Get a client stream for reading and writing.
                Stream = Client.GetStream();

                // Translate the passed message into UTF8 and store it as a Byte array.
                byte[] message = System.Text.Encoding.UTF8.GetBytes(blobPathJson);

                // Send the message to the connected TcpServer. 
                Stream.Write(message, 0, message.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                message = new byte[2048];

                // String to store the response ASCII representation.
                string responseData = string.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = Stream.Read(message, 0, message.Length);
                responseData = System.Text.Encoding.UTF8.GetString(message, 0, bytes);
                Match match = regex.Match(responseData);

                // Close everything.
                Stream.Close();
                Client.Close();

                return match.ToString();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            return "";
        }

        private AzureSongModel MapResponseToAzureSongModel(string message, Guid id)
        {
            AzureSongModel songModel = new AzureSongModel();
            string[] stringValues = message.Split(',');
            decimal[] decimalValues = new decimal[46];

            if (stringValues.Length == 46)
            {
                int i = 0;
                foreach (var value in stringValues)
                {
                    decimalValues[i] = decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                    i++;
                }

                songModel.Song_name = id.ToString();
                songModel.Song_length = decimalValues[0];
                songModel.Tempo = decimalValues[1];
                songModel.Total_beats = decimalValues[2];
                songModel.Average_beats = decimalValues[3];
                songModel.Zcr_mean = decimalValues[4];
                songModel.Zcr_var = decimalValues[5];
                songModel.Zcr_std = decimalValues[6];
                songModel.Cent_mean = decimalValues[7];
                songModel.Cent_var = decimalValues[8];
                songModel.Cent_std = decimalValues[9];
                songModel.Rolloff_mean = decimalValues[10];
                songModel.Rolloff_var = decimalValues[11];
                songModel.Rolloff_std = decimalValues[12];
                songModel.Chroma_mean = decimalValues[13];
                songModel.Chroma_var = decimalValues[14];
                songModel.Chroma_std = decimalValues[15];
                songModel.Chroma_cqt_mean = decimalValues[16];
                songModel.Chroma_cqt_var = decimalValues[17];
                songModel.Chroma_cqt_std = decimalValues[18];
                songModel.Chroma_cens_mean = decimalValues[19];
                songModel.Chroma_cens_var = decimalValues[20];
                songModel.Chroma_cens_std = decimalValues[21];
                songModel.Mfccs_mean = decimalValues[22];
                songModel.Mfccs_var = decimalValues[23];
                songModel.Mfccs_std = decimalValues[24];
                songModel.Mfcc_delta_mean = decimalValues[25];
                songModel.Mfcc_delta_var = decimalValues[26];
                songModel.Mfcc_delta_std = decimalValues[27];
                songModel.Mel_mean = decimalValues[28];
                songModel.Mel_var = decimalValues[29];
                songModel.Mel_std = decimalValues[30];
                songModel.Tonnetz_mean = decimalValues[31];
                songModel.Tonnetz_var = decimalValues[32];
                songModel.Tonnetz_std = decimalValues[33];
                songModel.Spec_bw_mean = decimalValues[34];
                songModel.Spec_bw_var = decimalValues[35];
                songModel.Spec_bw_std = decimalValues[36];
                songModel.Spec_con_mean = decimalValues[37];
                songModel.Spec_con_var = decimalValues[38];
                songModel.Spec_con_std = decimalValues[39];
                songModel.Harmonic_mean = decimalValues[40];
                songModel.Harmonic_var = decimalValues[41];
                songModel.Harmonic_std = decimalValues[42];
                songModel.Percussive_mean = decimalValues[43];
                songModel.Percussive_var = decimalValues[44];
                songModel.Percussive_std = decimalValues[45];
            }

            return songModel;
        }

        private async Task<string> GetSongGenreFromAzureML(AzureSongModel model)
        {
            var scoreRequest = new
            {
                Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"song_name", "song_length", "tempo", "total_beats", "average_beats", "zcr_mean", "zcr_var", "zcr_std", "cent_mean", "cent_var", "cent_std", "rolloff_mean",
                                    "rolloff_var", "rolloff_std", "chroma_mean", "chroma_var", "chroma_std", "chroma_cqt_mean", "chroma_cqt_var", "chroma_cqt_std", "chroma_cens_mean", "chroma_cens_var",
                                    "chroma_cens_std", "mfccs_mean", "mfccs_var", "mfccs_std", "mfcc_delta_mean", "mfcc_delta_var", "mfcc_delta_std", "mel_mean", "mel_var", "mel_std", "tonnetz_mean",
                                    "tonnetz_var", "tonnetz_std", "spec_bw_mean", "spec_bw_var", "spec_bw_std", "spec_con_mean", "spec_con_var", "spec_con_std", "harmonic_mean", "harmonic_var",
                                    "harmonic_std", "percussive_mean", "percussive_var", "percussive_std", "genre"},
                                Values = new string[,] {  { model.Song_name, model.Song_length.ToString(), model.Tempo.ToString(), model.Total_beats.ToString(), model.Average_beats.ToString(), model.Zcr_mean.ToString(), model.Zcr_var.ToString(), model.Zcr_std.ToString(),
                                        model.Cent_mean.ToString(), model.Cent_var.ToString(), model.Cent_std.ToString(), model.Rolloff_mean.ToString(), model.Rolloff_var.ToString(), model.Rolloff_std.ToString(), model.Chroma_mean.ToString(),
                                        model.Chroma_var.ToString(), model.Chroma_std.ToString(), model.Chroma_cqt_mean.ToString(), model.Chroma_cqt_var.ToString(), model.Chroma_cqt_std.ToString(), model.Chroma_cens_mean.ToString(),
                                        model.Chroma_cens_var.ToString(), model.Chroma_cens_std.ToString(), model.Mfccs_mean.ToString(), model.Mfccs_var.ToString(), model.Mfccs_std.ToString(), model.Mfcc_delta_mean.ToString(),
                                        model.Mfcc_delta_var.ToString(), model.Mfcc_delta_std.ToString(), model.Mel_mean.ToString(), model.Mel_var.ToString(), model.Mel_std.ToString(), model.Tonnetz_mean.ToString(), model.Tonnetz_var.ToString(),
                                        model.Tonnetz_std.ToString(), model.Spec_bw_mean.ToString(), model.Spec_bw_var.ToString(), model.Spec_bw_std.ToString(), model.Spec_con_mean.ToString(), model.Spec_con_var.ToString(), model.Spec_con_std.ToString(),
                                        model.Harmonic_mean.ToString(), model.Harmonic_var.ToString(), model.Harmonic_std.ToString(), model.Percussive_mean.ToString(), model.Percussive_var.ToString(), model.Percussive_std.ToString(), "" }, 
                                        { model.Song_name, model.Song_length.ToString(), model.Tempo.ToString(), model.Total_beats.ToString(), model.Average_beats.ToString(), model.Zcr_mean.ToString(), model.Zcr_var.ToString(), model.Zcr_std.ToString(),
                                        model.Cent_mean.ToString(), model.Cent_var.ToString(), model.Cent_std.ToString(), model.Rolloff_mean.ToString(), model.Rolloff_var.ToString(), model.Rolloff_std.ToString(), model.Chroma_mean.ToString(),
                                        model.Chroma_var.ToString(), model.Chroma_std.ToString(), model.Chroma_cqt_mean.ToString(), model.Chroma_cqt_var.ToString(), model.Chroma_cqt_std.ToString(), model.Chroma_cens_mean.ToString(),
                                        model.Chroma_cens_var.ToString(), model.Chroma_cens_std.ToString(), model.Mfccs_mean.ToString(), model.Mfccs_var.ToString(), model.Mfccs_std.ToString(), model.Mfcc_delta_mean.ToString(),
                                        model.Mfcc_delta_var.ToString(), model.Mfcc_delta_std.ToString(), model.Mel_mean.ToString(), model.Mel_var.ToString(), model.Mel_std.ToString(), model.Tonnetz_mean.ToString(), model.Tonnetz_var.ToString(),
                                        model.Tonnetz_std.ToString(), model.Spec_bw_mean.ToString(), model.Spec_bw_var.ToString(), model.Spec_bw_std.ToString(), model.Spec_con_mean.ToString(), model.Spec_con_var.ToString(), model.Spec_con_std.ToString(),
                                        model.Harmonic_mean.ToString(), model.Harmonic_var.ToString(), model.Harmonic_std.ToString(), model.Percussive_mean.ToString(), model.Percussive_var.ToString(), model.Percussive_std.ToString() , "" },  }
                            }
                        },
                    },
                GlobalParameters = new Dictionary<string, string>()
                {
                }
            };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", azureMLApiKey);

            client.BaseAddress = new Uri(scoringUri);

            HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest);


            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                JObject jObject = JObject.Parse(result);
                JToken jUser = jObject["Results"]["output1"]["value"]["Values"][0].Last;
                return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(jUser.ToString());
            }
            else
            {
                return String.Empty;
            }
        }


    }
}
