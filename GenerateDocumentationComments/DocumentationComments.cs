using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenerateDocumentationComments
{
    internal class DocumentationComments
    {
        internal DocumentationComments()
        {
            summaryComment = new SummaryDocumentationComment("///");
        }

        internal SyntaxTrivia CreateCommentsTrivia()
        {
            List<XmlNodeSyntax> docComments = summaryComment.CreateListOfNodeSyntaxes();
            XmlNodeSyntax[] nodes = docComments.ToArray();

            return SyntaxFactory.Trivia(
                SyntaxFactory.DocumentationCommentTrivia(
                    SyntaxKind.SingleLineDocumentationCommentTrivia,
                    SyntaxFactory.List<XmlNodeSyntax>(nodes)));
        }

        private BaseDocumentationComment summaryComment;
    }
    /*        public DocumentationComments(SyntaxTriviaList leadingTrivia,
                string summaryDocComment = "")
            {
                initialLeadingTrivia = leadingTrivia;
                var xmlTrivia = leadingTrivia
                    .Select(i => i.GetStructure())
                    .OfType<DocumentationCommentTriviaSyntax>()
                    .FirstOrDefault();
                CreateSummaryDocumentationComment(xmlTrivia, summaryDocComment);
            }

            private void CreateSummaryDocumentationComment(
                        DocumentationCommentTriviaSyntax xmlTrivia,
                        string summaryDocComment)
            {
                if(xmlTrivia != null)
                { 
                    var sumTrivia = xmlTrivia.ChildNodes()
                        .Select(i => i)
                        .OfType<XmlElementSyntax>();
                    var summaryTrivia = sumTrivia
                        .Where(t => t.StartTag.Name.ToString().Equals("summary"))
                        .FirstOrDefault();
                    var sumComment = summaryTrivia.ChildNodes()
                        .OfType<XmlTextSyntax>().FirstOrDefault();
                    var literalToken = sumComment.ChildTokens()
                        .Where(t => t.IsKind(SyntaxKind.XmlTextLiteralToken))
                        .FirstOrDefault();
                    summaryComment = BaseDocumentationComment.CreateDocumentationComment(
                        BaseDocumentationComment.CommentType.Summary,
                        literalToken.Text.TrimStart());
                }
                else
                {
                    summaryComment = BaseDocumentationComment.CreateDocumentationComment(
                        BaseDocumentationComment.CommentType.Summary, summaryDocComment);
                }
            }

            internal SyntaxTriviaList GenerateLeadingTrivia()
            {
                SyntaxTriviaList initialTriviaList = GenerateInitialLeadingTrivia();
                SyntaxTriviaList leadingTriviaList = initialTriviaList;
                initialTriviaList = initialTriviaList.Add(
                    SyntaxFactory.DocumentationCommentExterior("/// "));

                leadingTriviaList = initialTriviaList.Add(
                    summaryComment.GenerateXmlComment(initialTriviaList));
                return leadingTriviaList;
            }

            private SyntaxTriviaList GenerateInitialLeadingTrivia()
            {
                SyntaxTriviaList leadingTriviaList = new SyntaxTriviaList();
                if (initialLeadingTrivia.Count() > 0)
                {
                    var lastLeadingTrivia = initialLeadingTrivia.Last();
                    if (lastLeadingTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                    {
                        leadingTriviaList = leadingTriviaList.Add(lastLeadingTrivia);
                    }
                    for (int i = 1; i < initialLeadingTrivia.Count(); ++i)
                    {
                        if (!initialLeadingTrivia[i].IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                            && !initialLeadingTrivia[i].IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                        {
                            leadingTriviaList = leadingTriviaList.Add(initialLeadingTrivia[i]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return leadingTriviaList;
            }
    private SyntaxTriviaList initialLeadingTrivia;

        }*/
}
