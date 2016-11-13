/*
The MIT License(MIT)
=====================

Copyright(c) 2008, Cagatay Dogan

Permission is hereby granted, free of charge, to any person obtaining a cop
of this software and associated documentation files (the "Software"), to deal  
in the Software without restriction, including without limitation the right
to use, copy, modify, merge, publish, distribute, sublicense, and/or sel
copies of the Software, and to permit persons to whom the Software is  
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in  
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS O
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,  
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL TH
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHE
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,  
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS I
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Sweet.BRE
{
    /// <summary>
    /// Used for internal exceptions which occures in data process.
    /// </summary>
    [Serializable]
    public class StatementException : ApplicationException
    {
        // Summary:
        //     Initializes a new instance of the AdaBusinessPlatform.StatementException class.
        public StatementException()
            : base()
        { }

        //
        // Summary:
        //     Initializes a new instance of the AdaBusinessPlatform.StatementException class with
        //     a specified error message.
        //
        // Parameters:
        //   message:
        //     A message that describes the error.
        public StatementException(string message)
            : base(message)
        { }

        //
        // Summary:
        //     Initializes a new instance of the AdaBusinessPlatform.StatementException class with
        //     a specified error message.
        //
        // Parameters:
        //   format:
        //     A message format that describes the error.
        //   args:
        //     Arguments of the message format.
        public StatementException(string format, params object[] args)
            : base(String.Format(format, args))
        { }

        //
        // Summary:
        //     Initializes a new instance of the AdaBusinessPlatform.StatementException class with
        //     serialized data.
        //
        // Parameters:
        //   info:
        //     The object that holds the serialized object data.
        //
        //   context:
        //     The contextual information about the source or destination.
        protected StatementException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        //
        // Summary:
        //     Initializes a new instance of the AdaBusinessPlatform.StatementException class with
        //     a specified error message and a reference to the inner exception that is
        //     the cause of this exception.
        //
        // Parameters:
        //   message:
        //     The error message that explains the reason for the exception.
        //
        //   innerException:
        //     The exception that is the cause of the current exception. If the innerException
        //     parameter is not a null reference, the current exception is raised in a catch
        //     block that handles the inner exception.
        public StatementException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
