using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal class BaseDocumentationComment
    {
        public enum CommentType
        {
            Summary,
            Parameter
        }

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

        protected static SyntaxToken CreateNewlineToken(string docCommentExterior)
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

        protected static XmlElementSyntax CreateExampleElementNode(XmlNodeSyntax textNode)
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
        }

        internal SyntaxList<XmlNodeSyntax> NodeList { get => nodeList; set => nodeList = value; }

        private SyntaxList<XmlNodeSyntax> nodeList = SyntaxFactory.List<XmlNodeSyntax>();
    }

    internal class SummaryDocumentationComment : BaseDocumentationComment
    {
        internal SummaryDocumentationComment(string docCommentExterior)
        {
            var textToken = CreateLiteralToken(" ", docCommentExterior);
            var textTokens = SyntaxFactory.TokenList();
            textTokens = textTokens.Add(textToken);
            var textNode = CreateTextNode(textTokens);

            var firstNewlineToken = CreateNewlineToken(docCommentExterior);
            var summaryTextToken = CreateLiteralToken(" ", docCommentExterior);
            var secondNewlineToken = CreateNewlineToken(docCommentExterior);
            var secondTextToken = CreateLiteralToken(" ", docCommentExterior);
            var exampleElementTextTokens = SyntaxFactory.TokenList();
            exampleElementTextTokens = exampleElementTextTokens.Add(firstNewlineToken);
            exampleElementTextTokens = exampleElementTextTokens.Add(summaryTextToken);
            exampleElementTextTokens = exampleElementTextTokens.Add(secondNewlineToken);
            exampleElementTextTokens = exampleElementTextTokens.Add(secondTextToken);
            var exampleTextNode = CreateTextNode(exampleElementTextTokens);
            var exampleElementNode = CreateExampleElementNode(exampleTextNode)
                .WithStartTag(CreateElementStartTag("summary"))
                .WithEndTag(CreateElementEndTag("summary"));


            var lastNewlineToken = CreateNewlineToken(docCommentExterior);
            var lastTextTokens = SyntaxFactory.TokenList();
            lastTextTokens = lastTextTokens.Add(lastNewlineToken);
            var lastTextNode = CreateTextNode(lastTextTokens);

            AddNode(textNode);
            AddNode(exampleElementNode);
            AddNode(lastTextNode);
        }

        /*        protected BaseDocumentationComment(CommentType type, string text)
                {
                    Type = type;
                    CommentText = text;
                }

                public static BaseDocumentationComment CreateDocumentationComment(
                    CommentType type, string text = "")
                {
                    switch (type)
                    {
                        case CommentType.Summary:
                            return new SummaryDocumentationComment(text);
                        default:
                            return new BaseDocumentationComment(type, text);

                    }
                }

                internal virtual SyntaxTrivia GenerateXmlComment(
                    SyntaxTriviaList leadingTrivia)
                { return new SyntaxTrivia(); }

                protected static SyntaxToken GenerateDocumentCommentLine(
                    SyntaxTriviaList leadingTriviaList,
                    string docCommentText)
                {
                    var docCommentTriviaList = GenerateTriviaList(leadingTriviaList);
                    var triviaLiteral = SyntaxFactory.XmlTextLiteral(
                        docCommentTriviaList,
                        docCommentText,
                        docCommentText,
                        SyntaxFactory.TriviaList());
                    return triviaLiteral;
                }

                private static SyntaxTriviaList GenerateTriviaList(SyntaxTriviaList trivia)
                {
                    var triviaList = SyntaxFactory.TriviaList();
                    foreach (var triv in trivia)
                    {
                        triviaList = triviaList.Add(triv);
                    }
                    return triviaList;
                }

                protected static XmlElementSyntax GenerateXmlExampleElement(
                    SyntaxTriviaList leadingTrivia,
                    string elementName,
                    string elementText)
                {
                    return SyntaxFactory.XmlExampleElement(
                        SyntaxFactory.SingletonList<XmlNodeSyntax>(
                            SyntaxFactory.XmlText()
                            .WithTextTokens(
                                SyntaxFactory.TokenList(
                                    new[]{
                            GenerateXmlNewLineTrivia(),
                            GenerateDocumentCommentLine(leadingTrivia, elementText),
                            GenerateXmlNewLineTrivia(),
                            GenerateDocumentCommentLine(leadingTrivia, "") }))))
                        .WithStartTag(
                            GenerateXmlElementStartTag(elementName))
                        .WithEndTag(
                            GenerateXmlElementEndTag(elementName));
                }

                protected static SyntaxToken GenerateXmlNewLineTrivia()
                {
                    return SyntaxFactory.XmlTextNewLine(
                        SyntaxFactory.TriviaList(),
                        System.Environment.NewLine,
                        System.Environment.NewLine,
                        SyntaxFactory.TriviaList());
                }

                private static XmlElementEndTagSyntax GenerateXmlElementEndTag(string eltName)
                {
                    return SyntaxFactory.XmlElementEndTag(
                        SyntaxFactory.XmlName(
                            SyntaxFactory.Identifier(eltName)));
                }

                private static XmlElementStartTagSyntax GenerateXmlElementStartTag(string eltName)
                {
                    return SyntaxFactory.XmlElementStartTag(
                                   SyntaxFactory.XmlName(
                                       SyntaxFactory.Identifier(eltName)));
                }

                public CommentType Type {
                    get;
                    private set;
                }

                public string CommentText
                {
                    get;
                    set;
                }
            }

            internal class SummaryDocumentationComment : BaseDocumentationComment
            {
                internal SummaryDocumentationComment(string text)
                : base(CommentType.Summary, text)
                    { }

                internal override SyntaxTrivia GenerateXmlComment(
                    SyntaxTriviaList leadingTrivia)
                {
                    var summaryDocumentation = SyntaxFactory.DocumentationCommentTrivia(
                            SyntaxKind.SingleLineDocumentationCommentTrivia,
                            SyntaxFactory.List<XmlNodeSyntax>(
                                new XmlNodeSyntax[]{
                                    GenerateXmlExampleElement(leadingTrivia, "summary", CommentText),
                                    SyntaxFactory.XmlText()
                                    .WithTextTokens(
                                        SyntaxFactory.TokenList(
                                            GenerateXmlNewLineTrivia())) }));
                    return SyntaxFactory.Trivia(summaryDocumentation);
                }
            }*/
    }
}
