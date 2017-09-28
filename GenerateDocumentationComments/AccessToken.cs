using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace GenerateDocumentationComments
{
    internal class AccessToken
    {
        public AccessToken(SyntaxNode node)
        {
            token = (SyntaxToken)node.DescendantNodesAndTokens()
                .Where((aNode) => aNode.IsKind(SyntaxKind.PublicKeyword)
                || aNode.IsKind(SyntaxKind.ProtectedKeyword)
                || aNode.IsKind(SyntaxKind.InternalKeyword))
                .FirstOrDefault();
        }

        public bool LeadingTriviaContainsComment(string eltType)
        {
            foreach (var trivia in LeadingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia)
                    || trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    if (trivia.ToString().Contains("<" + eltType + ">"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void ReplaceLeadingTrivia(SyntaxTriviaList trivia)
        {
            token = token.WithLeadingTrivia(trivia);
        }

        public SyntaxTriviaList LeadingTrivia
        {
            get { return token.LeadingTrivia; }
        }

        public bool IsKind(SyntaxKind kind)
        {
            return token.IsKind(kind);
        }

        private SyntaxToken token;

        public SyntaxToken Token
        {
            get { return token; }
        }


    }
}
