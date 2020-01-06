using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FamilyTreeWebTools.Data
{

  public class IssueCounter
  {
    public IDictionary<Issue.IssueType, int> issueCounter;

    public IssueCounter()
    {
      issueCounter = new Dictionary<Issue.IssueType, int>();
    }

    public void AddIssue(Issue.IssueType type)
    {
      if (!issueCounter.ContainsKey(type))
      {
        issueCounter.Add(type, 1);
      }
      else
      {
        issueCounter[type] = issueCounter[type] + 1;
      }
    }

    public static string ToJson(IssueCounter o)
    {
      return JsonSerializer.Serialize(o);
    }
  }

  public class AnalysisSettings
  {
    [Required]
    [DefaultValue("")]
    [Display(Name = "Start person")]
    public string StartPersonName { get; set; }
    [Required]
    [DefaultValue("")]
    [Display(Name = "Start person xref")]
    public string StartPersonXref { get; set; }
    [Required]
    [DefaultValue(5)]
    [Display(Name = "Generations back")]
    public int GenerationsBack { get; set; }
    [Required]
    [DefaultValue(2)]
    [Display(Name = "Generations forward")]
    public int GenerationsForward { get; set; }
    [Required]
    [DefaultValue(1900)]
    [Display(Name = "End year (most recent birth for distant cousins)")]
    public int EndYear { get; set; }
    [Required]
    [DefaultValue(true)]
    [Display(Name = "Check duplicates")]
    public bool DuplicateCheck { get; set; }
    [Required]
    [DefaultValue(true)]
    [Display(Name = "Send email")]
    public bool SendEmail { get; set; }
    [Required]
    [DefaultValue(false)]
    [Display(Name = "Export gedcom")]
    public bool ExportGedcom { get; set; }
    [Required]
    [DefaultValue(false)]
    [Display(Name = "Export Json")]
    public bool ExportJson { get; set; }
    [DefaultValue(false)]
    [Display(Name = "Export Kml (Map)")]
    public bool ExportKml{ get; set; }
    [Required]
    [DefaultValue(false)]
    [Display(Name = "Check whole file")]
    public bool CheckWholeFile { get; set; }
    [Required]
    [DefaultValue(false)]
    [Display(Name = "Update database")]
    public bool UpdateDatabase { get; set; }

    public static AnalysisSettings FromJson(string json)
    {
      return JsonSerializer.Deserialize<AnalysisSettings>(json);
    }

    public static string ToJson(AnalysisSettings o)
    {
      return JsonSerializer.Serialize(o);
    }
  }

  public class DatabaseResults
  {
    public int NoOfProfilesAdded { get; set; }
    public int NoOfIssuesUpdated { get; set; }
    public int NoOfIssuesAdded { get; set; }
    public int NoOfIssuesCorrected { get; set; }
  }


  public class AnalysisResults
  {
    [Required]
    [DefaultValue(0)]
    [Display(Name = "No of searched profiles")]
    public int SearchedProfiles { get; set; }
    [Required]
    [DefaultValue(0)]
    [Display(Name = "No of searched families")]
    public int SearchedFamilies { get; set; }
    [Required]
    [DefaultValue(0)]
    [Display(Name = "No of profiles with issues")]
    public int NoOfProfiles { get; set; }
    [DefaultValue(0)]
    [Display(Name = "No of issues")]
    public int NoOfIssues { get; set; }
    public DatabaseResults DbResults { get; set; }
    [DefaultValue(null)]
    [Display(Name = "Export gedcom filename")]
    public string ExportedGedcomName { get; set; }
    public IssueCounter IssueCounters { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int JobId { get; set; }

    public AnalysisResults()
    {
      IssueCounters = new IssueCounter();
    }

    public static AnalysisResults FromJson(string json)
    {
      return JsonSerializer.Deserialize<AnalysisResults>(json);
    }

    public static string ToJson(AnalysisResults o)
    {
      return JsonSerializer.Serialize(o);
    }
  }


  public class UserInformation
  {
    [Key]
    public string UserId { get; set; }
    [Required]
    public DateTime GeniAuthenticationTime { get; set; }
    [Required]
    public string GeniAccessToken { get; set; }
    [Required]
    public string GeniRefreshToken { get; set; }
    [Required]
    public int GeniExpiresIn { get; set; }
  }

  public class WebAppIdentity
  {
    public string AppId { get; set; }
    public string AppSecret { get; set; }
  }

  public class EmailSendSource
  {
    public string Address { get; set; }
    public string CredentialAddress { get; set; }
    public string CredentialPassword { get; set; }
  }


  public class AppSetting
  {
    [Key]
    public int Id { get; set; }
    [DefaultValue(3)]
    public int MaxSimultaneousJobs { get; set; }
  }

  public class Analysis
  {
    [Key]
    public int Id { get; set; }
    public int ParentId { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public string UserEmail { get; set; }
    [Required]
    public string FileName { get; set; }
    [Required]
    [Display(Name = "File name")]
    public string OriginalFilename { get; set; }
    [Required]
    public int FamilyTreeId { get; set; }
    [Required]
    [Display(Name = "Start person")]
    public string StartPersonName { get; set; }
    [Required]
    public string StartPersonXref  { get; set; }
    [Required]
    [Display(Name = "Start time")]
    public DateTime StartTime { get; set; }
    [Required]
    [Display(Name = "End time")]
    public DateTime EndTime { get; set; }
    [Required]
    public string Settings { get; set; }
    [Required]
    public string Results { get; set; }
    [Required]
    [DefaultValue(0)]
    public int StartCount { get; set; }

    [Required]
    public ICollection<IssueLink> IssueLinks { get; set; }

    public AnalysisResults DecodeResults()
    {
      return AnalysisResults.FromJson(Results);
    }
    public AnalysisSettings DecodeSettings()
    {
      return AnalysisSettings.FromJson(Settings);
    }
  }

  public class FamilyTree
  {
    public enum TreeType
    {
      GedcomFile,
      GeniDotCom
    }
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Filename { get; set; }
    public string Url { get; set; }
    [Required]
    [EnumDataType(typeof(TreeType))]
    public int Type { get; set; }
  }

  public class Profile
  {
    public enum SexType
    {
      Unknown,
      Male,
      Female
    }
    [Key]
    public int Id { get; set; }
    //public int AnalysisId { get; set; }
    public string TreeId { get; set; }
    public int FamilyTreeId { get; set; }
    public string Name { get; set; }

    [EnumDataType(typeof(SexType))]
    public SexType Sex { get; set; }
    public string Birth { get; set; }
    public string Death { get; set; }
    public string Url { get; set; }

    //public ICollection<Problem> Problems { get; set; }

    public ICollection<Issue> Issues { get; set; }
  }

  public class Issue
  {
    public enum IssueType
    {
      ParentLimitMin,
      MotherLimitMax,
      FatherLimitMax,
      EventLimitMin,
      EventLimitMax,
      NoOfChildrenMin,
      NoOfChildrenMax,
      DaysBetweenChildren,
      Twins,
      InexactBirthDeath,
      UnknownBirthDeath,
      ParentsMissing,
      ParentsProblem,
      MarriageProblem,
      MissingWeddingDate,
      MissingPartner,
      GenerationLimited,
      DuplicateCheck,
      UnknownSex,
      OldPrivateProfile,
      ShortAddress,
      UnknownLocation,
      MissingPartnerMitigated,
    }

    [Flags]
    public enum IssueStatus
    {
      None = 0x00,
      Identified = 0x01,
      Verified = 0x02,
      Discarded = 0x04,
      Ignored = 0x08,
      Corrected = 0x10
    }
    [Key]
    public int Id { get; set; }
    [Required]
    public int ProfileId { get; set; }
    [Required]
    [EnumDataType(typeof(IssueType))]
    public IssueType Type { get; set; }
    [Required]
    [EnumDataType(typeof(IssueStatus))]
    public IssueStatus Status { get; set; }
    [Required]
    public string Description { get; set; }

    public string Parameters { get; set; }

    public ICollection<IssueLink> IssueLinks { get; set; }
  }

  public class IssueLink
  {
    [Key]
    public int Id { get; set; }
    [Required]
    public DateTime Time { get; set; }
    [Required]
    [EnumDataType(typeof(Issue.IssueStatus))]
    public Issue.IssueStatus Status { get; set; }
    [Required]
    public int AnalysisId { get; set; }
    [Required]
    public int IssueId { get; set; }
    public string RelationDistance { get; set; }
    public string Relation { get; set; }
  }
}