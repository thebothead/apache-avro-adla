//-----------------------------------------------------------------------
// <copyright company="Schneider Electric">
//     Copyright (c) Schneider Electric. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Avro.File;
using FluentAssertions;
using Microsoft.Analytics.Interfaces;
using NSubstitute;
using SE.DSP.DataLake.Analytics.Tests;
using SE.DSP.DataLake.Analytics.Tests.Avro;
using Xbehave;
using Xunit;

namespace Avro.IO.ADLA.Tests
{
    /// <summary>
    /// Class AvroDataFileReaderStreamTests.
    /// </summary>
    public class AvroDataFileReaderStreamTests
    {
        #region Fields
        #endregion

        #region Constructors/Destructors
        #endregion

        #region Events
        #endregion

        #region Enums
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the with null reader error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CreateWithNullReaderError()
        {
            Assert.Throws<ArgumentNullException>(() => AvroDataFileReaderStream.OpenStream(null));
        }

        /// <summary>
        /// Creates the with null stream error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CreateWithNullStreamError()
        {
            Assert.Throws<ArgumentNullException>(() => AvroDataFileReaderStream.OpenStream(Substitute.For<IUnstructuredReader>()));
        }

        /// <summary>
        /// Creates the with no magic data error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CreateWithNoMagicDataError()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\empty.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));

                // Act
                // Assert
                Assert.Throws<ArgumentException>(() => AvroDataFileReaderStream.OpenStream(inputReader));
            }
        }

        /// <summary>
        /// Positions the of magic stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void GetStartPositionOfMagicStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Equal(0, baseStream.Position);
            }
        }

        /// <summary>
        /// Ends the position of magic stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void GetEndPositionOfMagicStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];
                
                // Act
                var read = baseStream.Read(buffer, 0, buffer.Length);

                // Assert
                Assert.Equal(buffer.Length, baseStream.Position);
                Assert.Equal(buffer.Length, read);
            }
        }

        /// <summary>
        /// Gets the position of last byte.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SetAndGetPositionOfLastByte()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                var currentPosition = baseStream.Position;
                baseStream.ReadByte();
                baseStream.Position = currentPosition;

                // Assert
                Assert.Equal(currentPosition, baseStream.Position);
            }
        }
        
        /// <summary>
        /// Sets the and get position of last byte.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void GetPositionOfLastByteInvalidDecrement()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.ReadByte();
                
                // Assert
                var error = Assert.Throws<InvalidOperationException>(() => { baseStream.Position = baseStream.Position + 1; });
                Assert.Equal("Can only decrement the position by one!", error.Message);
            }
        }

        /// <summary>
        /// Gets the position of last byte invalid last byte.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void GetPositionOfLastByteInvalidLastByte()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);

                // Assert
                var error = Assert.Throws<InvalidOperationException>(() => { baseStream.Position = baseStream.Position - 1; });
                Assert.Equal("There is no last byte to read!", error.Message);
            }
        }

        /// <summary>
        /// Gets the position of base stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void GetPositionOfBaseStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Read(buffer, 0, buffer.Length);

                // Assert
                Assert.Equal(buffer.Length * 2, baseStream.Position);
            }
        }

        /// <summary>
        /// Reads the magic stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadMagicStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                var read = baseStream.Read(buffer, 0, buffer.Length);

                // Assert
                Assert.Equal(buffer.Length, read);
                Assert.Equal(DataFileConstants.Magic, buffer);
            }
        }

        /// <summary>
        /// Reads the base stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadBaseStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Read(buffer, 0, buffer.Length);

                // Assert
                Assert.NotEqual(DataFileConstants.Magic, buffer);
            }
        }

        /// <summary>
        /// Reads the base byte.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadBaseByte()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                var baseByte = baseStream.ReadByte();

                // Assert
                Assert.NotEqual(-1, baseByte);
            }
        }

        /// <summary>
        /// Reads the last byte.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadLastByte()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                var currentPosition = baseStream.Position;
                var lastByte1 = baseStream.ReadByte();
                baseStream.Position = currentPosition;
                var lastByte2 = baseStream.ReadByte();

                // Assert
                Assert.Equal(lastByte1, lastByte2);
            }
        }

        /// <summary>
        /// Seeks the magic stream.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SeekMagicStream()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                baseStream.Read(buffer, 0, buffer.Length);
                var seek = baseStream.Seek(0, SeekOrigin.Begin);

                // Assert
                Assert.Equal(0, seek);
            }
        }

        /// <summary>
        /// Seeks the magic stream invalid offset.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SeekMagicStreamInvalidOffset()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Read(buffer, 0, buffer.Length);
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Seek(1, SeekOrigin.Begin));
                Assert.Equal("Can only seek the magic stream with no offset!", error.Message);
            }
        }

        /// <summary>
        /// Seeks the magic stream bad origin.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SeekMagicStreamBadOrigin()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Read(buffer, 0, buffer.Length);
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Seek(0, SeekOrigin.Current));
                Assert.Equal("Can only seek the magic stream from the beginning!", error.Message);
            }
        }

        /// <summary>
        /// Reads the magic stream bad position.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadMagicStreamBadPosition()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Read(buffer, 0, buffer.Length);
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Read(buffer, 0, buffer.Length));
                Assert.Equal("Can only read the magic stream from the beginning!", error.Message);
            }
        }

        /// <summary>
        /// Reads the magic stream bad count.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadMagicStreamBadCount()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Read(buffer, 0, 1));
                Assert.Equal("Can only read the entire magic stream!", error.Message);
            }
        }

        /// <summary>
        /// Reads the magic stream bad offset.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void ReadMagicStreamBadOffset()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Read(buffer, 1, buffer.Length));
                Assert.Equal("Can only read the entire magic stream!", error.Message);
            }
        }

        /// <summary>
        /// Reads the magic stream too many times.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SeekBaseStreamInvalid()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                baseStream.Read(buffer, 0, buffer.Length);
                var error = Assert.Throws<InvalidOperationException>(() => baseStream.Seek(0, SeekOrigin.Begin));
                Assert.Equal("Cannot seek the base stream!", error.Message);
            }
        }

        /// <summary>
        /// Bads the read byte on magic string1.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void BadReadByteOnMagicStringBeforeRead()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                
                // Act
                // Assert
                Assert.Throws<InvalidOperationException>(() => baseStream.ReadByte());
            }
        }

        /// <summary>
        /// Reads the byte on magic string2.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void BadReadByteOnMagicStringAfterRead()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Read(buffer, 0, buffer.Length);
                Assert.Throws<InvalidOperationException>(() => baseStream.ReadByte());
            }
        }

        /// <summary>
        /// Bads the read byte on magic string after seek.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void BadReadByteOnMagicStringAfterSeek()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);
                var buffer = new byte[DataFileConstants.Magic.Length];

                // Act
                // Assert
                baseStream.Read(buffer, 0, buffer.Length);
                baseStream.Seek(0, SeekOrigin.Begin);
                Assert.Throws<InvalidOperationException>(() => baseStream.ReadByte());
            }
        }

        /// <summary>
        /// Lengthes this instance.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void Length()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Equal(inputStream.Length, baseStream.Length);
            }
        }

        /// <summary>
        /// Determines whether this instance can read.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CanRead()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Equal(inputStream.CanRead, baseStream.CanRead);
            }
        }

        /// <summary>
        /// Determines whether this instance can seek.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CanSeek()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Equal(true, baseStream.CanSeek);
            }
        }

        /// <summary>
        /// Determines whether this instance can write.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void CanWrite()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Equal(false, baseStream.CanWrite);
            }
        }

        /// <summary>
        /// Flushes the error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void FlushError()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Throws<NotSupportedException>(() => baseStream.Flush());
            }
        }

        /// <summary>
        /// Sets the length error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void SetLengthError()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Throws<NotSupportedException>(() => baseStream.SetLength(0));
            }
        }

        /// <summary>
        /// Writes the error.
        /// </summary>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Unit test")]
        [Fact]
        public void WriteError()
        {
            // Arrange
            using (var inputStream = System.IO.File.OpenRead(@"Data\twitter.avro"))
            {
                var inputReader = AvroUtil.GetReaderFromStream(new NonSeekableStream(inputStream));
                var baseStream = AvroDataFileReaderStream.OpenStream(inputReader);

                // Act
                // Assert
                Assert.Throws<NotSupportedException>(() => baseStream.Write(new byte[0], 0, 0));
            }
        }

        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        #endregion
    }
}