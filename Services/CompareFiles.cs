using FamilyTreeLibrary.FamilyData;
using FamilyTreeLibrary.FamilyTreeStore;
using FamilyTreeTools.CompareResults;
using FamilyTreeTools.FamilyTreeSanityCheck;
using FamilyTreeWebTools.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FamilyTreeWebTools.Compare
{
  public class FileCompare
  {
    private readonly static TraceSource trace = new TraceSource("FileCompare", SourceLevels.Information);
    private IFamilyTreeStoreBaseClass familyTree1, familyTree2;
    //private AncestorStatistics stats;
    //private SanityCheckLimits limits;
    private IProgressReporterInterface progressReporter;
    //private AsyncFamilyTreeWorkerClass analyseTreeWorker;
    IList<MatchingProfiles> matches;
    private int currentProgress;
    //private string email;
    //private string userId;

    Profile.SexType ConvertSex(IndividualClass.IndividualSexType sex)
    {
      switch (sex)
      {
        case IndividualClass.IndividualSexType.Male:
          return Profile.SexType.Male;
        case IndividualClass.IndividualSexType.Female:
          return Profile.SexType.Female;
        case IndividualClass.IndividualSexType.Unknown:
        default:
          return Profile.SexType.Unknown;
      }
    }
    public class Profile
    {
      public enum SexType
      {
        Unknown,
        Male,
        Female
      }

      public string Xref { get; set; }
      public string Name { get; set; }
      public string Url { get; set; }

      public SexType Sex { get; set; }
      public string Birth { get; set; }
      public string Death { get; set; }
    }

    public class MatchingProfiles
    {
      public Profile profile1;
      public Profile profile2;
      public string CompareUrl;
    }

    public FileCompare()
    {
      //email = userEmail;
      //this.userId = userId;
    }

    private Profile DecodeProfile(IndividualClass person)
    {
      Profile profile = new Profile();
      profile.Name = person.GetName();
      profile.Xref = person.GetXrefName();
      profile.Birth = AncestorStatistics.GetEventDateString(person, IndividualEventClass.EventType.Birth);
      profile.Death = AncestorStatistics.GetEventDateString(person, IndividualEventClass.EventType.Death);
      profile.Sex = ConvertSex(person.GetSex());

      IList<string> urlList = person.GetUrlList();
      if (urlList.Count > 0)
      {
        profile.Url = urlList[0];
        //AddToList(person1, thisRelationStack, thisGenerations, SanityCheckLimits.SanityProblemId.duplicateCheck_e, builder.ToString(), url);
      }
      return profile;
    }
    private void ReportMatchingProfiles(IFamilyTreeStoreBaseClass familyTree1, string person1, IFamilyTreeStoreBaseClass familyTree2, string person2)
    {
      IndividualClass person1full = familyTree1.GetIndividual(person1);
      IndividualClass person2full = familyTree2.GetIndividual(person2);

      if ((person1full != null) && (person2full != null))
      {
        StringBuilder builder = new StringBuilder();
        builder.Append("Possible duplicate profile: ");

        MatchingProfiles match = new MatchingProfiles();
        match.profile1 = DecodeProfile(person1full);
        match.profile2 = DecodeProfile(person2full);
        int ix1 = match.profile1.Url.LastIndexOf('/');
        string id1 = "";


        if (ix1 >= 0)
        {
          id1 = match.profile1.Url.Substring(ix1 + 1);
        }
        int ix2 = match.profile2.Url.LastIndexOf('/');
        string id2 = "";
        if (ix2 >= 0)
        {
          id2 = match.profile2.Url.Substring(ix2 + 1);
        }

        if (id1.Length > 0 && id2.Length > 0)
        {
          match.CompareUrl = "https://www.geni.com/merge/compare/" + id1 + "?return=match%3B&to=" + id2;
          trace.TraceEvent(TraceEventType.Verbose, 0, "url " + match.CompareUrl);
          match.profile2.Url = match.CompareUrl;
        }
        else
        {
          trace.TraceEvent(TraceEventType.Verbose, 0, "comp-url " + match.profile1.Url + " " + ix1 + " " + id1 + " " + match.profile2.Url + " " + ix2 + " " + id2);
        }


        // https://www.geni.com/merge/compare/6000000086954097870?return=duplicates__6000000086954006034_6000000086954006034_6000000086954006034_2&to=6000000077556402263


        matches.Add(match);
        //trace.TraceData(TraceEventType.Warning, 0, builder.ToString());
      }
      else
      {
        if (person1full == null)
        {
          trace.TraceData(TraceEventType.Warning, 0, "Error person1 is null:" + person1);
        }
        if (person2full == null)
        {
          trace.TraceData(TraceEventType.Warning, 0, "Error person2 is null:" + person2);
        }
      }
    }


    public IList<MatchingProfiles> CompareFiles(FamilyWebTree webTree1, FamilyWebTree webTree2)
    {
      familyTree1 = webTree1.GetFamilyTree();
      familyTree2 = webTree2.GetFamilyTree();
      trace.TraceData(TraceEventType.Information, 0, "Compare job " + " started");
      matches = new List<MatchingProfiles>();

      progressReporter = new AsyncWorkerProgress(1, CompletenessProgress, StopRequestHandler);
      CompareTreeClass.CompareTrees(familyTree1, familyTree2, ReportMatchingProfiles, progressReporter);

      trace.TraceData(TraceEventType.Warning, 0, "Compare job done, found " + matches.Count + " matches");
      return matches;
    }

    private void CompletenessProgress(int jobId, int progressPercent, string text = null)
    {
      if (progressPercent >= currentProgress + 10)
      {
        trace.TraceData(TraceEventType.Information, 0, " Job " + jobId + " progress " + progressPercent + "%");
        currentProgress = progressPercent;
      }
      ProgressDbClass.Instance.UpdateProgress(jobId, progressPercent);

      if (progressPercent < 0)
      {
        //TODO: How can we extract updated geni tokens from genicodec to database after a long run...?
        trace.TraceData(TraceEventType.Information, 0, " Job " + jobId + " progress " + progressPercent + "% (finished)");
      }
      if (progressPercent >= 100)
      {
        trace.TraceData(TraceEventType.Information, 0, " Job " + jobId + " progress " + progressPercent + "% (finished)");
      }
    }

    private bool StopRequestHandler(int id)
    {
      return ProgressDbClass.Instance.StopRequested(id);
    }
  }

}
