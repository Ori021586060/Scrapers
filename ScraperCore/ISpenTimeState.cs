using System;

namespace ScraperCore
{
    public interface ISpenTimeState
    {
        DateTime DateStart { get; }
        TimeSpan SpentTime { get ; }
    }
}