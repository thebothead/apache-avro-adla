//-----------------------------------------------------------------------
// <copyright company="Schneider Electric">
//     Copyright (c) Schneider Electric. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;

namespace SE.DSP.DataLake.Analytics.Tests
{
    /// <summary>
    /// Class NonSeekableStream.
    /// </summary>
    /// <seealso cref="System.IO.Stream" />
    public class NonSeekableStream : Stream
    {
        #region Fields

        private readonly Stream stream;

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSeekableStream"/> class.
        /// </summary>
        /// <param name="baseStream">The base stream.</param>
        public NonSeekableStream(Stream baseStream)
        {
            this.stream = baseStream;
        }

        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Properties

        /// <inheritdoc/>
        public override bool CanRead
        {
            get { return this.stream.CanRead; }
        }

        /// <inheritdoc/>
        public override bool CanSeek
        {
            get { return false; }
        }

        /// <inheritdoc/>
        public override bool CanWrite
        {
            get { return this.stream.CanWrite; }
        }

        /// <inheritdoc/>
        public override long Length
        {
            get { return this.stream.Length; }
        }

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                return this.stream.Position;
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override void Flush()
        {
            this.stream.Flush();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.stream.Read(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
        }

        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}