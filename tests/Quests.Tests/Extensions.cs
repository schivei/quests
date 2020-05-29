using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Quests.Tests
{
    public static class Extensions
    {
        public static T RandomGet<T>(this IEnumerable<T> items)
        {
            var arr = items.ToArray();
            var random = new Random();
            int pos = random.Next(0, arr.Length);
            return arr[pos];
        }
        
        public static Task TimedOutAsync(this Task task, int timeoutMilliseconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeoutMilliseconds);

            return Task.Run(() => task, cts.Token);
        }

        public static Task<T> TimedOutAsync<T>(this Task<T> task, int timeoutMilliseconds)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeoutMilliseconds);

            return Task.Run(() => task, cts.Token);
        }
    }
}
