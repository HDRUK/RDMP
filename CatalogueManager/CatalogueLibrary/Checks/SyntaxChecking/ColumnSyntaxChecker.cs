using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using CatalogueLibrary.Data;
using ReusableLibraryCode.Checks;

namespace CatalogueLibrary.Checks.SyntaxChecking
{
    /// <summary>
    /// Checks whether an IColumn has an alias and if so whether it is wrapped and whether it contains invalid characters or whitespace
    /// </summary>
    public class ColumnSyntaxChecker : SyntaxChecker
    {
        private readonly IColumn _column;

        public  ColumnSyntaxChecker(IColumn column)
        {
            _column = column;
        }

        /// <summary>
        /// Checks to see if there is an alias and if there is whether it is wrapped. If it is not wrapped and there are invalid characters or whitespace in the alias this causes a SyntaxErrorException to be thrown.
        /// </summary>
        /// <param name="notifier"></param>
        public override void Check(ICheckNotifier notifier)
        {
            string regexIsWrapped = @"^[\[`].*[\]`]$";
            char[] invalidColumnValues = new[] { ',', '[', ']', '`', '.' };
            char[] whiteSpace = new[] { ' ', '\t', '\n', '\r' };

            char[] openingCharacters = new[] { '[', '(' };
            char[] closingCharacters = new[] { ']', ')' };

            //it has an alias
            if (!String.IsNullOrWhiteSpace(_column.Alias))
                if (!Regex.IsMatch(_column.Alias, regexIsWrapped)) //alias is NOT wrapped
                    if (_column.Alias.Any(invalidColumnValues.Contains)) //there are invalid characters
                        throw new SyntaxErrorException("Invalid characters found in Alias \"" + _column.Alias + "\"");
                    else
                        if (_column.Alias.Any(whiteSpace.Contains))
                            throw new SyntaxErrorException("Whitespace found in unwrapped Alias \"" + _column.Alias + "\"");

            ParityCheckCharacterPairs(openingCharacters, closingCharacters, _column.SelectSQL);
        }
    }
}