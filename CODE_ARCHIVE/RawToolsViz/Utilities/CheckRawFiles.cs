using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThermoFisher.CommonCore.Data.Business;
using System.Windows.Forms;

namespace RawToolsViz.Utilities
{
    static class CheckRawFiles
    {
        public static bool IsErrorFree(string rawFile)
        {
            using (var file = RawFileReaderFactory.ReadFile(rawFile))
            {
                var error = file.FileError;

                if (error.HasError)
                {
                    string message = String.Format("{0} has error code: {1}" +
                        "\n\n" +
                        "Error message: {2}", Path.GetFileName(rawFile), error.ErrorCode, error.ErrorMessage);

                    var result = MessageBox.Show(message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool IsWarningFree(string rawFile)
        {
            using (var file = RawFileReaderFactory.ReadFile(rawFile))
            {
                var error = file.FileError;

                if (error.HasWarning)
                {
                    string message = String.Format("{0} has a warning message: {1}\n\n" +
                        "Do you wish to continue?", Path.GetFileName(rawFile), error.WarningMessage);

                    var result = MessageBox.Show(message, "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    return (result == DialogResult.Yes);
                }
                else
                {
                    return true;
                }
            }
        }

        public static bool HasErrorsOrWarnings(string rawFile)
        {
            return !(IsErrorFree(rawFile) && IsWarningFree(rawFile));
        }
    }
}
