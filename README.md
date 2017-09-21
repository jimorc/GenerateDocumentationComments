# GenerateDocumentationComments
A Visual Studio extension for generating C# documentation comments

At this time, boilerplate summary documentation comments are added to public, protected,
and internal class declarations when the Tools->Invoke GenerateDocumentationCommentsInEditor
menu item is selected.

I blog about this extension. See the following posts for information about this extension:

1. [I Was Doing It All Wrong, And How I Changed To Doing It Right!](
https://jaipblog.wordpress.com/2017/09/15/i-was-doing-it-all-wrong-and-how-i-changed-to-doing-it-right/)
defines an initial set of user stories and a set of 9 unit tests for adding summary documentation
comments for class declarations.
2. Creating A Documentation Comment Generator For C#: High-Level Design Decisions (to be published)
discusses various ways of triggering the generation of boilerplate documentation comments, and why I
chose to create this as a Visual Studio extension.
3. Creating A Documentation Comment Generator For C#: Setting Up The Framework (to be published) looks 
at creating a Visual Studio extension with a menu item that will generate documentation comments for
the active document (the document in the active editor window). It shows how to retrieve the source code
in the active document, at a high level how to modify the source code to add documentation comments, and
finally, how to write the modified source code back to the active document.

Future posts will be used to extend the discussion.
