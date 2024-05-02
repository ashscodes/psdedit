namespace PsdEdit;

internal class Strings
{
    public const string ArgumentOutsideOfIndex = "The value provided '{0}' was outside the total values '{1}'.";

    public const string ArrayIsEmpty = "The value provided for property '{0}' is an empty array.";

    public const string CannotOverwriteFile = "The target file specified '{0}' already exists, if you intended to overwrite it, please set the 'force' parameter to true.";

    public const string CollectionIsEmpty = "The value provided for property '{0}' is an empty collection.";

    public const string CollectionTrackerEmpty = "Could not {0} the current collection from the stack, because it is empty. Token index: {1}.";

    public const string CollectionTrackerInvalidAddMember = "Could not add an item to a collection in the stack, because the stack is empty.";

    public const string CommentedMapEntry = "# {0} = @()";

    public const string CouldNotSetValue = "The value provided could not be cast to the specific type '{0}'.";

    public const string DataFileInvalid = "The data file '{0}' could not be parsed.";

    public const string DataFileNotFound = "A PowerShell data file could not be found or there was more than one in the given directory. '{0}'";

    public const string DataFileReadError = "An item of type '{0}' could not be read at token index '{1}'.";

    public const string InsufficientValuesInArray = "Cannot validate a set of items, because the set '{0}: {1}' provided does not have enough items to satisfy all conditions of the validator. (Should be {2})";

    public const string InvalidFileType = "The provided file '{0}' does not have the required extension ({1}).";

    public const string InvalidKeyword = "A keyword must be followed by either a condition or a scriptblock i.e. if (things), or else { do things }";

    public const string InvalidValidationItem = "The validator provided of type '{0}' cannot be used in an union operation with the item of type '{1}'.";

    public const string LookBehindRangeInvalid = "A look-behind operation for '{0}' items was requested, but the current index '{1}' is less than this figure.";

    public const string NegativeNumberInvalid = "The number provided to the method was '{0}' and is invalid as the method requires a value of '0' or higher.";

    public const string NoTargetSpecified = "There was no target file defined for the writer to use.";

    public const string ObjectIsNull = "The value provided for property '{0}' is null.";

    public const string RangesNotEqual = "Cannot validate a set of items, because they do not have an equal number of items. First '{0}'; Second '{1}'";

    public const string ScriptBlockInvalid = @"
The ScriptBlock provided could not be parsed:

{0}

Original ScriptBlock:

{1}
";

    public const string StringIsNullOrEmpty = "The value provided for property '{0}' is null or empty.";


    #region Operators

    public static readonly string[] ComparisonOperators =
    [
        "-eq",
        "-ieq",
        "-ceq",
        "-ne",
        "-ine",
        "-cne",
        "-gt",
        "-igt",
        "-cgt",
        "-ge",
        "-ige",
        "-cge",
        "-lt",
        "-ilt",
        "-clt",
        "-le",
        "-ile",
        "-cle",
        "-like",
        "-ilike",
        "-clike",
        "-notlike",
        "-inotlike",
        "-cnotlike",
        "-match",
        "-imatch",
        "-cmatch",
        "-notmatch",
        "-inotmatch",
        "-cnotmatch",
        "-replace",
        "-ireplace",
        "-creplace",
        "-contains",
        "-icontains",
        "-ccontains",
        "-notcontains",
        "-inotcontains",
        "-cnotcontains",
        "-in",
        "-notin",
        "-is",
        "-isnot"
    ];

    public static readonly string[] LogicalOperators =
    [
        "-and",
        "-or",
        "-xor"
    ];

    #endregion Operators
}