using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[assembly: InternalsVisibleTo("GenerateDocumentationCommentsTests,PublicKey=" +
    "002400000480000094000000060200000024000052534131000400000100010019aa30c" +
    "7a9e5935e9ac11a6b35377b9afd0e5bd720b0f54628fb5e143d90f7e05696ea49062f2a" +
    "2d4d06cae1cd6a774a285648846e0789ef89b66818496b7b4ebb58f3637df7f3059bd0b" +
    "8dca6ac53e9618c8272572506d6df0fb37850c6603007a65c85eae18c4887ca04ed8b29" +
    "c62339dd60c6f0643da8287f4cbe0c01279e")]

namespace GenerateDocumentationComments
{
    internal class DocumentCommentsRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var accessToken = new AccessToken(node);
            if(!accessToken.IsKind(SyntaxKind.None))
            {
                DocumentationComments comments = new DocumentationComments(
                    node.GetLeadingTrivia());
                var leadingTrivia = comments.GenerateLeadingTrivia();
                accessToken.ReplaceSummaryTrivia(leadingTrivia);

                var oldAccessToken = new AccessToken(node);
                node = node.ReplaceToken(oldAccessToken.Token, accessToken.Token);
            }
            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var accessToken = new AccessToken(node);
            if (!accessToken.IsKind(SyntaxKind.None))
            {
                var identifier = node.DescendantTokens()
                     .Where(token => token.IsKind(SyntaxKind.IdentifierToken))
                     .First();
                string summary = string.Format(
                    "Initializes a new instance of the <see cref=\"{0}\"/> class.",
                    identifier.ValueText);

                DocumentationComments comments = new DocumentationComments(
                    accessToken.LeadingTrivia, summary);
                var leadingTrivia = comments.GenerateLeadingTrivia();
                accessToken.ReplaceSummaryTrivia(leadingTrivia);

                var oldAccessToken = new AccessToken(node);
                node = node.ReplaceToken(oldAccessToken.Token, accessToken.Token);
           }
            return base.VisitConstructorDeclaration(node);
        }
    }
}
