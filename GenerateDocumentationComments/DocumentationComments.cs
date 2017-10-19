using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class DocumentationComments
    {
        internal DocumentationComments(SyntaxNode nodeToDocument)
        {
            lastLeadingTrivia = nodeToDocument.GetLeadingTrivia().LastOrDefault();
            docCommentDelimiter = lastLeadingTrivia.ToFullString() + DocumentationComments.commentDelimiter;
        }

        protected const string commentDelimiter = "///";

        protected SyntaxTrivia lastLeadingTrivia;
        protected string docCommentDelimiter;
    }

    internal class ClassDocumentationComments : DocumentationComments
    {
        internal ClassDocumentationComments(SyntaxNode nodeToDocument)
            : base(nodeToDocument)
        {
            summaryComment = new ClassSummaryDocumentationComment(nodeToDocument, docCommentDelimiter);
        }

        internal SyntaxTrivia CreateCommentsTrivia()
        {
            var comments = new Nodes();
            var textLiteralToken = new LiteralTextToken(lastLeadingTrivia.ToFullString());
            var indentNode = new TextNode("");
            indentNode.AddToken(textLiteralToken);

            var firstTextToken = new LiteralTextToken(" ");
            var textNode = new TextNode(docCommentDelimiter);
            textNode.AddToken(firstTextToken);
            comments.AddNode(textNode);
            comments.AddNodesRange(summaryComment.Nodes);

            var lastNewlineToken = new NewlineToken();
            var lastTextNode = new TextNode(docCommentDelimiter);
            lastTextNode.AddToken(lastNewlineToken);
            comments.AddNode(lastTextNode);

            comments.AddNode(indentNode);
            XmlNodeSyntax[] nodes = comments.CreateXmlNodes();

            return SyntaxFactory.Trivia(
                SyntaxFactory.DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    SyntaxFactory.List<XmlNodeSyntax>(nodes)));
        }

        private BaseDocumentationComment summaryComment;
    }

    internal class ConstructorDocumentationComments : DocumentationComments
    {
        internal ConstructorDocumentationComments(SyntaxNode nodeToDocument)
            : base(nodeToDocument)
        {
            summaryComment = new ConstructorSummaryDocumentationComment(nodeToDocument, docCommentDelimiter);
            var paramList = nodeToDocument.ChildNodes()
                .Where(n => n.IsKind(SyntaxKind.ParameterList))
                .First();
            var parameters = paramList.ChildNodes()
                .Where(p => p.IsKind(SyntaxKind.Parameter));
            foreach(var param in parameters)
            {
                var paramName = param.ChildTokens()
                    .Where(n => n.IsKind(SyntaxKind.IdentifierToken))
                    .First()
                    .Text;
                paramComments.Add(new ParameterDocumentationComment(paramName, nodeToDocument, docCommentDelimiter));
            }
        }

        internal SyntaxTrivia CreateCommentsTrivia()
        {
            var comments = new Nodes();
            var textLiteralToken = new LiteralTextToken(lastLeadingTrivia.ToFullString());
            var indentNode = new TextNode("");
            indentNode.AddToken(textLiteralToken);

            var firstTextToken = new LiteralTextToken(" ");
            var textNode = new TextNode(docCommentDelimiter);
            textNode.AddToken(firstTextToken);
            comments.AddNode(textNode);
            comments.AddNodesRange(summaryComment.Nodes);
            if(paramComments.Count > 0)
            {
                foreach(var paramComment in paramComments)
                {
                    comments.AddNodesRange(paramComment.Nodes);
                }
            }

            var lastNewlineToken = new NewlineToken();
            var lastTextNode = new TextNode(docCommentDelimiter);
            lastTextNode.AddToken(lastNewlineToken);
            comments.AddNode(lastTextNode);

            comments.AddNode(indentNode);
            XmlNodeSyntax[] nodes = comments.CreateXmlNodes();

            return SyntaxFactory.Trivia(
                SyntaxFactory.DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    SyntaxFactory.List<XmlNodeSyntax>(nodes)));
        }

        private BaseDocumentationComment summaryComment;
        private List<BaseDocumentationComment> paramComments = new List<BaseDocumentationComment>();
    }
}
