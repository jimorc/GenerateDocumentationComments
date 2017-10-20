using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class BaseDocumentationComment
    {
        public enum CommentType
        {
            Summary,
            Parameter
        }

        internal BaseDocumentationComment(SyntaxNode nodeToDocument)
        {
            nodeBeingDocumented = nodeToDocument;
        }

        internal abstract SyntaxList<XmlNodeSyntax> CreateXmlNodes(string commentDelimiter);
        internal abstract void CreateNewComment(string docCommentExterior);

        protected XmlElementSyntax GetSummaryElement()
        {
            XmlElementSyntax summaryElement = null;
            var xmlTriviaList = nodeBeingDocumented.GetLeadingTrivia().Select(i => i.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>();
            if (xmlTriviaList != null)
            {
                var xmlTrivia = xmlTriviaList.FirstOrDefault();
                if (xmlTrivia != null)
                {
                    var elementTriviaList = xmlTrivia.ChildNodes()
                        .Select(i => i)
                        .OfType<XmlElementSyntax>();
                    summaryElement = elementTriviaList
                        .Where(t => t.StartTag.Name.ToString().Equals("summary"))
                        .FirstOrDefault();
                }
            }
            return summaryElement;
        }

        protected void AddNode(Node node)
        {
            nodes.Add(node);
        }

        protected List<Node> nodes = new List<Node>();
        internal List<Node> Nodes { get => nodes; }

        protected SyntaxNode nodeBeingDocumented;
    }

    internal abstract class SummaryDocumentationComment : BaseDocumentationComment
    {
        internal SummaryDocumentationComment(SyntaxNode nodeToDocument, string docCommentExterior)
            : base(nodeToDocument)
        {
            var summaryElement = GetSummaryElement();
            if (summaryElement != null)
            {
                var newNodes = SyntaxFactory.List<SyntaxNode>();
                var textNodes = summaryElement.ChildNodes();
                string startTag = string.Empty;
                string endTag = string.Empty;
                var tNode = new TextNode(docCommentExterior);
                foreach (var textNode in textNodes)
                {
                    switch (textNode.Kind())
                    {
                        case SyntaxKind.XmlElementStartTag:
                            var xmlName = textNode.ChildNodes()
                                .OfType<XmlNameSyntax>()
                                .FirstOrDefault();
                            if (xmlName != null)
                            {
                                startTag = xmlName.GetText().ToString();
                            }
                            break;
                        case SyntaxKind.XmlElementEndTag:
                            xmlName = textNode.ChildNodes()
                                .OfType<XmlNameSyntax>()
                                .FirstOrDefault();
                            if (xmlName != null)
                            {
                                endTag = xmlName.GetText().ToString();
                            }
                            break;
                        case SyntaxKind.XmlText:
                            tNode = new TextNode(docCommentExterior);
                            foreach (var token in textNode.ChildTokens())
                            {
                                switch (token.Kind())
                                {
                                    case SyntaxKind.XmlTextLiteralNewLineToken:
                                        tNode.AddToken(new NewlineToken());
                                        break;
                                    case SyntaxKind.XmlTextLiteralToken:
                                        var text = token.ValueText.ToString();
                                        var textLiteralToken = new LiteralTextToken(text);
                                        tNode.AddToken(textLiteralToken);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                var elt = new ExampleElementNode(docCommentExterior);
                elt.AddNode(tNode);
                if (!String.IsNullOrEmpty(startTag))
                {
                    elt.StartTag = new StartTag(startTag);
                }
                if (!String.IsNullOrEmpty(endTag))
                {
                    elt.EndTag = new EndTag(endTag);
                }
                AddNode(elt);

            }
            else
            {
                CreateNewComment(docCommentExterior);
            }
        }

        internal override SyntaxList<XmlNodeSyntax> CreateXmlNodes(string commentDelimiter)
        {
            var xmlNodes = SyntaxFactory.List<XmlNodeSyntax>();
            foreach (var node in nodes)
            {
                xmlNodes = xmlNodes.Add(node.CreateXmlNode());
            }
            return xmlNodes;
        }

        protected void AddExampleElementNode(Node[] nodes, string docCommentExterior)
        {
            var exampleElementNode = new ExampleElementNode(docCommentExterior);
            foreach (var node in nodes)
            {
                exampleElementNode.AddNode(node);
            }
            var tagName = "summary";
            exampleElementNode.StartTag = new StartTag(tagName);
            exampleElementNode.EndTag = new EndTag(tagName);
            AddNode(exampleElementNode);
        }
    }

    internal class ClassSummaryDocumentationComment : SummaryDocumentationComment
    {
        internal ClassSummaryDocumentationComment(SyntaxNode nodeToDocument, string docCommentExterior)
            : base(nodeToDocument, docCommentExterior) { }

        internal override void CreateNewComment(string docCommentExterior)
        {
            var firstNewlineToken = new NewlineToken();
            var firstPartSummaryComment = new LiteralTextToken(" ");
            var secondNewLineToken = new NewlineToken();
            var secondTextLiteral = new LiteralTextToken(" ");

            var elementTextNode = new TextNode(docCommentExterior);
            elementTextNode.AddToken(firstNewlineToken);
            elementTextNode.AddToken(firstPartSummaryComment);
            elementTextNode.AddToken(secondNewLineToken);
            elementTextNode.AddToken(secondTextLiteral);

            Node[] nodes = { elementTextNode };
            AddExampleElementNode(nodes, docCommentExterior);
        }
    }

    internal class ConstructorSummaryDocumentationComment : SummaryDocumentationComment
    {
        internal ConstructorSummaryDocumentationComment(SyntaxNode nodeToDocument, string docCommentExterior)
            : base(nodeToDocument, docCommentExterior) { }

        internal override void CreateNewComment(string docCommentExterior)
        {
            var firstNewlineToken = new NewlineToken();
            var firstPartSummaryComment = new LiteralTextToken(" Initializes a new instance of the ");
            var firstTextNode = new TextNode(docCommentExterior);
            firstTextNode.AddToken(firstNewlineToken);
            firstTextNode.AddToken(firstPartSummaryComment);

            var className = nodeBeingDocumented.ChildTokens()
                .Where(t => t.IsKind(SyntaxKind.IdentifierToken))
                .First()
                .ToFullString();
            var cref = new CrefNode(className);

            var secondPartSummaryComment = new LiteralTextTokenWithNoDocCommentExterior(" class.");
            var secondNewlineToken = new NewlineToken();
            var thirdPartSummaryComment = new LiteralTextToken(" ");
            var secondTextNode = new TextNode(docCommentExterior);
            secondTextNode.AddToken(secondPartSummaryComment);
            secondTextNode.AddToken(secondNewlineToken);
            secondTextNode.AddToken(thirdPartSummaryComment);

            Node[] nodes = { firstTextNode, cref, secondTextNode };
            AddExampleElementNode(nodes, docCommentExterior);
        }
    }

    internal class ParameterDocumentationComment : BaseDocumentationComment
    {
        internal ParameterDocumentationComment(string parameterName, SyntaxNode nodeToDocument, string docCommentExterior)
            : base(nodeToDocument)
        {
            paramName = parameterName;
            CreateNewComment(docCommentExterior);
        }

        internal override void CreateNewComment(string docCommentExterior)
        {
            var firstTextNewLineToken = new NewlineToken();
            var firstTextPartToken = new LiteralTextToken(" ");
            var firstTextNode = new TextNode(docCommentExterior);
            firstTextNode.AddToken(firstTextNewLineToken);
            firstTextNode.AddToken(firstTextPartToken);

            var literalText = CreateParameterTextString();
            var textToken = new LiteralTextToken(literalText);
            var paramTextNode = new TextNode("");
            paramTextNode.AddToken(textToken);
            var startTag = new StartTag("param");
            var nameAttribute = new Attribute("name", paramName);
            startTag.Attribute = nameAttribute;
            var endTag = new EndTag("param");

            var exampleElementNode = new ExampleElementNode(docCommentExterior);
            exampleElementNode.AddNode(paramTextNode);
            exampleElementNode.StartTag = startTag;
            exampleElementNode.EndTag = endTag;

            AddNode(firstTextNode);
            AddNode(exampleElementNode);
        }

        internal override SyntaxList<XmlNodeSyntax> CreateXmlNodes(string commentDelimiter)
        {
            var xmlNodes = SyntaxFactory.List<XmlNodeSyntax>();
            foreach (var node in nodes)
            {
                xmlNodes = xmlNodes.Add(node.CreateXmlNode());
            }
            return xmlNodes;
        }

        internal string CreateParameterTextString()
        {
            string parameterTextString;
            var parameterNameParts = SplitParameterNameIntoParts();
            ReplaceDocStringWithDocument(parameterNameParts);
            if (parameterNameParts.Last() == "name" && parameterNameParts.Count != 1)
            {
                parameterTextString = "Name of the";
                for (int i = 0; i < parameterNameParts.Count - 1; i++)
                {
                    parameterTextString += " " + parameterNameParts[i];
                }
            }
            else
            {
                parameterTextString = "The";
                foreach (var part in parameterNameParts)
                {
                    parameterTextString += " " + part;
                }
            }
            return parameterTextString + ".";
        }

        private static void ReplaceDocStringWithDocument(List<string> parameterNameParts)
        {
            for (int i = 0; i < parameterNameParts.Count; i++)
            {
                if (parameterNameParts[i] == "doc")
                {
                    parameterNameParts[i] = "document";
                }
            }
        }

        private List<string> SplitParameterNameIntoParts()
        {
            var splitParts = Regex.Replace(Regex.Replace(paramName, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
            string[] separators = { " " };
            var parts = splitParts.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var paramParts = new List<string>();
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;
            foreach(var part in parts)
            {
                var modifiedPart = textInfo.ToLower(part);
                paramParts.Add(modifiedPart);
            }
            return paramParts;
        }

        private string paramName;
    }
}
