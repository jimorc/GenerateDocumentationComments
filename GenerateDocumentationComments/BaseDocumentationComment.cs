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
            Summary
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

        internal virtual DocumentationCommentTriviaSyntax GenerateXmlComment(
            LeadingTriviaList leadingTrivia)
        { return null; }

        protected static SyntaxToken GenerateDocumentCommentLine(
    LeadingTriviaList leadingTriviaList,
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

        private static SyntaxTriviaList GenerateTriviaList(LeadingTriviaList trivia)
        {
            var triviaList = SyntaxFactory.TriviaList();
            foreach (var triv in trivia)
            {
                triviaList = triviaList.Add(triv);
            }
            return triviaList;
        }

        protected static XmlElementSyntax GenerateXmlExampleElement(
            LeadingTriviaList leadingTrivia,
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

        internal override DocumentationCommentTriviaSyntax GenerateXmlComment(
            LeadingTriviaList leadingTrivia)
        {
            var summaryDocumentation = SyntaxFactory.DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    SyntaxFactory.List<XmlNodeSyntax>(
                        new XmlNodeSyntax[]{
                            SyntaxFactory.XmlText()
                            .WithTextTokens(
                                SyntaxFactory.TokenList(
                                    GenerateDocumentCommentLine(leadingTrivia, ""))),
                            GenerateXmlExampleElement(leadingTrivia, "summary", ""),
                            SyntaxFactory.XmlText()
                            .WithTextTokens(
                                SyntaxFactory.TokenList(
                                GenerateXmlNewLineTrivia())) }));
            return summaryDocumentation;
        }
    }


}
