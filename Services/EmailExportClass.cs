//using FamilyStudioData.Controllers;
//using FamilyStudioData.FamilyData;
using Ekmansoft.FamilyTree.Tools.FamilyTreeSanityCheck;
using Ekmansoft.FamilyTree.WebTools.Compare;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ekmansoft.FamilyTree.WebTools.Email
{
  public class EmailExportClass
  {
    public static String GetLinefeed()
    {
      return "\r\n";
    }

    static public string ExportHtml(JobInfo jobInfo)
    {
      StringBuilder builder = new StringBuilder();
      //ancestorList.OrderBy<int, depth>();
      builder.Append("<!DOCTYPE html>" + GetLinefeed() +
        "<html lang=\"en\"><head><meta charset=\"UTF-8\"/>" + GetLinefeed() +
        "<title> List of profile problems </title></head>" + GetLinefeed() +
        "<body>" + GetLinefeed());

      builder.Append("Analysis started at " + jobInfo.StartTime.ToString("yyyy-MM-dd HH:mm") + " done after " + (jobInfo.EndTime - jobInfo.StartTime).ToString() + "<br/>" + GetLinefeed());
      builder.Append("Ancestor overview:" + GetLinefeed());
      builder.Append("  analysed " + jobInfo.Profiles + " people and " + jobInfo.Families + " families " + "<br/>" + GetLinefeed());
      builder.Append("  found " + jobInfo.IssueList.Count + " issues.<br/>" + GetLinefeed());

      builder.Append("<table>" + GetLinefeed() +
        "<tr><th>Name</th><th>Birth</th><th>Death</th><th>Comment</th><th>Dup links</th></tr>" + GetLinefeed());
      //trace.TraceInformation("  max children: " + maxNoOfChildren+ " parents " + maxNoOfParents);
      //familyTree.Print();

      IEnumerable<AncestorLineInfo> query = jobInfo.IssueList.OrderBy(ancestor => ancestor.depth);

      foreach (AncestorLineInfo root in query)
      {
        builder.Append("<tr><td>");

        if (root.url.Length > 0)
        {
          builder.Append("<a href=\"" + root.url + "\">" + root.name + "</a>");
        }
        else
        {
          builder.Append(root.name);
        }
        builder.Append("</td><td>" + root.birth + "</td><td>"
        + root.death + "</td><td>" + root.GetDetailString() + "</td><td>");

        int dupIx = 1;
        foreach (string url in root.duplicate)
        {
          builder.Append("<a href=\"" + url + "\">Dup " + dupIx.ToString() + "</a>" + GetLinefeed());
          dupIx++;
        }

        builder.Append("</td></tr>\n" + GetLinefeed());
      }
      builder.Append("</table></body></html>" + GetLinefeed());
      return builder.ToString();
    }

    static public string ExportDuplicatesHtml(IList<FileCompare.MatchingProfiles> matches, DateTime startTime, DateTime endTime, string tree1, string tree2)
    {
      StringBuilder builder = new StringBuilder();
      //ancestorList.OrderBy<int, depth>();
      builder.Append("<!DOCTYPE html>" + GetLinefeed() +
        "<html lang=\"en\"><head><meta charset=\"UTF-8\"/>" + GetLinefeed() +
        "<title> List of matching profiles </title></head>" + GetLinefeed() +
        "<body>" + GetLinefeed());

      builder.Append("Analysis started at " + startTime.ToString("yyyy-MM-dd HH:mm") + " done after " + (endTime - startTime).ToString() + "<br/>" + GetLinefeed());
      builder.Append("Ancestor overview:" + GetLinefeed());
      builder.Append("  compared " + tree1 + " with " + tree2 + " " + "<br/>" + GetLinefeed());
      builder.Append("  found " + matches.Count + " matches.<br/>" + GetLinefeed());

      builder.Append("<table>" + GetLinefeed() +
        "<tr><th>Name</th><th>Birth</th><th>Death</th><th>Name</th><th>Birth</th><th>Death</th></tr>" + GetLinefeed());
      //trace.TraceInformation("  max children: " + maxNoOfChildren+ " parents " + maxNoOfParents);
      //familyTree.Print();

      foreach (FileCompare.MatchingProfiles match in matches)
      {

        // compare url  https://www.geni.com/merge/compare/6000000083384695337?return=match%3B&to=6000000075044146008
        // https://www.geni.com/merge/compare/6000000086954097870?return=duplicates__6000000086954006034_6000000086954006034_6000000086954006034_2&to=6000000077556402263

        if (match.profile1 != null)
        {
          builder.Append("<tr><td>");

          if (!string.IsNullOrEmpty(match.profile1.Url))
          {
            builder.Append("<a href=\"" + match.profile1.Url + "\">" + match.profile1.Name + "</a>");
          }
          else
          {
            builder.Append(match.profile1.Name);
          }
          builder.Append("</td><td>" + match.profile1.Birth + "</td><td>" + match.profile1.Death + "</td><td>");

          if (!string.IsNullOrEmpty(match.profile2.Url))
          {
            builder.Append("<a href=\"" + match.profile2.Url + "\">" + match.profile2.Name + "</a>");
          }
          else
          {
            builder.Append(match.profile2.Name);
          }
          builder.Append("</td><td>" + match.profile2.Birth + "</td><td>" + match.profile2.Death + "</td><td>");

          builder.Append("</td></tr>\n" + GetLinefeed());
        }
      }
      builder.Append("</table></body></html>" + GetLinefeed());
      return builder.ToString();
    }

  }
}
