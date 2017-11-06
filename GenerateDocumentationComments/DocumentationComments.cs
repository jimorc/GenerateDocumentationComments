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

        protected static IEnumerable<XmlElementSyntax> GetParameterElementsFromNode(SyntaxNode nodeToDocument)
        {
            IEnumerable<XmlElementSyntax> paramElements = null;
            var xmlTriviaList = nodeToDocument.GetLeadingTrivia()
                .Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>();
            if (xmlTriviaList != null)
            {
                var xmlTrivia = xmlTriviaList.FirstOrDefault();
                if (xmlTrivia != null)
                {
                    var elementTriviaList = xmlTrivia.ChildNodes()
                        .Select(i => i)
                        .OfType<XmlElementSyntax>();
                    paramElements = elementTriviaList
                        .Where(t => t.StartTag.Name.ToString().Equals("param"));
                }
            }
            return paramElements;
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
            IEnumerable<XmlElementSyntax> existingParameterElements = GetParameterElementsFromNode(nodeToDocument);
            var parameterComments = new List<ParameterDocumentationComment>();
            if (existingParameterElements != null && existingParameterElements.Count() != 0)
            {
                foreach(var paramElement in existingParameterElements)
                {
                    parameterComments.Add(new ParameterDocumentationComment(paramElement, nodeToDocument, docCommentDelimiter));
                }
            }
            var paramList = nodeToDocument.ChildNodes()
                .Where(n => n.IsKind(SyntaxKind.ParameterList))
                .First();
            var parameters = paramList.ChildNodes()
                .Where(p => p.IsKind(SyntaxKind.Parameter));
            foreach (var param in parameters)
            {
                var paramName = param.ChildTokens()
                    .Where(n => n.IsKind(SyntaxKind.IdentifierToken))
                    .First()
                    .Text;
                var paramComment = parameterComments
                    .Where(c => c.ParamName.Equals(paramName))
                    .FirstOrDefault();
                // if parameter name matches, keep old comment
                if(paramComment != null)
                {
                        paramComments.Add(paramComment);
                }
                else            // create new comment
                {
                    paramComments.Add(new ParameterDocumentationComment(paramName, nodeToDocument, docCommentDelimiter));
                }
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
                    var newlineToken = new NewlineToken();
                    var newlineNode = new TextNode(docCommentDelimiter);
                    newlineNode.AddToken(newlineToken);
                    comments.AddNode(newlineNode);
                    comments.AddNode(textNode);
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
