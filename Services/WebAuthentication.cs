using System.Diagnostics;
using FamilyTreeCodecGeni;
//using FamilyTreeWebTools.Services;

namespace FamilyTreeWebTools.Services
{

  public class WebAuthentication
  {    
    static readonly TraceSource trace = new TraceSource("WebAuthentication", SourceLevels.Information);
    public WebAuthentication(string userId, string tClientId, string tClientSecret)
    {
      this.geniAuthentication = new GeniAppAuthenticationClass(AuthenticationUpdate, tClientId, tClientSecret);
      this.userId = userId;
      //geniAuthentication.SetUserId(userId);
    }
    public GeniAppAuthenticationClass geniAuthentication;
    public string userId;

    private void AuthenticationUpdate(string accessToken, string refreshToken, int expiresIn)
    {
      trace.TraceData(TraceEventType.Information, 0, "Updated geni authentication!");
      //FamilyDbContextClass.UpdateGeniAuthentication(userId, accessToken, refreshToken, expiresIn);
    }


    public string getWebRedirect()
    {
      return "https://www.geni.com/platform/oauth/authorize?client_id=" + geniAuthentication.GetClientId().ToString();
    }
  }
}