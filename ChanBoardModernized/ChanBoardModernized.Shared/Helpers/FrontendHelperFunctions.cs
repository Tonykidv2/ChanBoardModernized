using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChanBoardModernized.Shared.Helpers;

public class FrontendHelperFunctions
{
    public static string FormatFileName(string fileName, int maxLength = 25)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return fileName;

        var extension = Path.GetExtension(fileName);
        var name = Path.GetFileNameWithoutExtension(fileName);

        // If already short enough, return as-is
        if (fileName.Length <= maxLength)
            return fileName;

        const string ellipsis = "(...)";

        // Reserve space for ellipsis + extension
        int allowedNameLength = maxLength - ellipsis.Length - extension.Length;

        if (allowedNameLength <= 0)
            return ellipsis + extension;

        var truncatedName = name.Substring(0, allowedNameLength);

        return $"{truncatedName}{ellipsis}{extension}";
    }

}
