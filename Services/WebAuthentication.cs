using System;
using System.Diagnostics;
using System.Web;
using FamilyTreeCodecGeni;
//using FamilyTreeWebTools.Services;

namespace FamilyTreeWebTools.Services
{
  public delegate void AuthenticationUpdateCallback(string userId, string accessToken, string refreshToken, int expiresIn);

  public class WebAuthentication
  {    
    static readonly TraceSource trace = new TraceSource("WebAuthentication", SourceLevels.Information);
    public WebAuthentication(string userId, string tClientId, string tClientSecret, AuthenticationUpdateCallback callback)
    {
      trace.TraceData(TraceEventType.Information, 0, "WebAuthentication " + userId + " " + tClientId);
      this.geniAuthentication = new GeniAppAuthenticationClass(AuthenticationUpdate, tClientId, tClientSecret);
      this.userId = userId;
      _callback = callback;
      //geniAuthentication.SetUserId(userId);
    }
    private GeniAppAuthenticationClass geniAuthentication;
    private string userId;
    private AuthenticationUpdateCallback _callback;

    public void UpdateAuthenticationData(string accessToken, string refreshToken, int expiresIn, DateTime authenticationTime, bool saveToDb = false)
    {
      trace.TraceData(TraceEventType.Information, 0, "UpdateAuthenticationData " + userId);
      geniAuthentication.UpdateAuthenticationData(accessToken, refreshToken, expiresIn, authenticationTime, saveToDb);
    }
    public GeniAppAuthenticationClass getGeniAuthentication()
    {
      return geniAuthentication;
    }

    private void AuthenticationUpdate(string accessToken, string refreshToken, int expiresIn)
    {
      trace.TraceData(TraceEventType.Information, 0, "AuthenticationUpdate " + userId);
      //FamilyDbContextClass.UpdateGeniAuthentication(userId, accessToken, refreshToken, expiresIn);
      _callback(userId, accessToken, refreshToken, expiresIn);
    }

    public string getRequestTokenUrl(string code, string redirectUrl)
    {
      return "https://www.geni.com/platform/oauth/request_token?client_id=" + geniAuthentication.GetClientId().ToString() +
        "&redirect_uri=" + HttpUtility.UrlEncode(redirectUrl) +
        "&client_secret=" + geniAuthentication.GetClientSecret() +
        "&code=" + code;
    }

    public string getAuthorizeUrl()
    {
      return "https://www.geni.com/platform/oauth/authorize?client_id=" + geniAuthentication.GetClientId().ToString();
    }
  }
}