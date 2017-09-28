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
