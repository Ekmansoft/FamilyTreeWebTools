using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Ekmansoft.FamilyTree.WebTools.Services
{
  public class TimeCounterClass
  {
    private static TraceSource traceSource = new TraceSource("TimeCounter", SourceLevels.Information);
    class TimeSection
    {
      private int executionCounter;
      private int section;
      private TimeSpan elapsedTime;
      private TimeSpan minTime;
      private TimeSpan maxTime;
      private DateTime startTime;

      public TimeSection(int section)
      {
        this.section = section;
        executionCounter = 0;
        elapsedTime = TimeSpan.Zero;
        startTime = DateTime.MinValue;
        minTime = TimeSpan.MaxValue;
        maxTime = TimeSpan.MinValue;
      }

      public void Start(DateTime time)
      {
        if (startTime == DateTime.MinValue)
        {
          executionCounter++;
          startTime = time;
        }
        else
        {
          traceSource.TraceData(TraceEventType.Error, 0, "Section " + section + " already active!");
        }
      }
      private void UpdateTime(TimeSpan time)
      {
        elapsedTime += time;
        if (time < minTime)
        {
          minTime = time;
        }
        if (time > maxTime)
        {
          maxTime = time;
        }
      }
      public void Stop(DateTime time)
      {
        if (startTime != DateTime.MinValue)
        {
          UpdateTime(time - startTime);
          startTime = DateTime.MinValue;
        }
        else
        {
          traceSource.TraceData(TraceEventType.Error, 0, "Section " + section + " not active!");
        }
      }
      public override string ToString()
      {
        return "ix:" + section + " count:" + executionCounter + " time:" + FmtTime(elapsedTime) + " (" + FmtTime(minTime) + " - " + FmtTime(maxTime) + ")";
      }
      public void Trace()
      {
        if ((executionCounter > 0) && (TotalTime() > TimeSpan.FromSeconds(10.0)))
        {
          traceSource.TraceData(TraceEventType.Information, 0, ToString());
        }
      }
      public TimeSpan TotalTime()
      {
        return elapsedTime;
      }
    }

    private IList<TimeSection> sectionList;
    private int currentSection = -1;
    private DateTime lastPrint;
    private int MaxCount;
    private const int DefaultSections = 50;
    private string Name;

    public static string FmtTime(TimeSpan delta, int MaxFields = 2)
    {
      string result = "";
      int fieldCount = 0;

      if (delta.TotalDays >= 1)
      {
        result += Math.Truncate(delta.TotalDays) + "d ";
        delta = delta.Add(TimeSpan.FromDays(-Math.Truncate(delta.TotalDays)));
        fieldCount++;
      }
      if (delta.TotalHours >= 1)
      {
        result += Math.Truncate(delta.TotalHours) + "h ";
        delta = delta.Add(TimeSpan.FromHours(-Math.Truncate(delta.TotalHours)));
        fieldCount++;
      }
      if ((fieldCount < MaxFields) && (delta.TotalMinutes >= 1))
      {
        result += Math.Truncate(delta.TotalMinutes) + "m ";
        delta = delta.Add(TimeSpan.FromMinutes(-Math.Truncate(delta.TotalMinutes)));
        fieldCount++;
      }
      if ((fieldCount < MaxFields) && (delta.TotalSeconds >= 1))
      {
        result += Math.Truncate(delta.TotalSeconds) + "s ";
        delta = delta.Add(TimeSpan.FromSeconds(-Math.Truncate(delta.TotalSeconds)));
        fieldCount++;
      }
      if ((fieldCount < MaxFields) && (delta.TotalMilliseconds >= 1))
      {
        result += Math.Truncate(delta.TotalMilliseconds) + "ms ";
        fieldCount++;
      }
      result = result.Trim(' ');

      if (result.Length == 0)
      {
        result = "0ms";
      }
      return result;
    }

    public TimeCounterClass(int MaxCount = 0, string name = null)
    {
      sectionList = new List<TimeSection>();
      for (int i = 0; i < DefaultSections; i++)
      {
        sectionList.Add(new TimeSection(i));
      }
      currentSection = -1;
      lastPrint = DateTime.Now;
      this.MaxCount = MaxCount;
      this.Name = name;
    }
    public void Start(int index)
    {
      DateTime time = DateTime.Now;

      while (index >= sectionList.Count)
      {
        traceSource.TraceData(TraceEventType.Warning, 0, " increase sections " + index + " > + " + sectionList.Count);
        sectionList.Add(new TimeSection(sectionList.Count));
      }
      if (currentSection >= 0)
      {
        sectionList[currentSection].Stop(time);
      }
      sectionList[index].Start(time);
      currentSection = index;

      if ((DateTime.Now - lastPrint) >= TimeSpan.FromMinutes(5))
      {
        traceSource.TraceData(TraceEventType.Information, 0, ToString());
        lastPrint = DateTime.Now;
      }
    }
    public void Next()
    {
      DateTime time = DateTime.Now;
      if (currentSection >= 0)
      {
        sectionList[currentSection].Stop(time);
      }
      if (currentSection < 0)
      {
        currentSection = 0;
      }
      else
      {
        if (currentSection < sectionList.Count - 1)
        {
          currentSection++;
        }
        else
        {
          Stop(time);
          traceSource.TraceData(TraceEventType.Error, 0, "error section " + sectionList.Count + " reached");
          return;
        }
      }
      sectionList[currentSection].Start(time);
    }

    public TimeSpan TotalTime()
    {
      TimeSpan totalTime = TimeSpan.Zero;
      for (int i = 0; i < sectionList.Count; i++)
      {
        totalTime += sectionList[i].TotalTime();
      }
      return totalTime;
    }
    public void Stop(DateTime time)
    {
      if (currentSection >= 0)
      {
        sectionList[currentSection].Stop(time);
      }
      currentSection = -1;
    }
    public override string ToString()
    {
      //Stop(DateTime.Now);
      StringBuilder builder = new StringBuilder();

      builder.AppendLine("Count = " + sectionList.Count + " (" + MaxCount + ") time = " + FmtTime(TotalTime()));

      for (int i = 0; i < sectionList.Count; i++)
      {
        if (sectionList[i].TotalTime() > TimeSpan.FromSeconds(10.0))
        {
          builder.AppendLine(sectionList[i].ToString());
        }
      }
      return builder.ToString();
    }
    public void Trace()
    {
      //Stop(DateTime.Now);
      if (string.IsNullOrEmpty(Name))
      {
        traceSource.TraceData(TraceEventType.Information, 0, "TimeCount:" + Name);
      }
      traceSource.TraceData(TraceEventType.Information, 0, "Count = " + sectionList.Count + " (" + MaxCount + ") time = " + FmtTime(TotalTime()));
      for (int i = 0; i < sectionList.Count; i++)
      {
        sectionList[i].Trace();
      }
    }
  }
}
