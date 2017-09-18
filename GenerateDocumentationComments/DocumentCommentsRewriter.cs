using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[assembly: InternalsVisibleTo("GenDocCommentsTests,PublicKey=0024000004800000" +
                              "9400000006020000002400005253413100040000010001" +
                              "00a18f862e7ba9c69c88135f4ab1f6b1412cc49de0a3b1" +
                              "fa75e5f76889d2b20ddc41d91a3194053bbb3ba38e706b" +
                              "f9e18078d40310ad659f2133441906200c69caec32f223" +
                              "084d539d5edb445d0ca1b4e6d699ed7a94944a79f14f4b" +
                              "82f61dfd606ffb1148f185444073aadb0024bac3161009" +
                              "629c705fdc89002d93ccf17bf0a6")]

namespace GenerateDocumentationComments
{
    internal class DocumentCommentsRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            SyntaxToken accessTypeToken = GetPublicProtectedOrInternalAccessToken(node);
            if (!accessTypeToken.IsKind(SyntaxKind.None))
            {
                var leadingTriviaList = new List<SyntaxTrivia>();
                var leadingTrivia = accessTypeToken.LeadingTrivia;
                if (leadingTrivia.Count > 0)
                {
                    if (CheckForXmlComment(leadingTrivia, "summary"))
                    {
                        return base.VisitClassDeclaration(node);
                    }
                    var lastLeadingTrivia = leadingTrivia.Last();
                    // if the access level keyword is not at start of a line, then add the whitespace
                    // to make the comments line up with the start of the text on the class line.
                    if (lastLeadingTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        leadingTriviaList.Add(lastLeadingTrivia);
                    }
                }
                var trailingTrivia = accessTypeToken.TrailingTrivia.First();
                var summaryComment = BaseDocumentationComment.CreateDocumentationComment(
                    BaseDocumentationComment.CommentType.Summary);
                leadingTriviaList.Add(
                    SyntaxFactory.DocumentationCommentExterior("/// "));
                var summaryDocumentation = summaryComment.GenerateXmlComment(
                    leadingTriviaList);
                var summaryTrivia = SyntaxFactory.Trivia(summaryDocumentation);

                var newLeadingTrivia =
                    SyntaxFactory.TriviaList(summaryTrivia);
                if (leadingTrivia.Count > 0
                    && leadingTrivia.Last().IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    newLeadingTrivia = newLeadingTrivia.Add(leadingTrivia.Last());
                }
                var token = accessTypeToken.WithLeadingTrivia(newLeadingTrivia);
                node = node.ReplaceToken(accessTypeToken, token);
            }
            return base.VisitClassDeclaration(node);
        }

        private static SyntaxToken GetPublicProtectedOrInternalAccessToken(ClassDeclarationSyntax node)
        {
            return (SyntaxToken)node.DescendantNodesAndTokens()
                .Where((aNode) => aNode.IsKind(SyntaxKind.PublicKeyword)
                || aNode.IsKind(SyntaxKind.ProtectedKeyword)
                || aNode.IsKind(SyntaxKind.InternalKeyword))
                .FirstOrDefault();
        }

        private static bool CheckForXmlComment(SyntaxTriviaList triviaList, string eltType)
        {
            foreach (var trivia in triviaList)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                    || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    if (trivia.ToString().Contains("<" + eltType + ">"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
