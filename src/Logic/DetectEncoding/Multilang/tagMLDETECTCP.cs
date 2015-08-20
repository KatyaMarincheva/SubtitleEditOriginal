// --------------------------------------------------------------------------------------------------------------------
// <copyright file="tagMLDETECTCP.cs" company="">
//   
// </copyright>
// <summary>
//   The mldetectcp.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiLanguage
{
    /// <summary>
    /// The mldetectcp.
    /// </summary>
    public enum MLDETECTCP
    {
        // Default setting will be used.
        /// <summary>
        /// The mldetectc p_ none.
        /// </summary>
        MLDETECTCP_NONE = 0, 

        // Input stream consists of 7-bit data.
        /// <summary>
        /// The mldetectc p_7 bit.
        /// </summary>
        MLDETECTCP_7BIT = 1, 

        // Input stream consists of 8-bit data.
        /// <summary>
        /// The mldetectc p_8 bit.
        /// </summary>
        MLDETECTCP_8BIT = 2, 

        // Input stream consists of double-byte data.
        /// <summary>
        /// The mldetectc p_ dbcs.
        /// </summary>
        MLDETECTCP_DBCS = 4, 

        // Input stream is an HTML page.
        /// <summary>
        /// The mldetectc p_ html.
        /// </summary>
        MLDETECTCP_HTML = 8, 

        // Not currently supported.
        /// <summary>
        /// The mldetectc p_ number.
        /// </summary>
        MLDETECTCP_NUMBER = 16
    }
}