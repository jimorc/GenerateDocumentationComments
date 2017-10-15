using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal abstract class Node
    {
        internal abstract XmlNodeSyntax CreateXmlNode(string docCommentExterior);

        internal void AddToken(Token token)
        {
            tokens.Add(token);
        }

        internal void AddTokenList(List<Token> tokenList)
        {
            tokens.AddRange(tokenList);
        }

        protected SyntaxTokenList CreateTokenList(string docCommentExterior)
        {
            var tokenList = SyntaxFactory.TokenList();
            foreach (var token in tokens)
            {
                SyntaxToken sToken = token.CreateXmlToken(docCommentExterior);
                tokenList = tokenList.Add(sToken);
            }
            return tokenList;
        }

        private List<Token> tokens = new List<Token>();
    }

    internal class TextNode : Node
    {
        internal override XmlNodeSyntax CreateXmlNode(string docCommentExterior)
        {
            var tokenList = CreateTokenList(docCommentExterior);
            return SyntaxFactory.XmlText()
                .WithTextTokens(tokenList);
        }
    }

    internal class ExampleElementNode : Node
    {
        internal SyntaxList<XmlNodeSyntax> CreateNodeList(string docCommentExterior)
        {
            var nodeList = SyntaxFactory.List<XmlNodeSyntax>();
            foreach (var node in nodes)
            {
                var xmlNode = node.CreateXmlNode(docCommentExterior);
                nodeList = nodeList.Add(xmlNode);
            }
            return nodeList;
        }

        internal override XmlNodeSyntax CreateXmlNode(string docCommentExterior)
        {
            var nodeList = CreateNodeList(docCommentExterior);
            XmlElementSyntax elementSyntax;
            if (nodeList.Count == 1)
            {
                elementSyntax = SyntaxFactory.XmlExampleElement(
                    SyntaxFactory.SingletonList<XmlNodeSyntax>(
                        nodeList.First()));
            }
            else
            {
                elementSyntax = SyntaxFactory.XmlExampleElement(
                    nodeList);
            }
            if(StartTag != null && EndTag != null)
            {
                elementSyntax = elementSyntax
                    .WithStartTag(StartTag.CreateTag())
                    .WithEndTag(EndTag.CreateTag());
            }
            return elementSyntax;
        }

        internal void AddNode(Node node)
        {
            nodes.Add(node);
        }

        private List<Node> nodes = new List<Node>();
        internal StartTag StartTag{ get; set; }
        internal EndTag EndTag { get; set; }
    }

}
