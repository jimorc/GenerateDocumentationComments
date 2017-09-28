using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GenerateDocumentationComments
{
    internal class LeadingTriviaList : IEnumerable<SyntaxTrivia>
    {
        public LeadingTriviaList(SyntaxTriviaList trivia)
        {
            if(trivia.Count > 0)
            {
                var lastTrivia = trivia.Last();
                // if not at start of line, add whitespace to line up with code
                if(lastTrivia.IsKind(SyntaxKind.WhitespaceTrivia))
                {
                    list.Add(lastTrivia);
                }
            }
        }

        public void Add(SyntaxTrivia trivia)
        {
            list.Add(trivia);
        }

        public IEnumerator<SyntaxTrivia> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private List<SyntaxTrivia> list = new List<SyntaxTrivia>();
    }
}
