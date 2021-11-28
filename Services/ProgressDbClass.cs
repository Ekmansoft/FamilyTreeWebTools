using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Ekmansoft.FamilyTree.WebTools.Services
{
  public sealed class ProgressDbClass
  {
    private static readonly TraceSource trace = new TraceSource("ProgressDbClass", SourceLevels.Information);
    private static readonly Lazy<ProgressDbClass> lazy =
        new Lazy<ProgressDbClass>(() => new ProgressDbClass());

    private class JobInformation
    {
      public int jobId;
      public int progress;
      public Thread threadId;
      public bool stopRequested;

      public JobInformation(int jobId, int progress = 0)
      {
        this.jobId = jobId;
        this.threadId = Thread.CurrentThread;
        this.progress = progress;
        stopRequested = false;
      }
    }

    static private IDictionary<int, JobInformation> progressDb;

    public static ProgressDbClass Instance { get { return lazy.Value; } }

    public ProgressDbClass()
    {
      progressDb = new Dictionary<int, JobInformation>();
    }

    public int GetNoOfJobs()
    {
      return progressDb.Count;
    }

    public bool StopRequested(int jobId)
    {
      if (progressDb.ContainsKey(jobId))
      {
        JobInformation jobInfo = progressDb[jobId];
        return jobInfo.stopRequested;
      }
      return false;
    }

    public void RequestStop(int jobId)
    {
      if (progressDb.ContainsKey(jobId))
      {
        JobInformation jobInfo = progressDb[jobId];
        jobInfo.stopRequested = true;
        progressDb[jobId] = jobInfo;
      }
    }

    public Thread GetThread(int jobId)
    {
      if (progressDb.ContainsKey(jobId))
      {
        JobInformation jobInfo = progressDb[jobId];
        return jobInfo.threadId;
      }
      return null;
    }

    public void UpdateProgress(int jobId, int progress)
    {
      if (progressDb.ContainsKey(jobId))
      {
        if (progress < 0)
        {
          progressDb.Remove(jobId);
          trace.TraceData(TraceEventType.Information, 0, "Removed job " + jobId);
        }
        else
        {
          JobInformation jobInfo = progressDb[jobId];
          jobInfo.progress = progress;
          progressDb[jobId] = jobInfo;
        }
      }
      else
      {
        if ((progress >= 0) && (progress <= 100))
        {
          JobInformation job = new JobInformation(jobId, progress);
          progressDb.Add(jobId, job);
          trace.TraceData(TraceEventType.Information, 0, "Added job " + jobId + " " + progress + " %, thread:" + job.threadId.ManagedThreadId);
        }
      }
    }

    public int GetProgress(int JobId)
    {
      if (progressDb.ContainsKey(JobId))
      {
        return progressDb[JobId].progress;
      }
      return -1;
    }
  }
}
