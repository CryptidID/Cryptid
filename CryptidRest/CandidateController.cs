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
    [RoutePrefix("CandidateDelegate")]
    public class CandidateController : ApiController {
        private class ApiError {
            public ApiError(string name, string msg) {
                ErrorName = name;
                Error = msg;
            }

            public string Error { get; set; }
            public string ErrorName { get; set; }
        }

        private static readonly string PublicKeyPath = "public.xml";

        [HttpPost, Route("FullVerifyFromChain")]
        public float FullVerifyFromChain([FromBody] JObject json) {
            try {
                var data = json.ToObject <CandidateDataObjects.FullVerifyFromChain>();

                using (var ms = new MemoryStream(Convert.FromBase64String(data.FingerprintImageBase64))) {
                    var b = new Bitmap(ms);
                    var f = new Fingerprint { AsBitmap = b };
                    return CandidateDelegate.FullVerifyFromChain(Convert.FromBase64String(data.ChainIdBase64), data.Password, f,
                        Keys.PublicKey(PublicKeyPath));
                }
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost, Route("GetCandidateFromChain")]
        public Candidate GetCandidateFromChain([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.GetCandidateFromChain>();

                var packed = CandidateDelegate.GetPackedCandidate(Convert.FromBase64String(data.ChainIdBase64));
                return CandidateDelegate.Unpack(packed, data.Password, Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost, Route("UnpackCandidate")]
        public Candidate UnpackCandidate([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.UnpackCandidate>();

                return CandidateDelegate.Unpack(Convert.FromBase64String(data.PackedBase64), data.Password,
                    Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [HttpPost, Route("VerifySignature")]
        public bool VerifySignature([FromBody] JObject json) {
            try {
                var data = json.ToObject<CandidateDataObjects.VerifySignature>();

                return CandidateDelegate.VerifySignature(Convert.FromBase64String(data.PackedBase64),
                    Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent(JsonConvert.SerializeObject(new ApiError(e.GetType().Name, e.Message))),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}