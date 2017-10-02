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
        protected BaseDocumentationComment(CommentType type, string text)
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
    }
}
