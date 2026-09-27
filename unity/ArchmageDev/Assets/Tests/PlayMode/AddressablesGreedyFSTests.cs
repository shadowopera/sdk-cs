// AssetDatabaseProvider only exists in the editor.
#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shadop.Archmage.Sdk;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

// Checks which path UnityAddressablesGreedyFS takes: with "Use Existing Build" it reads every file in the
// bundle into memory on the first read, so a later read of another file in the bundle completes without
// waiting for a frame;
// with "Use Asset Database" it reads each file through UnityAddressablesFS.
public class AddressablesGreedyFSTests
{
    [Test]
    public async Task ReadsFromBundle()
    {
        var reference = new UnityAddressablesFS();
        var hero = await reference.ReadAllBytesAsync("Assets/Configs/hero.json");
        var item = await reference.ReadAllBytesAsync("Assets/Configs/item.json");

        // "Use Asset Database" registers AssetDatabaseProvider; "Use Existing Build" loads bundles instead.
        var packed = !Addressables.ResourceManager.ResourceProviders.OfType<AssetDatabaseProvider>().Any();

        if (packed)
        {
            // Validates the signal below: an asynchronous load from a bundle takes at least one frame.
            var single = reference.ReadAllBytesAsync("Assets/Configs/race.json");
            Assert.IsFalse(single.IsCompleted, "UnityAddressablesFS completed a bundle read synchronously");
            await single;
        }

        var fs = new UnityAddressablesGreedyFS();
        CollectionAssert.AreEqual(hero, await fs.ReadAllBytesAsync("Assets/Configs/hero.json"));

        var second = fs.ReadAllBytesAsync("Assets/Configs/item.json");
        if (packed)
            Assert.IsTrue(second.IsCompleted, "The second read did not come from the cache");
        CollectionAssert.AreEqual(item, await second);

        // A file already served is read again on its own.
        CollectionAssert.AreEqual(hero, await fs.ReadAllBytesAsync("Assets/Configs/hero.json"));

        // Assert.ThrowsAsync blocks the main thread, so await instead.
        try
        {
            await fs.ReadAllBytesAsync("Assets/Configs/missing.json");
            Assert.Fail("Expected FileNotFoundException");
        }
        catch (FileNotFoundException)
        {
        }
    }
}

#endif
