using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal class DocumentationComments
    {
        const string commentDelimiter = "///";

        internal DocumentationComments(SyntaxTriviaList leadingTrivia)
        {
            lastLeadingTrivia = leadingTrivia.LastOrDefault();
            docCommentDelimiter = lastLeadingTrivia.ToFullString() + commentDelimiter;

            XmlElementSyntax summaryElement = null;
            var xmlTrivia = leadingTrivia.Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();
            if (xmlTrivia != null)
            {
                var elementTriviaList = xmlTrivia.ChildNodes()
                    .Select(i => i)
                    .OfType<XmlElementSyntax>();
                summaryElement = elementTriviaList
                    .Where(t => t.StartTag.Name.ToString().Equals("summary"))
                    .FirstOrDefault();
            }
            summaryComment = new SummaryDocumentationComment(summaryElement, docCommentDelimiter);
        }

        internal SyntaxTrivia CreateCommentsTrivia()
        {
            SyntaxList<XmlNodeSyntax> comments = SyntaxFactory.List<XmlNodeSyntax>();
            var textTokens = SyntaxFactory.TokenList();
            var textLiteralToken = new LiteralTextToken(lastLeadingTrivia.ToFullString());
            textTokens = textTokens.Add(textLiteralToken.CreateXmlToken(""));
            var indentNode = BaseDocumentationComment.CreateTextNode(textTokens);

            var firstTextToken = new LiteralTextToken(" ");
            textTokens = textTokens.Add(firstTextToken.CreateXmlToken(commentDelimiter));
            var textNode = BaseDocumentationComment.CreateTextNode(textTokens);
            comments = comments.Add(textNode);
            var summaryComments = summaryComment.CreateXmlNodes(docCommentDelimiter);
            comments = comments.AddRange(summaryComments);

            var lastNewlineToken = new NewlineToken();
            var lastTextTokens = SyntaxFactory.TokenList();
            lastTextTokens = lastTextTokens.Add(lastNewlineToken.CreateXmlToken(docCommentDelimiter));
            var lastTextNode = BaseDocumentationComment.CreateTextNode(lastTextTokens);
            comments = comments.Add(lastTextNode);

            comments = comments.Add(indentNode);
            XmlNodeSyntax[] nodes = comments.ToArray();

            return SyntaxFactory.Trivia(
                SyntaxFactory.DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    SyntaxFactory.List<XmlNodeSyntax>(nodes)));
        }

        private BaseDocumentationComment summaryComment;
        private SyntaxTrivia lastLeadingTrivia;
        private string docCommentDelimiter;
    }
}
