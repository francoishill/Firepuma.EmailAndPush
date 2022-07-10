using System;
using System.Runtime.Serialization;

namespace Firepuma.EmailAndPush.FunctionApp.Infrastructure.Exceptions;

[Serializable]
public class WebPushDeviceGoneException : Exception
{
    public WebPushDeviceGoneException()
    {
    }

    public WebPushDeviceGoneException(string message)
        : base(message)
    {
    }

    public WebPushDeviceGoneException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected WebPushDeviceGoneException(
        SerializationInfo info,
        StreamingContext context)
        : base(info, context)
    {
    }
}