using System;
using System.Collections.Generic;
using System.Linq;
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

        internal abstract SyntaxList<XmlNodeSyntax> CreateXmlNodes(string commentDelimiter);
        internal abstract void CreateNewComment(string docCommentExterior);

        protected void AddNode(Node node)
        {
            nodes.Add(node);
        }

        protected List<Node> nodes = new List<Node>();
        internal List<Node> Nodes { get => nodes; }


    }

    internal abstract class SummaryDocumentationComment : BaseDocumentationComment
    {
        internal SummaryDocumentationComment(XmlElementSyntax summaryElement, string docCommentExterior)
        {
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
    }

    internal class ClassSummaryDocumentationComment : SummaryDocumentationComment
    {
        internal ClassSummaryDocumentationComment(XmlElementSyntax summaryElement, string docCommentExterior)
            : base(summaryElement, docCommentExterior) { }

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

            var exampleElementNode = new ExampleElementNode(docCommentExterior);
            exampleElementNode.AddNode(elementTextNode);
            var tagName = "summary";
            exampleElementNode.StartTag = new StartTag(tagName);
            exampleElementNode.EndTag = new EndTag(tagName);
            AddNode(exampleElementNode);
        }
    }

    internal class ConstructorSummaryDocumentationComment : SummaryDocumentationComment
    {
        internal ConstructorSummaryDocumentationComment(XmlElementSyntax summaryElement, string docCommentExterior)
            : base(summaryElement, docCommentExterior) { }

        internal override void CreateNewComment(string docCommentExterior)
        {
            var firstNewlineToken = new NewlineToken();
            var firstPartSummaryComment = new LiteralTextToken(" Initializes a new instance of the ");
            var firstTextNode = new TextNode(docCommentExterior);
            firstTextNode.AddToken(firstNewlineToken);
            firstTextNode.AddToken(firstPartSummaryComment);

            var cref = new CrefNode("Class1");

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

        private void AddExampleElementNode(Node[] nodes, string docCommentExterior)
        {
            var exampleElementNode = new ExampleElementNode(docCommentExterior);
            foreach(var node in nodes)
            {
                exampleElementNode.AddNode(node);
            }
            var tagName = "summary";
            exampleElementNode.StartTag = new StartTag(tagName);
            exampleElementNode.EndTag = new EndTag(tagName);
            AddNode(exampleElementNode);
        }
    }
}
