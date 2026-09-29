using System.Threading.Tasks;
using Shadop.Archmage.Sdk;
using Environment = System.Environment;

namespace ArchmageDev.Tests
{
    // Merges the en and fr l10n files from res://configs.
    static class I18nTests
    {
        [GodotTest]
        public static Task MergeL10nFile()
        {
            var i18n = new I18n("en");
            i18n.MergeL10nFile($"{LoadTests.CfgRoot}/l10n.json", "en", new GodotFileAccessFS());
            i18n.MergeL10nFile($"{LoadTests.CfgRoot}/l10n.fr.json", "fr", new GodotFileAccessFS());

            AssertTexts(i18n);
            return Task.CompletedTask;
        }

        [GodotTest]
        public static async Task MergeL10nFileAsync()
        {
            var i18n = new I18n("en");
            await i18n.MergeL10nFileAsync($"{LoadTests.CfgRoot}/l10n.json", "en", new GodotFileAccessFS());
            LoadTests.AssertMainThread(Environment.CurrentManagedThreadId, "The code after the first await");
            await i18n.MergeL10nFileAsync($"{LoadTests.CfgRoot}/l10n.fr.json", "fr", new GodotFileAccessFS());
            LoadTests.AssertMainThread(Environment.CurrentManagedThreadId, "The code after the second await");

            AssertTexts(i18n);
        }

        static void AssertTexts(I18n i18n)
        {
            // Translated into fr.
            Assert.AreEqual("Légendes d’Avalon", i18n.Text("l10n.xlsx[game.title]", "fr"));
            // Not translated: falls back to en.
            Assert.AreEqual("Welcome back!", i18n.Text("game.tips.login", "fr"));
        }
    }
}
