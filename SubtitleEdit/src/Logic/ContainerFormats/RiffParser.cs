// (c) Giora Tamir (giora@gtamir.com), 2005

namespace Nikse.SubtitleEdit.Logic.ContainerFormats
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    #region RiffParserException

    [Serializable]
    public class RiffParserException : ApplicationException
    {
        public RiffParserException()
        {
        }

        public RiffParserException(string message)
            : base(message)
        {
        }

        public RiffParserException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public RiffParserException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    #endregion RiffParserException

    public class RiffParser : IDisposable
    {
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Determine if the file is a valid RIFF file
        /// </summary>
        /// <param name="filename">File to examine</param>
        /// <returns>True if file is a RIFF file</returns>
        public void OpenFile(string filename)
        {
            // Sanity check
            if (null != this.m_stream)
            {
                throw new RiffParserException("RIFF file already open " + this.FileName);
            }

            bool errorOccured = false;

            // Opening a new file
            try
            {
                FileInfo fi = new FileInfo(filename);
                this.FileName = fi.FullName;
                this.ShortName = fi.Name;
                this.m_filesize = fi.Length;
                fi = null;

                // Console.WriteLine(ShortName + " is a valid file.");

                // Read the RIFF header
                this.m_stream = new FileStream(this.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                int FourCC;
                int datasize;
                int fileType;

                this.ReadTwoInts(out FourCC, out datasize);
                this.ReadOneInt(out fileType);

                this.FileRiff = FourCC;
                this.FileType = fileType;

                // Check for a valid RIFF header
                string riff = FromFourCC(FourCC);
                if (riff == RIFF4CC || riff == RIFX4CC)
                {
                    // Good header. Check size
                    // Console.WriteLine(ShortName + " has a valid type \"" + riff + "\"");
                    // Console.WriteLine(ShortName + " has a specific type of \"" + FromFourCC(fileType) + "\"");
                    this.DataSize = datasize;
                    if (this.m_filesize >= this.DataSize + TWODWORDSSIZE)
                    {
                        // Console.WriteLine(ShortName + " has a valid size");
                    }
                    else
                    {
                        this.m_stream.Close();
                        this.m_stream = null;
                        throw new RiffParserException("Error. Truncated file " + this.FileName);
                    }
                }
                else
                {
                    this.m_stream.Close();
                    this.m_stream.Dispose();
                    this.m_stream = null;
                    throw new RiffParserException("Error. Not a valid RIFF file " + this.FileName);
                }
            }
            catch (RiffParserException)
            {
                errorOccured = true;
                throw;
            }
            catch (Exception exception)
            {
                errorOccured = true;
                throw new RiffParserException("Error. Problem reading file " + this.FileName, exception);
            }
            finally
            {
                if (errorOccured && (null != this.m_stream))
                {
                    this.m_stream.Close();
                    this.m_stream.Dispose();
                    this.m_stream = null;
                }
            }
        }

        /// <summary>
        ///     Read the next RIFF element invoking the correct delegate.
        ///     Returns true if an element can be read
        /// </summary>
        /// <param name="bytesleft">Reference to number of bytes left in the current list</param>
        /// <param name="chunk">Method to invoke if a chunk is found</param>
        /// <param name="list">Method to invoke if a list is found</param>
        /// <returns></returns>
        public bool ReadElement(ref int bytesleft, ProcessChunkElement chunk, ProcessListElement list)
        {
            // Are we done?
            if (TWODWORDSSIZE > bytesleft)
            {
                return false;
            }

            // Console.WriteLine(m_stream.Position.ToString() + ", " + bytesleft.ToString());

            // We have enough bytes, read
            int FourCC;
            int size;

            this.ReadTwoInts(out FourCC, out size);

            // Reduce bytes left
            bytesleft -= TWODWORDSSIZE;

            // Do we have enough bytes?
            if (bytesleft < size)
            {
                // Skip the bad data and throw an exception
                this.SkipData(bytesleft);
                bytesleft = 0;
                throw new RiffParserException("Element size mismatch for element " + FromFourCC(FourCC) + " need " + size + " but have only " + bytesleft);
            }

            // Examine the element, is it a list or a chunk
            string type = FromFourCC(FourCC);
            if (type == LIST4CC)
            {
                // We have a list
                this.ReadOneInt(out FourCC);

                if (null == list)
                {
                    this.SkipData(size - 4);
                }
                else
                {
                    // Invoke the list method
                    list(this, FourCC, size - 4);
                }

                // Adjust size
                bytesleft -= size;
            }
            else
            {
                // Calculated padded size - padded to WORD boundary
                int paddedSize = size;
                if (0 != (size & 1))
                {
                    ++paddedSize;
                }

                if (null == chunk)
                {
                    this.SkipData(paddedSize);
                }
                else
                {
                    chunk(this, FourCC, size, paddedSize);
                }

                // Adjust size
                bytesleft -= paddedSize;
            }

            return true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.m_stream != null)
                {
                    this.m_stream.Dispose();
                    this.m_stream = null;
                }
            }
        }

        #region CONSTANTS

        public const int DWORDSIZE = 4;

        public const int TWODWORDSSIZE = 8;

        public const string RIFF4CC = "RIFF";

        public const string RIFX4CC = "RIFX";

        public const string LIST4CC = "LIST";

        // Known file types
        public static readonly int ckidAVI = ToFourCC("AVI ");

        public static readonly int ckidWAV = ToFourCC("WAVE");

        public static readonly int ckidRMID = ToFourCC("RMID");

        #endregion CONSTANTS

        #region private members

        private long m_filesize;

        private FileStream m_stream;

        // For non-thread-safe memory optimization
        private readonly byte[] m_eightBytes = new byte[TWODWORDSSIZE];

        private readonly byte[] m_fourBytes = new byte[DWORDSIZE];

        #endregion private members

        #region Delegates

        /// <summary>
        ///     Method to be called when a list element is found
        /// </summary>
        /// <param name="FourCCType"></param>
        /// <param name="length"></param>
        public delegate void ProcessListElement(RiffParser rp, int FourCCType, int length);

        /// <summary>
        ///     Method to be called when a chunk element is found
        /// </summary>
        /// <param name="FourCCType"></param>
        /// <param name="unpaddedLength"></param>
        /// <param name="paddedLength"></param>
        public delegate void ProcessChunkElement(RiffParser rp, int FourCCType, int unpaddedLength, int paddedLength);

        #endregion Delegates

        #region public Members

        /// <summary>
        ///     RIFF data segment size
        /// </summary>
        public int DataSize { get; private set; }

        /// <summary>
        ///     Current file name
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        ///     Current short (name only) file name
        /// </summary>
        public string ShortName { get; private set; }

        /// <summary>
        ///     Return the general file type (RIFF or RIFX);
        /// </summary>
        public int FileRiff { get; private set; }

        /// <summary>
        ///     Return the specific file type (AVI/WAV...)
        /// </summary>
        public int FileType { get; private set; }

        #endregion public Members

        #region Stream access

        /// <summary>
        ///     Non-thread-safe method to read two ints from the stream
        /// </summary>
        /// <param name="FourCC">Output FourCC int</param>
        /// <param name="size">Output chunk/list size</param>
        public unsafe void ReadTwoInts(out int FourCC, out int size)
        {
            try
            {
                int readsize = this.m_stream.Read(this.m_eightBytes, 0, TWODWORDSSIZE);

                if (TWODWORDSSIZE != readsize)
                {
                    throw new RiffParserException("Unable to read. Corrupt RIFF file " + this.FileName);
                }

                fixed (byte* bp = &this.m_eightBytes[0])
                {
                    FourCC = *((int*)bp);
                    size = *((int*)(bp + DWORDSIZE));
                }
            }
            catch (Exception ex)
            {
                throw new RiffParserException("Problem accessing RIFF file " + this.FileName, ex);
            }
        }

        /// <summary>
        ///     Non-thread-safe read a single int from the stream
        /// </summary>
        /// <param name="FourCC">Output int</param>
        public unsafe void ReadOneInt(out int FourCC)
        {
            try
            {
                int readsize = this.m_stream.Read(this.m_fourBytes, 0, DWORDSIZE);

                if (DWORDSIZE != readsize)
                {
                    throw new RiffParserException("Unable to read. Corrupt RIFF file " + this.FileName);
                }

                fixed (byte* bp = &this.m_fourBytes[0])
                {
                    FourCC = *((int*)bp);
                }
            }
            catch (Exception ex)
            {
                throw new RiffParserException("Problem accessing RIFF file " + this.FileName, ex);
            }
        }

        /// <summary>
        ///     Skip the specified number of bytes
        /// </summary>
        /// <param name="skipBytes">Number of bytes to skip</param>
        public void SkipData(int skipBytes)
        {
            try
            {
                this.m_stream.Seek(skipBytes, SeekOrigin.Current);
            }
            catch (Exception ex)
            {
                throw new RiffParserException("Problem seeking in file " + this.FileName, ex);
            }
        }

        /// <summary>
        ///     Read the specified length into the byte array at the specified
        ///     offset in the array
        /// </summary>
        /// <param name="data">Array of bytes to read into</param>
        /// <param name="offset">Offset in the array to start from</param>
        /// <param name="length">Number of bytes to read</param>
        /// <returns>Number of bytes actually read</returns>
        public int ReadData(byte[] data, int offset, int length)
        {
            try
            {
                return this.m_stream.Read(data, offset, length);
            }
            catch (Exception ex)
            {
                throw new RiffParserException("Problem reading data in file " + this.FileName, ex);
            }
        }

        /// <summary>
        ///     Close the RIFF file
        /// </summary>
        public void CloseFile()
        {
            if (null != this.m_stream)
            {
                this.m_stream.Close();
                this.m_stream = null;
            }
        }

        #endregion Stream access

        #region FourCC conversion methods

        public static string FromFourCC(int FourCC)
        {
            char[] chars = new char[4];
            chars[0] = (char)(FourCC & 0xFF);
            chars[1] = (char)((FourCC >> 8) & 0xFF);
            chars[2] = (char)((FourCC >> 16) & 0xFF);
            chars[3] = (char)((FourCC >> 24) & 0xFF);

            return new string(chars);
        }

        public static int ToFourCC(string FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC strings must be 4 characters long " + FourCC);
            }

            int result = FourCC[3] << 24 | FourCC[2] << 16 | FourCC[1] << 8 | FourCC[0];

            return result;
        }

        public static int ToFourCC(char[] FourCC)
        {
            if (FourCC.Length != 4)
            {
                throw new Exception("FourCC char arrays must be 4 characters long " + new string(FourCC));
            }

            int result = FourCC[3] << 24 | FourCC[2] << 16 | FourCC[1] << 8 | FourCC[0];

            return result;
        }

        public static int ToFourCC(char c0, char c1, char c2, char c3)
        {
            int result = c3 << 24 | c2 << 16 | c1 << 8 | c0;

            return result;
        }

        #endregion FourCC conversion methods
    }
}