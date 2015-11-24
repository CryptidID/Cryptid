#region

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cryptid;
using Cryptid.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SourceAFIS.Simple;

#endregion

namespace CryptidRest.Controllers {
    /// <summary>
    /// The rest API controller for CandidateDelegate
    /// </summary>
    [RoutePrefix("CandidateDelegate")]
    public class CandidateController : ApiController {
        private static readonly string PublicKeyPath = "public.xml";

        /// <summary>
        /// API method to fully verify a candidate from the block chain
        /// </summary>
        /// <param name="json">A json object representing CandidateDataObjects.FullVerifyFromChain</param>
        /// <returns>A float representing the verification confidence</returns>
        [HttpPost, Route("FullVerifyFromChain")]
        public float FullVerifyFromChain([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.FullVerifyFromChain>();

                using (var ms = new MemoryStream(Convert.FromBase64String(data.FingerprintImageBase64))) {
                    var b = new Bitmap(ms);
                    var f = new Fingerprint {AsBitmap = b};
                    return CandidateDelegate.FullVerifyFromChain(Convert.FromBase64String(data.ChainIdBase64),
                        data.Password, f,
                        Keys.PublicKey(PublicKeyPath));
                }
            }
            catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// API method to get a packed candidate from the block chain
        /// </summary>
        /// <param name="json">The json object representing CandidateDataObjects.GetCandidateFromChain</param>
        /// <returns></returns>
        [HttpPost, Route("GetCandidateFromChain")]
        public Candidate GetCandidateFromChain([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.GetCandidateFromChain>();

                var packed = CandidateDelegate.GetPackedCandidate(Convert.FromBase64String(data.ChainIdBase64));
                return CandidateDelegate.Unpack(packed, data.Password, Keys.PublicKey(PublicKeyPath));
            }
            catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// API method to unpack a packed candidate
        /// </summary>
        /// <param name="json">A json object representing CandidateDataObjects.UnpackCandidate</param>
        /// <returns>The unpacked candidate JSON</returns>
        [HttpPost, Route("UnpackCandidate")]
        public Candidate UnpackCandidate([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.UnpackCandidate>();

                return CandidateDelegate.Unpack(Convert.FromBase64String(data.PackedBase64), data.Password,
                    Keys.PublicKey(PublicKeyPath));
            }
            catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// API method to verify the signature of packed candidate data
        /// </summary>
        /// <param name="json">A json object representing CandidateDataObjects.VerifySignature</param>
        /// <returns>Whether or not the RSA signature is valid</returns>
        [HttpPost, Route("VerifySignature")]
        public bool VerifySignature([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.VerifySignature>();

                return CandidateDelegate.VerifySignature(Convert.FromBase64String(data.PackedBase64),
                    Keys.PublicKey(PublicKeyPath));
            }
            catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        /// <summary>
        /// Represents the API error json structure
        /// </summary>
        private class ApiError {
            /// <summary>
            /// Creates a new ApiError instance
            /// </summary>
            /// <param name="name">The name of this error</param>
            /// <param name="msg">The message for this error</param>
            public ApiError(string name, string msg) {
                ErrorName = name;
                Error = msg;
            }

            /// <summary>
            /// The error message
            /// </summary>
            public string Error { get; set; }
            /// <summary>
            /// The error name
            /// </summary>
            public string ErrorName { get; set; }
        }
    }
}