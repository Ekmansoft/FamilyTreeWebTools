using Ekmansoft.FamilyTree.Codec.Gedcom;
using Ekmansoft.FamilyTree.Codec.Geni;
using Ekmansoft.FamilyTree.Library.FamilyData;
//using Ekmansoft.FamilyTree.Library.FamilyFileFormat;
using Ekmansoft.FamilyTree.Library.FamilyTreeStore;
using Ekmansoft.FamilyTree.Tools.FamilyTreeSanityCheck;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using Ekmansoft.FamilyTree.WebTools.Services;

namespace Ekmansoft.FamilyTree.WebTools.Services
{

  public class FamilyWebTree
  {
    static readonly TraceSource trace = new TraceSource("FamilyWebTree", SourceLevels.Information);
    private string filepath;
    //private string accessToken;
    //private string refreshToken;
    //private string expiresIn;
    private string startPersonXref;
    private IFamilyTreeStoreBaseClass familyTree;

    public class SimplePerson
    {
      public string name { get; set; }
      public string xrefName { get; set; }
      public string lifespan { get; set; }
      public SimplePerson(string name, string xref, string lifespan)
      {
        this.name = name;
        this.xrefName = xref;
        this.lifespan = lifespan;
      }
    }

    public void LoadCompletedCallback(Boolean result)
    {
      trace.TraceData(TraceEventType.Information, 0, "File loaded result = " + result);
    }

    public void Dispose()
    {
      if (familyTree != null)
      {
        familyTree.Dispose();
      }
    }

    public FamilyWebTree(string filepath, WebAuthentication authenticationClass)
    {
      this.filepath = filepath;
      //this.expiresIn = expiresIn;
      //this.accessToken = accessToken;
      //this.refreshToken = refreshToken;
      IndividualClass rootPerson = null;

      familyTree = null;
      if ((filepath != null) && (filepath.Length > 0) && (filepath != "Geni.com"))
      {
        familyTree = GetGedcomTree(filepath, LoadCompletedCallback);
      }
      else if (authenticationClass != null)
      {
        familyTree = GetWebTree(authenticationClass, LoadCompletedCallback);
      }
      else
      {
        trace.TraceData(TraceEventType.Error, 0, "FamilyWebTree no valid tree parameters ");
      }
      if (familyTree != null)
      {
        rootPerson = familyTree.GetIndividual();
      }
      else
      {
        trace.TraceData(TraceEventType.Error, 0, "FamilyWebTree no valid tree  " + filepath);
        return;
      }

      if (rootPerson != null)
      {
        startPersonXref = rootPerson.GetXrefName();
      }
      else
      {
        trace.TraceData(TraceEventType.Error, 0, "FamilyWebTree no root person found");
        startPersonXref = null;
      }
    }

    public bool IsValid()
    {
      return startPersonXref != null;
    }

    public IFamilyTreeStoreBaseClass GetGedcomTree(string filepath, CompletedCallback CompletedCallback)
    {
      FamilyFileTypeCollection codec = new FamilyFileTypeCollection();

      familyTree = codec.CreateFamilyTreeStore("", CompletedCallback);

      codec.OpenFile(filepath, ref familyTree, CompletedCallback);

      return familyTree;
    }

    public static void ExportGedcom(IFamilyTreeStoreBaseClass familyTree, string filename)
    {
      GedcomEncoder encoder = new GedcomEncoder();

      encoder.StoreFile(familyTree, filename, FamilyFileTypeOperation.Export);
    }


    public string GetPersonName(string xrefName)
    {
      if (familyTree != null)
      {
        IndividualClass startPerson = familyTree.GetIndividual(xrefName);

        if (startPerson == null)
        {
          return null;
        }

        return startPerson.GetName().ToString();
      }
      return "";
    }

    public IEnumerator<SimplePerson> SearchPerson(string searchString)
    {
      if (familyTree != null)
      {
        IEnumerator<IndividualClass> personIterator = familyTree.SearchPerson(searchString);

        while (personIterator.MoveNext())
        {
          IndividualClass person = personIterator.Current;
          string lifespan = AncestorStatistics.GetEventDateString(person, IndividualEventClass.EventType.Birth) + " - " +
            AncestorStatistics.GetEventDateString(person, IndividualEventClass.EventType.Death);
          SimplePerson resultPerson = new SimplePerson(person.GetName().ToString(), person.GetXrefName(), lifespan);

          yield return resultPerson;
        }
      }
    }

    public string GetCurrentStartPerson()
    {
      return startPersonXref;
    }

    public void SetStartPerson(string xrefName)
    {
      startPersonXref = xrefName;
    }


    public IFamilyTreeStoreBaseClass GetWebTree(WebAuthentication authenticationClass, CompletedCallback CompletedCallback)
    {
      trace.TraceData(TraceEventType.Information, 0, "GetWebTree");

      FamilyTreeStoreGeni2 webTree = new FamilyTreeStoreGeni2(CompletedCallback, authenticationClass.getGeniAuthentication());

      trace.TraceData(TraceEventType.Information, 0, "FamilyWebTree::FamilyTreeStoreGeni2( )");

      webTree.SetFile("geni.com");

      //webTree.SetAuthentication(accessToken, refreshToken, expiresIn);

      return (IFamilyTreeStoreBaseClass)webTree;
    }

    public IFamilyTreeStoreBaseClass GetFamilyTree()
    {
      return familyTree;
    }
  }
}