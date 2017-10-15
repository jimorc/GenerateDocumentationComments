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
            var textLiteralToken = new LiteralTextToken(lastLeadingTrivia.ToFullString());
            var indentNode = new TextNode();
            indentNode.AddToken(textLiteralToken);

            var firstTextToken = new LiteralTextToken(" ");
            var textNode = new TextNode();
            textNode.AddToken(firstTextToken);
            comments = comments.Add(textNode.CreateXmlNode(docCommentDelimiter));
            var summaryComments = summaryComment.CreateXmlNodes(docCommentDelimiter);
            comments = comments.AddRange(summaryComments);

            var lastNewlineToken = new NewlineToken();
            var lastTextNode = new TextNode();
            lastTextNode.AddToken(lastNewlineToken);
            comments = comments.Add(lastTextNode.CreateXmlNode(docCommentDelimiter));

            comments = comments.Add(indentNode.CreateXmlNode(""));
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
