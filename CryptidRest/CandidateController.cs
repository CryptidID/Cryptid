#region

using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Cryptid;
using Cryptid.Utils;
using SourceAFIS.Simple;

#endregion

namespace CryptidRest.Controllers {
    [RoutePrefix("api/candidate")]
    public class CandidateController : ApiController {
        private const string PublicKeyPath = "public.xml";

        [Route("FullVerifyFromChain/{chainIdBase64}/{password}/{fpImageBase64}")]
        [HttpPost]
        public float FullVerifyFromChain(string chainIdBase64, string password, string fpImageBase64) {
            try {
                using (var ms = new MemoryStream(Convert.FromBase64String(fpImageBase64))) {
                    var b = new Bitmap(ms);
                    var f = new Fingerprint { AsBitmap = b };
                    return CandidateDelegate.FullVerifyFromChain(Convert.FromBase64String(chainIdBase64), password, f,
                        Keys.PublicKey(PublicKeyPath));
                }
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [Route("GetCandidateFromChain/{chainIdBase64}/{password}")]
        [HttpGet]
        public Candidate GetCandidateFromChain(string chainIdBase64, string password) {
            try {
                var packed = CandidateDelegate.GetPackedCandidate(Convert.FromBase64String(chainIdBase64));
                return CandidateDelegate.Unpack(packed, password, Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [Route("UnpackCandidate/{packedBase64}/{password}")]
        [HttpPost]
        public Candidate UnpackCandidate(string packedBase64, string password) {
            try {
                return CandidateDelegate.Unpack(Convert.FromBase64String(packedBase64), password,
                    Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }

        [Route("UnpackCandidate/{packedBase64}")]
        [HttpPost]
        public bool VerifySignature(string packedBase64) {
            try {
                return CandidateDelegate.VerifySignature(Convert.FromBase64String(packedBase64),
                    Keys.PublicKey(PublicKeyPath));
            } catch (Exception e) {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError) {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = "Internal Error Occured"
                };
                throw new HttpResponseException(resp);
            }
        }
    }
}