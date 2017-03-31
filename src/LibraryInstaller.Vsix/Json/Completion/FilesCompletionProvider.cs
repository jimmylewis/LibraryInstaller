﻿using LibraryInstaller.Contracts;
using Microsoft.JSON.Core.Parser.TreeItems;
using Microsoft.JSON.Editor.Completion;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace LibraryInstaller.Vsix
{
    [Export(typeof(IJSONCompletionListProvider))]
    [Name(nameof(FilesCompletionProvider))]
    class FilesCompletionProvider : BaseCompletionProvider
    {
        public override JSONCompletionContextType ContextType
        {
            get { return JSONCompletionContextType.ArrayElement; }
        }

        protected override IEnumerable<JSONCompletionEntry> GetEntries(JSONCompletionContext context)
        {
            var member = context.ContextItem?.Parent?.Parent as JSONMember;

            if (member == null || member.UnquotedNameText != "files")
                yield break;

            var parent = member.Parent as JSONObject;

            if (!TryGetProviderId(parent, out string providerId, out string libraryId))
                yield break;

            if (string.IsNullOrEmpty(libraryId))
                yield break;

            var dependencies = Dependencies.FromConfigFile(ConfigFilePath);
            IProvider provider = dependencies.GetProvider(providerId);
            ILibraryCatalog catalog = provider?.GetCatalog();

            if (catalog == null)
                yield break;

            Task<ILibrary> task = catalog.GetLibraryAsync(libraryId, CancellationToken.None);
            FrameworkElement presenter = GetPresenter(context);
            IEnumerable<string> usedFiles = GetUsedFiles(context);

            if (task.IsCompleted)
            {
                if (!(task.Result is ILibrary library))
                    yield break;

                foreach (string file in library.Files.Keys)
                {
                    if (!usedFiles.Contains(file))
                    {
                        ImageSource glyph = WpfUtil.GetIconForFile(presenter, file, out bool isThemeIcon);
                        yield return new SimpleCompletionEntry(file, glyph, context.Session);
                    }
                }
            }
            else
            {
                yield return new SimpleCompletionEntry(Resources.Text.Loading, KnownMonikers.Loading, context.Session);

                task.ContinueWith((a) =>
                {
                    if (!(task.Result is ILibrary library))
                        return;

                    if (!context.Session.IsDismissed)
                    {
                        var results = new List<JSONCompletionEntry>();

                        foreach (string file in library.Files.Keys)
                        {
                            if (!usedFiles.Contains(file))
                            {
                                ImageSource glyph = WpfUtil.GetIconForFile(presenter, file, out bool isThemeIcon);
                                results.Add(new SimpleCompletionEntry(file, glyph, context.Session));
                            }
                        }

                        UpdateListEntriesSync(context, results);
                    }
                });
            }

            Telemetry.TrackUserTask("completionfiles");
        }

        private static IEnumerable<string> GetUsedFiles(JSONCompletionContext context)
        {
            JSONArray array = context.ContextItem.FindType<JSONArray>();

            if (array == null)
                yield break;

            foreach (JSONArrayElement arrayElement in array.Elements)
            {
                if (arrayElement.Value is JSONTokenItem token)
                {
                    yield return token.CanonicalizedText;
                }
            }
        }

        private FrameworkElement GetPresenter(JSONCompletionContext context)
        {
            var presenter = context?.Session?.Presenter as FrameworkElement;

            if (presenter != null)
            {
                presenter.SetBinding(ImageThemingUtilities.ImageBackgroundColorProperty, new Binding("Background")
                {
                    Source = presenter,
                    Converter = new BrushToColorConverter()
                });
            }

            return presenter;
        }
    }
}