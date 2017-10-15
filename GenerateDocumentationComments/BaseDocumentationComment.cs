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

//        internal BaseDocumentationComment(string startEndTag = "", string docCommentExterior = null)
//        { }

        internal static SyntaxToken CreateLiteralToken(string literalText, string docCommentExterior)
        {
            return SyntaxFactory.XmlTextLiteral(
                SyntaxFactory.TriviaList(
                    SyntaxFactory.DocumentationCommentExterior(docCommentExterior)),
                literalText,
                literalText,
                SyntaxFactory.TriviaList());
        }

        internal static SyntaxToken CreateNewlineToken(string docCommentExterior)
        {
            return SyntaxFactory.XmlTextNewLine(
                SyntaxFactory.TriviaList(),
                Environment.NewLine,
                Environment.NewLine,
                SyntaxFactory.TriviaList());
        }

        internal static XmlNodeSyntax CreateTextNode(SyntaxTokenList withTokens)
        {
            return SyntaxFactory.XmlText()
                .WithTextTokens(withTokens);
        }

/*        protected static XmlElementSyntax CreateExampleElementNode(XmlNodeSyntax textNode)
        {
            return SyntaxFactory.XmlExampleElement(
                    SyntaxFactory.SingletonList<XmlNodeSyntax>(
                        textNode));
        }

        protected static XmlElementStartTagSyntax CreateElementStartTag(string identifier)
        {
            return SyntaxFactory.XmlElementStartTag(
                SyntaxFactory.XmlName(
                    SyntaxFactory.Identifier(identifier)));
        }

        protected static XmlElementEndTagSyntax CreateElementEndTag(string identifier)
        {
            return SyntaxFactory.XmlElementEndTag(
                SyntaxFactory.XmlName(
                    SyntaxFactory.Identifier(identifier)));
        }

        protected void AddNode(XmlNodeSyntax node)
        {
            NodeList = NodeList.Add(node);
        }*/

        protected void AddNode(Node node)
        {
            nodes.Add(node);
        }

//        internal SyntaxList<XmlNodeSyntax> NodeList { get => nodeList; set => nodeList = value; }

//        private SyntaxList<XmlNodeSyntax> nodeList = SyntaxFactory.List<XmlNodeSyntax>();

        protected List<Node> nodes = new List<Node>();


    }

    internal class SummaryDocumentationComment : BaseDocumentationComment
    {
        internal SummaryDocumentationComment(XmlElementSyntax summaryElement, string docCommentExterior)
        {
            if (summaryElement != null)
            {
                var newNodes = SyntaxFactory.List<SyntaxNode>();
                var textNodes = summaryElement.ChildNodes();
                string startTag = string.Empty;
                string endTag = string.Empty;
                var tNode = new TextNode();
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
                            tNode = new TextNode();
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
                var elt = new ExampleElementNode();
                elt.AddNode(tNode);
                if(!String.IsNullOrEmpty(startTag))
                {
                    elt.StartTag = new StartTag(startTag);
                }
                if(!String.IsNullOrEmpty(endTag))
                {
                    elt.EndTag = new EndTag(endTag);
                }
                AddNode(elt);

            }
            else
            {
                var firstNewlineToken = new NewlineToken();
                var firstPartSummaryComment = new LiteralTextToken(" ");
                var secondNewLineToken = new NewlineToken();
                var secondTextLiteral = new LiteralTextToken(" ");

                var elementTextNode = new TextNode();
                elementTextNode.AddToken(firstNewlineToken);
                elementTextNode.AddToken(firstPartSummaryComment);
                elementTextNode.AddToken(secondNewLineToken);
                elementTextNode.AddToken(secondTextLiteral);

                var exampleElementNode = new ExampleElementNode();
                exampleElementNode.AddNode(elementTextNode);
                var tagName = "summary";
                exampleElementNode.StartTag = new StartTag(tagName);
                exampleElementNode.EndTag = new EndTag(tagName);
                AddNode(exampleElementNode);
            }
        }

        internal override SyntaxList<XmlNodeSyntax> CreateXmlNodes(string commentDelimiter)
        {
            var xmlNodes = SyntaxFactory.List<XmlNodeSyntax>();
            foreach(var node in nodes)
            {
                xmlNodes = xmlNodes.Add(node.CreateXmlNode(commentDelimiter));
            }
            return xmlNodes;
        }
    }
}
