using System.ComponentModel;

namespace Firepuma.EmailAndPush.Abstractions.Models.ValueObjects;

public enum PushMessageUrgency
{
    // [Description("very-low")]
    // VeryLow, Google Chrome browser actually does not support "very-low", it says it can only be low, normal or high 

    [Description("low")]
    Low,

    [Description("normal")]
    Normal,

    [Description("high")]
    High,
}