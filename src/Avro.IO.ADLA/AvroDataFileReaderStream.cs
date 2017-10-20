//-----------------------------------------------------------------------
// <copyright company="Schneider Electric">
//     Copyright (c) Schneider Electric. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using Avro.File;
using Microsoft.Analytics.Interfaces;
using Stateless;

namespace Avro.IO.ADLA
{
    /// <summary>
    /// Class AvroDataFileReaderStream.
    /// </summary>
    /// <seealso cref="System.IO.Stream" />
    /// <remarks>
    /// <para>
    /// This class is implemented to use with the <see cref="DataFileReader{T}"/> class.
    /// </para>
    /// <para>
    /// ADLA does not currently provide a seekable stream via <see cref="IUnstructuredReader.BaseStream"/>,
    /// which is required by <see cref="DataFileReader{T}"/> class.  This class provides a workaround to
    /// the nonseekable <see cref="IUnstructuredReader.BaseStream"/> by specifically supporting the seek
    /// pattern employed by <see cref="DataFileReader{T}"/>.
    /// </para>
    /// <para>
    /// As soon as ADLA provides support for a seekable stream this class should be deprecated.
    /// </para>
    /// </remarks>
    public class AvroDataFileReaderStream : Stream
    {
        #region Fields

        /// <summary>
        /// The base stream that contains the input data from <see cref="IUnstructuredReader.BaseStream"/>.
        /// </summary>
        private readonly Stream baseStream;

        /// <summary>
        /// The stream that holds the <see cref="DataFileConstants.Magic"/> contents from <see cref="baseStream"/>.
        /// </summary>
        private readonly MemoryStream magicStream;

        /// <summary>
        /// The state machine for the reader.
        /// </summary>
        /// <remarks>
        /// Do not abstract this state machine with an interface because it should not be mocked
        /// during unit testing and there is no need to introduce the complexity of an abstraction.
        /// The internal state management of a particular class is just that, internal to it.  The tool in
        /// which we use to manage state is also not subject to manipulation by the consumer of this class.
        /// </remarks>
        private readonly StateMachine<ReadState, ReadTrigger> stateMachine;
        
        /// <summary>
        /// The last read byte from <see cref="baseStream"/> with <see cref="ReadByte"/>
        /// </summary>
        private int lastReadByte = -1;

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AvroDataFileReaderStream" /> class.
        /// </summary>
        /// <param name="baseStream">The base stream.  This is not disposed by this class.</param>
        private AvroDataFileReaderStream(Stream baseStream)
        {
            // The base stream, which is expected to be non-seekable.
            this.baseStream = baseStream ?? throw new ArgumentNullException(nameof(baseStream));

            // Read the magic buffer from the base stream and cache it
            // so that is can be read multiple times.
            var magicBuffer = new byte[DataFileConstants.Magic.Length];
            if (baseStream.Read(magicBuffer, 0, magicBuffer.Length) != magicBuffer.Length)
            {
                throw new ArgumentException(AvroErrors.BaseStreamMagicBytesWrong);
            }

            // The magic stream to hold the magic buffer contents.
            this.magicStream = new MemoryStream(magicBuffer);

            // The read state machine for governing how the reader is expcted to
            // interact with the underlying streams and operations.
            this.stateMachine = new StateMachine<ReadState, ReadTrigger>(ReadState.MagicStream);
            this.ConfigureStateMachine();
        }

        #endregion

        #region Events
        #endregion

        #region Enums

        /// <summary>
        /// This enumeration reflects the source of data that should be read from.
        /// </summary>
        private enum ReadState
        {
            /// <summary>
            /// Read from the magic stream.
            /// </summary>
            MagicStream,

            /// <summary>
            /// Read from the base stream.
            /// </summary>
            BaseStream,

            /// <summary>
            /// Read from The last byte.
            /// </summary>
            LastByte
        }

        /// <summary>
        /// This enumeration reflects triggers that may change the source of data to be read from.
        /// </summary>
        private enum ReadTrigger
        {
            /// <summary>
            /// The magic stream was read.
            /// </summary>
            ReadMagic,

            /// <summary>
            /// The base stream was read for one byte.
            /// </summary>
            ReadBaseByte,

            /// <summary>
            /// The last byte cached was read.
            /// </summary>
            ReadLastByte,

            /// <summary>
            /// The base stream was read.
            /// </summary>
            ReadBase,

            /// <summary>
            /// The base stream position was decremented by one.
            /// </summary>
            DecrementPosition,

            /// <summary>
            /// The seek operation on the magic stream.
            /// </summary>
            SeekMagic
        }
        
        #endregion

        #region Properties

        /// <inheritdoc/>
        public override long Length => this.baseStream.Length;

        /// <inheritdoc/>
        public override bool CanRead => this.baseStream.CanRead;

        /// <inheritdoc/>
        public override bool CanSeek => true;

        /// <inheritdoc/>
        public override bool CanWrite => false;

        /// <inheritdoc/>
        public override long Position
        {
            get
            {
                // If the reader is still working with the magic stream return the position of it.
                if (this.stateMachine.State == ReadState.MagicStream)
                {
                    return this.magicStream.Position;
                }

                // If the reader is reading the last byte then return the decremented position.
                if (this.stateMachine.State == ReadState.LastByte)
                {
                    return this.baseStream.Position - 1;
                }
                
                // Otherwise return the position of the base stream.
                return this.baseStream.Position;
            }

            set
            {
                // Ensure there is a valid last byte read before decrementing.
                if (this.lastReadByte == -1)
                {
                    throw new InvalidOperationException(AvroErrors.LastByteNotAvailable);
                }

                // We only support decrementing the position byte one to the last read byte.
                if (value != this.baseStream.Position - 1)
                {
                    throw new InvalidOperationException(AvroErrors.PositionCanOnlyDecrementByOne);
                }
                
                this.stateMachine.Fire(ReadTrigger.DecrementPosition);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open the stream from the unstructured reader to use with <see cref="DataFileReader{T}"/>.
        /// </summary>
        /// <param name="unstructuredReader">The unstructured reader.</param>
        /// <returns>Returnes a seekable stream for use with <see cref="DataFileReader{T}"/>.</returns>
        public static Stream OpenStream(IUnstructuredReader unstructuredReader)
        {
            if (unstructuredReader == null)
            {
                throw new ArgumentNullException(nameof(unstructuredReader));
            }
            
            return new AvroDataFileReaderStream(unstructuredReader.BaseStream);
        }

        /// <inheritdoc/>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return this.stateMachine.State == ReadState.MagicStream ?
                this.ReadMagicStream(buffer, offset, count) :
                this.ReadBaseStream(buffer, offset, count);
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            return this.stateMachine.State == ReadState.LastByte ?
                this.ReadLastByte() :
                this.ReadBaseByte();
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stateMachine.State == ReadState.MagicStream ?
                this.SeekMagicStream(offset, origin) :
                throw new InvalidOperationException(AvroErrors.BaseStreamUnseekable);
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[] beginBuffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.magicStream.Dispose();
            }
        }

        #endregion

        #region Private Methods

        private int ReadLastByte()
        {
            // The last byte is used after the reader decrements the position of the
            // stream to the previous read.  This occurs when the reader uses read byte
            // of end of stream checking.
            this.stateMachine.Fire(ReadTrigger.ReadLastByte);
            return this.lastReadByte;
        }

        private int ReadBaseByte()
        {
            // The reader uses ReadByte for end of stream detection and later will
            // decrement the position to before this read, so we need to save this
            // byte for later.
            this.stateMachine.Fire(ReadTrigger.ReadBaseByte);
            this.lastReadByte = this.baseStream.ReadByte();
            return this.lastReadByte;
        }
        
        private long SeekMagicStream(long offset, SeekOrigin origin)
        {
            this.stateMachine.Fire(ReadTrigger.SeekMagic);

            if (offset != 0)
            {
                throw new InvalidOperationException(AvroErrors.MagicStreamSeekOffsetNotAllowed);
            }

            if (origin != SeekOrigin.Begin)
            {
                throw new InvalidOperationException(AvroErrors.MagicStreamSeekOnlyBegin);
            }

            return this.magicStream.Seek(offset, origin);
        }

        private int ReadBaseStream(byte[] buffer, int offset, int count)
        {
            this.stateMachine.Fire(ReadTrigger.ReadBase);
            return this.baseStream.Read(buffer, offset, count);
        }

        private int ReadMagicStream(byte[] buffer, int offset, int count)
        {
            this.stateMachine.Fire(ReadTrigger.ReadMagic);
            
            if (this.magicStream.Position != 0)
            {
                throw new InvalidOperationException(AvroErrors.MagicStreamReadOnlyBegin);
            }

            if (count != DataFileConstants.Magic.Length || offset != 0)
            {
                throw new InvalidOperationException(AvroErrors.MagicStreamMustReadAll);
            }
            
            return this.magicStream.Read(buffer, offset, count);
        }

        private void ConfigureStateMachine()
        {
            // The number of times we expect the magic stream to be read by DataFileReader.
            const int expectedMagicReads = 2;
            
            // The count of how many times magic stream has been read.
            var magicReadCount = 0;

            // The magic stream state represents that we are currently reading the magic
            // stream and all operations should be performed against that stream.
            this.stateMachine.Configure(ReadState.MagicStream)
                .OnEntry(transition =>
                {
                    if (transition.Trigger == ReadTrigger.ReadMagic)
                    {
                        magicReadCount++;

                        // If this is the last magic read then transition to read the base stream.
                        if (magicReadCount == expectedMagicReads)
                        {
                            this.stateMachine.Fire(ReadTrigger.ReadBase);
                        }
                    }
                })
                .PermitReentryIf(ReadTrigger.ReadMagic, () => magicReadCount < expectedMagicReads)
                .PermitReentry(ReadTrigger.SeekMagic)
                .PermitIf(ReadTrigger.ReadBase, ReadState.BaseStream, () => magicReadCount == expectedMagicReads);

            // The base stream state represents that we are currently reading the base
            // stream and all operations should be performed against that stream.
            this.stateMachine.Configure(ReadState.BaseStream)
                .PermitReentry(ReadTrigger.ReadBase)
                .PermitReentry(ReadTrigger.ReadBaseByte)
                .Permit(ReadTrigger.DecrementPosition, ReadState.LastByte);

            // The last byte state represents that the position has been decremented
            // by the reader and we should read the last byte next before doing any
            // other operations on the base stream.
            this.stateMachine.Configure(ReadState.LastByte)
                .Permit(ReadTrigger.ReadLastByte, ReadState.BaseStream);
        }

        #endregion
    }
}