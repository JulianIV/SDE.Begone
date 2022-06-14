using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SDE.Begone
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(SDEBegoneCodeFixProvider)), Shared]
    public class SDEBegoneCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(SDEBegoneAnalyzer.DiagnosticId); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach(var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;
            
                var usingDirectiveDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<UsingDirectiveSyntax>().First();

                context.RegisterCodeFix(
                   CodeAction.Create(
                       title: CodeFixResources.CodeFixTitle,
                       createChangedDocument: c => RemoveUsing(context.Document, usingDirectiveDeclaration, c),
                       equivalenceKey: nameof(CodeFixResources.CodeFixTitle)),
                   diagnostic);
            }
        }

        private async Task<Document> RemoveUsing(Document document, UsingDirectiveSyntax usingDirective, CancellationToken cancellationToken)
        {
            var oldRoot = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = oldRoot.RemoveNode(usingDirective, SyntaxRemoveOptions.KeepNoTrivia);

            return document.WithSyntaxRoot(newRoot);
        }
    }
}
