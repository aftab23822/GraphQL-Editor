using System.Text.RegularExpressions;
namespace GraphQL_Editor_WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            richTextBoxGraphQL.TextChanged += RichTextBoxGraphQL_TextChanged;
            richTextBoxGraphQL.KeyPress += RichTextBoxGraphQL_KeyPress; // Handle the KeyPress event
            richTextBoxGraphQL.KeyDown += RichTextBoxGraphQL_KeyDown; // Handle the KeyDown event
            this.Load += MainForm_Load;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            // Set up the RichTextBox properties
            richTextBoxGraphQL.AcceptsTab = true;
            richTextBoxGraphQL.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
           

            // Set up a button to execute the GraphQL query (optional)
            Button executeButton = new Button();
            executeButton.Text = "Execute Query";
            executeButton.Click += ExecuteQuery;
            executeButton.Dock = DockStyle.Bottom;

            // Add controls to the form
            Controls.Add(richTextBoxGraphQL);
            Controls.Add(executeButton);
        }



        private void RichTextBoxGraphQL_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the user typed a brace character
            if (IsBraceCharacter(e.KeyChar))
            {
                // Insert the corresponding closing brace at the current cursor position
                int cursorPosition = richTextBoxGraphQL.SelectionStart;
                richTextBoxGraphQL.SelectedText = GetClosingBrace(e.KeyChar);

                // Move the cursor inside the braces and set the indentation
                richTextBoxGraphQL.SelectionStart = cursorPosition + 1;
                richTextBoxGraphQL.SelectionLength = 0;

                // Prevent the brace character from being added to the text
                e.Handled = true;
            }
        }

        private void RichTextBoxGraphQL_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if the Enter key is pressed
            if (e.KeyCode == Keys.Enter)
            {
                // Check if the cursor is between an opening and closing brace
                int cursorPosition = richTextBoxGraphQL.SelectionStart;
                string textBeforeCursor = richTextBoxGraphQL.Text.Substring(0, cursorPosition);
                

                // Calculate the depth of nested braces
                int openingBraceCount = textBeforeCursor.Count(c => c == '{');
                int closingBraceCount = textBeforeCursor.Count(c => c == '}');
                int braceDepth = openingBraceCount - closingBraceCount;

                if (braceDepth > 0)
                {
                    // Insert a new line with proper indentation
                    richTextBoxGraphQL.SelectedText = "\n" + new string('\t', braceDepth) + GetEndingText(textBeforeCursor,braceDepth);

                    // Set the cursor position after the indentation on the next line
                    richTextBoxGraphQL.SelectionStart = cursorPosition + 1 + braceDepth;

                    // Prevent the Enter key from being added to the text
                    e.Handled = true;
                }
            }
        }

        private string GetEndingText(string textBeforeCursor, int braceDepth)
        {
            return textBeforeCursor.EndsWith("{") ?
                        "\n" + new string('\t', braceDepth - 1)
                        : "";
        }

        private bool IsBraceCharacter(char character)
        {
            // Check if the character is one of the brace characters
            return character == '(' || character == '{' || character == '[';
        }

        private string GetClosingBrace(char openingBrace)
        {
            // Return the corresponding closing brace for the given opening brace
            switch (openingBrace)
            {
                case '(':
                    return "()";
                case '{':
                    return "{}";
                case '[':
                    return "[]";
                default:
                    return "";
            }
        }

        private void SetGraphQLQuery(string query)
        {


            // Return the formatted query

            // Set the GraphQL query text
            richTextBoxGraphQL.Text = query;

            // Apply syntax highlighting
            //HighlightSyntax();

        }
        // Example GraphQL query



        private void RichTextBoxGraphQL_TextChanged(object sender, EventArgs e)
        {
            // Call the syntax highlighting method
            HighlightSyntax();
        }


        private void HighlightSyntax()
        {
            // Preserve the current selection start and length
            int originalSelectionStart = richTextBoxGraphQL.SelectionStart;
            int originalSelectionLength = richTextBoxGraphQL.SelectionLength;


            // Define regex patterns for GraphQL keywords
            string[] queryMutationKeywords = { "query", "mutation" };
            string[] schemaDefinitionKeywords = { "type", "interface", "enum", "union", "input" };

            string queryMutationPattern = "\\b(" + string.Join("|", queryMutationKeywords) + ")\\b";
            string schemaDefinitionPattern = "\\b(" + string.Join("|", schemaDefinitionKeywords) + ")\\b";

            // Define regex patterns for additional GraphQL keywords
            string fragmentPattern = "\\bfragment\\b";
            string variablePattern = "\\$[a-zA-Z_][a-zA-Z0-9_]*";
            string directivePattern = "@[a-zA-Z_][a-zA-Z0-9_]*";

            // Set the default color
            richTextBoxGraphQL.SelectionColor = Color.Black;

            // Process the text for query and mutation keywords
            ProcessRegexPattern(queryMutationPattern, Color.Blue);

            // Process the text for schema definition keywords
            ProcessRegexPattern(schemaDefinitionPattern, Color.DarkOrange);

            //Process the text for additional keywords
            ProcessRegexPattern(fragmentPattern, Color.Purple); // Choose a color for fragment keyword
            ProcessRegexPattern(variablePattern, Color.DarkGreen); // Choose a color for variables
            ProcessRegexPattern(directivePattern, Color.DarkRed); // Choose a color for directives


            // Restore the original selection
            richTextBoxGraphQL.Select(originalSelectionStart, originalSelectionLength);

            // Define regex pattern for the last word before an opening brace
            string lastWordBeforeOpeningBracePattern = @"(\w+)\s*(?=\{)";

            // Process the text for the last word before an opening brace
            ProcessRegexPattern(lastWordBeforeOpeningBracePattern, Color.Blue);


            // Restore the cursor position
            richTextBoxGraphQL.SelectionStart = originalSelectionStart;
            richTextBoxGraphQL.SelectionLength = originalSelectionLength;

            // Set the default color back
            richTextBoxGraphQL.SelectionColor = Color.Black;


        }

        private void ProcessRegexPattern(string pattern, Color color)
        {
            foreach (Match match in Regex.Matches(richTextBoxGraphQL.Text, pattern, RegexOptions.IgnoreCase))
            {
                richTextBoxGraphQL.Select(match.Index, match.Length);
                richTextBoxGraphQL.SelectionColor = color;

                // Move the cursor to the end of the matched text
                richTextBoxGraphQL.SelectionStart = richTextBoxGraphQL.SelectionStart + richTextBoxGraphQL.SelectionLength;
                richTextBoxGraphQL.SelectionLength = 0;
            }
        }

        private void ExecuteQuery(object sender, EventArgs e)
        {
            // You can implement GraphQL query execution logic here
            string query = richTextBoxGraphQL.Text;

            // Execute the GraphQL query (replace this with your actual logic)
            // For example, you might want to send the query to a GraphQL server
            // and display the results in another part of your UI.
            MessageBox.Show($"Executing GraphQL query:\n\n{query}");
        }

    }
}