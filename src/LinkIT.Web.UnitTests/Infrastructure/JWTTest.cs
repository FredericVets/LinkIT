using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIT.Web.UnitTests.Infrastructure
{
	[TestClass]
	public class JWTTest
	{
		private string _jwt = @"eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJmbWtCMTdqSXB4SmpJWVFOWjlGQ3FpckN1ejk4aDdpUzZLZWdMbmN5SXpRIn0.eyJleHAiOjE2MDQzMzI4MDcsImlhdCI6MTYwNDMzMjUwNywiYXV0aF90aW1lIjoxNjA0MzMyNTAzLCJqdGkiOiIzZTEwNjZiYy02ZTczLTQ4MjMtYjUwMy03OWZiNTA5ZmNhYzMiLCJpc3MiOiJodHRwczovL2lkcC50LmljdHMua3VsZXV2ZW4uYmUvYXV0aC9yZWFsbXMva3VsZXV2ZW4iLCJzdWIiOiI3ODg0NGFkYy00ZGRlLTRlMTMtYWI5NC00NGVkZmI1MDI1YWUiLCJ0eXAiOiJCZWFyZXIiLCJhenAiOiJwb3N0bWFuIiwic2Vzc2lvbl9zdGF0ZSI6ImI1MWU4YTA5LTc1NGEtNGQ5Zi04ODVjLWRjMTUxNjM5NTA0NyIsImFjciI6IjEiLCJzY29wZSI6Im9wZW5pZCBpYW0tZmFjaWxpdHltZ210LndyaXRlIGVtYWlsIGxpb28taGFyZHdhcmUubWFuYWdlIGlhbS1hY2NvdW50LnJlYWQgcHJvZmlsZSIsIm5hbWUiOiJGcmVkZXJpYyBWZXRzIiwicHJlZmVycmVkX3VzZXJuYW1lIjoidTAxMjI3MTMiLCJnaXZlbl9uYW1lIjoiRnJlZGVyaWMiLCJmYW1pbHlfbmFtZSI6IlZldHMiLCJlbWFpbCI6ImZyZWRlcmljLnZldHNAa3VsZXV2ZW4uYmUifQ.L4tqEc4HnFHbHT-n6G3_sPDqaIATQJeJ93A889CNfDcE0CsOc3mDkKFZvBDkfBqfDDUMuECcIYlP34voYZ4oMxLPxpoLEmLRtk9qAJ2l1DRhij7sMk9Oh7JixoGmgyh8UUCU6kGUsIedhaydbJOoiKFaKSohffFIuGBoN6qVn7pmc_EfB4CBRl_ibGaLv2om3FPD0NqrirA0WstNJaeh9B79XZliKSATQUpZRbsvGftdGm7Zjk4KGIe-lV-yQtIsArRzGL-0bIQt_j3XGedbhIAu_GHE9SfHynq0M20m6c_m4ndU2QEdsKhZ1Ubbo_d-3N59T5h2_Abcn30Ed0G18w";
		
		private string _jwksJson = @"
			{ ""keys"": [
				{
					""kid"":""fmkB17jIpxJjIYQNZ9FCqirCuz98h7iS6KegLncyIzQ"",
					""kty"":""RSA"",
					""alg"":""RS256"",
					""use"":""sig"",
					""n"":""w59Elybgb2NNq7Q6fYiRPb8j-2GS9TYpuV_INT-qkptFrTwf98cVOilQpRPLXDFoJYS88rzAoIa02wDNKXXG8-LFBSjTMvAPvX_MuuA5iSD6qQ0bNNTjqYPLKNj8jv8u7DOSicTekEUDv6QxijpqKMORYmm8E20doNxf3PgJora8O_ekzSoXgExonKcK7IqIoW3TLRTWmD8o_J5pMWdNKdZNJwIfYM3_Z-OEN4lPgSBbuVzx32-FHjGRawJaWsZIn-1iQoCs8KTuW8ZRiAnOF3wL5EFVA9LXBsWE60HdzEC-glhhkf-HKGnIxGv_aMzLgk_uzcuTiq4TZX8Nk9Yb3Q"",
					""e"":""AQAB"",
					""x5c"": [
						""MIICnzCCAYcCBgFkId3LFTANBgkqhkiG9w0BAQsFADATMREwDwYDVQQDDAhrdWxldXZlbjAeFw0xODA2MjExMDIwNDBaFw0yODA2MjExMDIyMjBaMBMxETAPBgNVBAMMCGt1bGV1dmVuMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAw59Elybgb2NNq7Q6fYiRPb8j+2GS9TYpuV/INT+qkptFrTwf98cVOilQpRPLXDFoJYS88rzAoIa02wDNKXXG8+LFBSjTMvAPvX/MuuA5iSD6qQ0bNNTjqYPLKNj8jv8u7DOSicTekEUDv6QxijpqKMORYmm8E20doNxf3PgJora8O/ekzSoXgExonKcK7IqIoW3TLRTWmD8o/J5pMWdNKdZNJwIfYM3/Z+OEN4lPgSBbuVzx32+FHjGRawJaWsZIn+1iQoCs8KTuW8ZRiAnOF3wL5EFVA9LXBsWE60HdzEC+glhhkf+HKGnIxGv/aMzLgk/uzcuTiq4TZX8Nk9Yb3QIDAQABMA0GCSqGSIb3DQEBCwUAA4IBAQAieGfS34B2heZiwK5P9Ci2iB+gMV/IclN5w6X+p9mkHlB0pOjyyPa3xOCAF1VsF4dih9Gs0U5yGYBW6GiblBjBZFKp3M5kVbRZVLwh2sgKi/w23/riEylWgg0cYInEtFavHLmLHAPfJ/Mmj5rb1/l9xadN2Z+bJZjmG9zCOxFG/IXQT6iQHfcbt7P4LFNb3c0Q806VOvwqqz3nSHY4O1m5PvR+JFoQy5bnRXHQQpD7oiK3PjliJDFhgFjzzQxJPnBcVDIKJorZN3IAQ1ywpVT5tO/yp2TmGPirK9vOeHc7H9IB+4SS40B+Flh0x9XtoWZBuoYCeA9Xpssw6FV5qBKi""
					],
					""x5t"":""vOGDeRJyo_TU1zx0z_qKU0xs0tQ"",
					""x5t#S256"":""c8EaBo_rHUDM65XuBH71LEtozF8Gdx2NbwKJkJz45jg""
				}
			]}";

		private static bool ValidateToken(string token, TokenValidationParameters validationParameters)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			try
			{
				var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

				string scope = claimsPrincipal.Claims.Single(x => x.Type == "scope").Value;
				string name = claimsPrincipal.Claims.Single(x => x.Type == "name").Value;
				string preferredUserName = claimsPrincipal.Claims.Single(x => x.Type == "preferred_username").Value;
				string givenName = claimsPrincipal.Claims.Single(x => x.Type.Contains("givenname")).Value;
				string familyName = claimsPrincipal.Claims.Single(x => x.Type.Contains("surname")).Value;
				string email = claimsPrincipal.Claims.Single(x => x.Type.Contains("email")).Value;

				return validatedToken != null;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		[TestMethod]
		public void TestMethod1()
		{
			var jwks = new JsonWebKeySet(_jwksJson);
			var jwk = jwks.Keys.First();

			var validationParameters = new TokenValidationParameters
			{
				IssuerSigningKey = jwk,
				ValidateLifetime = false,
				ValidateAudience = false,
				ValidateIssuer = false,
				// ValidAudience = "", // Your API Audience, can be disabled via ValidateAudience = false,
				// ValidIssuer = ""  // Your token issuer, can be disabled via ValidateIssuer = false
			};

			var isValid = ValidateToken(_jwt, validationParameters);

			Assert.IsTrue(isValid);
		}
	}
}