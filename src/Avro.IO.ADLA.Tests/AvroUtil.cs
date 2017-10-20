//-----------------------------------------------------------------------
// <copyright company="Schneider Electric">
//     Copyright (c) Schneider Electric. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Types.Sql;
using Microsoft.Analytics.UnitTest;
using NSubstitute;
using Xbehave;
using Xunit;

namespace SE.DSP.DataLake.Analytics.Tests.Avro
{
    /// <summary>
    /// Class AvroUtil.
    /// </summary>
    public static class AvroUtil
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
        /// Gets the reader from stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>IUnstructuredReader.</returns>
        public static IUnstructuredReader GetReaderFromStream(Stream stream)
        {
            var input = Substitute.For<IUnstructuredReader>();
            input.BaseStream.Returns(stream);
            input.Length.Returns(stream.Length);
            return input;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #endregion
    }
}