using EasePass.Core.Database;
using EasePass.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EasePass.Tests.Core;

[TestClass]
public class SaveDatabaseSchedulingTests
{
    [TestMethod]
    public async Task TestDeferredSave()
    {
        var deferred = new DeferredSaveHelper(TimeSpan.FromMilliseconds(200));
        bool saved = false;

        var task = deferred.RequestSaveAsync(() =>
        {
            saved = true;
            return Task.FromResult(true);
        });

        Assert.IsTrue(deferred.SaveScheduled);

        await task;

        Assert.IsFalse(deferred.SaveScheduled);
        Assert.IsTrue(saved);
    }
}