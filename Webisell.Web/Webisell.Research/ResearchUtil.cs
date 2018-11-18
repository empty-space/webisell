using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Webisell.Research
{
    class ResearchUtil
    {

        public class ResearchResult
        {
            public double AvgTimeMilliseconds { get; set; }
        }


        public static ResearchResult MeasureSearchTime(ITableRepository repository, int insertRowsCount = 100, int measuresCount = 3)
        {
            repository.DeleteAll();
            repository.InsertMany(insertRowsCount);
            var time = MeasureTime(() => repository.NextSearch(), measuresCount);
            return new ResearchResult { AvgTimeMilliseconds = time };
        }

        //ms
        public static double MeasureTime(Action action, int count)
        {
            double avg = 0;
            for (int i = 0; i < count; i++)
                avg += MeasureTime(action).TotalMilliseconds / count;

            return avg;
        }

        public static TimeSpan MeasureTime(Action action)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

    }
}
